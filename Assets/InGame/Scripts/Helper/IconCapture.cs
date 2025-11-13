#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class ModelIconGenerator : MonoBehaviour
{
    [MenuItem("Tools/Capture Model Icon (Runtime Render)")]
    static void CaptureIcon()
    {
        // Chọn prefab cần chụp
        GameObject prefab = Selection.activeObject as GameObject;
        if (prefab == null)
        {
            Debug.LogWarning("⚠️ Hãy chọn một Prefab model 3D trong Project trước!");
            return;
        }

        // Tạo RenderTexture tạm
        RenderTexture rt = new RenderTexture(512, 512, 16);
        Camera cam = new GameObject("TempCamera").AddComponent<Camera>();
        cam.backgroundColor = Color.clear;
        cam.clearFlags = CameraClearFlags.Color;
        cam.targetTexture = rt;
        cam.cullingMask = LayerMask.GetMask("Default");

        // Tạo instance tạm của prefab
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;

        // Căn camera nhìn vào model
        Bounds b = CalculateBounds(instance);
        Vector3 camPos = b.center + new Vector3(0, b.extents.y * 0.6f, b.size.magnitude * 1.5f);
        cam.transform.position = camPos;
        cam.transform.LookAt(b.center);
        cam.fieldOfView = 30f;

        // Render
        cam.Render();

        // Chuyển sang Texture2D
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        // Lưu PNG
        string folder = "Assets/Resources/Icons";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = $"{folder}/{prefab.name}_icon.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());

        Debug.Log($"📸 Icon saved: {path}");

        // Cleanup
        RenderTexture.active = null;
        cam.targetTexture = null;
        DestroyImmediate(cam.gameObject);
        DestroyImmediate(instance);
        Object.DestroyImmediate(rt);
        AssetDatabase.Refresh();
    }

    static Bounds CalculateBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds(go.transform.position, Vector3.one);
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }
}
#endif
