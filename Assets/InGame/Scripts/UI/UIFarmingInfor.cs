using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFarmingInfor : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text cropTxt;
    [SerializeField] private TMP_Text stageTxt;
    [SerializeField] private TMP_Text fruitTxt;
    [SerializeField] private TMP_Text unharvestTxt;
    [SerializeField] private Slider stageSlider;
    [SerializeField] private Slider spawnSlider;
    [SerializeField] private Button harvestBtn;

    private CultivationData currentData;

    void Start()
    {
        harvestBtn.onClick.AddListener(OnHarvestButtonClicked);
    }

    public void Bind(CultivationData data)
    {
        currentData = data;
        UpdateUI();
    }

    private void Update()
    {
        if (currentData != null)
            UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentData == null || currentData.seed == null) return;

        cropTxt.text = $"Crop: {currentData.seed.name}";
        stageTxt.text = $"{currentData.CropStage}";

        float stageProgress = currentData.IsMature ? 1f : currentData.growthTimer / currentData.seed.stageDuration;
        stageSlider.value = stageProgress;

        fruitTxt.text = $"Fruit: {currentData.fruitCount}/{currentData.seed.maxFruitCount}";

        float spawnProgress = currentData.IsMature ? currentData.fruitTimer / currentData.seed.fruitInterval : 0f;
        spawnSlider.value = spawnProgress;

        int unharvested = ProductManager.Instance.GetProductCountByPlot(currentData.plot);
        unharvestTxt.text = $"Unharvested: {unharvested}";

        harvestBtn.interactable = unharvested > 0;
    }

    private void OnHarvestButtonClicked()
    {
        if (currentData == null) return;
        ProductManager.Instance.CollectByPlot(currentData.plot);
    }
}
