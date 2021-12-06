using Launcher.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    public class ProfileManager
    {
        private readonly IServiceProvider _services;
        private readonly AppManager _appManager;
        public ProfileManager(IServiceProvider services)
        {
            _services = services;
            this._appManager = _services.GetRequiredService<AppManager>();
        }
        public void SaveChanges()
        {
            FileManager.SaveConfiguration(_appManager.Configuration);
        }
        public static void SetProfile(UserProfile profile) => RegistryManager.Import(Directory.GetCurrentDirectory() + @"\" + FileManager.ProfilesPath + profile.Id + ".p");
        public static void SaveProfile(UserProfile profile) => RegistryManager.Export(RegistryManager.RegWhaleboxStudio, FileManager.ProfilesPath + profile.Id + ".p");
        public void UnselectAllProfiles()
        {
            foreach (var g in _appManager.Configuration.Profiles.Where(a => a.IsSelected))
            {
                g.IsSelected = false;
            }
        }
        public List<UserProfile> GetAll() => _appManager.Configuration.Profiles;
        public UserProfile GetSelectedGame() => _appManager.Configuration.Profiles.Where(a => a.IsSelected).FirstOrDefault();
        public void AddProfile(UserProfile profile) => _appManager.Configuration.Profiles.Add(profile);
        public void RemoveProfile(UserProfile profile)
        {
            _appManager.Configuration.Profiles.Remove(profile);
            FileManager.DeleteProfile(profile);
        }
    }
}
