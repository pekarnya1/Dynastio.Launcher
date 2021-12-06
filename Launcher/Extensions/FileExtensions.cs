using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Extensions
{
    internal static class FileExtensions
    {
        public static string ResourcesPath(this string path)
        {
            string uri = "/";
            string currentDirectory = Directory.GetCurrentDirectory();
            path = path.Replace("/", uri).Replace("\\", uri);
            return $@"{currentDirectory}{uri}Resources{uri}{path}";
        }
      
    }
}
