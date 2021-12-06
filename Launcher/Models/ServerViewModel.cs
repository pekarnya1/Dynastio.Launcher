using Dynastio.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Models
{
    public class ServerViewModel : Server
    {
        public bool IsSelectAble { get; set; }
        public int Index { get; set; }
        public string Latency { get; set; }
        public string DisplayMember
        {
            get
            {
                var x = "";
                string row = Index.ToString();
                if (Index < 10) row = $"0{Index}";

                string serverName = ServerName.PadRight(20).Substring(0, 20);

                var clientCount = $"{ClientCount}";
                if (ClientCount < 10) clientCount = $"0{ClientCount}";

                x = row + " " + serverName.Replace(" ", "") + " (" + clientCount + " players)";

                return x;
            }
        }
        public string Category
        {
            get
            {
                return "  " + Region;
            }
        }
        public static List<ServerViewModel> ConvertForSelectMenu(List<Server> servers)
        {
            var text = JsonConvert.SerializeObject(servers);
            var _servers = JsonConvert.DeserializeObject<List<ServerViewModel>>(text);



            var list = new List<ServerViewModel>();
            var index = 0;
            foreach (var x in _servers.OrderBy(a => a.Region).Where(a => !a.IsPrivate && a.Region != "private"))
            {
                index++;
                x.Index = index;
                list.Add(x);
            }
            index = 0;
            foreach (var x in _servers.OrderBy(a => a.Region).Where(a => !a.IsPrivate && a.Region == "private"))
            {
                x.Region = "Special Servers";
                index++;
                x.Index = index;
                list.Add(x);
            }
            index = 0;
            foreach (var x in _servers.OrderBy(a => a.Region).Where(a => a.IsPrivate))
            {
                index++;
                x.Region = "Private Servers";
                x.Index = index;
                list.Add(x);
            }
            return list;
        }


    }
}
