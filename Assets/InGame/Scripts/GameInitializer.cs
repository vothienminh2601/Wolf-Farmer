using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Khởi tạo toàn bộ farm lúc Start:
/// - Sinh các Plot.
/// - Phân loại (nhà, trồng trọt, chăn nuôi, đất trống).
/// - Gọi BuilderManager để xây dựng.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FarmManager fieldManager;

    [Header("Farm Settings")]
    [SerializeField] private int plotsPerRow = 3;
    [SerializeField] private int tilesPerPlot = 5;
    [SerializeField] private float tileSpacing = 2f;
    [SerializeField] private float plotGap = 1f;

    [Header("Materials")]
    [SerializeField] private Material cropPlotMaterial;
    [SerializeField] private Material animalPlotMaterial;
    [SerializeField] private Material housePlotMaterial;
    [SerializeField] private Material emptyPlotMaterial;

    void Start()
    {
        GenerateFarm();
        SetupInitialObjects();
    }

    // -------------------------------------------------------------
    // Tạo farm ban đầu
    // -------------------------------------------------------------
    private void GenerateFarm()
    {
        fieldManager.GenerateInitialPlots();
        Debug.Log("✅ Farm generated successfully.");
    }

    // -------------------------------------------------------------
    // Setup từng plot sau khi farm đã được sinh
    // -------------------------------------------------------------
    private void SetupInitialObjects()
    {
        if (fieldManager.Plots.Count == 0)
        {
            Debug.LogWarning("⚠️ No plots found to setup!");
            return;
        }

        // Toạ độ logic cố định cho demo
        Vector2Int houseCoord = new(-1, 1);
        Vector2Int[] cropCoords =
        {
            new(-1, -1),
            new(0, -1),
            new(1, -1)
        };
        Vector2Int[] animalCoords =
        {
            new(1, 0)
        };

        List<Plot> cropPlots = new();
        List<Plot> animalPlots = new();
        Plot housePlot = null;

        // Phân loại
        foreach (var kvp in fieldManager.Plots)
        {
            Vector2Int coord = kvp.Key;
            Plot plot = kvp.Value;

            if (coord == houseCoord)
                housePlot = plot;
            else if (System.Array.Exists(cropCoords, c => c == coord))
                cropPlots.Add(plot);
            else if (System.Array.Exists(animalCoords, c => c == coord))
                animalPlots.Add(plot);
        }

        // ---------------------------------------------------------
        // Plot trồng trọt
        // ---------------------------------------------------------
        foreach (var p in cropPlots)
        {
            BuilderManager.Instance.BuildFence(p);
            BuilderManager.Instance.BuildCropPlot(p);
        }

        // ---------------------------------------------------------
        // Plot chăn nuôi
        // ---------------------------------------------------------
        foreach (var p in animalPlots)
        {
            BuilderManager.Instance.BuildFence(p);
            BuilderManager.Instance.BuildAnimalPlot(p);
        }

        // ---------------------------------------------------------
        // Nhà
        // ---------------------------------------------------------
        if (housePlot != null)
        {
            BuilderManager.Instance.SetupPlot(housePlot, ePlotPurpose.Building, null);
            BuilderManager.Instance.BuildHouse(
                housePlot,
                new Vector3(0, 2, 0),
                Quaternion.Euler(0, 180, 0)
            );
        }

        // ---------------------------------------------------------
        // Các plot trống còn lại
        // ---------------------------------------------------------
        foreach (var kvp in fieldManager.Plots)
        {
            Plot plot = kvp.Value;
            if (!cropPlots.Contains(plot) && !animalPlots.Contains(plot) && plot != housePlot)
                BuilderManager.Instance.SetupPlot(plot, ePlotPurpose.Empty, null);
        }

        Debug.Log("✅ Setup complete: House(-1,1), Animal(1,0), Crops bottom row (-1,-1),(0,-1),(1,-1)");
    }
}
