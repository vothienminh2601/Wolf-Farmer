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
    private bool isSeed; 

    public void Setup(string id, string name, int price, int startQty, System.Action onChanged, int maxQty = 999, bool isSeed = false)
    {
        this.id = id;
        this.isSeed = isSeed;
        unitPrice = price;
        quantity = startQty;
        maxQuantity = maxQty;
        onValueChanged = onChanged;

        nameTxt.text = name;
        priceTxt.text = $"{price}$";

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
            quantityInput.SetTextWithoutNotify("0");
            onValueChanged?.Invoke();
            return;
        }

        if (int.TryParse(newValue, out int val))
        {
            quantity = Mathf.Clamp(val, 0, maxQuantity);

            if (isSeed)
                quantity = Mathf.FloorToInt(quantity / 10f) * 10;

            quantityInput.SetTextWithoutNotify(quantity.ToString());
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
        int step = isSeed ? 10 : 1; // ✅ bước tăng nếu là seed
        quantity = Mathf.Clamp(quantity + delta * step, 0, maxQuantity);
        quantityInput.SetTextWithoutNotify(quantity.ToString());
        onValueChanged?.Invoke();
    }

    public int GetQuantity() => quantity;
    public string GetId() => id;
    public int GetTotalPrice() => quantity * unitPrice;
}
