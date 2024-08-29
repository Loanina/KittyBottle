using System;
using UnityEngine;
using System.IO;
using Unity.Properties;

namespace Common.DataManagement
{
    public class SaveSystem : MonoBehaviour
    {
        private static readonly string filePath = Application.persistentDataPath + "/playerdata.dat";

        public static void SavePlayerData(PlayerData data)
        {
            string json = JsonUtility.ToJson(data);
            string encryptedJson = EncryptionUtility.Encrypt(json);
            File.WriteAllText(filePath, encryptedJson);
        }

        public static PlayerData LoadPlayerData()
        {
            if (File.Exists(filePath))
            {
                string encryptedJson = File.ReadAllText(filePath);
                string decryptedJson = EncryptionUtility.Decrypt(encryptedJson);
                return JsonUtility.FromJson<PlayerData>(decryptedJson);
            }
            else
            {
                string json = JsonUtility.ToJson(new PlayerData());
                string encryptedJson = EncryptionUtility.Encrypt(json);
                File.WriteAllText(filePath, encryptedJson);
                return new PlayerData();
            }
        }
         
        public static void Clear()
        {
            var json = JsonUtility.ToJson(new PlayerData());
            string encryptedJson = EncryptionUtility.Encrypt(json);
            File.WriteAllText(filePath, encryptedJson);
        }
    }
}
