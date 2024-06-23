using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebServer.http;

namespace WebServer.classes
{
    public class RequestHandler
    {
        public string serverPath;
        public string message404Path;
        public string message403Path;

        bool ssl;

        X509Certificate2 certificate;


        GETHandler GET = new();
        POSTHandler POST = new();

        public async Task Initialize()
        {
            ssl = Convert.ToBoolean(Config.GetConfigValue("ssl"));

            if (ssl)
            {
                AcmeCert certGetter = new();

                certificate = await certGetter.GetCert(Config.GetConfigValue("domain"), Config.GetGitConfigValue("certificate-passphrase"), Config.certificateExpirationDate);
            }
        }

        public async Task HandleRequest(TcpClient client)
        {
            DateTime startHandle = DateTime.Now;

            Stream stream = client.GetStream();

            if(ssl)
            {
                SslStream sslStream = new SslStream(stream, false);

                bool authSucces = await SslAuth(sslStream);

                stream = sslStream;

                if (!authSucces)
                {
                    EndAllComunication(stream, client);
                    return;
                }
            }

            try
            {
                HttpRequest request = new();

                bool goodRequest = await request.ReadHttpRequest(stream);
                if(!goodRequest)
                {
                    EndAllComunication(stream,client);
                    return;
                }

                Console.WriteLine(request.referer);

                switch(request.Method)
                {
                    case HttpRequest.RequestMethod.GET:
                        await GET.HandleRequest(stream, request);
                        break;

                    case HttpRequest.RequestMethod.POST:
                        await POST.HandleRequest(stream, request);
                        break;
                }

                EndAllComunication(stream, client);

                Console.WriteLine($"Handling took: {DateTime.Now - startHandle} For request: {request.RequestedResource}");
            }
            catch (Exception e) 
            {
                Console.WriteLine("Processing request failed: " + e.ToString());
            }
        }

        void EndAllComunication(Stream stream, TcpClient client)
        {
            stream.Close();
            client.Close();
            stream.Dispose();
            client.Dispose();
        }

        async Task<bool> SslAuth(SslStream sslStream) 
        {
            try
            {
                await sslStream.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false).ConfigureAwait(false);

                if(!sslStream.IsEncrypted || !sslStream.IsAuthenticated)
                {
                    return false;
                }

                return true; //success
            }
            catch (Exception e)
            {
                Console.WriteLine("SSL AUTH FAILED!!! exception: " + e.Message);
                return false;
            }
        }
    }
}
