using System;
using UnityEngine;

public enum ItemCategory
{
    CropSeed,
    CropProduct,
    Animal,
    AnimalProduct,
    Other
}


[Serializable]
public class ItemData
{
    public ItemSO itemSO;
    public int quantity;
}
