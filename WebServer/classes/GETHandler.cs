using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace WebServer.classes
{
    public class GETHandler : RequestResolver
    {
        public GETHandler(string serverPath, string message404Path, string message403Path) : base(serverPath, message404Path, message403Path)
        {
            
        }

        public override void HandleRequest(HttpListenerContext context, string serverPath)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentEncoding = Encoding.UTF8;

            GetResource(context.Response.OutputStream, context.Request.RawUrl, serverPath);

            context.Response.Close();
            throw new NotImplementedException();
        }
    }
}
