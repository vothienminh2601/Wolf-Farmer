using UnityEngine;

[System.Serializable]
public class FruitData
{
    public string id;               // fruit_001
    public string name;             // Tên hiển thị
    public int value;               // Giá trị bán hoặc điểm
    public string iconAddress;      // Addressables key của icon
    public string prefabAddress;    // Addressables key của prefab
    public string description;      // Mô tả thêm (tuỳ chọn)

    // --- Cách truy cập runtime ---
    public void LoadIcon(System.Action<Sprite> callback)
    {
        if (string.IsNullOrEmpty(iconAddress))
        {
            Debug.LogWarning($"FruitData[{id}] không có iconAddress");
            callback?.Invoke(null);
            return;
        }

        SpriteManager.Instance.LoadSpriteAsync(iconAddress, callback);
    }

    public void LoadPrefab(System.Action<GameObject> callback)
    {
        if (string.IsNullOrEmpty(prefabAddress))
        {
            Debug.LogWarning($"FruitData[{id}] không có prefabAddress");
            callback?.Invoke(null);
            return;
        }

        PrefabManager.Instance.LoadPrefabAsync(prefabAddress, callback);
    }

    public GameObject GetCachedPrefab() => PrefabManager.Instance.GetCached(prefabAddress);
    public Sprite GetCachedIcon() => SpriteManager.Instance.GetCached(iconAddress);
}
