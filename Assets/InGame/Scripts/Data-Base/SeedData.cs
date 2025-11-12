using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeedData
{
    public string id;                     // seed_001
    public string name;                   // Tên hiển thị
    public int baseValue;
    public float stageDuration;           // Thời gian qua mỗi stage
    public float fruitInterval;           // Khoảng thời gian giữa các lần sinh trái
    public int maxFruitCount;             // Tổng số trái cây có thể sinh ra

    public string iconAddress;            // Key Addressables của icon (Sprite)
    public List<string> cropStepAddresses = new(); // Các prefab key Addressables cho từng stage
    public string fruitId;                // Liên kết tới FruitData

    public void LoadIcon(System.Action<Sprite> callback)
    {
        if (string.IsNullOrEmpty(iconAddress))
        {
            Debug.LogWarning($"SeedData[{id}] không có iconAddress.");
            callback?.Invoke(null);
            return;
        }

        SpriteManager.Instance.LoadSpriteAsync(iconAddress, callback);
    }

    public void LoadCropSteps(System.Action<List<GameObject>> callback)
    {
        if (cropStepAddresses == null || cropStepAddresses.Count == 0)
        {
            Debug.LogWarning($"SeedData[{id}] không có cropStepAddresses.");
            callback?.Invoke(new List<GameObject>());
            return;
        }

        List<GameObject> results = new();
        int loaded = 0;

        foreach (string addr in cropStepAddresses)
        {
            PrefabManager.Instance.LoadPrefabAsync(addr, go =>
            {
                loaded++;
                if (go != null) results.Add(go);

                if (loaded >= cropStepAddresses.Count)
                    callback?.Invoke(results);
            });
        }
    }

    public List<GameObject> GetCachedCropSteps()
    {
        List<GameObject> list = new();
        foreach (var addr in cropStepAddresses)
        {
            var go = PrefabManager.Instance.GetCached(addr);
            if (go != null) list.Add(go);
        }
        return list;
    }

    public Sprite GetCachedIcon() => SpriteManager.Instance.GetCached(iconAddress);
}
