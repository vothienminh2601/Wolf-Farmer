using UnityEngine;

public class Fruit : MonoBehaviour
{
    private ItemSO itemSO;
    private Plot sourcePlot;
    public Plot SourcePlot => sourcePlot;
    private bool collected;

    public void Init(ItemSO itemSO, Plot plot)
    {
        this.itemSO = itemSO;
        sourcePlot = plot;
        collected = false;
    }

    private void OnMouseDown()
    {
        if (!collected)
            CollectAnimated();
    }

    public void CollectAnimated()
    {
        collected = true;
        FruitManager.Instance.NotifyCollected(this);

        // Thêm vào kho
        InventoryManager.Instance.AddItem(itemSO, 1);

        // Bay vào UI kho (cần tham chiếu vị trí UI)
        // if (UIWarehouse.Instance != null)
        // {
        //     Vector3 targetPos = UIWarehouse.Instance.GetWorldPosition();
        //     StartCoroutine(MoveTo(targetPos));
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    public void CollectInstant()
    {
        collected = true;
        FruitManager.Instance.NotifyCollected(this);
        InventoryManager.Instance.AddItem(itemSO, 1);
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // tốc độ bay
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
