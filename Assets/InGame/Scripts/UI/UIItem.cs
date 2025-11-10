using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIItem : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Button selectBtn;

    public void Setup(Sprite icon, int quantity, Action onSelect)
    {
        iconImg.sprite = icon;
        quantityTxt.text = $"x{quantity}";
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(() => onSelect?.Invoke());
    }
}
