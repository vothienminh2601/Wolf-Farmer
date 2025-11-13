using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStockDetailItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameTxt;
    public void Setup(string name, int quantity)
    {
        nameTxt.text = $"{name}: x{quantity}";
    }
}
