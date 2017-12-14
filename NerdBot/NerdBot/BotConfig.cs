using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot
{
    public class BotConfig
    {
        public string BotName { get; set; }
        public string HostUrl { get; set; }
        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }

        public List<BotRoute> BotRoutes { get; set; }

        public BotConfig()
        {
            this.BotRoutes = new List<BotRoute>();
        }
    }

    public class BotRoute
    {
        public string SecretToken { get; set; }
        public string BotId { get; set; }
    }
}
