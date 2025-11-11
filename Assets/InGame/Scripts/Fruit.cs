using UnityEngine;

public class Fruit : MonoBehaviour
{
    private FruitData fruitData;
    private Plot sourcePlot;
    public Plot SourcePlot => sourcePlot;
    private bool collected;

    public void Init(FruitData fruit, Plot plot)
    {
        fruitData = fruit;
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
        InventoryManager.Instance.AddItem(fruitData.id, 1);

        Destroy(gameObject);
    }
}
