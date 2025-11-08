using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private Transform target; // có thể bỏ trống
    [SerializeField] private float distance = 15f;
    [SerializeField] private float xSpeed = 100f;
    [SerializeField] private float ySpeed = 50f;
    [SerializeField] private float yMinLimit = 20f;
    [SerializeField] private float yMaxLimit = 60f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 25f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    private float x = 45f;  // góc mặc định
    private float y = 35f;

    void LateUpdate()
    {
        // --- Xoay quanh chuột phải ---
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
        }

        // --- Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        // --- Tính vị trí ---
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        if (target != null)
        {
            Vector3 position = rotation * new Vector3(0, 0, -distance) + target.position;
            transform.position = position;
            transform.rotation = rotation;
        }
        else
        {
            // Nếu không có target: chỉ xoay camera quanh chính nó
            transform.rotation = rotation;
        }

        // --- Di chuyển (WASD / Arrow keys) ---
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(move * Time.deltaTime * moveSpeed, Space.Self);
    }
}
