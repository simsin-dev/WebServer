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
        public string serverPath;
        public string message404Path;
        public string message403Path;

        GETHandler GET;
        BanHandler banHandler = new();

        public RequestHandler(string serverPath, string message404Path, string message403Path)
        {
            this.serverPath = serverPath;
            this.message404Path = message404Path;
            this.message403Path = message403Path;

            GET = new GETHandler(serverPath, message404Path, message403Path);
        }

        public async Task HandleRequest(HttpListenerContext context)
        {
            /*if (banHandler.Banned(context.Request.RemoteEndPoint.Address.ToString(), context.Request.UserAgent))
            {
                context.Response.Abort();
                return;
            }*/

            GET.HandleRequest(context, "C:\\Users\\jacul\\Desktop\\simsin.dev");
        }
    }
}
