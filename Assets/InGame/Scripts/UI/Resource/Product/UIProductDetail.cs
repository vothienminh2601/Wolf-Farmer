using System.Collections.Generic;
using UnityEngine;

public class UIProductDetail : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIProductDetailItem productDetailItemPrefab;
    [SerializeField] private RectTransform contentParent;

    private List<ResourceStack> currentProducts = new();
    private readonly Dictionary<string, UIProductDetailItem> productItems = new();

    public void UpdateProductDetails(List<ResourceStack> updatedProducts = null)
    {
        if (updatedProducts != null)
            currentProducts = updatedProducts;

        if (currentProducts == null || currentProducts.Count == 0)
        {
            ClearList();
            return;
        }

        foreach (var stack in currentProducts)
        {
            if (stack == null) continue;

            if (productItems.TryGetValue(stack.id, out var existingItem))
            {
                var fruitData = DataManager.GetProductById(stack.id);
                string productName = fruitData != null ? fruitData.name : stack.id;
                existingItem.SetProductDetailItem(productName, stack.quantity);
                existingItem.gameObject.SetActive(stack.quantity > 0);
            }
            else if (stack.quantity > 0)
            {
                // Nếu là sản phẩm mới xuất hiện → thêm mới
                var fruitData = DataManager.GetProductById(stack.id);
                string productName = fruitData != null ? fruitData.name : stack.id;

                var newItem = Instantiate(productDetailItemPrefab, contentParent);
                newItem.SetProductDetailItem(productName, stack.quantity);
                productItems[stack.id] = newItem;
            }
        }
    }

    private void ClearList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        productItems.Clear();
    }
}
