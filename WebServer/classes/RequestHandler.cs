using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public class RequestHandler
    {
        public string serverPath;
        public string message404Path;
        public string message403Path;

        GETHandler GET;

        public RequestHandler(string serverPath, string message404Path, string message403Path)
        {
            this.serverPath = serverPath;
            this.message404Path = message404Path;
            this.message403Path = message403Path;

            GET = new GETHandler(serverPath, message404Path, message403Path);
        }

        public async Task HandleRequest(Stream stream, TcpClient client)
        {
            DateTime start = DateTime.Now;

            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string httpRequest = reader.ReadLine();

            string[] request = httpRequest.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (request[0] == "GET")
            {
                await GET.HandleRequest(stream, request[1]);
            }

            stream.Close();

            client.Close();

            Console.WriteLine($"Handling took: {DateTime.Now - start} For request: {httpRequest}");
        }
    }
}
