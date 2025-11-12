using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý tất cả trái cây spawn trong game, gắn liền với Plot.
/// </summary>
public class ProductManager : Singleton<ProductManager>
{
    // Lưu danh sách Fruit theo từng Plot
    private readonly Dictionary<Plot, List<Product>> productByPlot = new();

    /// <summary>
    /// Spawn một Fruit dựa trên FruitData và prefabAddress.
    /// </summary>
    public void SpawnProduct(ProductData productData, Vector3 position, Plot plot)
    {

        if (productData == null || string.IsNullOrEmpty(productData.prefabAddress) || plot == null)
        {
            Debug.LogWarning("FruitManager.SpawnFruit: dữ liệu không hợp lệ");
            return;
        }

        // Load prefab qua Addressables
        PrefabManager.Instance.LoadPrefabAsync(productData.prefabAddress, prefab =>
        {
            if (prefab == null)
            {
                Debug.LogWarning($"FruitManager: prefab không load được với key = {productData.prefabAddress}");
                return;
            }

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            Product product = obj.GetComponent<Product>();
            if (product == null)
                product = obj.AddComponent<Product>();

            product.Init(productData, plot);

            // Thêm vào danh sách quản lý
            if (!productByPlot.ContainsKey(plot))
                productByPlot[plot] = new List<Product>();

            productByPlot[plot].Add(product);
        });
    }

    /// <summary>
    /// Lấy số lượng fruit hiện có trên 1 plot.
    /// </summary>
    public int GetProductCountByPlot(Plot plot)
    {
        if (plot == null || !productByPlot.ContainsKey(plot)) return 0;

        int count = 0;
        foreach (var f in productByPlot[plot])
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
        if (plot == null || !productByPlot.ContainsKey(plot)) return;

        var list = productByPlot[plot];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var f = list[i];
            if (f == null) continue;
            f.CollectInstant();
        }

        list.Clear();
        productByPlot.Remove(plot);
    }

    /// <summary>
    /// Thu hoạch tất cả fruit trong toàn bộ bản đồ.
    /// </summary>
    public void CollectAll()
    {
        foreach (var kv in productByPlot)
        {
            var list = kv.Value;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var f = list[i];
                if (f == null) continue;
                f.CollectInstant();
            }
        }
        productByPlot.Clear();
    }

    /// <summary>
    /// Gọi khi fruit đã được thu hoạch (click hoặc auto).
    /// </summary>
    public void NotifyCollected(Product product)
    {
        if (product == null || product.SourcePlot == null) return;

        Plot plot = product.SourcePlot;
        if (!productByPlot.ContainsKey(plot)) return;

        productByPlot[plot].Remove(product);

        if (productByPlot[plot].Count == 0)
            productByPlot.Remove(plot);
    }

    /// <summary>
    /// Xóa toàn bộ product của một plot (khi cây bị hủy hoặc chết).
    /// </summary>
    public void ClearPlot(Plot plot)
    {
        if (plot == null || !productByPlot.ContainsKey(plot)) return;

        var list = productByPlot[plot];
        foreach (var f in list)
        {
            if (f != null)
                Destroy(f.gameObject);
        }

        list.Clear();
        productByPlot.Remove(plot);
    }
}
