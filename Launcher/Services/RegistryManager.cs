using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    public class RegistryManager
    {
        public const string RegWhaleboxStudio = @"HKEY_CURRENT_USER\SOFTWARE\Whalebox Studio";
        public const string RegDynastio = @"SOFTWARE\Whalebox Studio\Dynast.io";
        public const string KeyNickname = "nickname_h1560818637";
        public const string KeySelectedServer = "selected_server_h1540196278";
        public static void Export(string strKey, string filepath)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "reg";
                proc.StartInfo.Arguments = "export \"" + strKey + "\" " + filepath;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }

        }
        public static void Import(string path)
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "regedit.exe";
                proc.StartInfo.Arguments = "/s \"" + path + "\"";
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.WaitForExit();
            }
        }
        public static void SetValue(string subkey, string key, object value)
        {
            using (RegistryKey _key = Registry.CurrentUser.CreateSubKey(subkey))
            {
                _key.SetValue(key, value);
                _key.Close();
            }
        }
        public static string ReadValue(string subKey, string key)
        {
            string value = "";
            using (RegistryKey Subkey = Registry.CurrentUser.OpenSubKey(subKey))
            {

                if (Subkey != null)
                {
                    value = Subkey.GetValue(key).ToString();
                    Subkey.Close();
                }
            }
            return value;
        }
    }
}
