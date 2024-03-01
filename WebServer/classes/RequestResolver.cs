using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public abstract class RequestResolver
    {
        public abstract void HandleRequest(HttpListenerContext context, string serverPath);

        public bool GetResource(Stream output, string requestedResource, string serverPath)
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
