using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebServer.http;
namespace WebServer.classes
{
    public class GETHandler
    {
        public GETHandler()
        {
        }

        public async Task HandleRequest(Stream stream, HttpRequest request)
        {
            var path = GetResourcePath(request.RequestedResource);
            var contentType = GetResourceType(path);

            var header = new HttpResponseHeaderBuilder();
            if(!Config.ConfirmAccess(path, request)) 
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.StartResponse(401).AddContentType(contentType).AddContentEncoding("gzip").Build());
                await stream.WriteAsync(headerBytes);

                await GetResource(stream, Config.GetErrorPath(401));

                return;
            }

            if (File.Exists(path))
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.StartResponse(200).AddContentType(contentType).AddContentEncoding("gzip").Build());
                await stream.WriteAsync(headerBytes);

                await GetResource(stream, path);
            }
            else
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.StartResponse(404).AddContentType(contentType).AddContentEncoding("gzip").Build());
                await stream.WriteAsync(headerBytes);

                if(contentType == "text/html")
                {
                    await GetResource(stream, Config.GetErrorPath(404));
                }
            }
        }


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

            switch (rtype)
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
            if (resource.Contains(Config.GetConfigValue("root-path")))
            {
                return resource;
            }
            else
            {
                var res = resource + GetSuffix(resource);
                res = res.Remove(0, 1);

                var path = Path.Combine(Config.GetConfigValue("root-path"), res);
                return path;
            }
        }

        string GetSuffix(string requested)
        {
            if (requested.EndsWith('/'))
            {
                return "index.html";
            }
            else if (requested.Split('.').Last().ToArray().Length < 5)
            {
                return "";
            }
            else
            {
                return "/index.html";
            }
        }
    }
}
