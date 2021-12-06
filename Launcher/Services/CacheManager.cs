using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    internal class CacheManager
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        public CacheManager(IServiceProvider services)
        {
            _services = services;
            this._appManager = _services.GetRequiredService<AppManager>();
        }

    }
}
