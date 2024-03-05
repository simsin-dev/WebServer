using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebServer.http;
using static System.Net.Mime.MediaTypeNames;

namespace WebServer.classes
{
    public abstract class RequestResolver
    {
        public string serverPath;
        public string message404Path;
        public string message403Path;

        public HttpResponseHeaderComposer header = new();
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
                using (GZipStream gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    await fs.CopyToAsync(gzip).ConfigureAwait(false);
                }
            }
        }

        public string GetResourceType(string path) 
        {
            string rtype = path.Split('.').Last();

            switch(rtype) 
            {
                case "html":
                    return "text/html";

                case "css":
                    return "text/css";

                case "js":
                    return "text/javascript";

                case "png":
                    return "image/png";

                case "jpg":
                case "jpeg":
                    return "image/jpeg";

                case "mpeg":
                case "mpg":
                    return "video/mpeg";

                case "mp4":
                    return "video/mp4";

                case "webm":
                    return "video/webm";

                case "ogv":
                    return "video/ogg";

                case "gif":
                    return "image/gif";

                case "mp3":
                case "mpga":
                case "mpeg3":
                    return "audio/mpeg";

                case "ogg":
                    return "audio/ogg";

                case "json":
                    return "application/json";

                case "zip":
                    return "application/zip";

                case "pdf":
                    return "application/pdf";

                case "ico":
                    return "image/x-icon";

                default:
                    return "text/plain";
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
