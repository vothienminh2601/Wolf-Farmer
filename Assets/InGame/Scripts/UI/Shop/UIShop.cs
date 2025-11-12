using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public enum eShopMode { Buy, Sell }
public enum eBuyCategory { Seed, Animal }

public class UIShop : Singleton<UIShop>
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button bg;
    [SerializeField] private Button btnBuyTab;
    [SerializeField] private Button btnSellTab;
    [SerializeField] private Button btnSeed;
    [SerializeField] private Button btnAnimal;
    [SerializeField] private RectTransform contentParent;
    [SerializeField] private UIShopItem itemPrefab;
    [SerializeField] private TMP_Text totalPriceTxt;
    [SerializeField] private Button confirmBtn;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.8f, 1f);

    private eShopMode currentMode = eShopMode.Buy;
    private eBuyCategory currentBuyCategory = eBuyCategory.Seed;

    private readonly List<UIShopItem> spawnedItems = new();

    private int totalPrice = 0;

    void Start()
    {
        btnBuyTab.onClick.AddListener(() => SwitchMode(eShopMode.Buy));
        btnSellTab.onClick.AddListener(() => SwitchMode(eShopMode.Sell));
        btnSeed.onClick.AddListener(() => SwitchBuyCategory(eBuyCategory.Seed));
        btnAnimal.onClick.AddListener(() => SwitchBuyCategory(eBuyCategory.Animal));
        confirmBtn.onClick.AddListener(OnConfirm);

        bg.onClick.AddListener(() => ShowShop(false));
        SwitchMode(eShopMode.Buy);
        ShowShop(false);
    }

    public void ShowShop(bool on)
    {
        canvasGroup.DOFade(on ? 1 : 0, 0.5f);
        canvasGroup.interactable = on;
        canvasGroup.blocksRaycasts = on;
    }

    private void SwitchMode(eShopMode mode)
    {
        currentMode = mode;
        totalPrice = 0;
        totalPriceTxt.text = $"Total Price: $0";
        ClearItems();

        HighlightTab(btnBuyTab, mode == eShopMode.Buy);
        HighlightTab(btnSellTab, mode == eShopMode.Sell);

        if (mode == eShopMode.Buy)
        {
            btnSeed.gameObject.SetActive(true);
            btnAnimal.gameObject.SetActive(true);
            SwitchBuyCategory(eBuyCategory.Seed);
        }
        else
        {
            btnSeed.gameObject.SetActive(false);
            btnAnimal.gameObject.SetActive(false);
            LoadSellProducts();
        }
    }

    private void SwitchBuyCategory(eBuyCategory category)
    {
        currentBuyCategory = category;
        totalPrice = 0;
        totalPriceTxt.text = $"Total Price: $0";
        ClearItems();

        HighlightTab(btnSeed, category == eBuyCategory.Seed);
        HighlightTab(btnAnimal, category == eBuyCategory.Animal);

        if (category == eBuyCategory.Seed)
            LoadBuySeeds();
        else
            LoadBuyAnimals();
    }

    private void HighlightTab(Button button, bool active)
    {
        if (button == null) return;
        var img = button.GetComponent<Image>();
        if (img != null)
            img.color = active ? highlightColor : normalColor;
    }


    private void LoadBuySeeds()
    {
        foreach (var kv in DataManager.SeedDict)
        {
            var data = kv.Value;
            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(data.id, data.name, data.baseValue, 0, OnValueChanged);
            spawnedItems.Add(item);
        }
    }

    private void LoadBuyAnimals()
    {
        foreach (var kv in DataManager.AnimalDict)
        {
            var data = kv.Value;
            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(data.id, data.name, data.baseValue, 0, OnValueChanged);
            spawnedItems.Add(item);
        }
    }

    private void LoadSellProducts()
    {
        var products = ResourceManager.Instance.GetAllProducts();

        foreach (var stack in products)
        {
            var data = DataManager.GetProductById(stack.id);
            if (data == null) continue;

            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(data.id, $"{data.name} (x{stack.quantity})", data.baseValue / 2, 0, OnValueChanged, stack.quantity);
            spawnedItems.Add(item);
        }
    }

    private void OnValueChanged()
    {
        totalPrice = 0;
        foreach (var item in spawnedItems)
        {
            totalPrice += item.GetTotalPrice();
        }
        totalPriceTxt.text = $"Total Price: {totalPrice}$";
    }

    private void OnConfirm()
    {
        if (totalPrice <= 0) return;

        if (currentMode == eShopMode.Buy)
        {
            if (!ResourceManager.Instance.SpendCoin(totalPrice))
            {
                Debug.LogWarning("Not enough money to buy!");
                return;
            }

            foreach (var item in spawnedItems)
            {
                int qty = item.GetQuantity();
                if (qty <= 0) continue;

                if (currentBuyCategory == eBuyCategory.Seed)
                    ResourceManager.Instance.AddSeed(item.GetId(), qty);
                else
                    ResourceManager.Instance.AddAnimal(item.GetId(), qty);
            }
            
        }
        else // SELL
        {
            ResourceManager.Instance.AddCoin(totalPrice);
            foreach (var item in spawnedItems)
            {
                int qty = item.GetQuantity();
                if (qty <= 0) continue;

                // Trừ số lượng sản phẩm
                var fruitData = DataManager.GetProductById(item.GetId());
                if (fruitData != null)
                    ResourceManager.Instance.ReduceProduct(fruitData.id, qty);
            }
        }

        SwitchMode(currentMode);
    }

    private void ClearItems()
    {
        foreach (var item in spawnedItems)
            Destroy(item.gameObject);
        spawnedItems.Clear();
    }
}
