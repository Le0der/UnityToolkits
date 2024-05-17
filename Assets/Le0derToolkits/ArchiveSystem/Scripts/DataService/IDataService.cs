using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Le0der.ArchiveSystem
{
    public interface IDataService
    {
        bool SaveData<T>(string path, T data, bool encrypted);

        T LoadData<T>(string path, bool encrypted);
    }
}