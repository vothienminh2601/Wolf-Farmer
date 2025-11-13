using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private TMP_Text timeScaleTxt;
    [SerializeField] private Toggle productToggle, farmToggle, stockToggle, equipmentToggle;

    [SerializeField] private Button shopBtn;


    [SerializeField] private UIProductDetail uIProductDetail;
    [SerializeField] private UIFarmDetail uIFarmDetail;
    [SerializeField] private UIStockDetail uIStockDetail;
    [SerializeField] private UIEquipmentDetail uIEquipmentDetail;


    void Awake()
    {
        ResourceManager.OnCoinChanged += SetCoin;
        ResourceManager.OnProductChanged += SetProduct;
        ResourceManager.OnStockChanged += SetStock;
        FarmManager.OnPlotChanged += SetFarm;
        CultivationManager.OnTickCultivation += SetFarmDetail;
        AnimalManager.OnTickCultivation += SetFarmDetail;
        ResourceManager.OnResourceChanged += RefreshStats;
        EquipmentManager.OnUpgrade += SetEquipment;
        GameManager.OnTimeScaleChanged += SetTimeScale;

        uIStockDetail.gameObject.SetActive(false);
        stockToggle.isOn = false;
        uIProductDetail.gameObject.SetActive(false);
        productToggle.isOn = false;
        uIFarmDetail.gameObject.SetActive(false);
        farmToggle.isOn = false;
        uIEquipmentDetail.gameObject.SetActive(false);
        equipmentToggle.isOn = false;
    }

    void Start()
    {
        productToggle.onValueChanged.AddListener(ToggleProductDetail);
        farmToggle.onValueChanged.AddListener(ToggleFarmDetail);
        stockToggle.onValueChanged.AddListener(ToggleStockDetail);
        equipmentToggle.onValueChanged.AddListener(ToggleEquipmentDetail);
        shopBtn.onClick.AddListener(() => UIShop.Instance.ShowShop(true));
        RefreshStats();
    }
    

    void OnDestroy()
    {
        ResourceManager.OnCoinChanged -= SetCoin;
        ResourceManager.OnProductChanged -= SetProduct;
        ResourceManager.OnStockChanged -= SetStock;
        FarmManager.OnPlotChanged -= SetFarm;
        CultivationManager.OnTickCultivation -= SetFarmDetail;
        ResourceManager.OnResourceChanged -= RefreshStats;
        EquipmentManager.OnUpgrade -= SetEquipment;
        GameManager.OnTimeScaleChanged -= SetTimeScale;


        productToggle.onValueChanged.RemoveListener(ToggleProductDetail);
        farmToggle.onValueChanged.RemoveListener(ToggleFarmDetail);
        stockToggle.onValueChanged.RemoveListener(ToggleStockDetail);
    }

    void SetTimeScale(float timeScale)
    {
        timeScaleTxt.SetText($"TimeScale: {timeScale}");
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

    void SetEquipment(int level, float efficency)
    {
        equipmentLvlTxt.SetText($"Equipment: Lv.{level}");

        uIEquipmentDetail?.SetEfficency(efficency);
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

    public void ToggleEquipmentDetail(bool on)
    {
        uIEquipmentDetail.gameObject.SetActive(on);
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
        var resource = ResourceManager.Instance;
        var farm = FarmManager.Instance;
        var equipment = EquipmentManager.Instance;

        SetTimeScale(Time.timeScale);
        // 1. Coin
        coinTxt.text = $"Coin: {resource.GetCoin()}";

        // 2. Equipment leveL
        SetEquipment(equipment.Level, equipment.Efficiency);

        // 3. Worker
        // int working = WorkerManager.Instance.GetWorkingCount();
        // int idle = WorkerManager.Instance.GetIdleCount();
        workerTxt.text = $"Workers: {0} working / {0} idle";

        SetStock(resource.GetAllSeeds(), resource.GetAllAnimalBreeds());

        // 5. Farm plots
        int used = farm.GetActivePlotCount();
        int emty = farm.GetEmptyPlotCount();
        SetFarm(used, emty, farm.GetFarmingPlots());
        // 6. Fruits đã thu hoạch

        SetProduct(resource.GetAllProducts());
    }

}
