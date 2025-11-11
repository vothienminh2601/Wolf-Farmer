using UnityEngine;

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
    public SeedSO seed;

    public eCropStage CropStage;
    public float growthTimer;
    public float stageDuration = 30f;
    [Header("Fruit")]
    public int fruitCount;
    public int maxFruitCount = 5;
    public float fruitInterval = 10f;
    public float fruitTimer;

    public bool IsMature => (int)CropStage >= seed.cropSteps.Count - 2;  // Stage cuối
    public bool IsDead => fruitCount >= maxFruitCount;

    public CultivationData(Plot plot, SeedSO seed)
    {
        this.plot = plot;
        this.seed = seed;
        this.CropStage = 0;
        this.growthTimer = 0f;

        // Đọc dữ liệu từ SeedSO nếu có
        if (seed != null)
        {
            stageDuration = seed.stageDuration > 0 ? seed.stageDuration : stageDuration;
            maxFruitCount = seed.maxFruitCount > 0 ? seed.maxFruitCount : maxFruitCount;
            fruitInterval = seed.fruitInterval > 0 ? seed.fruitInterval : fruitInterval;
        }

        UpdateVisual();
    }

    public void Tick(float deltaTime)
    {
        if (IsDead) return;

        if (!IsMature)
        {
            // Phát triển tới stage 3
            growthTimer += deltaTime;
            if (growthTimer >= stageDuration)
            {
                growthTimer = 0f;
                AdvanceStage();
            }
        }
        else
        {
            // Khi đã trưởng thành: bắt đầu ra trái định kỳ
            fruitTimer += deltaTime;
            if (fruitTimer >= fruitInterval)
            {
                fruitTimer = 0f;
                SpawnFruit();
            }
        }
    }

    private void AdvanceStage()
    {
        // Chặn không cho vượt qua stage trưởng thành
        if ((int)CropStage >= seed.cropSteps.Count - 1)
        {
            CropStage = (eCropStage)(seed.cropSteps.Count - 1);
            return;
        }

        CropStage++;
        UpdateVisual();
    }

    private void SpawnFruit()
    {
        if (IsDead) return;

        fruitCount++;

        foreach (Tile tile in plot.GetAllTiles())
        {
            if (tile == null || tile.Type != eTileType.Farming) continue;
            Vector3 pos = tile.transform.position + new Vector3(0, 3, 0);

            // Gọi FruitManager spawn prefab và quản lý
            FruitManager.Instance.SpawnFruit(seed, seed.fruitPrefab, pos, plot);
        }

        if (fruitCount >= maxFruitCount)
        {
            // Die();
        }
    }

    private void Die()
    {
        BuilderManager.Instance.ClearPlot(plot);
        CultivationManager.Instance.UnregisterPlot(plot);
    }

    private void UpdateVisual()
    {
        if (plot == null || seed == null) return;
        if (seed.cropSteps == null || seed.cropSteps.Count == 0) return;

        GameObject prefab = seed.cropSteps[Mathf.Clamp((int)CropStage, 0, seed.cropSteps.Count - 1)];
        if (prefab == null) return;

        foreach (Tile tile in plot.GetAllTiles())
        {
            if (tile == null || tile.Type != eTileType.Farming) continue;
            tile.RemovePlacement();
            tile.PlaceObject(prefab);
        }
    }
}
