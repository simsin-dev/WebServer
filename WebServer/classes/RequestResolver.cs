using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public abstract class RequestResolver
    {
        public string serverPath;
        public string message404Path;
        public string message403Path;

        public RequestResolver(string serverPath, string message404Path, string message403Path)
        {
            this.serverPath = serverPath;
            this.message404Path = message404Path;
            this.message403Path = message403Path;
        }

        public abstract void HandleRequest(NetworkStream context, string resource);

        public bool GetResource(NetworkStream output, string requestedResource)
        {
            var res = requestedResource + getSuffix(requestedResource);
            res = res.Remove(0,1);

            var path = Path.Combine(serverPath,res);

            Console.WriteLine(path);


            if (!File.Exists(path))
            {
                return false;
            }
            else 
            {
                using (output)
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        fs.CopyTo(output);
                    }
                }


                return true;
            }
        }

        string getSuffix(string requested)
        {
            if(requested.Contains('.'))
            {
                return "";
            }
            else if(requested == "/")
            {
                return "index.html";
            }
            else
            {
                return ".html";
            }
        }
    }
}
