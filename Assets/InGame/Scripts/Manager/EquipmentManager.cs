using System;
using UnityEngine;

public class EquipmentManager : Singleton<EquipmentManager>
{
    [Header("Upgrade Settings")]
    [SerializeField] private int level = 0;

    public int Level => level;
    public float Efficiency => 1f + level * 0.1f; // +10% mỗi level

    public static event Action<int, float> OnUpgrade;

    void Start()
    {
        LoadData();
    }

    public void Upgrade()
    {
        if (ResourceManager.Instance.GetCoin() < GameConfigs.PRICE_UPGRADE_EQUIPMENT)
        {
            Debug.LogWarning("Không đủ tiền để nâng cấp!");
            return;
        }

        ResourceManager.Instance.SpendCoin(GameConfigs.PRICE_UPGRADE_EQUIPMENT);
        level++;

        OnUpgrade?.Invoke(level, Efficiency);
        SaveData();

        Debug.Log($"Đã nâng cấp thiết bị lên cấp {level} (hiệu suất: {(Efficiency * 100f):F1}%)");
    }

    public void SaveData()
    {
        UserData.Instance.SetData("equipment_level", level);
    }

    public void LoadData()
    {
        level = UserData.Instance.GetData<int>("equipment_level");
    }
}
