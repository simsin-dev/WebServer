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

        public override void HandleRequest(NetworkStream stream, string requestedResource)
        {
            stream.FlushAsync();


            var path = GetResourcePath(requestedResource);

            if (File.Exists(path))
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(200));
                stream.Write(headerBytes);

                GetResource(stream, path);
            }
            else
            {
                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(404));
                stream.Write(headerBytes);

                GetResource(stream, message404Path);
            }


            stream.Close();

            throw new NotImplementedException();
        }
    }
}
