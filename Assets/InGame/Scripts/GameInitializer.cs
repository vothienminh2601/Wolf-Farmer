using UnityEngine;
using System.Collections.Generic;

public class GameInitializer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FarmManager fieldManager;
    [SerializeField] private GameObject housePrefab;
    [SerializeField] private GameObject cropMarkerPrefab;
    [SerializeField] private GameObject animalMarkerPrefab;
    
    [Header("Fence Prefabs")]
    [SerializeField] private GameObject fenceIPrefab;  // hàng rào cạnh
    [SerializeField] private GameObject fenceLPrefab;  // hàng rào góc

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
    // Generate Farm
    // -------------------------------------------------------------
    private void GenerateFarm()
    {
        fieldManager.GenerateInitialPlots();
        Debug.Log("✅ Farm generated thành công.");
    }

    // -------------------------------------------------------------
    // Setup các loại plot
    // -------------------------------------------------------------
    private void SetupInitialObjects()
    {
        if (fieldManager.Plots.Count == 0)
        {
            Debug.LogWarning("⚠️ Chưa có plot nào để setup!");
            return;
        }

        // --- Định nghĩa vị trí cố định ---
        Vector2Int houseCoord = new Vector2Int(-1, 1);
        Vector2Int[] cropCoords =
        {
        new Vector2Int(-1, -1),
        new Vector2Int( 0, -1),
        new Vector2Int( 1, -1)
    };
        Vector2Int[] animalCoords =
        {
        new Vector2Int(1, 0)
    };

        // --- Danh sách ---
        List<FarmPlot> cropPlots = new();
        List<FarmPlot> animalPlots = new();
        FarmPlot housePlot = null;

        // --- Phân loại theo tọa độ ---
        foreach (var kvp in fieldManager.Plots)
        {
            Vector2Int coord = kvp.Key;
            FarmPlot plot = kvp.Value;

            if (coord == houseCoord)
            {
                housePlot = plot;
            }
            else if (System.Array.Exists(cropCoords, c => c == coord))
            {
                cropPlots.Add(plot);
            }
            else if (System.Array.Exists(animalCoords, c => c == coord))
            {
                animalPlots.Add(plot);
            }
        }

        // --- Gán vật thể ---
        foreach (var p in cropPlots)
        {
            SetupPlot(p, "Farming Plot", cropMarkerPrefab);
            SetCenterTilesToType(p, eTileType.Farming);
            PlaceFencesForFarmingPlot(p);
        }

        foreach (var p in animalPlots)
        {
            SetupPlot(p, "Animal Plot", animalMarkerPrefab);
            SetCenterTilesToType(p, eTileType.Animal);
        }

        if (housePlot != null)
        {
            SetupPlot(housePlot, "House Plot", housePrefab, new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(0, 180, 0)));
        }

        // --- Các plot còn lại = đất trống ---
        foreach (var kvp in fieldManager.Plots)
        {
            var plot = kvp.Value;
            if (!cropPlots.Contains(plot) && !animalPlots.Contains(plot) && plot != housePlot)
                SetupPlot(plot, "Empty Plot", null);
        }

        Debug.Log("✅ Setup complete: house(-1,1), animal(1,0), crops bottom row (-1,-1),(0,-1),(1,-1)");
    }


    private FarmPlot GetCenterPlot()
    {
        FarmPlot center = null;
        float minDist = float.MaxValue;
        foreach (var plot in fieldManager.Plots)
        {
            float dist = plot.Value.transform.position.sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                center = plot.Value;
            }
        }
        return center;
    }

    private void SetupPlot(FarmPlot plot, string name, GameObject markerPrefab , Vector3 pos = default, Quaternion rot = default)
    {
        plot.name = name + $" ({plot.PlotX},{plot.PlotZ})";

        if (markerPrefab != null)
        {
            GameObject marker = Instantiate(markerPrefab, plot.transform);
            marker.transform.localPosition = pos;
            marker.transform.localRotation = rot;
        }
    }

    private void SetCenterTilesToType(FarmPlot plot, eTileType type)
    {
        int total = plot.TilesPerRow;
        int start = (total - 3) / 2;
        int end = start + 3;

        for (int x = start; x < end; x++)
        {
            for (int z = start; z < end; z++)
            {
                Tile tile = plot.GetTile(x, z);
                if (tile != null)
                    tile.SetType(type);
            }
        }
    }

    // -------------------------------------------------------------
    // 🪵 Đặt hàng rào quanh plot trồng cây
    // -------------------------------------------------------------
    private void PlaceFencesForFarmingPlot(FarmPlot plot)
    {
        if (fenceIPrefab == null || fenceLPrefab == null)
        {
            Debug.LogWarning("⚠️ Chưa gán prefab Fence I / Fence L!");
            return;
        }

        int n = plot.TilesPerRow;
        int midX = n / 2; // ô giữa hàng trên cùng (ví dụ 2 nếu 5x5)

        // --- Góc (Fence L) ---
        plot.GetTile(0, 0)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));       // bottom-left
        plot.GetTile(n - 1, 0)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, -90, 0)); // bottom-right
        plot.GetTile(0, n - 1)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0));  // top-left
        plot.GetTile(n - 1, n - 1)?.PlaceObject(fenceLPrefab, Vector3.zero, Quaternion.Euler(0, -180, 0)); // top-right

        // --- Cạnh dưới (Fence I) ---
        for (int x = 1; x < n - 1; x++)
        {
            var tile = plot.GetTile(x, 0);
            tile?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        // --- Cạnh trên (Fence I) ---
        for (int x = 1; x < n - 1; x++)
        {
            if (x == midX) continue; // bỏ trống ô giữa làm cổng
            var tile = plot.GetTile(x, n - 1);
            tile?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, -180, 0));
        }

        // --- Cạnh trái (Fence I) ---
        for (int z = 1; z < n - 1; z++)
        {
            var tile = plot.GetTile(0, z);
            tile?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, 90, 0));
        }

        // --- Cạnh phải (Fence I) ---
        for (int z = 1; z < n - 1; z++)
        {
            var tile = plot.GetTile(n - 1, z);
            tile?.PlaceObject(fenceIPrefab, Vector3.zero, Quaternion.Euler(0, -90, 0));
        }

        Debug.Log($"🪵 Đã đặt hàng rào quanh plot ({plot.PlotX},{plot.PlotZ}), chừa cổng giữa cạnh trên (x={midX}, z={n - 1}).");
    }

}
