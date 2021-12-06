using Launcher.Extensions;
using Launcher.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    internal class FileManager
    {
        private const string _configPath = "config.json";
        public const string InstallGamesPath = @"bin\games\";
        public const string DownloadGamePath = @"bin\downloads\";
        public const string ProfilesPath = @"bin\profiles\";
        public static Configuration GetConfiguration()
        {
            var file = File.ReadAllText(_configPath.ResourcesPath());
            return JsonConvert.DeserializeObject<Configuration>(file);
        }
        public static void SaveConfiguration(Configuration configuration)
        {
            var content = JsonConvert.SerializeObject(configuration);
            File.WriteAllText(_configPath.ResourcesPath(), content);
        }
        public static void DeleteGameFiles(IGame game, bool DownloadedFiles = true, bool InstalledFiles = true)
        {
            try
            {
                if (InstalledFiles) DeleteInstalledGameFiles(game);
                if (DownloadedFiles) DeleteDownloadGameFiles(game);
            }
            catch { }
        }
        public static void DeleteDownloadGameFiles(IGame game) => Directory.Delete($@"{DownloadGamePath}{game.Id}\", true);

        public static void DeleteInstalledGameFiles(IGame game) => Directory.Delete($@"{InstallGamesPath}{game.Id}\", true);
        public static void DeleteProfile(UserProfile profile)
        {
            if (File.Exists(FileManager.ProfilesPath + profile.Id + ".p")) File.Delete(FileManager.ProfilesPath + profile.Id + ".p");
        }
    }
}
