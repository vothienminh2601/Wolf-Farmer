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
    [SerializeField] private Toggle productToggle, farmToggle, stockToggle;

    [SerializeField] private Button shopBtn;


    [SerializeField] private UIProductDetail uIProductDetail;
    [SerializeField] private UIFarmDetail uIFarmDetail;
    [SerializeField] private UIStockDetail uIStockDetail;


    void Awake()
    {
        ResourceManager.OnCoinChanged += SetCoin;
        ResourceManager.OnProductChanged += SetProduct;
        ResourceManager.OnStockChanged += SetStock;
        FarmManager.OnPlotChanged += SetFarm;
        CultivationManager.OnTickCultivation += SetFarmDetail;

        uIStockDetail.gameObject.SetActive(false);
        stockToggle.isOn = false;
        uIProductDetail.gameObject.SetActive(false);
        productToggle.isOn = false;
        uIFarmDetail.gameObject.SetActive(false);
        farmToggle.isOn = false;
    }

    void Start()
    {
        productToggle.onValueChanged.AddListener(ToggleProductDetail);
        farmToggle.onValueChanged.AddListener(ToggleFarmDetail);
        stockToggle.onValueChanged.AddListener(ToggleStockDetail);
        shopBtn.onClick.AddListener(() => UIShop.Instance.ShowShop(true));
        RefreshAll();

    }
    

    void OnDestroy()
    {
        ResourceManager.OnCoinChanged -= SetCoin;
        ResourceManager.OnProductChanged -= SetProduct;
        ResourceManager.OnStockChanged -= SetStock;
        FarmManager.OnPlotChanged -= SetFarm;
        CultivationManager.OnTickCultivation -= SetFarmDetail;

        productToggle.onValueChanged.RemoveListener(ToggleProductDetail);
        farmToggle.onValueChanged.RemoveListener(ToggleFarmDetail);
        stockToggle.onValueChanged.RemoveListener(ToggleStockDetail);
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
        uIStockDetail?.ShowStockDetail(seeds, animalBreeds);
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

        uIProductDetail?.UpdateProductDetails(products);
    }

    public void ToggleStockDetail(bool on)
    {
        uIStockDetail.gameObject.SetActive(on);
    }

    public void ToggleProductDetail(bool on)
    {
        uIProductDetail.gameObject.SetActive(on);
    }

    public void ToggleFarmDetail(bool on)
    {
        uIFarmDetail.gameObject.SetActive(on);
    }

    public void SetFarm(int activePlots, int emptyPlots, List<Plot> plots)
    {
        farmTxt.SetText($"Farm: {activePlots}/{activePlots + emptyPlots}");
        uIFarmDetail?.ShowDetails();
    }

    public void SetFarmDetail()
    {
        uIFarmDetail?.ShowDetails();
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
        equipmentLvlTxt.text = $"Equipment: {equipLevel}";

        // 3. Worker
        // int working = WorkerManager.Instance.GetWorkingCount();
        // int idle = WorkerManager.Instance.GetIdleCount();
        workerTxt.text = $"Workers: {0} working / {0} idle";

        // 4. Seeds chưa sử dụng
        int totalSeeds = 0;
        foreach (var s in inv.GetAllSeeds())
            totalSeeds += s.quantity;
        stockTxt.text = $"Seeds: {totalSeeds}";

        // 5. Farm plots
        int used = FarmManager.Instance.GetActivePlotCount();
        int total = FarmManager.Instance.GetTotalPlotCount();
        farmTxt.text = $"Farm: {used}/{total} used";

        // 6. Fruits đã thu hoạch
        productTxt.text = $"Products: {0}";
    }

}
