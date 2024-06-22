using LibGit2Sharp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mime;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using WebServer.http;

namespace WebServer.classes
{
    public class POSTHandler
    {
        HttpResponseHeaderComposer header;

        public POSTHandler(HttpResponseHeaderComposer header)
        {
            this.header = header;
        }

        //on my server POST method is only used for login page
        //for more functionality switch request.referer
        [Obsolete]
        public async Task HandleRequest(Stream stream, HttpRequest request)
        {
            Console.WriteLine(request.body);

            NameValueCollection form = ParseMultipartFormData(request.body);

            if(request.body.Contains("run-git-pls"))
            {
                if (Config.IsCookieValid(new Cookie("ADMINSESSION", request.RequestCookies["ADMINSESSION"])))
                {
                    GitHandler.RunGitPull();
                }

                return;
            }

            bool valid = Config.AreCredentialsValid(form["username"], form["passwd"], out var cook); //passwdMan.AreCredentialsValid(form["username"], form["passwd"]);
            Cookie cookie = cook.Value;

            if (valid) 
            {
                Random rand = new();

                //var name = "ADMINSESSION";
                //var value = rand.Next() + ""; //also temporary

                //.AddCookie(name,value, 2); // error
                //Console.WriteLine("cookie added");

                string redirect = request.referer;
                if(!request.referer.EndsWith('/'))
                {
                    redirect += "/";
                }

                redirect += "success.html";

                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(303, "text/plain", "gzip", $"{cookie.Name}={cookie.Value}")); //temporary solution

                await stream.WriteAsync(headerBytes);
                Console.WriteLine(header.GetHeader(303, "text/plain", "gzip", $"{cookie.Name}={cookie.Value}", redirect));
            }
            else 
            {
                Console.WriteLine("bad credentials");
                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(401, "text/plain", "gzip"));
                await stream.WriteAsync(headerBytes);
            }
        }

        NameValueCollection ParseMultipartFormData(string formDataString)
        {
            NameValueCollection formData = new NameValueCollection();

            string[] parts = Regex.Split(formDataString, @"\r\n--");

            foreach (string part in parts)
            {
                Match match = Regex.Match(part, @"name=""(.*?)""[\r\n]*[\r\n]+(.+)[\r\n]*");

                if (match.Success)
                {
                    string name = match.Groups[1].Value;
                    string value = match.Groups[2].Value;

                    formData.Add(name, value);
                }
            }
            return formData;
        }
    }
}
