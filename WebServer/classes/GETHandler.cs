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
