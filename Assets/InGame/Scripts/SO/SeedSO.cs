using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSeed", menuName = "Game/SeedSO")]
public class SeedSO : ItemSO
{
    public List<GameObject> cropSteps = new();
}
