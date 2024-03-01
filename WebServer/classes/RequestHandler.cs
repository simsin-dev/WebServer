using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public class RequestHandler
    {
        GETHandler GET = new();
        BanHandler banHandler = new();
        public async Task HandleRequest(HttpListenerContext context)
        {
            if (banHandler.Banned(context.Request.RemoteEndPoint.Address.ToString(), context.Request.UserAgent))
            {
                context.Response.Abort();
                return;
            }

            GET.HandleRequest(context, "C:\\Users\\jacul\\Desktop\\simsin.dev");
        }
    }
}
