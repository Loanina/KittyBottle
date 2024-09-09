using System;
using UnityEngine;
using System.IO;

namespace Common.DataManagement
{
    public class SaveSystem<T> where T : new()
    {
        private static readonly Lazy<SaveSystem<T>> _instance = new(() => new SaveSystem<T>());
        private static readonly string filePath = Application.persistentDataPath + "/playerdata.dat";

        public static SaveSystem<T> Instance => _instance.Value;
        
        public void Save(T data)
        {
            try
            {
                var json = JsonUtility.ToJson(data);
                var encryptedJson = EncryptionUtility.Encrypt(json);
                File.WriteAllText(filePath, encryptedJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving data: {ex.Message}");
            }
        }

        public T Load()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var encryptedJson = File.ReadAllText(filePath);
                    var decryptedJson = EncryptionUtility.Decrypt(encryptedJson);
                    return JsonUtility.FromJson<T>(decryptedJson);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading data: {ex.Message}");
                SetInitialData();
            }
            return default;
        }

        private void SetInitialData()
        {
            Save(new T());
        }

        public void Clear()
        { 
            SetInitialData();
        }
    }
}
