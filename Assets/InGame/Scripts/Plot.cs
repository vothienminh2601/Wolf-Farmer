using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum ePlotPurpose
{
    Empty,
    Cultivation,
    Farming,
    Animal,
    Building
}


public class Plot : MonoBehaviour
{
    public ePlotPurpose Purpose = ePlotPurpose.Empty;
    public int PlotX { get; private set; }
    public int PlotZ { get; private set; }
    private FarmManager field;
    private float spacing;
    private GameObject tilePrefab;
    private GameObject entityPlot;
    
    [SerializeField] private GameObject outline;
    private Material baseMaterial;

    [SerializeField] private List<Tile> tiles = new();

    [ContextMenu("Ref Components")]
    void RefComponents()
    {
        tiles = GetComponentsInChildren<Tile>(true).ToList();
    }

    // Gọi từ FarmFieldManager
    public void Initialize(FarmManager field, int plotX, int plotZ, float spacing,
                           GameObject tilePrefab, Material baseMaterial)
    {
        this.field = field;
        this.PlotX = plotX;
        this.PlotZ = plotZ;
        this.spacing = spacing;
        this.tilePrefab = tilePrefab;
        this.baseMaterial = baseMaterial;

        outline.transform.localScale = Vector3.one * GameConfigs.TILES_PER_PLOT * 2;
        outline.transform.localPosition = new Vector3(0, GameConfigs.TILES_PER_PLOT, 0);

        GenerateTiles();
    }

    public void Select(bool selected)
    {
        outline?.gameObject.SetActive(selected);
    }

    public void SetEntity(GameObject entity)
    {
        entityPlot = entity;
    }

    public void RemoveEntity()
    {
        Destroy(entityPlot);
        entityPlot = null;
    }

    private void GenerateTiles()
    {
        float offsetX = (GameConfigs.TILES_PER_PLOT - 1) * spacing * 0.5f;
        float offsetZ = (GameConfigs.TILES_PER_PLOT - 1) * spacing * 0.5f;

        for (int x = 0; x < GameConfigs.TILES_PER_PLOT; x++)
        {
            for (int z = 0; z < GameConfigs.TILES_PER_PLOT; z++)
            {
                Vector3 localPos = new Vector3(x * spacing - offsetX, 0, z * spacing - offsetZ);

                GameObject tileObj = Instantiate(tilePrefab, transform);
                tileObj.transform.localPosition = localPos;
                tileObj.transform.localRotation = Quaternion.identity;
                tileObj.name = $"Tile_{x}_{z}";

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null) tile = tileObj.AddComponent<Tile>();
                tile.Setup(this, x, z);

                if (baseMaterial != null)
                {
                    var rend = tileObj.GetComponent<Renderer>();
                    if (rend != null) rend.material = baseMaterial;
                }

                tiles.Add(tile);
            }
        }
    }

    public void SetPurpose(ePlotPurpose purpose)
    {
        Purpose = purpose;
    }

    public IEnumerable<Tile> GetAllTiles() => tiles;
    public Tile GetTile(int x, int z) => tiles.Find(t => t.X == x && t.Z == z);
}
