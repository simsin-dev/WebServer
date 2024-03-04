using System.Net;
using System;
using WebServer.classes;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Globalization;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;

class Program
{
    public static async Task Main(string[] args)
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
            certificate = new X509Certificate2(certPath, configObj.GetValue("certificate-passphrase"));
        }

        TcpListener listener = new TcpListener(IPAddress.Parse(configObj.GetValue("ip-to-listen-on")), Convert.ToInt32(configObj.GetValue("port")));
        listener.Start();

        if(useCertificate)
        {
            await HttpsListen(listener, certificate, handler);
        }
        else
        {
            await HttpListen(listener, handler);
        }
    }

    private static async Task HttpListen(TcpListener listener, RequestHandler handler)
    {
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync(); //async later

            handler.HandleRequest(client.GetStream(), client);
        }
    }

    private static async Task HttpsListen(TcpListener listener, X509Certificate2 certificate, RequestHandler handler)
    {

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync(); //async later

            using (SslStream sslStream = new SslStream(client.GetStream(), false))
            {
                await sslStream.AuthenticateAsServerAsync(certificate,false, SslProtocols.Tls13 ,true);


                handler.HandleRequest(sslStream, client);
            }
        }
    }
}