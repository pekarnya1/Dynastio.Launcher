using Launcher.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Models
{
    public interface IGame
    {
        string Id { get; set; }
        GameType Type { get; set; }
        string Tittle { get; set; }
        string Description { get; set; }
        string Details { get; }
        string Version { get; set; }
        string ImageDirectory { get; set; }
        bool IsUpdateAble { get; set; }
        bool IsSelected { get; set; }
        bool IncludePrivateServers { get; set; }

        GameUpdate Update { get; set; }
        DateTime InstalledAt { get; set; }
    }
    public interface IGameUpdate
    {
        string DownloadLink { get; set; }
        long DownloadSize { get; set; }
        string Version { get; set; }
    }
    public enum GameType
    {
        Main=0,
        Nightly=1,
    }
    public class Game : IGame
    {
        public string Id { get; set; }
        public GameType Type { get; set; }
        public string Tittle { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string ImageDirectory { get; set; }
        public bool IsUpdateAble { get; set; }
        public GameUpdate Update { get; set; }
        public DateTime InstalledAt { get; set; }
        public bool IsSelected { get; set; }
        public bool IncludePrivateServers { get; set; }

        public string Details
        {
            get
            {
                if (Update == null)
                {
                    return $"Version {Version} Installed {InstalledAt.TimeWordagelizition()} ago";
                }
                return $"Download Size {Update.DownloadSize.SizeSuffix()}";
            }
        }

    }
    public class GameUpdate : IGameUpdate
    {
        public string Version { get; set; }
        public string DownloadLink { get; set; }
        public long DownloadSize { get; set; }

    }


}
