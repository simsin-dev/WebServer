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
        Configuration configObj = new();
        configObj.LoadConfig();

        Console.WriteLine("Starting server!!");

        RequestHandler handler = new(configObj);

        TcpListener listener = new TcpListener(IPAddress.Parse(configObj.GetValue("ip-to-listen-on")), Convert.ToInt32(configObj.GetValue("port")));
        listener.Start();

        await HttpListen(listener, handler);
    }

    private static async Task HttpListen(TcpListener listener, RequestHandler handler)
    {
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false); //async later

            handler.HandleRequest(client);
        }
    }
}