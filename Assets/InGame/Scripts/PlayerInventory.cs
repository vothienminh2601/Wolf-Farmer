using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ vật phẩm runtime của người chơi (hạt giống, trái, tài nguyên...)
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [System.Serializable]
    public class ItemStack
    {
        public string itemId;
        public int quantity;
    }

    [Header("Runtime Inventory")]
    [SerializeField] private List<ItemStack> items = new(); // danh sách vật phẩm hiện có

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // -------------------- LẤY DỮ LIỆU --------------------

    /// <summary>
    /// Lấy danh sách hạt giống (Seed) hiện có trong túi.
    /// </summary>
    public List<ItemStack> GetSeedItems()
    {
        List<ItemStack> seeds = new();
        foreach (var item in items)
        {
            if (DataManager.GetSeedById(item.itemId) != null)
                seeds.Add(item);
        }
        return seeds;
    }

    /// <summary>
    /// Lấy số lượng của 1 item cụ thể.
    /// </summary>
    public int GetQuantity(string itemId)
    {
        var item = items.Find(i => i.itemId == itemId);
        return item != null ? item.quantity : 0;
    }

    // -------------------- THAO TÁC VẬT PHẨM --------------------

    /// <summary>
    /// Thêm vật phẩm (Fruit hoặc Seed).
    /// </summary>
    public void AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return;

        var item = items.Find(i => i.itemId == itemId);
        if (item != null)
            item.quantity += amount;
        else
            items.Add(new ItemStack { itemId = itemId, quantity = amount });
    }

    /// <summary>
    /// Dùng 1 hạt giống khi trồng.
    /// </summary>
    public void UseSeed(string seedId)
    {
        var item = items.Find(i => i.itemId == seedId);
        if (item != null && item.quantity > 0)
        {
            item.quantity--;
            if (item.quantity <= 0)
                items.Remove(item);
        }
    }

    /// <summary>
    /// Kiểm tra có đủ hạt giống không.
    /// </summary>
    public bool HasSeed(string seedId)
    {
        var item = items.Find(i => i.itemId == seedId);
        return item != null && item.quantity > 0;
    }

    // -------------------- DEBUG / TEST --------------------
    [ContextMenu("Print Inventory")]
    private void PrintInventory()
    {
        Debug.Log("=== INVENTORY ===");
        foreach (var item in items)
            Debug.Log($"{item.itemId} x{item.quantity}");
    }
}
