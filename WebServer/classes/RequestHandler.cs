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

        bool ssl;

        Configuration config;

        GETHandler GET;

        public RequestHandler(Configuration config)
        {
            this.config = config;
            this.serverPath = config.GetValue("root-path");
            this.message404Path = config.GetValue("404-path");
            this.message403Path = config.GetValue("403-path");

            GET = new GETHandler(serverPath, message404Path, message403Path);

            ssl = !string.IsNullOrEmpty(config.GetValue("certificate-path"));
        }

        public async Task HandleRequest(Stream stream, TcpClient client, int debug)
        {
            try
            {
                DateTime start = DateTime.Now;

                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                bool readRequest = true;


                // later change for detecting cookies and etc...
                string httpRequest = "";
                while (readRequest)
                {
                    var read = reader.ReadLine();
                    httpRequest += read + "\n";

                    if (read.Contains("Accept:"))
                    {
                        readRequest = false;
                    }
                }

                string[] request = httpRequest.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (request[0] == "GET")
                {
                    await GET.HandleRequest(stream, request[1], debug);
                }

                stream.Close();
                client.Close();
                stream.Dispose();
                client.Dispose();

                Console.WriteLine($"{debug}  Handling took: {DateTime.Now - start} For request: {request[1]}");
            }
            catch (Exception ex)
            {
                stream.Close();
                client.Close();
                stream.Dispose();
                client.Dispose();

                Console.WriteLine(ex.ToString());
            }
        }
    }
}
