using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace WebServer.classes
{
    public class GETHandler : RequestResolver
    {
        public GETHandler(string serverPath, string message404Path, string message403Path) : base(serverPath, message404Path, message403Path)
        {
            
        }

        public async Task HandleRequest(Stream stream, string requestedResource)
        {
            var path = GetResourcePath(requestedResource);
            var contentType = GetResourceType(path);

            if (File.Exists(path))
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(200, contentType, "gzip"));
                await stream.WriteAsync(headerBytes);

                await GetResource(stream, path);
            }
            else
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(404, contentType, "gzip"));
                await stream.WriteAsync(headerBytes);

                if(contentType == "text/html")
                {
                    await GetResource(stream, message404Path);
                }
            }
        }
    }
}
