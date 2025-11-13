
using System;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;


[System.Serializable]
public class UserProfile
{
    public string id;
    public string name;
    public string avatar;
    public string country;
    public string frame;
    public string data = "{}";
}

public class UserData : Singleton<UserData>
{
    private string LOCAL_KEY = "USER_DATA";
    public UserProfile userProfile;

    [HideInInspector]
    public UnityEvent<UserProfile> UserProfileChangedEvent = new UnityEvent<UserProfile>();


    public void LoadUserProfile(UnityAction<UserProfile> callback)
    {
        var userProfileJson = "";
        if (!PlayerPrefs.HasKey(LOCAL_KEY))
        {
            var newUserProfile = new UserProfile();
            newUserProfile.id = "";
            newUserProfile.name = "Guest";
            string countryCode = "";
            // string countryName = "";
            // GameUtils.GetCountryName(out countryCode, out countryName);
            newUserProfile.country = countryCode;
            newUserProfile.avatar = "";
            newUserProfile.frame = "";
            // newUserProfile.coin = GameConfigs.START_COIN;
            userProfileJson = JsonUtility.ToJson(newUserProfile);
        }
        else userProfileJson = PlayerPrefs.GetString(LOCAL_KEY);

        try
        {
            userProfile = JsonUtility.FromJson<UserProfile>(userProfileJson);
            callback?.Invoke(userProfile);
        }
        catch
        {
            callback?.Invoke(null);
        }
    }

    private void SetUserProfile(UserProfile userProfile, UnityAction<UserProfile> callback = null)
    {
        var userProfileJson = JsonUtility.ToJson(userProfile);
        PlayerPrefs.SetString(LOCAL_KEY, userProfileJson);

        callback?.Invoke(userProfile);
        UserProfileChangedEvent?.Invoke(userProfile);
    }

    public void SaveUserProfile(string type, object value, UnityAction<UserProfile> callback = null)
    {
        if (userProfile != null)
        {
            switch (type)
            {
                case "id":
                    userProfile.id = (string)value;
                    break;
                case "name":
                    userProfile.name = (string)value;
                    break;
                case "avatar":
                    userProfile.avatar = (string)value;
                    break;
                case "frame":
                    userProfile.frame = (string)value;
                    break;
                case "data":
                    userProfile.data = JsonUtility.ToJson(value);
                    break;
            }
            SetUserProfile(userProfile, callback);
        }
    }

    public T GetData<T>(string key)
    {
        if (!string.IsNullOrEmpty(userProfile.data))
        {
            var jData = JObject.Parse(userProfile.data);
            if (jData != null)
                if (jData.TryGetValue(key, out JToken value))
                {
                    return value.ToObject<T>(); // Convert JToken to the specified type T
                }
        }
        return default; // Return the default value for type T (null for reference types, 0 for int, etc.)
    }

    public void SetData(string key, object value)
    {
        var jData = !string.IsNullOrEmpty(userProfile.data) ? JObject.Parse(userProfile.data) : new JObject();
        jData[key] = JToken.FromObject(value); // Add or update the key-value pair
        userProfile.data = jData.ToString(); // Update the original data string with the new JSON
        SetUserProfile(userProfile, null);
    }
    public void SaveLastSavedTime()
    {
        if (userProfile != null)
        {
            long lastTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SetData("lastSavedTime", lastTime);
        }
        else
        {
            Debug.LogWarning("UserData.Instance or userProfile is null when quitting!");
        }
    }

    public void DeleteAllData()
    {
        Debug.Log("Clearing all user data...");
        PlayerPrefs.DeleteAll(); 
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.persistentDataPath);

        foreach (var file in dir.GetFiles())
        {
            try { file.Delete(); }
            catch { Debug.LogWarning($"Failed to delete {file.Name}"); }
        }

        Debug.Log("All save data deleted.");
    }

    protected void OnDestroy()
    {
        SaveLastSavedTime();
    }

    #region GAME SAVE / LOAD

    public void SaveGame()
    {
        GameSaveData gameSave = new GameSaveData();
        gameSave.resourceData = ResourceManager.Instance.GetSaveData();
        gameSave.farmSaveData = FarmManager.Instance.GetSaveData();
        SetData("gameSave", gameSave);
        SaveLastSavedTime();
    }


    // Load
    public void LoadGame()
    {
        var gameSave = GetData<GameSaveData>("gameSave");

        if (gameSave == null) return;

        if(gameSave.resourceData != null) ResourceManager.Instance.LoadFromSave(gameSave.resourceData);
    }
    #endregion

}

