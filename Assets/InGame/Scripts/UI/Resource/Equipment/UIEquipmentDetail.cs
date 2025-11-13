using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentDetail : MonoBehaviour
{
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private TMP_Text priceTxt, efficencyTxt;

    void Start()
    {
        priceTxt.SetText($"UPGRADE: {GameConfigs.PRICE_UPGRADE_EQUIPMENT}");
        upgradeBtn.onClick.AddListener(() => EquipmentManager.Instance.Upgrade());
    }

    public void SetEfficency(float efficency)
    {
        efficencyTxt.SetText($"Efficency: {efficency * 100}%");
    }
}
