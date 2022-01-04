using Launcher.Models;
using Launcher.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Managers
{
    public static class PrivateServerManager
    {
        static void ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("output>>" + e.Data);
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                Console.WriteLine("error>>" + e.Data);
            process.BeginErrorReadLine();

            process.WaitForExit();

            Console.WriteLine("ExitCode: {0}", process.ExitCode);
            process.Close();
        }
        public static void CreateBatFile(IGame game, string serverName = "private", string region = "private", string map = "standard", string networkmode = "relay")
        {
            var batFile = $@"bin\games\{game.Id}\run_private.bat";
            if (File.Exists(batFile)) File.Delete(batFile);

            File.WriteAllLines($@"bin\games\{game.Id}\run_private.bat",
                new string[] {
                    "rem cd protos",
                    "rem copy /b auth.proto +,,",
                    "rem cd ..",
                    $".\\dynastio.exe -i 127.0.0.1 -r {region} -n {serverName} -m {map} --networkmode={networkmode}" });
        }
        public static void OpenBatFile(IGame game)
        {
            Process.Start($@"bin\games\{game.Id}\run_private.bat");
        }
        public static void ShowFile(IGame game)
        {
            Process.Start($@"{FileManager.InstallGamesPath}{game.Id}");
        }
    }
}
