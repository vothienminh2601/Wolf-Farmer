using UnityEngine;
using UnityEngine.UI;

public class UITilePopup : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button btnFarm;
    [SerializeField] private Button btnAnimal;

    private Tile currentTile;

    void Awake()
    {
        panel.SetActive(false);
        btnFarm.onClick.AddListener(() => OnSelect(eTileType.Farming));
        btnAnimal.onClick.AddListener(() => OnSelect(eTileType.Animal));
    }

    public void Show(Tile tile, Vector3 screenPos)
    {
        currentTile = tile;
        panel.SetActive(true);

        // đặt vị trí popup theo vị trí con trỏ
        panel.transform.position = screenPos;
    }

    public void Hide()
    {
        panel.SetActive(false);
        currentTile = null;
    }

    private void OnSelect(eTileType type)
    {
        if (currentTile != null)
            currentTile.SetType(type);

        Hide();
    }
}
