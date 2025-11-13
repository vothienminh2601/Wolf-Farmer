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

    void Start()
    {
        GenerateFarm();
        SetupInitialObjects();
        UserData.Instance.LoadUserProfile(hasProfile =>
        {
            Debug.Log("📂 Found existing profile → loading game...");
            UserData.Instance.LoadGame();
        });
    }

    // Tạo farm ban đầu
    private void GenerateFarm()
    {
        fieldManager.GenerateInitialPlots();
        Debug.Log("✅ Farm generated successfully.");
    }


    // Setup từng plot sau khi farm đã được sinh
    private void SetupInitialObjects()
    {
        if (fieldManager.Plots.Count == 0)
        {
            Debug.LogWarning("⚠️ No plots found to setup!");
            return;
        }

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

        foreach (var p in cropPlots)
        {
            BuilderManager.Instance.BuildFence(p);
            BuilderManager.Instance.BuildCultivationPlot(p);
        }

        if (housePlot != null)
        {
            FarmManager.Instance.SetupPlot(housePlot, ePlotPurpose.Building, null);
            BuilderManager.Instance.BuildHouse(
                housePlot,
                new Vector3(0, 2, 0),
                Quaternion.Euler(0, 180, 0)
            );
        }

        foreach (var kvp in fieldManager.Plots)
        {
            Plot plot = kvp.Value;
            if (!cropPlots.Contains(plot) && !animalPlots.Contains(plot) && plot != housePlot)
                FarmManager.Instance.SetupPlot(plot, ePlotPurpose.Empty, null);
        }

    }
}
