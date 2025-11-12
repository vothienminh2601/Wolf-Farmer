using UnityEngine;
using System.Collections.Generic;

public enum eCropStage
{
    Seed = 0,
    Growing = 1,
    Mature = 2,
    Withered = 3
}

[System.Serializable]
public class CultivationData
{
    public Plot plot;
    public SeedData seed;

    public eCropStage CropStage;
    public float growthTimer;
    
    public int fruitCount;
    public float fruitTimer;

    public bool IsMature => CropStage == eCropStage.Mature;
    public bool IsDead => CropStage == eCropStage.Withered;

    private List<GameObject> stagePrefabs = new();

    public CultivationData(Plot plot, SeedData seed)
    {
        this.plot = plot;
        this.seed = seed;
        this.CropStage = eCropStage.Seed;
        this.growthTimer = 0f;

        // Load các prefab stage qua PrefabManager (Addressables)
        seed.LoadCropSteps(prefabs =>
        {
            stagePrefabs = prefabs;
            UpdateVisual();
        });
    }

    public void Tick(float deltaTime)
    {
        if (IsDead) return;

        if (CropStage != eCropStage.Mature)
        {
            growthTimer += deltaTime;
            if (growthTimer >= seed.stageDuration)
            {
                growthTimer = 0f;
                AdvanceStage();
            }
        }
        else
        {
            fruitTimer += deltaTime;
            if (fruitTimer >= seed.fruitInterval)
            {
                fruitTimer = 0f;
                SpawnFruit();
            }
        }
    }

    private void AdvanceStage()
    {
        if (CropStage == eCropStage.Seed)
            CropStage = eCropStage.Growing;
        else if (CropStage == eCropStage.Growing)
            CropStage = eCropStage.Mature;
        else
            CropStage = eCropStage.Withered;

        UpdateVisual();
    }

    private void SpawnFruit()
    {
        if (CropStage != eCropStage.Mature) return;
        fruitCount++;

        // Lấy FruitData từ Database qua ID
        var fruit = DataManager.GetProductById(seed.fruitId);
        if (fruit == null) return;

        foreach (Tile tile in plot.GetAllTiles())
        {
            if (tile == null || tile.Type != eTileType.Farming) continue;
            Vector3 pos = tile.transform.position + Vector3.up * 3f;

            ProductManager.Instance.SpawnProduct(fruit, pos, plot);
        }

        if (fruitCount >= seed.maxFruitCount)
            CropStage = eCropStage.Withered;
    }

    private void UpdateVisual()
    {
        if (plot == null || stagePrefabs == null || stagePrefabs.Count == 0) return;

        int stageIndex = (int)CropStage;
        stageIndex = Mathf.Clamp(stageIndex, 0, stagePrefabs.Count - 1);
        GameObject prefab = stagePrefabs[stageIndex];
        if (prefab == null) return;

        foreach (Tile tile in plot.GetAllTiles())
        {
            if (tile == null || tile.Type != eTileType.Farming) continue;
            tile.RemovePlacement();
            tile.PlaceObject(prefab);
        }
    }
}
