using System.Collections.Generic;
using UnityEngine;

public class UIFarmDetail : MonoBehaviour
{
    [SerializeField] private UIFarmDetailItem itemPrefab;
    [SerializeField] private RectTransform contentParent;

    private readonly List<UIFarmDetailItem> activeItems = new();

    public void ShowDetails()
    {
        ClearList();
        
        var emptyPlots = FarmManager.Instance.GetEmptyPlots();
        foreach (var plot in emptyPlots)
        {
            var item = Instantiate(itemPrefab, contentParent);
            item.Setup("Empty", "", plot);
            activeItems.Add(item);
        }

        // Plot trồng cây
        var farmingPlots = FarmManager.Instance.GetFarmingPlots();
        foreach (var plot in farmingPlots)
        {
            var data = CultivationManager.Instance.GetCultivationData(plot);
            string cropName = data != null ? data.seed.name.Split("-")[0] : "Farm";
            string timeTxt = data != null && data.IsMature
                ? $"{data.GetTimeToNextProduct()}s"
                : "Is Growing";
            if (data == null) timeTxt = "Empty";
            
            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(cropName, timeTxt, plot);
            activeItems.Add(item);
        }

        // Plot chăn nuôi
        var animalPlots = FarmManager.Instance.GetAnimalPlots();
        foreach (var plot in animalPlots)
        {
            var animal = AnimalManager.Instance.GetAnimalData(plot);
            string name = animal != null ? animal.data.name : "Animal";
            string timeTxt = animal != null
                ? (animal.IsAdult ? $"{Mathf.CeilToInt(animal.GetTimeToNextProduct())}s" : "Growing...")
                : "—";

            if (animal == null) timeTxt = "Empty";

            var item = Instantiate(itemPrefab, contentParent);
            item.Setup(name, timeTxt, plot);
            activeItems.Add(item);
        }
    }

    private void ClearList()
    {
        foreach (var i in activeItems)
            Destroy(i.gameObject);
        activeItems.Clear();
    }
}
