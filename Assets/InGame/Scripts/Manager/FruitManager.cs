using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý tất cả trái cây spawn trong game, gắn liền với Plot.
/// </summary>
public class FruitManager : Singleton<FruitManager>
{
    // Lưu danh sách Fruit theo từng Plot
    private readonly Dictionary<Plot, List<Fruit>> fruitByPlot = new();

    /// <summary>
    /// Spawn một Fruit dựa trên FruitData và prefabAddress.
    /// </summary>
    public void SpawnFruit(FruitData fruitData, Vector3 position, Plot plot)
    {
        if (fruitData == null || string.IsNullOrEmpty(fruitData.prefabAddress) || plot == null)
        {
            Debug.LogWarning("FruitManager.SpawnFruit: dữ liệu không hợp lệ");
            return;
        }

        // Load prefab qua Addressables
        PrefabManager.Instance.LoadPrefabAsync(fruitData.prefabAddress, prefab =>
        {
            if (prefab == null)
            {
                Debug.LogWarning($"FruitManager: prefab không load được với key = {fruitData.prefabAddress}");
                return;
            }

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            Fruit fruit = obj.GetComponent<Fruit>();
            if (fruit == null)
                fruit = obj.AddComponent<Fruit>();

            fruit.Init(fruitData, plot);

            // Thêm vào danh sách quản lý
            if (!fruitByPlot.ContainsKey(plot))
                fruitByPlot[plot] = new List<Fruit>();

            fruitByPlot[plot].Add(fruit);
        });
    }

    /// <summary>
    /// Lấy số lượng fruit hiện có trên 1 plot.
    /// </summary>
    public int GetFruitCountByPlot(Plot plot)
    {
        if (plot == null || !fruitByPlot.ContainsKey(plot)) return 0;

        int count = 0;
        foreach (var f in fruitByPlot[plot])
        {
            if (f != null)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Thu hoạch tất cả fruit thuộc một plot cụ thể.
    /// </summary>
    public void CollectByPlot(Plot plot)
    {
        if (plot == null || !fruitByPlot.ContainsKey(plot)) return;

        var list = fruitByPlot[plot];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var f = list[i];
            if (f == null) continue;
            f.CollectInstant();
        }

        list.Clear();
        fruitByPlot.Remove(plot);
    }

    /// <summary>
    /// Thu hoạch tất cả fruit trong toàn bộ bản đồ.
    /// </summary>
    public void CollectAll()
    {
        foreach (var kv in fruitByPlot)
        {
            var list = kv.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var f = list[i];
                if (f == null) continue;
                f.CollectInstant();
            }
        }
        fruitByPlot.Clear();
    }

    /// <summary>
    /// Gọi khi fruit đã được thu hoạch (click hoặc auto).
    /// </summary>
    public void NotifyCollected(Fruit fruit)
    {
        if (fruit == null || fruit.SourcePlot == null) return;

        Plot plot = fruit.SourcePlot;
        if (!fruitByPlot.ContainsKey(plot)) return;

        fruitByPlot[plot].Remove(fruit);

        if (fruitByPlot[plot].Count == 0)
            fruitByPlot.Remove(plot);
    }

    /// <summary>
    /// Xóa toàn bộ fruit của một plot (khi cây bị hủy hoặc chết).
    /// </summary>
    public void ClearPlot(Plot plot)
    {
        if (plot == null || !fruitByPlot.ContainsKey(plot)) return;

        var list = fruitByPlot[plot];
        foreach (var f in list)
        {
            if (f != null)
                Destroy(f.gameObject);
        }

        list.Clear();
        fruitByPlot.Remove(plot);
    }
}
