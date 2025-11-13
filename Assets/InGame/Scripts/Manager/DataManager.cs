using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public TextAsset productCSV;
    public TextAsset seedCSV;
    public TextAsset animalCSV;

    public static Dictionary<string, SeedData> SeedDict { get; private set; } = new();
    public static Dictionary<string, ProductData> ProductDict { get; private set; } = new();
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
        if (reader != null && productCSV != null)
            ProductDict = await reader.LoadDataFromCSV(productCSV);
        else
            ProductDict = new Dictionary<string, ProductData>();

        Debug.Log($"Loaded {ProductDict.Count} Products");
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

    public static AnimalData GetAnimalById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("GetAnimalById: id trống hoặc null");
            return null;
        }

        if (AnimalDict.TryGetValue(id, out var animal))
            return animal;

        Debug.LogWarning($"Animal ID '{id}' không tồn tại trong DataManager.");
        return null;
    }


    public static ProductData GetProductById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("GetFruitById: id trống hoặc null");
            return null;
        }

        if (ProductDict.TryGetValue(id, out var product))
            return product;

        Debug.LogWarning($"Product ID '{id}' không tồn tại trong DataManager.");
        return null;
    }
}
