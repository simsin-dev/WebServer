using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebServer.classes
{
    public class BanHandler
    {
        private List<string> ips;
        private List<string> agents;

        public BanHandler() 
        {
            LoadBans();
        }

        public bool Banned(string ip,string userAgent)
        {
            foreach (var item in agents)
            {
                if(item.Contains(userAgent))
                {
                    return true;
                }
            }

            foreach (var item in ips)
            {
                if (item.Contains(ip))
                {
                    return true;
                }
            }

            return false;
        }
        public void LoadBans()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "bans.json");
            if (!File.Exists(path))
            {
                ips = new List<string>();
                agents = new List<string>();
                SaveBans();
            }

            string json = File.ReadAllText(path);

            var save = JsonConvert.DeserializeObject<Save>(json);
            ips = save.IPs;
            agents = save.UserAgents;
        }

        public void SaveBans()
        {
            var json = JsonConvert.SerializeObject(new Save(ips,agents));
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "bans.json"), json);
        }

        class Save
        {
            public Save(List<string> iPs, List<string> userAgents)
            {
                IPs = iPs;
                UserAgents = userAgents;
            }

            public List<string> IPs { get; set; } = new List<string>();
            public List<string> UserAgents { get; set; } = new List<string>();

        }
    }
}
