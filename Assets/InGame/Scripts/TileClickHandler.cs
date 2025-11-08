using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private UITilePopup uITilePopup;

    void Start()
    {
        if (!mainCam) mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    // Hiện popup ở vị trí chuột
                    uITilePopup.Show(tile, Input.mousePosition);
                }
            }
        }

        // ẩn popup khi nhấn chuột phải
        if (Input.GetMouseButtonDown(1))
            uITilePopup.Hide();
    }
}
