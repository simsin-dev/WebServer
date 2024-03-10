using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public class Configuration
    {
        Dictionary<string,string> configuration = new();

        public void LoadConfig()
        {
            try
            {
                string config = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"));

                configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(config);

            }
            catch (FileNotFoundException e)
            {
                GenerateConfig();
            }
        }

        private void GenerateConfig()
        {
            configuration.Add("root-path", "");
            configuration.Add("ip-to-listen-on", "127.0.0.1");
            configuration.Add("port", "80");
            configuration.Add("certificate-path", "");
            configuration.Add("certificate-passphrase", "");
            configuration.Add("404-path", "");
            configuration.Add("401-path", "");
            configuration.Add("git-repo-dir", "");
            configuration.Add("git-username","");
            configuration.Add("git-passwd","");
            configuration.Add("git-mail", "");

            string config = JsonConvert.SerializeObject(configuration);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), config);
        }

        public string? GetValue(string name)
        {
            return configuration[name];
        }
    }
}
