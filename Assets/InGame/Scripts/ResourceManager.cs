using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ tài nguyên và vật phẩm trong game.
/// Bao gồm Coin, Seed, Fruit, Equipment, Worker, Farm và thống kê tổng.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [System.Serializable]
    public class ResourceStack
    {
        public string id;
        public int quantity;
    }

    // ==============================
    [Header("Currency")]
    [SerializeField] private int coin;

    [Header("Gameplay Resources")]
    [SerializeField] private List<ResourceStack> seeds = new();
    [SerializeField] private List<ResourceStack> fruits = new();
    [SerializeField] private List<ResourceStack> equipments = new();
    [SerializeField] private List<ResourceStack> workers = new();
    [SerializeField] private List<ResourceStack> farms = new();

    [Header("Statistics")]
    [SerializeField] private int totalHarvest;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ============================================================
    // 🪙 COIN
    // ============================================================
    public int GetCoin() => coin;

    public void AddCoin(int amount)
    {
        coin += amount;
        if (coin < 0) coin = 0;
    }

    public bool SpendCoin(int amount)
    {
        if (coin < amount) return false;
        coin -= amount;
        return true;
    }

    // ============================================================
    // 🌱 SEED
    // ============================================================
    public void AddSeed(string id, int amount = 1) => AddToList(seeds, id, amount);
    public void UseSeed(string id, int amount = 1) => RemoveFromList(seeds, id, amount);
    public int GetSeedCount(string id) => GetCount(seeds, id);
    public List<ResourceStack> GetAllSeeds() => seeds;

    // ============================================================
    // 🍎 FRUIT
    // ============================================================
    public void AddFruit(string id, int amount = 1)
    {
        AddToList(fruits, id, amount);
        totalHarvest += amount;
    }

    public int GetFruitCount(string id) => GetCount(fruits, id);
    public List<ResourceStack> GetAllFruits() => fruits;

    // ============================================================
    // ⚙️ EQUIPMENT
    // ============================================================
    public void AddEquipment(string id, int amount = 1) => AddToList(equipments, id, amount);
    public int GetEquipmentCount(string id) => GetCount(equipments, id);
    public List<ResourceStack> GetAllEquipments() => equipments;

    // ============================================================
    // 👷 WORKER
    // ============================================================
    public void AddWorker(string id, int amount = 1) => AddToList(workers, id, amount);
    public void RemoveWorker(string id, int amount = 1) => RemoveFromList(workers, id, amount);
    public int GetWorkerCount(string id) => GetCount(workers, id);
    public List<ResourceStack> GetAllWorkers() => workers;

    // ============================================================
    // 🌾 FARM
    // ============================================================
    public void AddFarm(string id, int amount = 1) => AddToList(farms, id, amount);
    public int GetFarmCount(string id) => GetCount(farms, id);
    public List<ResourceStack> GetAllFarms() => farms;

    // ============================================================
    // 📊 STATISTICS
    // ============================================================
    public int GetTotalHarvest() => totalHarvest;
    public void ResetTotalHarvest() => totalHarvest = 0;

    // ============================================================
    // 🧩 UTILITY
    // ============================================================
    private void AddToList(List<ResourceStack> list, string id, int amount)
    {
        if (string.IsNullOrEmpty(id) || amount <= 0) return;

        var item = list.Find(i => i.id == id);
        if (item != null)
            item.quantity += amount;
        else
            list.Add(new ResourceStack { id = id, quantity = amount });
    }

    private void RemoveFromList(List<ResourceStack> list, string id, int amount)
    {
        var item = list.Find(i => i.id == id);
        if (item == null) return;

        item.quantity -= amount;
        if (item.quantity <= 0)
            list.Remove(item);
    }

    private int GetCount(List<ResourceStack> list, string id)
    {
        var item = list.Find(i => i.id == id);
        return item != null ? item.quantity : 0;
    }

    // ============================================================
    // 🧾 DEBUG
    // ============================================================
    [ContextMenu("Print Resource Summary")]
    private void PrintResources()
    {
        Debug.Log($"=== RESOURCE SUMMARY ===");
        Debug.Log($"Coin: {coin} | Total Harvest: {totalHarvest}");
        PrintList("Seeds", seeds);
        PrintList("Fruits", fruits);
        PrintList("Equipments", equipments);
        PrintList("Workers", workers);
        PrintList("Farms", farms);
    }

    private void PrintList(string title, List<ResourceStack> list)
    {
        Debug.Log($"--- {title} ---");
        foreach (var item in list)
            Debug.Log($"{item.id} x{item.quantity}");
    }
}
