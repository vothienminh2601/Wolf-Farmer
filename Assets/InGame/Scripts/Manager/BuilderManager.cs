using UnityEngine;
using System;

public class BuilderManager : Singleton<BuilderManager>
{
    [Header("Prefabs")]
    [SerializeField] private GameObject fenceIPrefab;
    [SerializeField] private GameObject fenceLPrefab;
    [SerializeField] private GameObject housePrefab;
    [SerializeField] public GameObject cropMarkerPrefab;
    [SerializeField] public GameObject animalMarkerPrefab;

    public static event Action<Plot> OnRequireCultivationTypeSelect; 

    public void BuildHouse(Plot plot, Vector3 pos = default, Quaternion rot = default)
    {
        if (plot == null || housePrefab == null)
        {
            Debug.LogWarning("⚠️ Không thể BuildHouse: plot hoặc prefab null");
            return;
        }

        GameObject house = Instantiate(housePrefab, plot.transform);
        house.transform.localPosition = pos;
        house.transform.localRotation = rot;

        plot.name = $"House Plot ({plot.PlotX},{plot.PlotZ})";
        Debug.Log($"House built at plot ({plot.PlotX},{plot.PlotZ})");

    }

    public void BuildCultivationPlot(Plot plot)
    {
        if (plot == null)
        {
            Debug.LogWarning("⚠️ BuildCultivationPlot: plot null");
            return;
        }

        Debug.Log($"🌱 Bắt đầu dựng đất canh tác tại plot ({plot.PlotX},{plot.PlotZ})...");

        FarmManager.Instance.SetupPlot(plot, ePlotPurpose.Cultivation, null);

        BuildFence(plot);

        OnRequireCultivationTypeSelect?.Invoke(plot);
    }
    public void BuildCropPlot(Plot plot)
    {
        if (plot == null)
        {
            Debug.LogWarning("⚠️ BuildCropPlot: plot null");
            return;
        }

        FarmManager.Instance.SetupPlot(plot, ePlotPurpose.Farming, cropMarkerPrefab);

        int total = GameConfigs.TILES_PER_PLOT;
        int start = (total - 3) / 2;
        int end = start + 3;

        for (int x = start; x < end; x++)
        {
            for (int z = start; z < end; z++)
            {
                Tile tile = plot.GetTile(x, z);
                if (tile != null)
                    tile.SetType(eTileType.Farming);
            }
        }

        Debug.Log($"🌾 Đã hoàn tất dựng đất trồng trọt cho plot ({plot.PlotX},{plot.PlotZ})");
    }

    public void BuildAnimalPlot(Plot plot)
    {
        if (plot == null)
        {
            Debug.LogWarning("⚠️ BuildAnimalPlot: plot null");
            return;
        }

        FarmManager.Instance.SetupPlot(plot, ePlotPurpose.Animal, animalMarkerPrefab);

        int total = GameConfigs.TILES_PER_PLOT;
        int start = (total - 3) / 2;
        int end = start + 3;

        for (int x = start; x < end; x++)
        {
            for (int z = start; z < end; z++)
            {
                Tile tile = plot.GetTile(x, z);
                if (tile != null)
                    tile.SetType(eTileType.Animal);
            }
        }

        Debug.Log($"🐮 Đã hoàn tất dựng đất chăn nuôi cho plot ({plot.PlotX},{plot.PlotZ})");
    }

    public void BuildFence(Plot plot)
    {
        if (plot == null)
        {
            Debug.LogWarning("⚠️ BuildFence: plot null");
            return;
        }

        if (fenceIPrefab == null || fenceLPrefab == null)
        {
            Debug.LogWarning("⚠️ Chưa gán Fence prefab!");
            return;
        }

        int n = GameConfigs.TILES_PER_PLOT;
        int mid = n / 2;

        // Lấy FarmManager
        FarmManager farm = FindAnyObjectByType<FarmManager>();
        if (farm == null)
        {
            Debug.LogError("Không tìm thấy FarmManager trong scene!");
            return;
        }

        // Kiểm tra xem có plot ở 4 hướng hay không
        bool hasTop = farm.HasPlotAt(new Vector2Int(plot.PlotX, plot.PlotZ + 1));
        bool hasBottom = farm.HasPlotAt(new Vector2Int(plot.PlotX, plot.PlotZ - 1));
        bool hasLeft = farm.HasPlotAt(new Vector2Int(plot.PlotX - 1, plot.PlotZ));
        bool hasRight = farm.HasPlotAt(new Vector2Int(plot.PlotX + 1, plot.PlotZ));

        // --- Góc ---
        plot.GetTile(0, 0)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));        // bottom-left
        plot.GetTile(n - 1, 0)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, -90, 0));  // bottom-right
        plot.GetTile(0, n - 1)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0));   // top-left
        plot.GetTile(n - 1, n - 1)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, 180, 0)); // top-right

        // --- Cạnh dưới (mở nếu có plot dưới) ---
        for (int x = 1; x < n - 1; x++)
        {
            if (hasBottom && x == mid) continue; // mở cổng dưới nếu có plot dưới
            plot.GetTile(x, 0)?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.identity);
        }

        // --- Cạnh trên (mở nếu có plot trên) ---
        for (int x = 1; x < n - 1; x++)
        {
            if (hasTop && x == mid) continue; // mở cổng trên nếu có plot trên
            plot.GetTile(x, n - 1)?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, 180, 0));
        }

        // --- Cạnh trái (mở nếu có plot trái) ---
        for (int z = 1; z < n - 1; z++)
        {
            if (hasLeft && z == mid) continue; // mở cổng trái nếu có plot trái
            plot.GetTile(0, z)?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0));
        }

        // --- Cạnh phải (mở nếu có plot phải) ---
        for (int z = 1; z < n - 1; z++)
        {
            if (hasRight && z == mid) continue;
            plot.GetTile(n - 1, z)?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, -90, 0));
        }
    }

    public void ClearPlot(Plot plot)
    {
        if (plot == null)
        {
            return;
        }

        int clearedCount = 0;

        foreach (Tile tile in plot.GetAllTiles())
        {
            if (tile == null) continue;

            if (tile.transform.childCount > 0)
            {
                tile.RemovePlacement();

                clearedCount++;
            }

            tile.SetType(eTileType.Empty);
        }

        FarmManager.Instance.SetupPlot(plot, ePlotPurpose.Empty, null);

        Debug.Log($"🧹 Cleared plot ({plot.PlotX},{plot.PlotZ}) — removed {clearedCount} objects, reset all tiles.");
    }
}
