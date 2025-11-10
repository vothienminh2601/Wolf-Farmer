using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ hoạt động canh tác.
/// Mỗi plot có 1 CultivationData, chia sẻ cùng stage giữa các tile.
/// </summary>
public class CultivationManager : Singleton<CultivationManager>
{

    [Header("Settings")]
    [SerializeField] private float updateInterval = 1f;

    [SerializeField] private List<CultivationData> activePlots = new();
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            TickCultivation();
        }
    }

    public CultivationData GetCultivationData(Plot plot)
    {
        return activePlots.Find(p => p.plot == plot);
    }

    public void RegisterCropPlot(Plot plot, ItemData seedItem)
    {
        if (plot == null || seedItem == null || seedItem.itemSO == null)
        {
            Debug.LogWarning("⚠️ RegisterCropPlot: Dữ liệu không hợp lệ!");
            return;
        }

        SeedSO seed = seedItem.itemSO as SeedSO;
        if (seed == null)
        {
            Debug.LogWarning("⚠️ Item không phải loại SeedSO!");
            return;
        }

        // Nếu plot đã được canh tác, bỏ qua
        if (activePlots.Exists(p => p.plot == plot))
            return;

        CultivationData data = new CultivationData(plot, seed);
        activePlots.Add(data);

        Debug.Log($"🌱 Bắt đầu canh tác {seed.itemName} trên plot ({plot.PlotX},{plot.PlotZ})");
    }

    public void UnregisterPlot(Plot plot)
    {
        if (plot == null) return;
        activePlots.RemoveAll(p => p.plot == plot);
    }


    public void HarvestPlot(Plot plot)
    {
        if (plot == null) return;

        CultivationData data = activePlots.Find(p => p.plot == plot);
        if (data == null)
        {
            Debug.LogWarning("⚠️ Plot chưa được canh tác!");
            return;
        }

        if (!data.IsReadyToHarvest)
        {
            Debug.Log($"⏳ Plot ({plot.PlotX},{plot.PlotZ}) chưa sẵn sàng thu hoạch.");
            return;
        }

        // TODO: thêm logic sản phẩm thu hoạch sau này
        BuilderManager.Instance.ClearPlot(plot);
        UnregisterPlot(plot);

        Debug.Log($"✅ Đã thu hoạch {data.seed.itemName} trên plot ({plot.PlotX},{plot.PlotZ})");
    }


    public List<Plot> GetReadyPlots()
    {
        List<Plot> ready = new();
        foreach (var p in activePlots)
        {
            if (p.IsReadyToHarvest)
                ready.Add(p.plot);
        }
        return ready;
    }
    private void TickCultivation()
    {
        foreach (var data in activePlots)
        {
            data.Tick(updateInterval);
        }
    }
}
