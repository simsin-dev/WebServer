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
            configuration.Add("ip-to-listen-on", "");
            configuration.Add("port", "80");
            configuration.Add("prefix", "");
            configuration.Add("certificate-path", "");
            configuration.Add("404-path", "");
            configuration.Add("403-path", "");

            string config = JsonConvert.SerializeObject(configuration);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), config);
        }

        public string? GetValue(string name)
        {
            return configuration[name];
        }
    }
}
