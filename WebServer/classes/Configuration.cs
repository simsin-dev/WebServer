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
        List<ConfigOption> configurtion = new();

        public void LoadConfig()
        {
            try
            {
                string config = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"));

                configurtion = JsonConvert.DeserializeObject<List<ConfigOption>>(config);

            }
            catch (FileNotFoundException e)
            {
                GenerateConfig();
            }
        }

        private void GenerateConfig()
        {
            configurtion.Add(new ConfigOption("root-path",""));
            configurtion.Add(new ConfigOption("prefix", ""));
            configurtion.Add(new ConfigOption("certificate-path", ""));
            configurtion.Add(new ConfigOption("404-path", ""));
            configurtion.Add(new ConfigOption("403-path", ""));

            string config = JsonConvert.SerializeObject(configurtion);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), config);
        }

        public string? GetValue(string name)
        {
            foreach (ConfigOption option in configurtion) 
            {
                if(option.name == name)
                {
                    return option.value;
                }
            }

            return null;
        }

        public class ConfigOption
        {
            public string name;
            public string value;

            public ConfigOption(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
        }
    }
}
