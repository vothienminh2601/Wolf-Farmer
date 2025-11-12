using System.Collections.Generic;
using UnityEngine;

public class UIProductDetail : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIProductDetailItem productDetailItemPrefab;
    [SerializeField] private RectTransform contentParent;

    // Lưu danh sách hiện tại để update sau
    private List<ResourceStack> currentProducts = new();
    private readonly Dictionary<string, UIProductDetailItem> productItems = new();

    /// <summary>
    /// Hiển thị danh sách sản phẩm lần đầu tiên.
    /// </summary>
    public void ShowProductDetails(List<ResourceStack> products)
    {
        ClearList();
        currentProducts = products ?? new List<ResourceStack>();
        productItems.Clear();

        if (currentProducts.Count == 0)
        {
            Debug.Log("UIProductDetail: No products to show.");
            return;
        }

        foreach (var stack in currentProducts)
        {
            if (stack == null || stack.quantity <= 0) continue;

            // Lấy dữ liệu FruitData để lấy tên hiển thị
            var fruitData = DataManager.GetProductById(stack.id);
            string productName = fruitData != null ? fruitData.name : stack.id;

            // Tạo UI item
            UIProductDetailItem item = Instantiate(productDetailItemPrefab, contentParent);
            item.SetProductDetailItem(productName, stack.quantity);

            // Lưu để update sau
            productItems[stack.id] = item;
        }
    }

    /// <summary>
    /// Cập nhật lại danh sách sản phẩm khi có thay đổi số lượng.
    /// </summary>
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

            // Nếu sản phẩm đã có trong UI, chỉ cập nhật lại số lượng
            if (productItems.TryGetValue(stack.id, out var existingItem))
            {
                var fruitData = DataManager.GetProductById(stack.id);
                string productName = fruitData != null ? fruitData.name : stack.id;
                existingItem.SetProductDetailItem(productName, stack.quantity);

                // Nếu số lượng = 0 thì ẩn
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

    /// <summary>
    /// Xóa toàn bộ danh sách UI hiện tại.
    /// </summary>
    private void ClearList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        productItems.Clear();
    }
}
