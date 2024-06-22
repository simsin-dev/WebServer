using System.Net;
using System;
using WebServer.classes;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Globalization;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Reflection.Metadata;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Starting server!!");

        RequestHandler handler = new();
        await handler.Initialize();

        TcpListener listener = new TcpListener(IPAddress.Parse(Config.GetConfigValue("ip-to-listen-on")), Convert.ToInt32(Config.GetConfigValue("port")));
        listener.Start();

        Console.WriteLine("Listening...");
        await HttpListen(listener, handler);
    }

    private static async Task HttpListen(TcpListener listener, RequestHandler handler)
    {
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false); //async later
            Console.WriteLine(client.Client.RemoteEndPoint.ToString());

            handler.HandleRequest(client);
        }
    }
}