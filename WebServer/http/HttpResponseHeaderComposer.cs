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
            headerCodes.Add(303, "See Other");
            headerCodes.Add(401, "Unauthorized");
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

        public string GetHeader(int code, string contentType, string encoding, string cookie)
        {
            string header = $"HTTP/1.1 {code} {headerCodes[code]}\r\nContent-Type: {contentType}\r\nContent-Encoding: {encoding}\r\n";

            header += $"Set-Cookie: {cookie}\r\n";

            return header + "\r\n";
        }

        public string GetHeader(int code, string contentType, string encoding, string[] cookies)
        {
            string header = $"HTTP/1.1 {code} {headerCodes[code]}\r\nContent-Type: {contentType}\r\nContent-Encoding: {encoding}\r\n";

            for (int i = 0; i < cookies.Length; i++)
            {
                header += $"Set-Cookie: {cookies[i]}\r\n";
            }

            return header + "\r\n";
        }

        public string GetHeader(int code, string contentType, string encoding, string cookie, string redirectTo)
        {
            string header = $"HTTP/1.1 {code} {headerCodes[code]}\r\nContent-Type: {contentType}\r\nContent-Encoding: {encoding}\r\n";

            header += $"Set-Cookie: {cookie}\r\n";

            header += $"Location: {redirectTo}";
            return header + "\r\n";
        }

        public string GetHeader(int code, string contentType, string encoding, string[] cookies, string redirectTo)
        {
            string header = $"HTTP/1.1 {code} {headerCodes[code]}\r\nContent-Type: {contentType}\r\nContent-Encoding: {encoding}\r\n";

            for (int i = 0; i < cookies.Length; i++)
            {
                header += $"Set-Cookie: {cookies[i]}\r\n";
            }

            header += $"Location: {redirectTo}";
            return header + "\r\n";
        }
    }
}
