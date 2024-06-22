using LibGit2Sharp;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.http;

namespace WebServer.classes
{
    public static class Config
    {
        //configurations
        static Dictionary<string, string> basicConfiguration = new();
        static Dictionary<string,string> gitConfiguration = new();
        static Dictionary<int,string> errorPaths = new();
        static Dictionary<string,(string,Cookie)> credentials = new();
        static Dictionary<string,string> resstrictedAccesPaths = new();

        //cookies
        static List<Cookie> Cookies;
        static Dictionary<Cookie, int> sessionCookies = new();


        static Random rand = new Random();

        static int sessionLifetime;

        public static DateTime certificateExpirationDate;

        static Config() 
        {
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.conf");
            if(File.Exists(configPath))
            {
                ReadConfig(configPath);

                sessionLifetime = Convert.ToInt32(basicConfiguration.GetValueOrDefault("session-lifetime"));

                string certExpPath = Path.Combine(Directory.GetCurrentDirectory(), ".certDate");
                bool certExpDate = File.Exists(certExpPath);
                if(certExpDate)
                {
                    string certDate = File.ReadAllText(certExpPath);

                    certificateExpirationDate = DateTime.Parse(certDate);
                }
                else
                {
                    certificateExpirationDate = DateTime.Now - TimeSpan.FromDays(6969);
                }



                Console.WriteLine("Loaded config");
            }
            else
            {
                CreateConfig(configPath);
                throw new Exception("Config file created - quitting!!");
            }

            CookieSweeper().Start();
        }

        static void ReadConfig(string configPath)
        {
            using (StreamReader sr = new StreamReader(configPath)) 
            {
                while (sr.EndOfStream)
                {
                    int read = 0;
                    string line = sr.ReadLine();
                    if (line.StartsWith('#'))
                    {
                        continue;
                    }
                    else if (line.StartsWith("---"))
                    {
                        read++;
                    }
                    else
                    {
                        string[] split;
                        switch (read)
                        {
                            case 0:
                                split = line.Split("::");
                                basicConfiguration.Add(split[0], split[1]);
                                break;
                            case 1:
                                split = line.Split("::");
                                gitConfiguration.Add(split[0], split[1]);
                                break;
                            case 2:
                                split = line.Split("::");
                                errorPaths.Add(Convert.ToInt32(split[0]), split[1]);
                                break;
                            case 3:
                                split = line.Split("::");
                                credentials.Add(split[0], (split[1],new Cookie(split[2], split[3])));
                                if (!split[3].Contains("RANDOM"))
                                {
                                    Cookies.Add(new Cookie(split[2], split[3]));
                                }
                                break;
                            case 4:
                                split = line.Split("::");
                                resstrictedAccesPaths.Add(split[0], split[1]);
                                break;
                        }
                    }
                }
            }
        }

        

        static void CreateConfig(string configPath)
        {
            //populate basic config

            basicConfiguration.Add("root-path", "");
            basicConfiguration.Add("ip-to-listen-on", "127.0.0.1");
            basicConfiguration.Add("port", "80");
            basicConfiguration.Add("ssl", "false");
            basicConfiguration.Add("domain", "");
            basicConfiguration.Add("certificate-passphrase", "");
            basicConfiguration.Add("session-lifetime", "5");

            gitConfiguration.Add("git-repo-dir", "");
            gitConfiguration.Add("git-username", "");
            gitConfiguration.Add("git-passwd", "");
            gitConfiguration.Add("git-mail", "");

            errorPaths.Add(404, "");
            errorPaths.Add(401, "");
            errorPaths.Add(403, "");
            errorPaths.Add(500, "");

            //create the file
            using (StreamWriter writer = new StreamWriter(configPath)) 
            {
                // YES I KNOW I CAN USE \n BUT IT LOOKS NICER THAT WAY

                writer.WriteLine("# Configuratin file for simsin's http server");
                writer.WriteLine("# DO NOT ADD/REMOVE LINES STARTING WITH '---' !!!!!");
                writer.WriteLine("# basic config - format: [key]::[value] DO NOT CHEANGE THE KEY!!!");

                foreach(var keyvalpair in basicConfiguration)
                {
                    writer.WriteLine($"{keyvalpair.Key}::{keyvalpair.Value}");
                }

                writer.WriteLine("---git---");
                writer.WriteLine("# git configuration, not required, format: [key]::[value] DO NOT CHEANGE THE KEY!!!");

                foreach (var keyvalpair in gitConfiguration)
                {
                    writer.WriteLine($"{keyvalpair.Key}::{keyvalpair.Value}");
                }

                writer.WriteLine("---errors---");
                writer.WriteLine("# http error filepaths config, - format: [int error]::[filepath]");
                foreach (var keyvalpair in errorPaths)
                {
                    writer.WriteLine($"{keyvalpair.Key}::{keyvalpair.Value}");
                }

                writer.WriteLine("---credentials---");
                writer.WriteLine("# Credential's for login pages");
                writer.WriteLine("# Succesful login sends over a cookie required to acces pages");
                writer.WriteLine("# Adding a 'RANDOM' keyword, makes the cookie session based and changes the keywaord to a random int");
                writer.WriteLine("# Format - [username]::[password]::[cookie]");
                writer.WriteLine("admin::admin::adminRANDOM");

                writer.WriteLine("---restricted-paths---");
                writer.WriteLine("# Paths restricted by cookies");
                writer.WriteLine("# Use the same cookies as in credentials for login restricted pages");
                writer.WriteLine("# 'RANDOM' keyword needs to also be written to make the access session based");
                writer.WriteLine("# Format - [path]::[cookie]");
            }
        }



        public static string? GetConfigValue(string key)
        {
            return basicConfiguration.GetValueOrDefault(key);
        }


        public static string? GetGitConfigValue(string key)
        {
            return gitConfiguration.GetValueOrDefault(key);
        }


        public static string? GetErrorPath(int key)
        {
            return errorPaths.GetValueOrDefault(key);
        }

        public static bool AreCredentialsValid(string username, string password, out Cookie? cookie)
        {
            bool validUsername = credentials.TryGetValue(username, out var value);

            if(validUsername && value.Item1 == password)
            {
                cookie = getCookie(value.Item2);
            }
            else
            {
                cookie = null;
                validUsername = false;
            }

            return validUsername;
        }

        public static bool IsPathProtected(string path)
        {
            return resstrictedAccesPaths.ContainsKey(path);
        }

        public static bool IsCookieValid(Cookie cookie)
        {
            bool staticCookie = Cookies.Contains(cookie);
            if(!staticCookie)
            {
                bool sessionCookie = sessionCookies.ContainsKey(cookie);
                return sessionCookie;
            }
            return staticCookie;
        }

        public static bool HasAccess(string path, string cookieName)
        {
            if(resstrictedAccesPaths[path] == cookieName)
            {
                return true;
            }

            return false;
        }

        public static bool ConfirmAccess(string path, HttpRequest request)
        {
            if(!IsPathProtected(path)) 
            {
                return true;
            }

            if(request.RequestCookies.Count <= 0)
            {
                return false;
            }

            foreach(var cookie in request.RequestCookies) 
            {
                if(IsCookieValid(new Cookie(cookie.Key, cookie.Value)) && HasAccess(path, cookie.Key))
                {
                    return true;
                }
            }

            return false;

        }



        // cookies stuff:
        static Cookie getCookie(Cookie initialCookie)
        {
            if(!initialCookie.Value.Contains("RANDOM"))
            {
                return initialCookie;
            }

            var sessionCookie = new Cookie(initialCookie.Name, initialCookie.Value.Replace("RANDOM", rand.Next().ToString()));
            sessionCookies.Add(sessionCookie, sessionLifetime);

            return sessionCookie;
        }

        static Task CookieSweeper()
        {
            while (true)
            {
                Task.Delay(60000);

                foreach (var key in sessionCookies.Keys)
                {
                    sessionCookies[key] -= 1;

                    if (sessionCookies[key] == 0)
                    {
                        sessionCookies.Remove(key);
                    }
                }
            }
        }


    }
}
