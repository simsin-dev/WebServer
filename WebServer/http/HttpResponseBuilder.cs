using System.Diagnostics.CodeAnalysis;
using WebServer.classes;

namespace WebServer;

public class HttpResponseHeaderBuilder
{
    string header;

    Dictionary<int,string> headerCodes = new();

        public HttpResponseHeaderBuilder() 
        {
            //add more later
            headerCodes.Add(200, "OK");
            headerCodes.Add(404, "Not Found");
            headerCodes.Add(303, "See Other");
            headerCodes.Add(401, "Unauthorized");
        }

    public HttpResponseHeaderBuilder StartResponse(int responseStatus)
    {
        header = $"HTTP/1.1 {responseStatus} {headerCodes[responseStatus]}\r\n";
        return this;
    }

    public HttpResponseHeaderBuilder AddContentType(string contentType)
    {
        header += $"Content-Type: {contentType}\r\n";
        return this;
    }

    public HttpResponseHeaderBuilder AddContentEncoding(string encoding)
    {
        header += $"Content-Encoding: {encoding}\r\n";
        return this;
    }

    public HttpResponseHeaderBuilder AddCookie(Cookie cookie)
    {
        header += $"Set-Cookie: {cookie.Name}={cookie.Value}\r\n";
        return this;
    }

    public HttpResponseHeaderBuilder AddCookies(Cookie[] cookies)
    {
        foreach(var cookie in cookies)
        {
            header += $"Set-Cookie: {cookie.Name}={cookie.Value}\r\n";
        }

        return this;
    }

    public HttpResponseHeaderBuilder AddRedirectTo(string redirectTo)
    {
        header += $"Location: {redirectTo}\r\n";
        return this;
    }

    public string Build()
    {
        return header + "\r\n";
    }

}
