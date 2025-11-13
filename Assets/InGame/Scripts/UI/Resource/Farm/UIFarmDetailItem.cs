using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFarmDetailItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private Button clickBtn;

    [SerializeField] private Plot linkedPlot;

    public void Setup(string name, string time, Plot plot = null)
    {
        nameTxt.text = name + ":"  + time;
        linkedPlot = plot;

        // Gỡ listener cũ để tránh double bind
        clickBtn.onClick.RemoveAllListeners();
        clickBtn.onClick.AddListener(OnClickItem);
    }

    private void OnClickItem()
    {
        if (linkedPlot == null)
        {
            Debug.LogWarning("UIFarmDetailItem: No plot linked to this item.");
            return;
        }

        // ✅ Gọi InputManager để focus camera vào plot
        InputManager.Instance.SelectPlot(linkedPlot);

        Debug.Log($"📍 Focused on plot: {linkedPlot.name}");
    }
}
