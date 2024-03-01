using System.Net;
using System;
using WebServer.classes;

class Program
{
    const string url = "http://192.168.1.26:5050/";
    const string loopback = "http://127.0.0.1:8080/";
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting server!!");

        RequestHandler handler = new();

        HttpListener listener = new HttpListener();

        //listener.TimeoutManager.HeaderWait = TimeSpan.FromMilliseconds(30);
        //listener.Prefixes.Add(url);
        listener.Prefixes.Add(loopback);
        listener.Prefixes.Add(url);

        listener.Start();

        while (true)
        {
            var request = listener.GetContext();
            handler.HandleRequest(request);
        }
    }
}