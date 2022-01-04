using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynastio.Api;
using Launcher.Extensions;
using Launcher.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;

namespace Launcher.Services
{
    internal class AppManager
    {
        private readonly IServiceProvider _services;
        private readonly FileManager _fileManager;
        public AppManager(IServiceProvider services)
        {
            this._services = services;
            this._fileManager = services.GetRequiredService<FileManager>();
        }
        public Configuration Configuration { get; set; }
        public void InitializeAsync()
        {
            Configuration = FileManager.GetConfiguration();
        }
        public static void CreateDirectoriesIfNotExist()
        {
            if (!Directory.Exists($@"bin\downloads\"))
            {
                Directory.CreateDirectory($@"bin\downloads\");
            }
            if (!Directory.Exists($@"bin\games\"))
            {
                Directory.CreateDirectory($@"bin\games\");
            }
            if (!Directory.Exists($@"bin\profiles\"))
            {
                Directory.CreateDirectory($@"bin\profiles\");
            }
        }
    }
}
