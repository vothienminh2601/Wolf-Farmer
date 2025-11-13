using System.Collections.Generic;
using UnityEngine;

public class ProductManager : Singleton<ProductManager>
{
    private readonly Dictionary<Plot, List<Product>> productByPlot = new();

    public void SpawnProduct(ProductData productData, Vector3 position, Plot plot)
    {

        if (productData == null || string.IsNullOrEmpty(productData.prefabAddress) || plot == null)
        {
            Debug.LogWarning("FruitManager.SpawnFruit: dữ liệu không hợp lệ");
            return;
        }

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

            if (!productByPlot.ContainsKey(plot))
                productByPlot[plot] = new List<Product>();

            productByPlot[plot].Add(product);
        });
    }
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

    public void NotifyCollected(Product product)
    {
        if (product == null || product.SourcePlot == null) return;

        Plot plot = product.SourcePlot;
        if (!productByPlot.ContainsKey(plot)) return;

        productByPlot[plot].Remove(product);

        if (productByPlot[plot].Count == 0)
            productByPlot.Remove(plot);
    }

    public Product GetNearestProduct(Vector3 fromPos)
    {
        Product nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var kv in productByPlot)
        {
            foreach (var prod in kv.Value)
            {
                if (prod == null) continue;

                float dist = Vector3.Distance(fromPos, prod.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = prod;
                }
            }
        }
        return nearest;
    }


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
