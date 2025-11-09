using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;
    [SerializeField] private float heightOffset = 3f; // độ cao bổ sung sau khi focus
    [SerializeField] private float isoAngle = 35f;

    private Transform cam;
    private Plot targetPlot;

    private float currentYaw = 45f; // góc xoay Y
    private float currentZoom = 20f; // khoảng cách từ plot
    private Vector3 defaultPos;
    private Quaternion defaultRot;

    void Awake()
    {
        cam = Camera.main.transform;
        defaultPos = transform.position;
        defaultRot = transform.rotation;
    }

    void Update()
    {
        HandleRotationInput();
        HandleZoom();
    }

    // -------------------------------------------------------------
    // Focus camera vào plot theo hướng hiện tại
    // -------------------------------------------------------------
    public void FocusOn(Plot plot)
    {
        if (plot == null) return;
        targetPlot = plot;

        // Tính vị trí camera theo hướng xoay hiện tại
        Quaternion yawRot = Quaternion.Euler(0, currentYaw, 0);
        Quaternion pitchRot = Quaternion.Euler(isoAngle, 0, 0);
        Vector3 offset = yawRot * (pitchRot * Vector3.back * currentZoom);

        Vector3 targetPos = plot.transform.position + offset;

        transform.DOMove(targetPos, moveDuration).SetEase(Ease.InOutSine);
        transform.DORotateQuaternion(Quaternion.LookRotation(plot.transform.position - targetPos), moveDuration);
    }

    // -------------------------------------------------------------
    // Xoay camera quanh plot
    // -------------------------------------------------------------
    private void HandleRotationInput()
    {
        if (targetPlot == null) return;

        if (Input.GetMouseButton(1))
        {
            float rotInput = Input.GetAxis("Mouse X");
            currentYaw += rotInput * rotationSpeed * Time.deltaTime;
        }

        // Giữ góc nghiêng cố định (isometric)
        Quaternion yawRot = Quaternion.Euler(0, currentYaw, 0);
        Quaternion pitchRot = Quaternion.Euler(isoAngle, 0, 0);

        Vector3 offset = yawRot * (pitchRot * Vector3.back * currentZoom);
        transform.position = targetPlot.transform.position + offset;
        transform.LookAt(targetPlot.transform.position);
    }

    // -------------------------------------------------------------
    // Zoom camera (orthographic)
    // -------------------------------------------------------------
    private void HandleZoom()
    {
        if (cam == null) return;
        Camera c = cam.GetComponent<Camera>();
        if (c == null || !c.orthographic) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            c.orthographicSize = Mathf.Clamp(c.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
            currentZoom = Mathf.Lerp(minZoom, maxZoom, Mathf.InverseLerp(maxZoom, minZoom, c.orthographicSize)) + heightOffset;
        }
    }

    public void ResetCamera()
    {
        targetPlot = null;
        currentYaw = 45f;
        currentZoom = 20f;

        transform.DOMove(defaultPos, 1f).SetEase(Ease.InOutSine);
        transform.DORotateQuaternion(defaultRot, 1f).SetEase(Ease.InOutSine);
    }
}
