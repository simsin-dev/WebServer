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
using System.Security.Principal;

namespace WebServer.classes
{
    public class AcmeCert
    {
        public async Task<X509Certificate2> GetCert(string domain, string certPassword, DateTime Expires)
        {
            Console.WriteLine("reading cert from a file");
            return ReadCert(certPassword);

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

            //Code below is commented out bc of my current inability to debug it

           /*  Console.WriteLine("Certificate expired or was not accesible, getting from acme");

            var context = new AcmeContext(WellKnownServers.LetsEncryptStagingV2);

            string pemKey;

            if(!File.Exists(Config.GetConfigValue("acme-priv-key-path")))
            {
                Console.WriteLine(await context.TermsOfService());


                var account = await context.NewAccount(Config.GetConfigValue("email"), true);

                Console.WriteLine("created the account");
                try
                {
                    pemKey = context.AccountKey.ToPem();

                    File.WriteAllText(Config.GetConfigValue("acme-priv-key-path"), pemKey);
                }
                catch(Exception e)
                {
                    Console.WriteLine("change the acme-priv-key-path config value to a valid path !!!!!\n\n");
                    throw e;
                }
            }
            else
            {
                Console.WriteLine("reading priv key");
                pemKey = File.ReadAllText(Config.GetConfigValue("acme-priv-key-path"));
            }
            var accountKey = KeyFactory.FromPem(pemKey);
            context = null;

            Console.WriteLine("Getting acme context");
            var acme = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, accountKey);
            accountKey = null;
            pemKey = null;
            
            GC.Collect();

            Console.WriteLine("creating an order...");
            var order = await acme.NewOrder(new[] { $"{domain}" });

            var orderLocation = order.Location;
            Console.WriteLine(orderLocation);

            var authz = (await order.Authorizations()).First();

            Console.WriteLine("getting challange...");

            var httpChallange = await authz.Http();
            var keyAuthz = httpChallange.KeyAuthz;
            var ChallangeToken = httpChallange.Token;

            Console.WriteLine(ChallangeToken);

            await ListenForRequest(domain,ChallangeToken, keyAuthz, httpChallange);

            var certKey = KeyFactory.NewKey(KeyAlgorithm.RS256);

            CertificateChain certChain = await order.Generate(new CsrInfo { CommonName = domain}, certKey);

            PfxBuilder pfxBuilder = certChain.ToPfx(certKey);
            var pfx = pfxBuilder.Build(domain, certPassword);

            var cert = new X509Certificate2(pfx, certPassword, X509KeyStorageFlags.Exportable);

            SaveCert(cert, certPassword);
            */
            return null;
        }

/*         private void SaveCert(X509Certificate2 cert, string certPassword)
        {
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(),"cert.pfx"), cert.Export(X509ContentType.Pkcs12, certPassword));
        } */

        private X509Certificate2 ReadCert(string certPassword)
        {
            byte[] certBytes = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "cert.pfx"));

            if (certBytes.Length == 0 || certBytes == null)
            {
                throw new FileLoadException();
            }

            return new X509Certificate2(certBytes, certPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
        }

/*         async Task ListenForRequest(string domain, string token, string keyAuthz, IChallengeContext? challange)
        {
            Console.WriteLine("listening for requests");
            HttpListener listener = new();
            listener.Prefixes.Add($"http://*:80/");
            listener.Start();

            var task = Task.Run(() =>
            {
                while (true)
                {

                    var context = listener.GetContext();
                    Console.WriteLine(context.Request.Url.AbsolutePath);

                    if (context.Request.Url.AbsolutePath.Contains($"well-known/acme-challenge/{token}"))
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
        } */


    }
}
