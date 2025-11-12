using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FarmManager : Singleton<FarmManager>
{
    [Header("Prefabs & Settings")]
    [SerializeField] private GameObject plotPrefab; 
    [SerializeField] private GameObject tilePrefab; 
    [SerializeField] private Material baseMaterial;
    [SerializeField] private int plotsPerRow = 3;
    [SerializeField] private float plotGap = 1f; 

    [SerializeField] private List<Plot> farmPlots = new();

    // Từ điển lưu tất cả plot và tile theo tọa độ
    private readonly Dictionary<Vector2Int, Plot> _plots = new();
    public Dictionary<Vector2Int, Plot> Plots => _plots;
    private readonly Dictionary<Vector2Int, Tile> _globalTileMap = new();

    [ContextMenu("Ref Components")]
    void RefComponents()
    {
        farmPlots = GetComponentsInChildren<Plot>(true).ToList();
    }
    private void RegisterExistingPlots()
    {
        _plots.Clear();
        _globalTileMap.Clear();


        foreach (var plot in farmPlots)
        {
            Vector2Int coord = new Vector2Int(plot.PlotX, plot.PlotZ);
            if (!_plots.ContainsKey(coord))
                _plots.Add(coord, plot);

            // đăng ký tile toàn cục
            foreach (Tile t in plot.GetAllTiles())
            {
                Vector2Int tileCoord = new Vector2Int(t.GlobalX, t.GlobalZ);
                if (!_globalTileMap.ContainsKey(tileCoord))
                    _globalTileMap.Add(tileCoord, t);
            }
        }

        Debug.Log($"FarmFieldManager: Loaded {_plots.Count} plots ({_globalTileMap.Count} tiles) from scene.");
    }

    public void GenerateInitialPlots()
    {
        int half = plotsPerRow / 2;
        for (int x = -half; x <= half; x++)
        {
            for (int z = -half; z <= half; z++)
            {
                CreatePlotAt(new Vector2Int(x, z));
            }
        }
        Debug.Log($"FarmField: sinh {_plots.Count} plot ({plotsPerRow}x{plotsPerRow}) thành công!");
    }

    // --------------------------------------------
    // Tạo 1 plot tại tọa độ logic (plotX, plotZ)
    // --------------------------------------------
    public void CreatePlotAt(Vector2Int coord)
    {
        if (_plots.ContainsKey(coord)) return;

        float plotSize = (GameConfigs.TILES_PER_PLOT - 1) * GameConfigs.TILES_SPACING;
        Vector3 pos = new Vector3(
            coord.x * (plotSize + plotGap),
            0,
            coord.y * (plotSize + plotGap)
        );

        GameObject plotObj = Instantiate(plotPrefab, pos, Quaternion.identity, transform);
        plotObj.name = $"Plot_{coord.x}_{coord.y}";

        Plot plot = plotObj.GetComponent<Plot>();
        if (plot == null) plot = plotObj.AddComponent<Plot>();

        plot.Initialize(this, coord.x, coord.y, GameConfigs.TILES_SPACING, tilePrefab, baseMaterial);
        _plots.Add(coord, plot);

        // Đăng ký các tile của plot vào global map
        foreach (Tile t in plot.GetAllTiles())
        {
            Vector2Int tileCoord = new Vector2Int(t.GlobalX, t.GlobalZ);
            if (!_globalTileMap.ContainsKey(tileCoord))
                _globalTileMap.Add(tileCoord, t);
        }
    }

    // --------------------------------------------
    // Lấy tile theo tọa độ toàn cục
    // --------------------------------------------
    public Tile GetTileAtGlobal(int gx, int gz)
    {
        _globalTileMap.TryGetValue(new Vector2Int(gx, gz), out var tile);
        return tile;
    }

    // --------------------------------------------
    // Lấy tile xung quanh tile được chọn
    // --------------------------------------------
    public List<Tile> GetNeighborTiles(Tile center)
    {
        List<Tile> result = new();
        Vector2Int[] dirs =
        {
            new( 1,  0),
            new(-1,  0),
            new( 0,  1),
            new( 0, -1)
        };

        foreach (var d in dirs)
        {
            int nx = center.GlobalX + d.x;
            int nz = center.GlobalZ + d.y;
            Tile neighbor = GetTileAtGlobal(nx, nz);
            if (neighbor != null)
                result.Add(neighbor);
        }

        return result;
    }

    public bool HasPlotAt(Vector2Int coord) => _plots.ContainsKey(coord);

    public int GetTotalPlotCount() => _plots.Count;
    public int GetActivePlotCount() => _plots.Count(p => p.Value.Purpose != ePlotPurpose.Empty);
    public int GetPlotCountByEnum(ePlotPurpose purpose)
    {
        int count = 0;
        foreach (var plot in _plots)
        {
            if (plot.Value == null) continue;
            if (plot.Value.Purpose == purpose)
                count++;
        }
        return count;
    }
    
    public Plot GetPlot(Vector2Int coord)
    {
        _plots.TryGetValue(coord, out var plot);
        return plot;
    }
}
