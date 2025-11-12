using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public TextAsset fruitCSV;
    public TextAsset seedCSV;
    public TextAsset animalCSV;

    public static Dictionary<string, SeedData> SeedDict { get; private set; } = new();
    public static Dictionary<string, ProductData> FruitDict { get; private set; } = new();
    public static Dictionary<string, AnimalData> AnimalDict { get; private set; } = new();

    void Awake()
    {
        LoadAllData();
    }

    [ContextMenu("LoadData")]
    public async void LoadAllData()
    {
        await LoadFruits();
        LoadSeeds().Forget();
        LoadAnimals().Forget();
    }

    async UniTask LoadFruits()
    {
        var reader = CSVReaderFactory.CreateReader<ProductData>();
        if (reader != null && fruitCSV != null)
            FruitDict = await reader.LoadDataFromCSV(fruitCSV);
        else
            FruitDict = new Dictionary<string, ProductData>();

        Debug.Log($"Loaded {FruitDict.Count} Fruits");
    }
    async UniTask LoadSeeds()
    {
        var reader = CSVReaderFactory.CreateReader<SeedData>();
        if (reader != null && seedCSV != null)
            SeedDict = await reader.LoadDataFromCSV(seedCSV);
        else
            SeedDict = new Dictionary<string, SeedData>();

        Debug.Log($"Loaded {SeedDict.Count} Seeds");
    }

    async UniTask LoadAnimals()
    {
        var reader = CSVReaderFactory.CreateReader<AnimalData>();
        if (reader != null && animalCSV != null)
            AnimalDict = await reader.LoadDataFromCSV(animalCSV);
        else
            AnimalDict = new Dictionary<string, AnimalData>();

        Debug.Log($"Loaded {AnimalDict.Count} Animals");
    }


    public static SeedData GetSeedById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("GetSeedById: id trống hoặc null");
            return null;
        }

        if (SeedDict.TryGetValue(id, out var seed))
            return seed;

        Debug.LogWarning($"Seed ID '{id}' không tồn tại trong DataManager.");
        return null;
    }

    /// <summary>
    /// Lấy dữ liệu Fruit theo ID.
    /// </summary>
    public static ProductData GetFruitById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("GetFruitById: id trống hoặc null");
            return null;
        }

        if (FruitDict.TryGetValue(id, out var fruit))
            return fruit;

        Debug.LogWarning($"Fruit ID '{id}' không tồn tại trong DataManager.");
        return null;
    }
}
