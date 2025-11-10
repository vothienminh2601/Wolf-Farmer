using UnityEngine;

[System.Serializable]
public class CultivationData
{
    public Plot plot;
    public SeedSO seed;
    public int currentStage;      // index của cropSteps
    public float growthTimer;     // thời gian đã trôi qua
    public float stageDuration = 30f; // mỗi giai đoạn (mặc định 30s)

    public bool IsReadyToHarvest => currentStage >= seed.cropSteps.Count - 1;

    public CultivationData(Plot plot, SeedSO seed)
    {
        this.plot = plot;
        this.seed = seed;
        this.currentStage = 0;
        this.growthTimer = 0f;
        UpdateVisual();
    }

    public void Tick(float deltaTime)
    {
        if (IsReadyToHarvest) return;

        growthTimer += deltaTime;
        if (growthTimer >= stageDuration)
        {
            growthTimer = 0f;
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        currentStage++;
        if (currentStage >= seed.cropSteps.Count)
            currentStage = seed.cropSteps.Count - 1;

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (plot == null || seed == null) return;
        if (seed.cropSteps == null || seed.cropSteps.Count == 0) return;


        GameObject prefab = seed.cropSteps[currentStage];
        if (prefab == null) return;

        
        foreach (Tile tile in plot.GetAllTiles())
        {
            if (tile == null || tile.Type != eTileType.Farming) continue;
            tile.RemovePlacement();

            tile.PlaceObject(prefab);
        }

        Debug.Log($"🌿 Plot ({plot.PlotX},{plot.PlotZ}) → Stage {currentStage + 1}/{seed.cropSteps.Count}");
    }
}
