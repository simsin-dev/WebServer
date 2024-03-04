using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebServer.other;

namespace WebServer.classes
{
    public abstract class RequestResolver
    {
        public string serverPath;
        public string message404Path;
        public string message403Path;

        public WebHttpResponseHeader header = new();
        public RequestResolver(string serverPath, string message404Path, string message403Path)
        {
            this.serverPath = serverPath;
            this.message404Path = message404Path;
            this.message403Path = message403Path;
        }

        public async Task HandleRequest(Stream context, string resource)
        {}

        public async Task GetResource(Stream output, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                await fs.CopyToAsync(output).ConfigureAwait(false);
            }
        }

        public string GetResourcePath(string resource)
        {
            if (resource.Contains(serverPath))
            {
                return resource;
            }
            else
            {
                var res = resource + GetSuffix(resource);
                res = res.Remove(0, 1);

                var path = Path.Combine(serverPath, res);
                return path;
            }
        }

        string GetSuffix(string requested)
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
