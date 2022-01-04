using Launcher.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Launcher.Extensions;
using Dynastio.Api;
using Newtonsoft.Json;

using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Launcher.Managers
{
    public static class ManagerExtensions
    {
        public static Output DeserializeObjectData<Input, Output>(this string data)
        {
            var _data = JsonConvert.DeserializeObject(data.ToString()).ToString();
            return JsonConvert.DeserializeObject<Output>(_data);
        }
    }
    public class PrivateServerDataManager : IDisposable
    {
        public SqliteConnection Connection { get; set; }
        public PrivateServerDataManager(string connection)
        {
            Connection = new SqliteConnection(connection);
        }
        public PrivateServerDataManager OpenConnection()
        {
            Connection.Open();
            return this;
        }
        public List<PSAccount> GetUser(string Id) => GetObjects<PSAccount>($"SELECT * FROM keyvalue WHERE key = '{Id}'").ToList();
        public List<PSAccount> GetAllUsers() => GetObjects<PSAccount>("SELECT * FROM keyvalue WHERE key LIKE 'discord:%' OR key LIKE 'google:%' OR key LIKE 'facebook:%';").ToList();
        public IEnumerable<Personalchest> GetUserChest(string Id)
        {
            var c = GetObjects<string>($"SELECT * FROM keyvalue WHERE key = 'entity:{Id}:storage_chest'").ToList();
            foreach (var d in c)
            {
                var data = d.DeserializeObjectData<string, JObject>().SelectToken("items").ToArray();
                var chestItems = new List<PersonalChestItem>();
                foreach (var item in data)
                {
                    var item_ = new PersonalChestItem()
                    {
                        index = int.Parse(item[0].ToString()),
                        ItemType = (ItemType)int.Parse(item[1].ToString()),
                        Count = int.Parse(item[2].ToString()),
                        Durablity = int.Parse(item[3].ToString()),
                        OwnerID = item[4].ToString(),
                        Token = item[5].ToString()
                    };
                    chestItems.Add(item_);
                }
                yield return new Personalchest()
                {
                    items = chestItems
                };
            }
        }
        public List<Personalchest> GetAllChests() => GetObjects<Personalchest>("SELECT * FROM keyvalue WHERE key like '%:storage_chest';").ToList();
        public void UpdateUser(PSAccount ac)
        {
            if (ac == null) return;
            var value = JsonConvert.SerializeObject(ac);
            Update($"update keyvalue set value = @value where key = '{ac.id}'", value);
        }
        public void UpdatePersonalChest(Personalchest pchest, string id)
        {
            if (pchest == null) return;
            var value = JsonConvert.SerializeObject(pchest);
            Update($"update keyvalue set value = @value where key = 'entity:{id}:storage_chest'", value);
        }
        public void Update(string query, string value)
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@value", value);
            command.ExecuteNonQuery();
        }
        public IEnumerable<T> GetObjects<T>(string query)
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var d = reader.GetString(2);
                    yield return JsonConvert.DeserializeObject<T>(d);
                }
            }
        }
        public T GetObject<T>(string query)
        {
            var command = Connection.CreateCommand();
            command.CommandText = query;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var d = reader.GetString(2);
                    return JsonConvert.DeserializeObject<T>(d);
                }
            }
            return default(T);
        }
        public void Dispose()
        {
            Connection.Dispose();
        }
    }
    public class PSAccount
    {
        public string id { get; set; }
        public string name { get; set; }
        public string verified_nickname { get; set; }
        public string email { get; set; }
        public string access_token { get; set; }
        public string session_token { get; set; }
        public string latest_server { get; set; }
        public int coins { get; set; }
        public List<BadgeType> badges { get; set; }
        public List<ItemType> unlocked_recipes { get; set; }
        public List<EntityType> unlocked_buildings { get; set; }
        public List<object> unlocked_skins { get; set; }
        public string paystation_token { get; set; }
        public bool is_paying { get; set; }
        public object role { get; set; }
    }

}
