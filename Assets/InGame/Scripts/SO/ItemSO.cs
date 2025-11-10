using UnityEngine;



[CreateAssetMenu(fileName = "NewItem", menuName = "Game/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;

    public ItemCategory category;
    public int baseValue;  
}
