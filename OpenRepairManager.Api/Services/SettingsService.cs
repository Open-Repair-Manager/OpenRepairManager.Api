using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenRepairManager.Common.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenRepairManager.Api.Services
{
    public static class SettingsService
    {
        public static bool AddOrUpdate(Setting setting)
        {
            var protector = DataProtectionService.GetDataProtector();

            if(!File.Exists(DataProtectionService.ConfigFileFullPath))
            {
                string json = "[]";
                var path = DataProtectionService.ConfigFileFullPath;
                File.WriteAllText(path, json);
            }

            try
            {
                var settingList = JsonSerializer.Deserialize<List<Setting>>(File.ReadAllText(DataProtectionService.ConfigFileFullPath));
                var itemNotExists = !settingList.Exists(w => w.Name.ToUpper() == setting.Name.ToUpper());
                setting.Value = protector.Protect(setting.Value);
                if (itemNotExists)
                {
                    settingList.Add(setting);
                }
                else
                {
                    settingList.First(w => w.Name.ToUpper() == setting.Name.ToUpper()).Value = setting.Value;
                }
                string json = JsonSerializer.Serialize(settingList);
                var path = DataProtectionService.ConfigFileFullPath;
                File.WriteAllText(path, json);
                Console.WriteLine($"Writing app settings secret to '${path}' completed successfully.");
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static List<Setting> GetAllSettings()
        {
            string newlist = File.OpenText(DataProtectionService.ConfigFileFullPath).ReadToEnd();
            var protector = DataProtectionService.GetDataProtector();
            var list = JsonSerializer.Deserialize<List<Setting>>(newlist);
            foreach (var item in list)
            {
                item.Value = protector.Unprotect(item.Value);
            }
            return list;
        }

        public static Setting GetSetting(string setting)
        {
            var protector = DataProtectionService.GetDataProtector();
            var settingList = JsonSerializer.Deserialize<List<Setting>>(File.ReadAllText(DataProtectionService.ConfigFileFullPath));
            var itemNotExists = !settingList.Exists(w => w.Name.ToUpper() == setting.ToUpper());
            if(itemNotExists)
            {
                return null;
            }
            else
            {
                var settingToReturn = settingList.Where(w => w.Name.ToUpper() == setting.ToUpper()).First();
                settingToReturn.Value = protector.Unprotect(settingToReturn.Value);
                return settingToReturn;
            }
        }

    }
}
