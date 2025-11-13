using System.Collections.Generic;
using UnityEngine;

public class UIStockDetail : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIStockDetailItem itemPrefab;
    [SerializeField] private RectTransform contentParent;

    private readonly List<UIStockDetailItem> activeItems = new();

    public void ShowStockDetail(List<ResourceStack> seeds, List<ResourceStack> animalBreeds)
    {
        ClearList();

        var resource = ResourceManager.Instance;
        if (resource == null)
        {
            Debug.LogWarning("ResourceManager not found!");
            return;
        }

        foreach (var stack in seeds)
        {
            if (stack == null || stack.quantity <= 0) continue;
            var seedData = DataManager.GetSeedById(stack.id);
            string name = seedData != null ? seedData.name : stack.id;

            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(name, stack.quantity);
            activeItems.Add(item);
        }

        foreach (var stack in animalBreeds)
        {
            if (stack == null || stack.quantity <= 0) continue;
            var animalData = DataManager.GetAnimalById(stack.id);
            Debug.Log(animalData);
            string name = animalData != null ? animalData.name : stack.id;

            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(name, stack.quantity);
            activeItems.Add(item);
        }

        Debug.Log($"UIStockDetail: hiển thị {activeItems.Count} mục trong kho.");
    }

    /// <summary>
    /// Xóa danh sách hiện có trước khi hiển thị mới.
    /// </summary>
    private void ClearList()
    {
        foreach (var item in activeItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        activeItems.Clear();
    }
}
