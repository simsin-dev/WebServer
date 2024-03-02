using System.Net;
using System;
using WebServer.classes;
using System.Security.Cryptography.X509Certificates;

class Program
{
    public static void Main(string[] args)
    {
        Configuration configObj = new();
        configObj.LoadConfig();

        Console.WriteLine("Starting server!!");

        RequestHandler handler = new(configObj.GetValue("root-path"), configObj.GetValue("404-path"), configObj.GetValue("403-path"));


        HttpListener listener = new HttpListener();

        //listener.TimeoutManager.HeaderWait = TimeSpan.FromMilliseconds(30);
        listener.Prefixes.Add(configObj.GetValue("prefix"));


        listener.Start();

        while (true)
        {
            //implement async handling later

            var request = listener.GetContext();
            handler.HandleRequest(request);
        }
    }
}