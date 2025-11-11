using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ hoạt động canh tác.
/// Mỗi plot có một CultivationData.
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

    private void TickCultivation()
    {
        for (int i = activePlots.Count - 1; i >= 0; i--)
        {
            CultivationData data = activePlots[i];
            if (data == null || data.plot == null)
            {
                activePlots.RemoveAt(i);
                continue;
            }

            data.Tick(updateInterval);

            // Nếu cây đã chết, tự xóa khỏi danh sách
            if (data.IsDead)
                activePlots.RemoveAt(i);
        }
    }

    public CultivationData GetCultivationData(Plot plot)
    {
        return activePlots.Find(p => p.plot == plot);
    }

    public void RegisterCropPlot(Plot plot, ItemData seedItem)
    {
        if (plot == null || seedItem == null || seedItem.itemSO == null)
            return;

        SeedSO seed = seedItem.itemSO as SeedSO;
        if (seed == null)
            return;

        if (activePlots.Exists(p => p.plot == plot))
            return;

        CultivationData data = new CultivationData(plot, seed);
        activePlots.Add(data);
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
        if (data == null || !data.IsMature) return;

        // Thu hoạch thủ công (nếu cần)
        BuilderManager.Instance.ClearPlot(plot);
        UnregisterPlot(plot);
    }

    public List<Plot> GetReadyPlots()
    {
        List<Plot> ready = new();
        foreach (var p in activePlots)
        {
            if (p.IsMature && !p.IsDead)
                ready.Add(p.plot);
        }
        return ready;
    }
}
