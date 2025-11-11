using System.Collections.Generic;
using UnityEngine;

public class FruitManager : Singleton<FruitManager>
{
    private List<Fruit> activeFruits = new();

    public void SpawnFruit(ItemSO itemSO, GameObject prefab, Vector3 position, Plot plot)
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        Fruit fruit = obj.GetComponent<Fruit>();
        if (fruit == null)
            fruit = obj.AddComponent<Fruit>();

        fruit.Init(itemSO, plot);
        activeFruits.Add(fruit);
    }

    public int GetFruitCountByPlot(Plot plot)
    {
        if (plot == null) return 0;

        int count = 0;
        foreach (var fruit in activeFruits)
        {
            if (fruit != null && fruit.SourcePlot == plot)
                count++;
        }
        return count;
    }

    public void CollectByPlot(Plot plot)
    {
        if (plot == null) return;

        for (int i = activeFruits.Count - 1; i >= 0; i--)
        {
            Fruit f = activeFruits[i];
            if (f == null) continue;

            if (f.SourcePlot == plot)
            {
                f.CollectInstant();
                activeFruits.RemoveAt(i);
            }
        }
    }

    public void CollectAll()
    {
        // Thu hoạch tất cả trái còn tồn tại
        for (int i = activeFruits.Count - 1; i >= 0; i--)
        {
            Fruit f = activeFruits[i];
            if (f == null) continue;
            f.CollectInstant();
        }
        activeFruits.Clear();
    }

    public void NotifyCollected(Fruit fruit)
    {
        if (activeFruits.Contains(fruit))
            activeFruits.Remove(fruit);
    }
}
