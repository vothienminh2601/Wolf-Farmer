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

    [Header("UI")]
    [SerializeField] private UIItemContain uIItemContain;
    [SerializeField] private UIFarmingInfor uIFarmingInfor;
    [SerializeField] private UIAnimalInfo uIAnimalInfo;

    [SerializeField] private RectTransform cultivationBtnContain;
    [SerializeField] private TMP_Text titleTxt;


    void Start()
    {
        cultivationBtn.onClick.AddListener(OnClickCultivation);
        buildingBtn.onClick.AddListener(OnClickBuilding);
        farmingBtn.onClick.AddListener(OnClickFarming);
        animalBtn.onClick.AddListener(OnClickAnimal);
        reassignBtn.onClick.AddListener(OnClickReassign);

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
                ShowAnimalPanel();
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
        uIFarmingInfor.gameObject.SetActive(false);
        uIAnimalInfo.gameObject.SetActive(false);
    }

    void ShowCultivationPanel()
    {
        ShowBuildPanel();
        cultivationBtnContain.gameObject.SetActive(selectedPlot.Purpose == ePlotPurpose.Cultivation);
        if(selectedPlot.Purpose == ePlotPurpose.Cultivation)
        {
            uIFarmingInfor.gameObject.SetActive(false);
            uIAnimalInfo.gameObject.SetActive(false);
            uIItemContain.gameObject.SetActive(false);
        }
    }

    void ShowFarmingPanel()
    {
        ShowCultivationPanel();
        CultivationData data = CultivationManager.Instance != null
            ? CultivationManager.Instance.GetCultivationData(selectedPlot)
            : null;

        uIItemContain.gameObject.SetActive(data == null);
        uIFarmingInfor.gameObject.SetActive(data != null);
        if (data != null)
        {
            uIFarmingInfor?.Bind(data);
        }
        else
        {
            uIItemContain?.ShowItemList(selectedPlot, eItemType.Seed);
            uIItemContain.RegisterOnClick(ShowFarmingPanel);
        }
    }
    
    void ShowAnimalPanel()
    {
        ShowCultivationPanel();
        AnimalUnit unit = AnimalManager.Instance != null
            ? AnimalManager.Instance.GetAnimalData(selectedPlot)
            : null;

        uIItemContain.gameObject.SetActive(unit == null);
        uIAnimalInfo.gameObject.SetActive(unit != null);
        if (unit != null)
        {
            uIAnimalInfo?.Bind(unit);
        }
        else
        {
            uIItemContain?.ShowItemList(selectedPlot, eItemType.Animal);
            uIItemContain.RegisterOnClick(ShowAnimalPanel);
        }
    }

    void OnClickCultivation()
    {
        ShowBuildPanel();
        selectedPlot.SetPurpose(ePlotPurpose.Cultivation);
        BuilderManager.Instance.BuildCultivationPlot(selectedPlot);
        if(selectedPlot.Purpose == ePlotPurpose.Cultivation)
        {
            cultivationBtnContain.gameObject.SetActive(true);
            uIFarmingInfor.gameObject.SetActive(false);
            uIAnimalInfo.gameObject.SetActive(false);
            uIItemContain.gameObject.SetActive(false);
        }
    }

    void OnClickFarming()
    {
        cultivationBtnContain.gameObject.SetActive(false);
        BuilderManager.Instance.BuildCropPlot(selectedPlot);
        selectedPlot.SetPurpose(ePlotPurpose.Farming);
        uIItemContain?.ShowItemList(selectedPlot, eItemType.Seed);
    }

    void OnClickAnimal()
    {
        cultivationBtnContain.gameObject.SetActive(false);
        BuilderManager.Instance.BuildAnimalPlot(selectedPlot);
        selectedPlot.SetPurpose(ePlotPurpose.Animal);
         uIItemContain?.ShowItemList(selectedPlot, eItemType.Animal);
    }

    void OnClickBuilding()
    {
        purposePanel.gameObject.SetActive(false);
        buildPanel.gameObject.SetActive(true);
        selectedPlot.SetPurpose(ePlotPurpose.Building);
    }

    void OnClickReassign()
    {
        ShowPurplePanel();
        cultivationBtnContain.gameObject.SetActive(true);
        selectedPlot.SetPurpose(ePlotPurpose.Empty);

        CultivationManager.Instance.UnregisterPlot(selectedPlot);
        AnimalManager.Instance.RemoveAnimalByPlot(selectedPlot);

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
