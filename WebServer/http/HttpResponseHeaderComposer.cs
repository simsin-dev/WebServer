using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.http
{
    public class HttpResponseHeaderComposer
    {
        Dictionary<int,string> headerCodes = new();

        public HttpResponseHeaderComposer() 
        {
            //add more later
            headerCodes.Add(200, "OK");
            headerCodes.Add(404, "Not Found");
        }

        public string GetHeader(int code)
        {
            return $"HTTP/1.1 {code} {headerCodes[code]}\r\n\r\n";
        }

        public string GetHeader(int code, string contentType)
        {
            return $"HTTP/1.1 {code} {headerCodes[code]}\r\nContent-Type: {contentType}\r\n\r\n";
        }

        public string GetHeader(int code, string contentType, string encoding)
        {
            return $"HTTP/1.1 {code} {headerCodes[code]}\r\nContent-Type: {contentType}\r\nContent-Encoding: {encoding}\r\n\r\n";
        }
    }
}
