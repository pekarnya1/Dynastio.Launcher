using Dynastio.Api;
using Launcher.Extensions;
using Launcher.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{

    internal partial class GameManager
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        private readonly DynastClient _dynastClient;
        public GameManager(IServiceProvider services)
        {
            _services = services;
            this._appManager = _services.GetRequiredService<AppManager>();
            this._dynastClient = _services.GetRequiredService<DynastClient>();
        }
        public static void ChnagePlayerNickname(string name)
        {
            RegistryManager.SetValue(RegistryManager.RegDynastio, RegistryManager.KeyNickname, name);
        }
        public static string GetPlayerNickname()
        {
            return RegistryManager.ReadValue(RegistryManager.RegDynastio, RegistryManager.KeyNickname);
        }
        public static void ChnagePlayerSelectedServer(string serverName)
        {
            byte[] selectedServerNameByte = ASCIIEncoding.ASCII.GetBytes(serverName + "x");
            selectedServerNameByte[selectedServerNameByte.Length - 1] = 0;
            RegistryManager.SetValue(RegistryManager.RegDynastio, RegistryManager.KeySelectedServer, selectedServerNameByte);
        }
        public static string GetSelectedServer()
        {
            var value = RegistryManager.ReadValue(RegistryManager.RegDynastio, RegistryManager.KeySelectedServer);
            return value;
        }
        public static Task OpenGameAsync(IGame game)
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = $@"bin\games\{game.Id}\dynast.io.exe";

                if (game.IncludePrivateServers) myProcess.StartInfo.Arguments = "include-custom true";

                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                myProcess.WaitForExit();
            }
            return Task.CompletedTask;
        }
        public void InstallGame(IGame game)
        {
            FileManager.DeleteGameFiles(game, false);
            ZipFile.ExtractToDirectory($@"{FileManager.DownloadGamePath}{game.Id}.dy", $@"{FileManager.InstallGamesPath}{game.Id}");
        }

        public async Task OpenSelectedGameAsync()
        {
            var game = GetSelectedGame();
            await GameManager.OpenGameAsync(game);
        }
        public void UnselectAllGames()
        {
            foreach (var g in _appManager.Configuration.DynastioGames.Where(a => a.IsSelected))
            {
                g.IsSelected = false;
            }
        }
        public IGame GetSelectedGame() => _appManager.Configuration.DynastioGames.Where(a => a.IsSelected).FirstOrDefault();
        public void AddGame(IGame game) => _appManager.Configuration.DynastioGames.Add(game as Game);
        public void RemoveGame(IGame game) => _appManager.Configuration.DynastioGames.Remove(game as Game);

        public async Task CheckGameUpdatesAsync()
        {

            var currentMain = await _dynastClient.Main.GetCurrentVersionAsync();
            var currentNightly = await _dynastClient.Nightly.GetCurrentVersionAsync();

            if (currentMain == null || currentNightly == null) return;
            if (currentMain.version == null || currentNightly.version == null) return;

            foreach (var g in _appManager.Configuration.DynastioGames)
            {
                switch (g.Type)
                {
                    case GameType.Main: UpdateGame(g, currentMain); break;
                    case GameType.Nightly: UpdateGame(g, currentNightly); break;
                }
            }
            void UpdateGame(IGame g, Dynastio.Api.Version version)
            {
                if (g.Version == version.version) g.Update = null;
                else
                {
                    g.Update = new GameUpdate()
                    {
                        DownloadLink = version.url,
                        DownloadSize = ConnectionManager.GetFileSize(version.url),
                        Version = version.version
                    };
                }
            }

        }
    }
}

