using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimalInfo : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text animalTxt;
    [SerializeField] private TMP_Text stageTxt;
    [SerializeField] private TMP_Text productTxt;
    [SerializeField] private TMP_Text spawnTimeTxt;
    [SerializeField] private TMP_Text unharvestTxt;
    [SerializeField] private Slider stageSlider;
    [SerializeField] private Slider spawnSlider;
    [SerializeField] private Button harvestBtn;

    private AnimalUnit unit;
    void Start()
    {
        harvestBtn.onClick.AddListener(OnHarvestButtonClicked);
    }
    
    public void Bind(AnimalUnit u)
    {
        unit = u;
        UpdateUI();
    }

    private void Update()
    {
        if (unit != null) UpdateUI();
    }

    private void UpdateUI()
    {
        if (unit == null || unit.data == null) return;

        animalTxt.text = $"Crop: {unit.data.name}";
        // stageTxt.text = $"{unit.data.CropStage}";

        // float stageProgress = unit.IsAdult ? 1f : unit.growTimer / unit.;
        // stageSlider.value = stageProgress;

        productTxt.text = $"Product: {unit.productCount}/{unit.data.maxProductCount}";

        float spawnProgress = unit.IsAdult ? unit.productTimer / unit.GetTimeInterval() : 0f;
        spawnSlider.value = spawnProgress;
        spawnTimeTxt.text = $"{TimeUtility.FormatTime((int)(unit.GetTimeToNextProduct()))}";

        int unharvested = ProductManager.Instance.GetProductCountByPlot(unit.plot);
        unharvestTxt.text = $"Unharvested: {unharvested}";

        harvestBtn.interactable = unharvested > 0;
    }

    private void OnHarvestButtonClicked()
    {
        if (unit == null) return;
        ProductManager.Instance.CollectByPlot(unit.plot);
    }
}
