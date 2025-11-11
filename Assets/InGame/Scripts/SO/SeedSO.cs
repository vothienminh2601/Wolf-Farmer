using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSeed", menuName = "Game/SeedSO")]
public class SeedSO : ItemSO
{
    public float stageDuration;
    public float fruitInterval;
    public int maxFruitCount;
    public GameObject fruitPrefab;
    public List<GameObject> cropSteps = new();
}
