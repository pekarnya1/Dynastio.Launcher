using Dynastio.Api;
using Launcher.Extensions;
using Launcher.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    public class Configuration
    {
        public string Version { get; set; }
        public string GitConfigurationUrl { get; set; }
        public string SiteUrl { get; set; }
        public string DiscorUrl { get; set; }
        public string SourceUrl { get; set; }
        public string DynastioUrl { get; set; }
        public string NightlyUrl { get; set; }
        public string ChangeLog { get; set; }
        public string AppChangeLog { get; set; }

        public List<UserProfile> Profiles { get; set; }

        public List<Game> DynastioGames { get; set; }
    }
}
