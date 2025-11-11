using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlotInfor : MonoBehaviour
{
    [SerializeField] private Plot selectedPlot;
    [Header("RectTransforms")]
    [SerializeField] private RectTransform purposePanel;
    [SerializeField] private RectTransform buildPanel;
    [Header("Buttons")]
    [SerializeField] private Button cultivationBtn;
    [SerializeField] private Button buildingBtn, reassignBtn;
    [SerializeField] private Button farmingBtn, animalBtn;
    [SerializeField] private UIItemContain uIItemContain;
    [SerializeField] private UIFarmingInfor uIFarmingInfor;

    [SerializeField] private RectTransform cultivationBtnContain;
    [SerializeField] private TMP_Text titleTxt;


    void Start()
    {
        cultivationBtn.onClick.AddListener(OnClickCultivation);
        buildingBtn.onClick.AddListener(OnClickBuilding);
        farmingBtn.onClick.AddListener(OnClickFarming);
        animalBtn.onClick.AddListener(OnClickAnimal);
        reassignBtn.onClick.AddListener(OnClickReassign);


        uIItemContain.RegisterOnClick(ShowFarmingPanel);
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        cultivationBtn.onClick.RemoveListener(OnClickCultivation);
        buildingBtn.onClick.RemoveListener(OnClickBuilding);
        farmingBtn.onClick.RemoveListener(OnClickFarming);
        animalBtn.onClick.RemoveListener(OnClickAnimal);
        reassignBtn.onClick.RemoveListener(OnClickReassign);
    }

    public void Show(Plot plot)
    {
        selectedPlot = plot;
        titleTxt.text = selectedPlot.Purpose.ToString();
        gameObject.SetActive(true);
        switch(plot.Purpose)
        {
            case ePlotPurpose.Empty:
                ShowPurplePanel();
                break;
            case ePlotPurpose.Cultivation:
                ShowCultivationPanel();
                break;
            case ePlotPurpose.Farming:
                ShowFarmingPanel();
                break;
            case ePlotPurpose.Animal:
                ShowCultivationPanel();
                break;
            case ePlotPurpose.Building:
                ShowBuildPanel();
                break;

        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        selectedPlot = null;
    }

    void ShowPurplePanel()
    {
        purposePanel.gameObject.SetActive(true);
        buildPanel.gameObject.SetActive(false);
    }

    void ShowBuildPanel()
    {
        titleTxt.text = selectedPlot.Purpose.ToString();
        purposePanel.gameObject.SetActive(false);
        buildPanel.gameObject.SetActive(true);
    }

    void ShowCultivationPanel()
    {
        ShowBuildPanel();
        cultivationBtnContain.gameObject.SetActive(selectedPlot.Purpose == ePlotPurpose.Cultivation);
    }

    void ShowFarmingPanel()
    {
        ShowCultivationPanel();
        CultivationData data = CultivationManager.Instance != null
            ? CultivationManager.Instance.GetCultivationData(selectedPlot)
            : null;
        uIItemContain.gameObject.SetActive(data == null);
        uIFarmingInfor.gameObject.SetActive(data != null);
        uIFarmingInfor?.Bind(data);
        
    }

    void OnClickCultivation()
    {
        ShowBuildPanel();
        cultivationBtnContain.gameObject.SetActive(true);
        selectedPlot.Purpose = ePlotPurpose.Cultivation;
        BuilderManager.Instance.BuildCultivationPlot(selectedPlot);
    }

    void OnClickFarming()
    {
        cultivationBtnContain.gameObject.SetActive(false);
        BuilderManager.Instance.BuildCropPlot(selectedPlot);
        selectedPlot.Purpose = ePlotPurpose.Farming;
        uIItemContain?.ShowSeedList(selectedPlot);
    }

    void OnClickAnimal()
    {
        cultivationBtnContain.gameObject.SetActive(false);
        BuilderManager.Instance.BuildAnimalPlot(selectedPlot);
        selectedPlot.Purpose = ePlotPurpose.Animal;
    }

    void OnClickBuilding()
    {
        purposePanel.gameObject.SetActive(false);
        buildPanel.gameObject.SetActive(true);
        selectedPlot.Purpose = ePlotPurpose.Building;
    }

    void OnClickReassign()
    {
        ShowPurplePanel();
        cultivationBtnContain.gameObject.SetActive(true);
        selectedPlot.Purpose = ePlotPurpose.Empty;

        CultivationManager.Instance.UnregisterPlot(selectedPlot);

        BuilderManager.Instance.ClearPlot(selectedPlot);
    }
    
    void HandleFarmingPlot()
    {
        CultivationData data = CultivationManager.Instance != null
            ? CultivationManager.Instance.GetCultivationData(selectedPlot)
            : null;

        // if (data == null)
        // {
        //     // Plot chưa trồng => cho chọn hạt giống
        //     uIItemContain?.ShowSeedList(selectedPlot);
        //     plotInfoPanel.gameObject.SetActive(false);
        //     currentData = null;
        // }
        // else
        // {
        //     // Plot đã trồng => hiển thị thông tin
        //     currentData = data;
        //     plotInfoPanel.gameObject.SetActive(true);
        //     uIItemContain.gameObject.SetActive(false);
        // }
    }
}
