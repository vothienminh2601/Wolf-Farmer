using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FarmPlot : MonoBehaviour
{
    public int PlotX { get; private set; }
    public int PlotZ { get; private set; }
    public int TilesPerRow => tilesPerRow;

    private FarmManager field;
    private int tilesPerRow;
    private float spacing;
    private GameObject tilePrefab;
    private Material baseMaterial;

    [SerializeField] private List<Tile> tiles = new();

    [ContextMenu("Ref Components")]
    void RefComponents()
    {
        tiles = GetComponentsInChildren<Tile>(true).ToList();
    }

    // Gọi từ FarmFieldManager
    public void Initialize(FarmManager field, int plotX, int plotZ, int tilesPerRow, float spacing,
                           GameObject tilePrefab, Material baseMaterial)
    {
        this.field = field;
        this.PlotX = plotX;
        this.PlotZ = plotZ;
        this.tilesPerRow = tilesPerRow;
        this.spacing = spacing;
        this.tilePrefab = tilePrefab;
        this.baseMaterial = baseMaterial;

        GenerateTiles();
    }

    // --------------------------------------------
    // Tạo 5x5 tile cho plot này
    // --------------------------------------------
    private void GenerateTiles()
    {
        float offsetX = (tilesPerRow - 1) * spacing * 0.5f;
        float offsetZ = (tilesPerRow - 1) * spacing * 0.5f;

        for (int x = 0; x < tilesPerRow; x++)
        {
            for (int z = 0; z < tilesPerRow; z++)
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

    public IEnumerable<Tile> GetAllTiles() => tiles;
    public Tile GetTile(int x, int z) => tiles.Find(t => t.X == x && t.Z == z);
}
