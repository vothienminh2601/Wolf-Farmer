using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<ItemData> items = new(); // danh sách vật phẩm runtime

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ✅ Lấy danh sách hạt giống còn lại
    public List<ItemData> GetSeedItems()
    {
        return items.FindAll(i => i.itemSO != null && i.itemSO.category == ItemCategory.CropSeed);
    }

    // ✅ Trừ hạt sau khi trồng
    public void UseSeed(ItemSO seedItem)
    {
        var data = items.Find(i => i.itemSO == seedItem);
        if (data != null && data.quantity > 0)
            data.quantity--;
    }

    // ✅ Thêm hạt (khi mua hoặc thưởng)
    public void AddItem(ItemSO item, int amount = 1)
    {
        var data = items.Find(i => i.itemSO == item);
        if (data != null) data.quantity += amount;
        else items.Add(new ItemData { itemSO = item, quantity = amount });
    }

    // ✅ Lấy số lượng cụ thể
    public int GetQuantity(ItemSO item)
    {
        var data = items.Find(i => i.itemSO == item);
        return data != null ? data.quantity : 0;
    }
}
