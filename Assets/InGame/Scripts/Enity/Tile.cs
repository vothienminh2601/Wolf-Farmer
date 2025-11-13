using System.Collections.Generic;
using UnityEngine;

public enum eTileType
{
    Empty,
    Farming,
    Animal
}
public class Tile : MonoBehaviour
{
    public List<Mesh> tileMeshes;
    public int X { get; private set; }
    public int Z { get; private set; }
    public int GlobalX { get; private set; }
    public int GlobalZ { get; private set; }

    public eTileType Type = eTileType.Empty;
    public bool IsOccupied => placedObject != null;
    [SerializeField] private Transform placement;
    private Plot parentPlot;
    [SerializeField] private GameObject placedObject; 
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public void Setup(Plot plot, int x, int z)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        
        parentPlot = plot;
        X = x;
        Z = z;
        Type = eTileType.Empty;

        GlobalX = plot.PlotX * GameConfigs.TILES_PER_PLOT + x;
        GlobalZ = plot.PlotZ * GameConfigs.TILES_PER_PLOT + z;
        UpdateVisual();
    }


    public void SetType(eTileType newType)
    {
        if (IsOccupied)
        {
            Debug.LogWarning($"Tile [{GlobalX},{GlobalZ}] đang có vật thể, không thể đổi kiểu!");
            return;
        }

        Type = newType;
        UpdateVisual();
    }

    public bool PlaceObject(GameObject prefab, Vector3 pos = default, Quaternion rot = default)
    {
        if (IsOccupied)
        {
            Debug.LogWarning($"Tile [{GlobalX},{GlobalZ}] đã có vật thể!");
            return false;
        }

        if (rot == default) rot = Quaternion.identity;

        placedObject = Instantiate(prefab, placement);
        placedObject.transform.localPosition = pos; 
        placedObject.transform.localRotation = rot;
        return true;
    }


    public void Select(bool selected)
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Color baseColor = Color.white;
            rend.material.color = selected ? Color.green : baseColor; 
        }
    }
    
    public void RemovePlacement()
    {
        if (!IsOccupied) return;
        Destroy(placedObject);
        placedObject = null;
    }

    private void UpdateVisual()
    {
        if (meshFilter == null) return;

        switch (Type)
        {
            case eTileType.Empty:
                meshFilter.mesh = tileMeshes[0];
                break;
            case eTileType.Farming:
                meshFilter.mesh = tileMeshes[1];
                break;
            case eTileType.Animal:
                meshFilter.mesh = tileMeshes[2];
                break;
        }
    }

    public Plot GetParentPlot() => parentPlot;
}
