using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using WebServer.http;

namespace WebServer.classes
{
    public class CookieManager
    {
        Dictionary<string,string> protectedResources = new(); //files/directories protected by cookies  -------- resource:cookie-name

        Dictionary<string,(string, int)> cookies = new();

        public CookieManager() 
        {
            LoadProtectedResources();
        }

        public bool IsCookieValid(string name, string value)
        {
            for (int i = 0; i < cookies.Keys.Count; i++)
            {
                if (cookies.Keys.ElementAt(i) == name && cookies.Values.ElementAt(i).Item1 == value)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddCookie(string name, string value, int Lifetime)
        {
            cookies.Add(name, (value, Lifetime));

            _ = Task.Run(CookieSweeper);
        }

        private async Task CookieSweeper()
        {
            while(cookies.Count > 0)
            {
                Thread.Sleep(60000); //wait 1m

                var keys = new List<string>(cookies.Keys);

                foreach (var key in keys)
                {
                    var value = cookies[key];

                    cookies[key] = (value.Item1, value.Item2 - 1);

                    Console.WriteLine(cookies[key].Item2);

                    if (value.Item2 <= 0)
                    {
                        cookies.Remove(key);
                    }
                }
            }
        }

        public bool ConfirmAccess(string path, HttpRequest request)
        {
            if (IsProtected(path))
            {
                if(request.RequestCookies == null)
                {
                    return false;
                }

                if (!IsCookieValid("ADMINSESSION", request.RequestCookies["ADMINSESSION"]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsProtected(string resource)
        {
            foreach (var protRes in protectedResources)
            {
                if (resource.Replace('/','\\').Contains(protRes.Key))
                {
                    return true;
                }
            }

            return false;
        }


        public void LoadProtectedResources()
        {
            try
            {
                string res = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "protectedResources.json"));

                protectedResources = JsonConvert.DeserializeObject<Dictionary<string,string>>(res);

            }
            catch (FileNotFoundException e)
            {
                GenerateFile();
            }
        }

        private void GenerateFile()
        {
            protectedResources.Add("meow", "woofwoof");
            string config = JsonConvert.SerializeObject(protectedResources);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "protectedResources.json"), config);
        }
    }
}
