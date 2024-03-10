using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public class passwdManager
    {
        Dictionary<string, string> credentials = new();

        public passwdManager() 
        {
            LoadConfig();
        }

        public bool AreCredentialsValid(string username, string password)
        {
            foreach(var credential in credentials)
            {
                if(credential.Key.Equals(username)  && credential.Value.Equals(password))
                {
                    return true;
                }
            }

            return false;
        }


        void LoadConfig()
        {
            try
            {
                string cred = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "credentials.json"));

                credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(cred);

            }
            catch (FileNotFoundException e)
            {
                GenerateConfig();
            }
        }

        private void GenerateConfig()
        {
            credentials.Add("admin", "12345");

            string cred = JsonConvert.SerializeObject(credentials);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "credentials.json"), cred);
        }
    }
}
