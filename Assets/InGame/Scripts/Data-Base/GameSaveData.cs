using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public ResourceSaveData resourceData;
}

[Serializable]
public class ResourceSaveData
{
    public int coin;
    public List<ResourceStack> seeds = new();
    public List<ResourceStack> animalBreeds = new();
    public List<ResourceStack> products = new();
    public List<ResourceStack> animals = new();
    public List<ResourceStack> equipments = new();
    public List<ResourceStack> workers = new();
}
