using System.Collections.Generic;
using UnityEngine;

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
            var data = activePlots[i];
            if (data == null || data.plot == null)
            {
                activePlots.RemoveAt(i);
                continue;
            }
            
            data.Tick(updateInterval);
        }
    }

    public CultivationData GetCultivationData(Plot plot)
    {
        return activePlots.Find(p => p.plot == plot);
    }

    public void RegisterCropPlot(Plot plot, SeedData seed)
    {
        if (plot == null || seed == null)
            return;

        if (activePlots.Exists(p => p.plot == plot))
            return;

        var data = new CultivationData(plot, seed);
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

        var data = activePlots.Find(p => p.plot == plot);
        if (data == null || !data.IsMature) return;

        BuilderManager.Instance.ClearPlot(plot);
        UnregisterPlot(plot);
    }
}
