using System.Net;
using System;
using WebServer.classes;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Globalization;
using System.Net.Security;
using System.Security.Authentication;

class Program
{
    public static void Main(string[] args)
    {
        Configuration configObj = new();
        configObj.LoadConfig();

        Console.WriteLine("Starting server!!");

        RequestHandler handler = new(configObj.GetValue("root-path"), configObj.GetValue("404-path"), configObj.GetValue("403-path"));

        X509Certificate2 certificate = null;

        string certPath = configObj.GetValue("certificate-path");
        bool useCertificate = !string.IsNullOrEmpty(certPath);

        if(useCertificate)
        {
            certificate = new X509Certificate2(certPath);

        }

        TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5050);
        listener.Start();

        if(useCertificate)
        {
            HttpsListen(listener, certificate, handler);
        }
        else
        {
            HttpListen(listener, handler);
        }
    }

    private static void HttpListen(TcpListener listener, RequestHandler handler)
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClientAsync().GetAwaiter().GetResult(); //async later

            handler.HandleRequest(client.GetStream());
        }
    }

    private static void HttpsListen(TcpListener listener, X509Certificate2 certificate, RequestHandler handler)
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClientAsync().GetAwaiter().GetResult(); //async later

            using (SslStream sslStream = new SslStream(client.GetStream()))
            {
                sslStream.AuthenticateAsServer(certificate, false, SslProtocols.Tls12, true);

                handler.HandleRequest(sslStream);
            }
        }
    }
}