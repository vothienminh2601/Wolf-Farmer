using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ tài nguyên và vật phẩm trong game.
/// Bao gồm Coin, Seed, Fruit, Equipment, Worker, Farm và thống kê tổng.
/// </summary>
    [System.Serializable]
    public class ResourceStack
    {
        public string id;
        public int quantity;
    }
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }


    // ==============================
    [Header("Currency")]
    [SerializeField] private int coin;

    [Header("Gameplay Resources")]
    [SerializeField] private List<ResourceStack> seeds = new();
    [SerializeField] private List<ResourceStack> animalBreeds = new();
    [SerializeField] private List<ResourceStack> products = new();
    [SerializeField] private List<ResourceStack> animals = new();
    [SerializeField] private List<ResourceStack> equipments = new();
    [SerializeField] private List<ResourceStack> workers = new();

    public static event Action<int> OnCoinChanged;
    public static event Action<List<ResourceStack>> OnProductChanged;
    public static event Action<List<ResourceStack>, List<ResourceStack>> OnStockChanged;

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
        OnCoinChanged?.Invoke(coin);
    }

    public bool SpendCoin(int amount)
    {
        if (coin < amount) return false;
        coin -= amount;
        OnCoinChanged?.Invoke(coin);
        return true;
    }

    // ============================================================
    // 🌱 SEED
    // ============================================================
    public void AddSeed(string id, int amount = 1) {
        AddToList(seeds, id, amount);
        OnStockChanged?.Invoke(seeds, animals);
    }

    public void UseSeed(string id, int amount = 1) {
        RemoveFromList(seeds, id, amount);
        OnStockChanged?.Invoke(seeds, animals);
    }
    public int GetSeedCount(string id) => GetCount(seeds, id);
    public List<ResourceStack> GetAllSeeds() => seeds;

    // ============================================================
    // 🌱 Animal Breed
    // ============================================================
    public void AddAnimalBreed(string id, int amount = 1) {
        AddToList(animalBreeds, id, amount);
        OnStockChanged?.Invoke(seeds, animals);
    }

    public void UseAnimalBreed(string id, int amount = 1) {
        RemoveFromList(animalBreeds, id, amount);
        OnStockChanged?.Invoke(seeds, animals);
    }
    public int GetAnimalBreedCount(string id) => GetCount(animalBreeds, id);
    public List<ResourceStack> GetAllAnimalBreeds() => seeds;

    // ============================================================
    // 🌱 ANIMAL
    // ============================================================
    public void AddAnimal(string id, int amount = 1) {
        AddToList(animals, id, amount);
    }
    public void UseAnimal(string id, int amount = 1) {
        RemoveFromList(animals, id, amount);
    }
    public int GetAnimalCount(string id) => GetCount(animals, id);
    public List<ResourceStack> GetAllAnimals() => seeds;

    // ============================================================
    // 🍎 FRUIT
    // ============================================================
    public void AddProduct(string id, int amount = 1)
    {
        AddToList(products, id, amount);
        totalHarvest += amount;
        OnProductChanged?.Invoke(products);
    }

    public void ReduceProduct(string id, int amount = 1) {
        RemoveFromList(products, id, amount);
        OnProductChanged?.Invoke(products);
    }

    public int GetProductCount(string id) => GetCount(products, id);
    public List<ResourceStack> GetAllProducts() => products;

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
}
