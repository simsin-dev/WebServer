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
        CookieManager cookieMan;
        passwdManager passwdMan;
        Configuration config;

        public POSTHandler(HttpResponseHeaderComposer header, CookieManager cookieMan, passwdManager passwdMan, Configuration config)
        {
            this.header = header;
            this.cookieMan = cookieMan;
            this.passwdMan = passwdMan;
            this.config = config;
        }

        //on my server POST method is only used for login page
        //for more functionality switch request.referer
        public async Task HandleRequest(Stream stream, HttpRequest request)
        {
            Console.WriteLine(request.body);

            NameValueCollection form = ParseMultipartFormData(request.body);

            if(request.body.Contains("run-git-pls"))
            {
                if (cookieMan.IsCookieValid("ADMINSESSION", request.RequestCookies["ADMINSESSION"]))
                {
                    RunGitPull();
                }

                return;
            }

            bool valid = passwdMan.AreCredentialsValid(form["username"], form["passwd"]);
            if (valid) 
            {
                Random rand = new();

                var name = "ADMINSESSION";
                var value = rand.Next() + "PASS" + rand.Next();

                cookieMan.AddCookie(name,value, 2); // error

                string redirect = request.referer;
                if(!request.referer.EndsWith('/'))
                {
                    redirect += "/";
                }

                redirect += "success.html";

                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(303, "text/plain", "gzip", $"{name}={value}", redirect)); //temporary solution

                await stream.WriteAsync(headerBytes);
            }
            else 
            {
                Console.WriteLine("bad credentials");
                var headerBytes = Encoding.UTF8.GetBytes(header.GetHeader(401, "text/plain", "gzip"));
                await stream.WriteAsync(headerBytes);
            }
        }

        private void RunGitPull()
        {
            try
            {
                using (var repo = new Repository(config.GetValue("git-repo-dir")))
                {
                    PullOptions options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials
                        {
                            Username = config.GetValue("git-username"),
                            Password = config.GetValue("git-passwd")
                        };

                    Commands.Pull(repo, new Signature(config.GetValue("git-username"), config.GetValue("git-mail"), DateTimeOffset.Now), options);
                    Console.WriteLine("Pull successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
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
