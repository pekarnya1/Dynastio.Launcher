using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Threading;

namespace Launcher.Services
{
    public class RpcManager
    {
        public const long ApplicationId = 915189019330113546;
        private readonly IServiceProvider _services;
        public RpcManager(IServiceProvider services)
        {
            this._services = services;
        }

        Discord.Discord _discord { get; set; }
        public void Intialize()
        {
            try
            {
                _discord = new Discord.Discord(RpcManager.ApplicationId, (UInt64)CreateFlags.NoRequireDiscord);
                RunCallbacks();
            }
            catch
            {

            }
        }
        public Task RunCallbacks()
        {
            _ = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        _discord.RunCallbacks();
                        Thread.Sleep(1000 / 60);
                    }
                }
                finally { _discord.Dispose(); }
            });
            return Task.CompletedTask;
        }
        public void UpdateActivityToLauncher(string details, string state)
        {
            try
            {
            if (_discord == null) Intialize();

                var activityManager = _discord.GetActivityManager();
                activityManager.UpdateActivity(new Activity()
                {
                    ApplicationId = RpcManager.ApplicationId,
                    Details = details,
                    State = state,
                    Assets = new ActivityAssets()
                    {
                        LargeImage = "launcher",
                        LargeText = "Dynastio Launcher",
                        SmallImage = "dynastio",
                        SmallText = "dynastio"
                    },
                    Type = ActivityType.Playing,
                    Timestamps = new ActivityTimestamps()
                    {
                        Start = DateTimeOffset.Now.ToUnixTimeSeconds()
                    },
                }, result => { });
            }
            catch
            {

            }
        }
        public void UpdateActivityToDynastio(string details, string state)
        {
            try
            {
            if (_discord == null) Intialize();

                var activityManager = _discord.GetActivityManager();
                activityManager.UpdateActivity(new Activity()
                {
                    ApplicationId = RpcManager.ApplicationId,
                    Details = details,
                    State = state,
                    Assets = new ActivityAssets()
                    {
                        LargeImage = "dynastio",
                        LargeText = "Dynast.io",
                        SmallImage = "launcher",
                        SmallText = "Dynastio Launcher"
                    },
                    Timestamps = new ActivityTimestamps()
                    {
                        Start = DateTimeOffset.Now.ToUnixTimeSeconds()
                    },
                    Type = ActivityType.Playing
                }, result => {});
            }
            catch
            {

            }
        }
    }
}
