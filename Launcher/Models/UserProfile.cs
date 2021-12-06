using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher.Extensions;
using Dynastio.Api;
namespace Launcher.Models
{
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
