using System;
using System.Collections.Generic;
using UnityEngine;

public class UIItemContain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private RectTransform contentParent;

    private Plot targetPlot;
    private Action onClick;

    void OnDestroy()
    {
        onClick = null;
    }

    /// <summary>
    /// Hiển thị toàn bộ seed có trong game, đồng thời hiển thị số lượng hiện có trong kho.
    /// </summary>
    public void ShowSeedList(Plot plot)
    {
        if (plot == null) return;
        targetPlot = plot;

        BuildSeedList();
        gameObject.SetActive(true);
    }

    public void RegisterOnClick(Action callback)
    {
        onClick += callback;
    }

    private void BuildSeedList()
    {
        ClearList();

        // Lấy tất cả seed từ DataManager
        var allSeeds = DataManager.SeedDict;
        Debug.Log($"Total seeds in game: {allSeeds.Count}");

        foreach (var kv in allSeeds)
        {
            string seedId = kv.Key;
            SeedData seedData = kv.Value;

            // Lấy số lượng hiện có trong kho
            int quantity = ResourceManager.Instance.GetSeedCount(seedId);

            // Tạo item UI
            GameObject go = Instantiate(itemPrefab, contentParent);
            UIItem ui = go.GetComponent<UIItem>();

            // Load icon qua Addressables
            seedData.LoadIcon(sprite =>
            {
                ui.Setup(sprite, quantity, () => OnSelectSeed(seedData, quantity));
            });
        }
    }

    private void ClearList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }

    private void OnSelectSeed(SeedData seedData, int quantity)
    {
        if (targetPlot == null || seedData == null) return;

        if (quantity <= 0)
        {
            Debug.LogWarning($"Không đủ hạt {seedData.name} để trồng");
            return;
        }

        // 🔹 Trồng cây mới
        CultivationManager.Instance.RegisterCropPlot(targetPlot, seedData);
        targetPlot.Purpose = ePlotPurpose.Farming;

        // 🔹 Giảm số lượng trong inventory
        ResourceManager.Instance.UseSeed(seedData.id);

        Debug.Log($"🌾 Đã trồng {seedData.name}, còn lại {ResourceManager.Instance.GetSeedCount(seedData.id)} hạt");
        onClick?.Invoke();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        targetPlot = null;
    }
}
