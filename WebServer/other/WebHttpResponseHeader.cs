using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.other
{
    public class WebHttpResponseHeader
    {
        Dictionary<int, string> codes = new();

        public WebHttpResponseHeader()
        {
            codes.Add(200, "OK");
            codes.Add(404, "Not Found");
        }

        public string GetHeader(int code)
        {
            return $"HTTP/1.1 {code} {codes[code]}\r\n\r\n";
        }
    }
}
