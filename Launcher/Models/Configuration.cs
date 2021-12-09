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
        public List<UserProfile> Profiles { get; set; }

        public List<Game> DynastioGames { get; set; }
    }
}
