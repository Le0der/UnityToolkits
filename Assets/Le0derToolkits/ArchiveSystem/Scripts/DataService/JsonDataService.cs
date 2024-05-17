
using System;
using System.IO;
using UnityEngine;
using Le0der.Toolbox;
using System.Security.Cryptography;
using System.Text;

namespace Le0der.ArchiveSystem
{
    public class JsonDataService : IDataService
    {
        private const string KEY = "aBDHL9b4ihsiT5fXGU+/voGjyq5JEmYlpZyOuITNSTI=";
        private const string IV = "meTrOAkkrpKWeUyuHDPP8w==";
        public bool SaveData<T>(string path, T data, bool encrypted)
        {
            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and waiting for new file.");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Waiting file for the first time.");
                }


                using FileStream stream = File.Create(path);
                if (encrypted)
                    WirteEncryptedData(data, stream);
                else
                    WirteUnEncryptedData(path, data, stream);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return false;
            }
        }

        public T LoadData<T>(string path, bool encrypted)
        {
            if (!File.Exists(path))
            {
                var errorInfo = $"Unable to load data due to: File not found at path: {path}";
                Debug.LogWarning(errorInfo);
                return default;
            }

            try
            {
                T data;
                if (encrypted)
                {
                    data = ReadEncryptedData<T>(path);
                }
                else
                {

                    data = JsonToolkit.DeserializeObject<T>(File.ReadAllText(path));
                }

                return data;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load data due to: {e.Message}; StackTrace: {e.StackTrace}");
                return default;
            }
        }

        private void WirteEncryptedData<T>(T data, FileStream stream)
        {
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);
            using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);

            // You can uncomment the below to see a generated value for the IV & key.
            // You can also generate your own if you wish
            // Debug.Log($"Key: {Convert.ToBase64String(aesProvider.Key)}");
            // Debug.Log($"Initialization vector: {Convert.ToBase64String(aesProvider.IV)}");

            string dataJson = JsonToolkit.SerializeObject(data);
            byte[] dataBytes = Encoding.UTF8.GetBytes(dataJson);
            cryptoStream.Write(dataBytes);
        }

        private void WirteUnEncryptedData<T>(string path, T data, Stream stream)
        {
            stream.Close();
            File.WriteAllText(path, JsonToolkit.SerializeObject(data));
        }

        private T ReadEncryptedData<T>(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
            using MemoryStream decryptionStream = new MemoryStream(fileBytes);
            using CryptoStream cryptoStream = new CryptoStream(decryptionStream, cryptoTransform, CryptoStreamMode.Read);
            using StreamReader reader = new StreamReader(cryptoStream);

            string result = reader.ReadToEnd();

            Debug.Log($"Decrypted result (if the following is not legible, probably the key or IV is wrong): {result}");
            return JsonToolkit.DeserializeObject<T>(result);
        }
    }
}