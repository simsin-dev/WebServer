using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.http
{
    public class HttpRequest
    {
        public RequestMethod Method { get; private set; }

        public string RequestedResource { get; private set; }
        public string RequestedResourcePath { get; private set;}

        public Dictionary<string, string> RequestCookies { get; private set; }

        public string referer { get ; private set; }

        public int contentLength { get ; private set; }

        public string body { get; private set; }


        public async Task<bool> ReadHttpRequest(Stream stream)
        {
            try
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string requestLine = reader.ReadLine();

                string[] request = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                switch (request[0])
                {
                    case "GET":
                        Method = RequestMethod.GET;
                        break;
                    case "POST":
                        Method = RequestMethod.POST;
                        break;
                    default:
                        return false;
                }

                RequestedResource = request[1];

                // base request done

                //cookies
                string line;
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    if (line.Contains("Cookie:"))
                    {
                        string cookieList = line.Replace("Cookie: ", "");

                        string[] cookies = cookieList.Split("; ");

                        foreach (string cookie in cookies)
                        {
                            string[] keValPair = cookie.Split('=');

                            RequestCookies = new();

                            RequestCookies.Add(keValPair[0], keValPair[1]);
                        }
                    }

                    if (line.Contains("Referer: "))
                    {
                        referer = line.Replace("Referer: ", "");
                    }

                    if(line.Contains("Content-Length: "))
                    {
                        contentLength = Convert.ToInt32(line.Replace("Content-Length: ", ""));
                    }
                }

                if (contentLength > 0)
                {
                    char[] buffer = new char[contentLength];
                    reader.Read(buffer, 0, contentLength);
                    body = new string(buffer);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public enum RequestMethod
        {
            GET,
            POST
        }
    }
}
