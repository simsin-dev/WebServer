using Certes.Acme;
using Certes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json.Linq;
using Certes.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace WebServer.classes
{
    public class AcmeCert
    {
        public async Task<X509Certificate2> GetCert(string domain, string certPassword, DateTime Expires)
        {
            if(Expires > DateTime.Now) 
            {
                Console.WriteLine("reading cert from a file");
                
                try
                {
                    return ReadCert(certPassword);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Certificate expired or was not accesible, getting from acme");

            var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);

            var order = await acme.NewOrder(new[] { domain });

            var orderLocation = order.Location;
            Console.WriteLine(orderLocation);

            var authz = (await order.Authorizations()).First();
            var httpChallange = await authz.Http();
            var keyAuthz = httpChallange.KeyAuthz;
            var ChallangeToken = httpChallange.Token;

            await ListenForRequest(ChallangeToken, keyAuthz, httpChallange);

            var certKey = KeyFactory.NewKey(KeyAlgorithm.RS256);
            CertificationRequestBuilder csr = new CertificationRequestBuilder();
            csr.AddName("CN", domain);
            CertificateChain certChain = await order.Generate(new CsrInfo { CommonName = domain}, certKey);

            PfxBuilder pfxBuilder = certChain.ToPfx(certKey);
            var pfx = pfxBuilder.Build(domain, certPassword);

            var cert = new X509Certificate2(pfx, certPassword, X509KeyStorageFlags.Exportable);

            SaveCert(cert, certPassword);

            return cert;
        }

        private void SaveCert(X509Certificate2 cert, string certPassword)
        {
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(),"cert.pfx"), cert.Export(X509ContentType.Pkcs12, certPassword));
        }

        private X509Certificate2 ReadCert(string certPassword)
        {
            byte[] certBytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "cert.pfx"));

            if (certBytes.Length == 0 || certBytes == null)
            {
                throw new FileLoadException();
            }

            return new X509Certificate2(certBytes, certPassword, X509KeyStorageFlags.Exportable);
        }

        async Task ListenForRequest(string token, string keyAuthz, IChallengeContext? challange)
        {
            HttpListener listener = new();
            listener.Prefixes.Add($"http://*:80/.well-known/acme-challenge/{token}");
            listener.Start();

            var task = Task.Run(() =>
            {
                while (true)
                {

                    var context = listener.GetContext();
                    if (context.Request.Url.AbsolutePath.EndsWith(token))
                    {
                        context.Response.StatusCode = 200;

                        byte[] buffer = Encoding.UTF8.GetBytes(keyAuthz);
                        context.Response.ContentLength64 = buffer.Length;
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Close();
                        return Task.CompletedTask;
                    }
                    else
                    {
                        context.Response.StatusCode = 503;

                        byte[] buffer = Encoding.UTF8.GetBytes("Server is fighting for a certificate, give it a minute :3");
                        context.Response.ContentLength64 = buffer.Length;
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Close();
                    }
                }
            });


            await challange.Validate();

            task.Wait();

            listener.Stop();
        }


    }
}
