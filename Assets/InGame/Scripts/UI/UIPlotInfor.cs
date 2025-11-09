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
                ShowCultivationPanel();
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
        selectedPlot.Purpose = ePlotPurpose.Farming;
    }

    void OnClickAnimal()
    {
        cultivationBtnContain.gameObject.SetActive(false);
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
        BuilderManager.Instance.ClearPlot(selectedPlot);
    }
}
