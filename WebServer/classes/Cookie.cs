using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.classes
{
    public struct Cookie
    {

        public string Name;
        public string Value;

        public Cookie(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
