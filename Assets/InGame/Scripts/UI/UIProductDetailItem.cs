using TMPro;
using UnityEngine;

public class UIProductDetailItem : MonoBehaviour
{
    [SerializeField] private TMP_Text productDetailTxt;

    public void SetProductDetailItem(string name, int quantity)
    {
        productDetailTxt.SetText($"{name}: {quantity}");
    }
}
