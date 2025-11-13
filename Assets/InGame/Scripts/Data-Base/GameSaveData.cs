using System;
using System.Collections.Generic;
using UnityEngine;

#region --- ROOT GAME SAVE ---

[Serializable]
public class GameSaveData
{
    public ResourceSaveData resourceData;
    public FarmSaveData farmSaveData;
}
#endregion


#region --- FARM DATA ---

[Serializable]
public class FarmSaveData
{
    public List<PlotSaveData> plots = new();
}

[Serializable]
public class PlotSaveData
{
    public int plotX;
    public int plotZ;
    public ePlotPurpose purpose;

    public CultivationSaveData cultivation;
    public AnimalSaveData animal;

    public string buildingId; // nếu có vật đặc biệt (house, silo,...)
    public string markerId;   // nếu có marker hiển thị trên plot

    public List<TilePlacementSaveData> placements = new(); // các vật được đặt trên tile
}
#endregion


#region --- CULTIVATION DATA ---

[Serializable]
public class CultivationSaveData
{
    public string seedId;
    public eCropStage stage;
    public float growthTimer;
    public int fruitCount;
    public float fruitTimer;
}
#endregion


#region --- ANIMAL DATA ---

[Serializable]
public class AnimalSaveData
{
    public string animalId;
    public float growthTimer;
    public int productCount;
    public bool isAdult;
}
#endregion


#region --- BUILDER / PLACEMENT DATA ---

[Serializable]
public class TilePlacementSaveData
{
    public string objectId;   // addressable id hoặc prefab name
    public Vector3 localPos;
    public Vector3 localRot;
    public Vector3 localScale;
}
#endregion


#region --- RESOURCE DATA ---

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
#endregion
