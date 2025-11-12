using UnityEngine;

[System.Serializable]
public class AnimalData
{
    public string id;
    public string name;
    public int baseValue;

    public float growDuration;      // Thời gian lớn lên (từ bê thành bò)
    public float productInterval;   // Thời gian giữa 2 lần cho sản phẩm (sữa, trứng, len,...)
    public int maxProductCount;     // Số lần cho sản phẩm trước khi nghỉ / cần reset
    public string iconAddress;
    public string prefabAddress;
    public string productId;        // ID của sản phẩm (milk, egg, wool,...)

    // Runtime Loaders
    public void LoadIcon(System.Action<Sprite> callback)
        => SpriteManager.Instance.LoadSpriteAsync(iconAddress, callback);

    public void LoadPrefab(System.Action<GameObject> callback)
        => PrefabManager.Instance.LoadPrefabAsync(prefabAddress, callback);
}
