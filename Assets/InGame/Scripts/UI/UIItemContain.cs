using System;
using System.Collections.Generic;
using UnityEngine;

public enum eItemType
{
    Seed,
    Animal,
    Product
}

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
    /// Hiển thị danh sách item theo loại (Seed, Animal, Product).
    /// </summary>
    public void ShowItemList(Plot plot, eItemType itemType)
    {
        targetPlot = plot;
        BuildItemList(itemType);
        gameObject.SetActive(true);
    }

    public void RegisterOnClick(Action callback)
    {
        onClick = callback;
    }

    private void BuildItemList(eItemType itemType)
    {
        ClearList();

        // ✅ Lấy toàn bộ dữ liệu item theo loại
        if (itemType == eItemType.Seed)
        {
            BuildSeedList(DataManager.SeedDict);
        }
        else if (itemType == eItemType.Animal)
        {
            BuildAnimalList(DataManager.AnimalDict);
        }
        else if (itemType == eItemType.Product)
        {
            // BuildProductList(DataManager.FruitDict); 
        }
    }

    // ---------------------- SEED ----------------------
    private void BuildSeedList(Dictionary<string, SeedData> seeds)
    {
        foreach (var kv in seeds)
        {
            string id = kv.Key;
            SeedData seedData = kv.Value;

            int quantity = ResourceManager.Instance.GetSeedCount(id);

            GameObject go = Instantiate(itemPrefab, contentParent);
            UIItem ui = go.GetComponent<UIItem>();

            seedData.LoadIcon(sprite =>
            {
                ui.Setup(sprite, quantity, () => OnSelectSeed(seedData, quantity));
            });
        }
    }

    // ---------------------- ANIMAL ----------------------
    private void BuildAnimalList(Dictionary<string, AnimalData> animals)
    {
        Debug.Log($"Total animals in game: {animals.Count}");

        foreach (var kv in animals)
        {
            string id = kv.Key;
            AnimalData data = kv.Value;

            int quantity = ResourceManager.Instance.GetAnimalBreedCount(id);

            GameObject go = Instantiate(itemPrefab, contentParent);
            UIItem ui = go.GetComponent<UIItem>();

            data.LoadIcon(sprite =>
            {
                ui.Setup(sprite, quantity, () => OnSelectAnimal(data, quantity));
            });
        }
    }

    // ---------------------- PRODUCT ----------------------
    // private void BuildProductList(Dictionary<string, FruitData> products)
    // {
    //     Debug.Log($"Total products in game: {products.Count}");

    //     foreach (var kv in products)
    //     {
    //         string id = kv.Key;
    //         FruitData data = kv.Value;

    //         int quantity = ResourceManager.Instance.GetFruitCount(id);

    //         GameObject go = Instantiate(itemPrefab, contentParent);
    //         UIItem ui = go.GetComponent<UIItem>();

    //         data.LoadIcon(sprite =>
    //         {
    //             ui.Setup(sprite, quantity, null); // sản phẩm không có hành động
    //         });
    //     }
    // }

    // =====================================================
    private void ClearList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
    }

    // =====================================================
    private void OnSelectSeed(SeedData seedData, int quantity)
    {
        if (targetPlot == null || seedData == null) return;

        if (quantity <= 0)
        {
            Debug.LogWarning($"Không đủ hạt {seedData.name} để trồng");
            return;
        }

        // Trồng cây
        CultivationManager.Instance.RegisterCropPlot(targetPlot, seedData);
        targetPlot.Purpose = ePlotPurpose.Farming;

        ResourceManager.Instance.UseSeed(seedData.id);
        Debug.Log($"🌾 Đã trồng {seedData.name}, còn lại {ResourceManager.Instance.GetSeedCount(seedData.id)} hạt");

        onClick?.Invoke();
        Hide();
    }

    private void OnSelectAnimal(AnimalData animalData, int quantity)
    {
        if (targetPlot == null || animalData == null)
        {
            Debug.LogWarning("AnimalData null khi chọn!");
            return;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning($"Không đủ hạt {animalData.name} để trồng");
            return;
        }

        // Nếu bạn có hệ thống chuồng (pen), gọi spawn bò tại vị trí đó
        AnimalManager.Instance.AddAnimal(animalData, targetPlot);

        ResourceManager.Instance.UseAnimalBreed(animalData.id);
        ResourceManager.Instance.AddAnimal(animalData.id, 1);
        Debug.Log($"🐮 Đã thêm {animalData.name} vào chuồng. Tổng: {ResourceManager.Instance.GetAnimalCount(animalData.id)}");

        onClick?.Invoke();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        targetPlot = null;
    }
}
