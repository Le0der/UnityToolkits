using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Le0der.ArchiveSystem
{
    public class JsonConfig
    {
        JsonDataService dataService = new JsonDataService();

        public T LoadConfig<T>(string path, bool encrypted = false) where T : ConfigBase, new()
        {
            return dataService.LoadData<T>(path, encrypted);
        }

        public void SaveConfig<T>(string path, T data, bool encrypted = false) where T : ConfigBase, new()
        {
            dataService.SaveData(path, data, encrypted);
        }
    }

    public abstract class ConfigBase { }
}