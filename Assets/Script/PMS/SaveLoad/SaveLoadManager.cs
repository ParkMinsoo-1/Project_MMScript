using System;
using System.IO;
using System.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
using UnityEngine;

public interface SaveTime
{
    long lastSaveTime { get; set; }
}

public static class SaveLoadManager
{
    public static string UserID => SystemInfo.deviceUniqueIdentifier;
    private static string GetLocalPath(string key)
    {
        return Path.Combine(Application.persistentDataPath, $"{key}.json");
    }
    public static async Task<bool> Save<T>(string key, T value) where T : SaveTime
    {
        value.lastSaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string json = JsonConvert.SerializeObject(value);

        try
        {
            File.WriteAllText(GetLocalPath(key), json);
        }
        catch (Exception e)
        {
            Debug.LogError($"로컬 저장 실패 {key}: {e.Message}");
            return false;
        }

        try
        {
            await FirebaseDatabase.DefaultInstance.GetReference($"users/{UserID}/{key}").SetValueAsync(json);
            Debug.Log($"저장 성공 {key}");
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"저장 실패 {key}. {exception.Message}");
            return false;
        }
    }
    
    public static async Task<T> Load<T>(string key, T defaultValue = default, bool ignoreLocal = false) where T : SaveTime
    {

        T localData = default;
        T remoteData = default;

        string localPath = GetLocalPath(key);
        if (!ignoreLocal && File.Exists(localPath))
        {
            try
            {
                string localJson = File.ReadAllText(localPath);
                localData = JsonConvert.DeserializeObject<T>(localJson);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"로컬 데이터 읽기 실패 {key}: {e.Message}");
            }
        }

        bool remoteOk = true;
        try
        {
            var data = await FirebaseDatabase.DefaultInstance
                .GetReference($"users/{UserID}/{key}").GetValueAsync();

            if (data.Exists && data.Value != null)
            {
                string json = data.Value.ToString();
                remoteData = JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                remoteData = default;
            }
        }
        catch (Exception exception)
        {
            remoteOk = false;
            Debug.LogError($"리모트 불러오기 실패 {key}: {exception.Message}");
        }
        if (remoteData != null && !remoteData.Equals(default))
        {
            if (localData != null && !localData.Equals(default))
            {
                return localData.lastSaveTime >= remoteData.lastSaveTime ? localData : remoteData;
            }
            else
            {
                return remoteData;
            }
        }
        else
        {
            if (localData != null && !localData.Equals(default))
            {
                return localData;
            }
        }

        return defaultValue;
    }
}
