using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionOnline.Settings
{
    public class Configurations : IConfigurations
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ClientToken { get; set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
