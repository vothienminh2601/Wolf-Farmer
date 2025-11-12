using UnityEngine;

public class Product : MonoBehaviour
{
    protected ProductData productData;
    private Plot sourcePlot;
    public Plot SourcePlot => sourcePlot;
    protected bool collected;

    public void Init(ProductData data, Plot plot)
    {
        productData = data;
        sourcePlot = plot;
        collected = false;

        transform.SetParent(plot.transform);
    }

    private void OnMouseDown()
    {
        if (!collected)
            CollectInstant();
    }

    public void CollectInstant()
    {
        collected = true;
        ProductManager.Instance.NotifyCollected(this);
        ResourceManager.Instance.AddProduct(productData.id, 1);

        Destroy(gameObject);
    }
}
