using System.Collections.Generic;
using UnityEngine;

public class UIItemContain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private RectTransform contentParent;

    private Plot targetPlot;

    public void ShowSeedList(Plot plot)
    {
        if (plot == null) return;
        Debug.Log(1);
        targetPlot = plot;
        BuildSeedList();
        gameObject.SetActive(true);
    }

    private void BuildSeedList()
    {
        ClearList();

        // Lấy dữ liệu từ PlayerInventory
        List<ItemData> seedItems = PlayerInventory.Instance.GetSeedItems();
        Debug.Log(seedItems.Count);
        foreach (var item in seedItems)
        {
            if (item.itemSO == null) continue; // bỏ qua hạt hết

            GameObject go = Instantiate(itemPrefab, contentParent);
            UIItem ui = go.GetComponent<UIItem>();
            ui.Setup(item.itemSO.icon, item.quantity,
                () => OnSelectSeed(item));
        }
    }

    private void ClearList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }

    private void OnSelectSeed(ItemData item)
    {
        if (targetPlot == null || item.itemSO == null) return;
        if (item.quantity <= 0)
        {
            Debug.LogWarning($"Không đủ hạt {item.itemSO.itemName}");
            return;
        }

        // 🔹 Trồng cây
        CultivationManager.Instance.RegisterCropPlot(targetPlot, item);
        targetPlot.Purpose = ePlotPurpose.Farming;

        // 🔹 Trừ hạt trong inventory
        PlayerInventory.Instance.UseSeed(item.itemSO);

        Debug.Log($"🌾 Đã trồng {item.itemSO.itemName}, còn {PlayerInventory.Instance.GetQuantity(item.itemSO)} hạt");
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        targetPlot = null;
    }
}
