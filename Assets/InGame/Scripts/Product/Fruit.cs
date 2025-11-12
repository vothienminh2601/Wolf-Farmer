using UnityEngine;

public class Fruit : Product
{
    private Plot sourcePlot;
    public Plot SourcePlot => sourcePlot;
    private bool collected;

    public void Init(ProductData data, Plot plot)
    {
        productData = data;
        sourcePlot = plot;
        collected = false;
    }

    private void OnMouseDown()
    {
        if (!collected)
            CollectInstant();
    }

    public void CollectInstant()
    {
        collected = true;
        FruitManager.Instance.NotifyCollected(this);

        // Thêm vào kho
        ResourceManager.Instance.AddFruit(productData.id, 1);

        Destroy(gameObject);
    }
}
