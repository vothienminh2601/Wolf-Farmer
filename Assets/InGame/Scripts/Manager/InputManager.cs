using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask plotMask;
    [SerializeField] private LayerMask tileMask;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UITilePopup uiTilePopUp;
    [SerializeField] private UIPlotInfor uIPlotInfor;
    [SerializeField] private Plot selectedPlot;   // plot hiện tại
    [SerializeField] private Tile selectedTile;   // tile hiện tại

    private bool isFocusedOnPlot = false;         // đã focus camera chưa
    private Camera cam;
    [SerializeField] private bool isTileEditMode = false;

    // 🔹 Sự kiện callback
    public static event Action<Plot> OnPlotClicked;
    public static event Action<Tile> OnTileClicked;
    public static event Action<Tile> OnTileSelected;
    public static event Action<Tile> OnTileDeselected;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // chạm UI thì bỏ qua
            HandleClick();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIOption.Instance.Toggle();
        }

    }

    // -------------------------------------------------------------
    // CLICK HANDLING
    // -------------------------------------------------------------
    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // 1️⃣ Nếu click trúng tile
        if (Physics.Raycast(ray, out RaycastHit hitTile, 300f, tileMask))
        {
            Tile tile = hitTile.collider.GetComponentInParent<Tile>();
            if (tile != null)
            {
                Plot plotOfTile = tile.GetParentPlot();

                // Nếu chưa focus vào plot hoặc đang focus plot khác
                if (!isFocusedOnPlot || selectedPlot != plotOfTile)
                {
                    SelectPlot(plotOfTile);
                    
                    isFocusedOnPlot = true;
                    DeselectTile();
                    return;
                }

                // // Nếu đã focus đúng plot rồi → chọn tile
                // if (isFocusedOnPlot && selectedPlot == plotOfTile)
                // {
                //     SelectTile(tile);
                //     OnTileClicked?.Invoke(tile);
                //     return;
                // }
            }
        }
        else
        {
            // click ra ngoài → hủy chọn
            DeselectTile();
            DeselectPlot();
        }
    }
    // -------------------------------------------------------------
    // TILE SELECTION MANAGEMENT
    // -------------------------------------------------------------

    public void SelectPlot(Plot Plot)
    {
        if (selectedPlot == Plot) return; // click lại cùng Plot
        DeselectPlot(); // hủy Plot cũ

        selectedPlot = Plot;
        selectedPlot.Select(true);

        uIPlotInfor?.Show(Plot);
        OnPlotClicked?.Invoke(Plot);
        cameraController.FocusOn(Plot);
    }

    private void DeselectPlot()
    {
        if (selectedPlot != null)
        {
            selectedPlot.Select(false);
            selectedPlot = null;
            uIPlotInfor?.Hide();
            cameraController.ResetCamera();
        }
    }

    private void SelectTile(Tile tile)
    {
        if (selectedTile == tile) return; // click lại cùng tile
        DeselectTile(); // hủy tile cũ

        selectedTile = tile;
        selectedTile.Select(true);

        uiTilePopUp?.Show(tile);
        OnTileSelected?.Invoke(tile);
    }

    private void DeselectTile()
    {
        if (selectedTile != null)
        {
            selectedTile.Select(false);
            OnTileDeselected?.Invoke(selectedTile);
            selectedTile = null;
            uiTilePopUp?.Hide();
        }
    }

    // -------------------------------------------------------------
    // MODE TOGGLE
    // -------------------------------------------------------------
    public void SetTileEditMode(bool enable)
    {
        isTileEditMode = enable;
    }
}
