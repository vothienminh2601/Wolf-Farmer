using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private TMP_InputField quantityInput;
    [SerializeField] private Button plusBtn;
    [SerializeField] private Button minusBtn;

    private int unitPrice;
    private int quantity;
    private int maxQuantity;
    private System.Action onValueChanged;
    private string id;

    public void Setup(string id, string name, int price, int startQty, System.Action onChanged, int maxQty = 999)
    {
        this.id = id;
        unitPrice = price;
        quantity = startQty;
        maxQuantity = maxQty;
        onValueChanged = onChanged;

        nameTxt.text = name;
        priceTxt.text = $"{price}$";
        quantityInput.text = quantity.ToString();

        quantityInput.SetTextWithoutNotify(quantity.ToString());
        quantityInput.onValueChanged.AddListener(OnInputChanged);

        plusBtn.onClick.AddListener(() => ChangeQuantity(1));
        minusBtn.onClick.AddListener(() => ChangeQuantity(-1));
    }

    private void OnInputChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            quantity = 0;
            quantityInput.SetTextWithoutNotify("0"); // hiển thị lại số 0
            onValueChanged?.Invoke();
            return;
        }

        if (int.TryParse(newValue, out int val))
        {
            quantity = Mathf.Clamp(val, 0, maxQuantity);
            onValueChanged?.Invoke();
        }
        else
        {
            quantity = 0;
            quantityInput.SetTextWithoutNotify("0");
            onValueChanged?.Invoke();
        }
    }

    private void ChangeQuantity(int delta)
    {
        quantity = Mathf.Clamp(quantity + delta, 0, maxQuantity);
        quantityInput.text = quantity.ToString();
        onValueChanged?.Invoke();
    }

    public int GetQuantity() => quantity;
    public string GetId() => id;
    public int GetTotalPrice() => quantity * unitPrice;
}
