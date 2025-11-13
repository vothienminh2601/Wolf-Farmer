using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.AI.Navigation;
using NUnit.Framework;

public class FarmManager : Singleton<FarmManager>
{
    [Header("Prefabs & Settings")]
    [SerializeField] private NavMeshSurface navSurface;
    [SerializeField] private GameObject plotPrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private float plotGap = 1f;


    [Header("Runtime Data")]
    [SerializeField] private List<Plot> farmPlots = new();
    private readonly Dictionary<Vector2Int, Plot> _plots = new();
    private readonly Dictionary<Vector2Int, Tile> _globalTileMap = new();

    public static event Action<int, int, List<Plot>> OnPlotChanged;
    public Dictionary<Vector2Int, Plot> Plots => _plots;


    [ContextMenu("Ref Components")]
    void RefComponents()
    {
        farmPlots = GetComponentsInChildren<Plot>(true).ToList();
    }

    public void ExpandFarm()
    {
        int cost = GameConfigs.PRICE_NEW_PLOT;

        if (ResourceManager.Instance.GetCoin() < cost)
        {
            Debug.LogWarning($"Không đủ tiền để mở rộng ({cost} coin)");
            return;
        }
        ResourceManager.Instance.SpendCoin(cost);

        Vector2Int nextCoord = GetNextPlotCoord();
        if (_plots.ContainsKey(nextCoord))
        {
            Debug.LogWarning($"Plot {nextCoord} đã tồn tại!");
            return;
        }

        CreatePlotAt(nextCoord);
        // UserData.Instance.SaveGame();

        Debug.Log($"Mở rộng farm: thêm Plot_{nextCoord.x}_{nextCoord.y} với giá {cost} coin");
    }

    private Vector2Int GetNextPlotCoord()
    {
        int count = _plots.Count;
        if (count == 0) return Vector2Int.zero;

        int size = Mathf.CeilToInt(Mathf.Sqrt(count));
        int maxCount = size * size;

        if (count < maxCount + 1)
        {
            int half = size / 2 ;
            for (int x = -half; x <= half; x++)
            {
                for (int z = -half; z <= half; z++)
                {
                    Vector2Int coord = new(x, z);
                    if (!_plots.ContainsKey(coord))
                        return coord;
                }
            }
        }

        int newSize = size + 1;
        int newHalf = newSize/ 2;
        for (int x = -newHalf; x <= newHalf; x++)
        {
            for (int z = -newHalf + 1; z <= newHalf; z++)
            {
                Vector2Int coord = new(x, z);
                if (!_plots.ContainsKey(coord))
                    return coord;
            }
        }

        return Vector2Int.zero;
    }

    public void GenerateInitialPlots()
    {
        int half = GameConfigs.PLOT_ROW_STARTER / 2;
        for (int x = -half; x <= half; x++)
        {
            for (int z = -half; z <= half; z++)
            {
                CreatePlotAt(new Vector2Int(x, z));
            }
        }

        // UserData.Instance.SaveGame();
    }

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
        if (plot == null)
            plot = plotObj.AddComponent<Plot>();

        plot.Initialize(this, coord.x, coord.y, GameConfigs.TILES_SPACING, tilePrefab, baseMaterial);
        _plots.Add(coord, plot);

        foreach (Tile t in plot.GetAllTiles())
        {
            Vector2Int tileCoord = new(t.GlobalX, t.GlobalZ);
            if (!_globalTileMap.ContainsKey(tileCoord))
                _globalTileMap.Add(tileCoord, t);
        }

        farmPlots.Add(plot);
    }

    public void SetupPlot(Plot plot, ePlotPurpose purpose, GameObject markerPrefab = null,
                          Vector3 pos = default, Quaternion rot = default)
    {
        if (plot == null) return;
        plot.Purpose = purpose;
        plot.name = $"{name} ({plot.PlotX},{plot.PlotZ})";

        if (markerPrefab != null)
        {
            GameObject marker = Instantiate(markerPrefab, plot.transform);
            marker.transform.localPosition = pos;
            marker.transform.localRotation = rot;
        }

        OnPlotChanged?.Invoke(GetActivePlotCount(), GetEmptyPlotCount(), farmPlots);
        Debug.Log($"SetupPlot: {name} tại ({plot.PlotX},{plot.PlotZ})");
    }

    public Tile GetTileAtGlobal(int gx, int gz)
    {
        _globalTileMap.TryGetValue(new Vector2Int(gx, gz), out var tile);
        return tile;
    }

    [Test]
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
    public int GetEmptyPlotCount() => _plots.Count(p => p.Value.Purpose == ePlotPurpose.Empty);

    public List<Plot> GetEmptyPlots()
    {
        List<Plot> result = new();
        foreach (var p in _plots.Values)
        {
            if (p == null) continue;
            if (p.Purpose == ePlotPurpose.Empty)
                result.Add(p);
        }
        return result;
    }

    public Plot GetNearestEmptyPlot(Vector3 fromPos)
    {
        Plot nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var p in GetEmptyPlots())
        {
            float dist = Vector3.Distance(fromPos, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p;
            }
        }
        return nearest;
    }

    public List<Plot> GetFarmingPlots()
    {
        List<Plot> result = new();
        foreach (var p in _plots.Values)
        {
            if (p == null) continue;
            if (p.Purpose == ePlotPurpose.Farming)
                result.Add(p);
        }
        return result;
    }

    public List<Plot> GetAnimalPlots()
    {
        List<Plot> result = new();
        foreach (var p in _plots.Values)
        {
            if (p == null) continue;
            if (p.Purpose == ePlotPurpose.Animal)
                result.Add(p);
        }
        return result;
    }

    public Plot GetPlot(Vector2Int coord)
    {
        _plots.TryGetValue(coord, out var plot);
        return plot;
    }



    #region SAVE / LOAD FARM STATE
    public FarmSaveData GetSaveData()
    {
        FarmSaveData data = new();

        foreach (var kv in _plots)
        {
            Plot plot = kv.Value;
            if (plot == null) continue;

            PlotSaveData pData = new()
            {
                plotX = plot.PlotX,
                plotZ = plot.PlotZ,
                purpose = plot.Purpose
            };

            // --- Nếu là cây trồng ---
            if (plot.Purpose == ePlotPurpose.Farming)
            {
                var c = CultivationManager.Instance.GetCultivationData(plot);
                if (c != null && c.seed != null)
                {
                    pData.cultivation = new CultivationSaveData()
                    {
                        seedId = c.seed.id,
                        stage = c.CropStage,
                        growthTimer = c.growthTimer,
                        fruitCount = c.fruitCount,
                        fruitTimer = c.fruitTimer
                    };
                }
            }

            // --- Nếu là vật nuôi ---
            if (plot.Purpose == ePlotPurpose.Animal)
            {
                var a = AnimalManager.Instance.GetAnimalData(plot);
                if (a != null && a.data != null)
                {
                    pData.animal = new AnimalSaveData()
                    {
                        animalId = a.data.id,
                        growthTimer = a.growTimer,
                        productCount = a.productCount,
                        isAdult = a.IsAdult
                    };
                }
            }

            data.plots.Add(pData);
        }

        Debug.Log($"FarmManager: Collected save data with {data.plots.Count} plots.");
        return data;
    }


    public void LoadFromSave(FarmSaveData data)
    {
        if (data == null || data.plots.Count == 0)
        {
            Debug.Log("FarmManager: No farm data found, generating new plots.");
            GenerateInitialPlots();
            return;
        }

        // Dọn farm hiện tại
        foreach (var kv in _plots)
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);

        _plots.Clear();
        farmPlots.Clear();

        foreach (var pData in data.plots)
        {
            Vector2Int coord = new(pData.plotX, pData.plotZ);
            CreatePlotAt(coord);
            Plot plot = _plots[coord];
            plot.SetPurpose(pData.purpose);


            if (pData.cultivation != null)
            {
                var seed = DataManager.GetSeedById(pData.cultivation.seedId);
                if (seed != null)
                {
                    CultivationManager.Instance.RegisterCropPlot(plot, seed);
                    BuilderManager.Instance.BuildFence(plot);

                    var cData = CultivationManager.Instance.GetCultivationData(plot);
                    if (cData != null)
                    {
                        cData.CropStage = pData.cultivation.stage;
                        cData.growthTimer = pData.cultivation.growthTimer;
                        cData.fruitCount = pData.cultivation.fruitCount;
                        cData.fruitTimer = pData.cultivation.fruitTimer;
                    }
                }
            }

            if (pData.animal != null)
            {
                var animal = DataManager.GetAnimalById(pData.animal.animalId);
                if (animal != null)
                {
                    AnimalManager.Instance.AddAnimal(animal, plot);
                    BuilderManager.Instance.BuildFence(plot);
                    var aData = AnimalManager.Instance.GetAnimalData(plot);
                    if (aData != null)
                    {
                        aData.growTimer = pData.animal.growthTimer;
                        aData.productCount = pData.animal.productCount;
                    }
                }
            }
        }

        Debug.Log($"FarmManager: Loaded {data.plots.Count} plots from save.");
    }
    #endregion

}
