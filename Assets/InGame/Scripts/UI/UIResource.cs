using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceManager : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private TMP_Text coinTxt;
    [SerializeField] private TMP_Text stockTxt;
    [SerializeField] private TMP_Text productTxt;
    [SerializeField] private TMP_Text equipmentLvlTxt;
    [SerializeField] private TMP_Text workerTxt;
    [SerializeField] private TMP_Text farmTxt;
    [SerializeField] private Toggle productToggle;


    [SerializeField] private UIProductDetail uIProductDetail;


    void Awake()
    {
        ResourceManager.OnCoinChanged += SetCoin;
        ResourceManager.OnProductChanged += SetProduct;
    }

    void Start()
    {
        productToggle.onValueChanged.AddListener(ShowProductDetail);
    }

    void OnDestroy()
    {
        ResourceManager.OnCoinChanged -= SetCoin;
        ResourceManager.OnProductChanged -= SetProduct;

        productToggle.onValueChanged.RemoveListener(ShowProductDetail);
    }
    
    private void OnEnable()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshStats();
    }


    void SetCoin(int coin)
    {
        coinTxt.SetText($"Coin: {coin}");
    }

    void SetStock(List<ResourceStack> seeds, List<ResourceStack> animalBreeds)
    {
        int totalSeeds = 0;
        int totalAnimals = 0;

        // Tính tổng hạt giống
        if (seeds != null)
        {
            foreach (var s in seeds)
            {
                if (s == null) continue;
                totalSeeds += s.quantity;
            }
        }

        // Tính tổng vật nuôi giống
        if (animalBreeds != null)
        {
            foreach (var a in animalBreeds)
            {
                if (a == null) continue;
                totalAnimals += a.quantity;
            }
        }

        stockTxt.SetText($"Stock: {totalSeeds + totalAnimals}");
    }

    void SetProduct(List<ResourceStack> products)
    {
        int totalProducts = 0;

        // Tính tổng hạt giống
        if (products != null)
        {
            foreach (var s in products)
            {
                if (s == null) continue;
                totalProducts += s.quantity;
            }
        }
        productTxt.SetText($"Product: {totalProducts}");

        uIProductDetail?.ShowProductDetails(products);
    }

    public void ShowProductDetail(bool on)
    {
        uIProductDetail.gameObject.SetActive(on);
    }
    
    private void RefreshStats()
    {
        var inv = ResourceManager.Instance;

        // 1. Coin
        coinTxt.text = $"Coin: {inv.GetCoin()}";

        // 2. Equipment level (ví dụ lấy level cao nhất)
        int equipLevel = 0;
        foreach (var e in inv.GetAllEquipments())
            equipLevel = Mathf.Max(equipLevel, e.quantity);
        equipmentLvlTxt.text = $"Equipment Lv: {equipLevel}";

        // 3. Worker
        // int working = WorkerManager.Instance.GetWorkingCount();
        // int idle = WorkerManager.Instance.GetIdleCount();
        // workerTxt.text = $"Workers: {working} working / {idle} idle";

        // 4. Seeds chưa sử dụng
        int totalSeeds = 0;
        foreach (var s in inv.GetAllSeeds())
            totalSeeds += s.quantity;
        stockTxt.text = $"Seeds: {totalSeeds} unused";

        // 5. Farm plots
        int used = FarmManager.Instance.GetActivePlotCount();
        int total = FarmManager.Instance.GetTotalPlotCount();
        farmTxt.text = $"Farm: {used}/{total} used";

        // 6. Fruits đã thu hoạch
        // int blueberries = inv.GetFruitCount("fruit_blueberry");
        // int tomatoes = inv.GetFruitCount("fruit_tomato");
        // int milk = inv.GetFruitCount("fruit_milk");
    }

}
