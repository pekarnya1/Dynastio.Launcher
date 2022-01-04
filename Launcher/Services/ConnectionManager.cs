using Launcher.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{

    internal class ConnectionManager
    {


        public static long GetFileSize(string uri, int timeout = 5000)
        {
            long size = 0;
            try
            {
                using (var client = new WebClient())
                {
                    client.OpenRead(uri);
                    Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);

                    size = bytes_total;
                }
                return size;
            }
            catch
            {
                return size;
            }
        }
        public static async Task<string> DownloadString(string uri, int timeout = 5000)
        {
            try
            {
                var x = "";
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout);
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " + "Windows NT 5.2; .NET CLR 1.0.3705;)");

                    var response = await client.GetStringAsync(uri);
                    x = response;
                }
                return x;
            }
            catch
            {
                return null;
            }
        }

    }
}
