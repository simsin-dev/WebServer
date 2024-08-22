using LibGit2Sharp;
using Newtonsoft.Json;
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
        //on my server POST method is only used for login page
        //for more functionality switch request.referer
        public async Task HandleRequest(Stream stream, HttpRequest request)
        {
            if(request.RequestedResource.Contains("api/login"))
            {
                LoginCredentials creds = JsonConvert.DeserializeObject<LoginCredentials>(request.body);
                Console.WriteLine(creds.username+":"+creds.password);

                bool valid = Config.AreCredentialsValid(creds.username, creds.password, out var cook); //passwdMan.AreCredentialsValid(form["username"], form["passwd"]);

                var header = new HttpResponseHeaderBuilder();
                if (valid) 
                {   
                    Cookie cookie = cook.Value;

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(header.StartResponse(303).AddContentType("text/plain").AddContentEncoding("gzip").AddCookie(cookie).Build()));
                    Console.WriteLine("logged in succesfully");
                }
                else 
                {
                    Console.WriteLine("bad credentials");
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(header.StartResponse(401).AddContentType("text/plain").AddContentType("gzip").Build()));
                }
            }
            else if(request.RequestedResource.Contains("api/git"))
            {
                if(request.body.Contains("run-git-pls"))
                {
                    if (Config.IsCookieValid(new Cookie("ADMINSESSION", request.RequestCookies["ADMINSESSION"])))
                    {
                        GitHandler.RunGitPull();
                    }

                    return;
                }
            }
            else
            {
                Console.WriteLine("someone posted :3");
                return;
            }
        }

        struct LoginCredentials
        {
            public string username;
            public string password;

            public LoginCredentials(string username, string password)
            {
                this.username = username;
                this.password = password;
            }

        }
    }
}
