using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher.Extensions;
using Dynastio.Api;
using Newtonsoft.Json;
using Launcher.Services;

namespace Launcher.Models
{
    public class LauncherTransferDataModel
    {
        public Profile Profile { get; set; }
        public Personalchest Personalchest { get; set; }
        public string ToEncryptedString()
        {
            var data = JsonConvert.SerializeObject(this);
            var result = DataManager.Encrypt(data, "x");
            return result;
        }
        public static LauncherTransferDataModel ToTransferDataModel(string value)
        {
            var data = DataManager.Decrypt(value, "x");
            var result = JsonConvert.DeserializeObject<LauncherTransferDataModel>(data);
            return result;
        }
    }
    public class UserProfile
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public bool IsSelected { get; set; }
        public Profile Profile { get; set; }
        public Personalchest Personalchest { get; set; }
        public PlayerStat PlayerStat { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Details
        {
            get
            {
                return $"Created At {CreatedAt.TimeWordagelizition()} ago";
            }
        }
    }
}
