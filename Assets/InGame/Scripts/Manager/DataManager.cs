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

    public static Dictionary<string, SeedData> SeedDict { get; private set; } = new();
    public static Dictionary<string, FruitData> FruitDict { get; private set; } = new();

    void Awake()
    {
        LoadAllData();
    }

    [ContextMenu("LoadData")]
    public async void LoadAllData()
    {
        await LoadFruits();
        await LoadSeeds();
    }

    async UniTask LoadFruits()
    {
        var reader = CSVReaderFactory.CreateReader<FruitData>();
        if (reader != null && fruitCSV != null)
            FruitDict = await reader.LoadDataFromCSV(fruitCSV);
        else
            FruitDict = new Dictionary<string, FruitData>();

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
    public static FruitData GetFruitById(string id)
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
