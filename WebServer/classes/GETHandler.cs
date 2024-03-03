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
            string httpResponse = $"HTTP/1.1 200 OK\r\n\r\n";
            byte[] responseBytes = Encoding.UTF8.GetBytes(httpResponse);

            stream.Write(responseBytes, 0, responseBytes.Length);
            GetResource(stream, requestedResource);
            stream.FlushAsync();
            stream.Close();

            //GetResource(context.Response.OutputStream, );

            throw new NotImplementedException();
        }
    }
}
