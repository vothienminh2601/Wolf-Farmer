using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteManager : Singleton<SpriteManager>
{
    // Cache sprite theo key (Addressables address) hoặc tên bạn chọn.
    private readonly Dictionary<string, Sprite> cache = new();

    // Lưu handle để giải phóng đúng cách
    private readonly Dictionary<string, AsyncOperationHandle<Sprite>> keyHandles = new();
    private readonly Dictionary<string, AsyncOperationHandle<IList<Sprite>>> labelHandles = new();

    void Start()
    {
        StartCoroutine(PreloadByLabel("Icon"));
    }

    // ------------------ LOAD ĐƠN LẺ THEO KEY ------------------
    public void LoadSpriteAsync(string key, Action<Sprite> callback)
    {
        if (cache.TryGetValue(key, out var s)) { callback?.Invoke(s); return; }

        var handle = Addressables.LoadAssetAsync<Sprite>(key);
        keyHandles[key] = handle;

        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                cache[key] = h.Result;
                callback?.Invoke(h.Result);
            }
            else
            {
                Debug.LogWarning($"SpriteManager: Load thất bại key={key}");
                keyHandles.Remove(key);
                callback?.Invoke(null);
            }
        };
    }

    // ------------------ PRELOAD THEO DANH SÁCH KEY ------------------
    public IEnumerator PreloadKeys(IEnumerable<string> keys, Action<float> onProgress = null)
    {
        var list = new List<string>(keys);
        int done = 0;
        foreach (var k in list)
        {
            if (cache.ContainsKey(k)) { done++; onProgress?.Invoke((float)done / list.Count); continue; }

            var h = Addressables.LoadAssetAsync<Sprite>(k);
            keyHandles[k] = h;

            yield return h;

            if (h.Status == AsyncOperationStatus.Succeeded)
                cache[k] = h.Result;
            else
                Debug.LogWarning($"SpriteManager: Preload key thất bại {k}");

            done++;
            onProgress?.Invoke((float)done / list.Count);
        }
    }

    // ------------------ PRELOAD THEO LABEL (KHUYÊN DÙNG CHO TOÀN BỘ ICON) ------------------
    public IEnumerator PreloadByLabel(string label, Action<float> onProgress = null)
    {
        if (labelHandles.ContainsKey(label)) yield break;

        var handle = Addressables.LoadAssetsAsync<Sprite>(
            label,
            s => { if (s != null) cache[GetStableKey(s)] = s; },
            releaseDependenciesOnFailure: true
        );

        while (!handle.IsDone)
        {
            onProgress?.Invoke(handle.PercentComplete);
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
            labelHandles[label] = handle;
        else
            Debug.LogError($"SpriteManager: Preload label thất bại: {label}");
    }

    // ------------------ TRUY XUẤT ------------------
    public Sprite GetCached(string key) => cache.TryGetValue(key, out var s) ? s : null;

    // Nếu bạn preload theo label và dùng key là tên sprite:
    private static string GetStableKey(Sprite s) => s.name; // hoặc tự đặt quy ước

    // ------------------ GIẢI PHÓNG ------------------
    public void ReleaseKey(string key)
    {
        if (keyHandles.TryGetValue(key, out var h))
        {
            Addressables.Release(h);
            keyHandles.Remove(key);
        }
        cache.Remove(key);
    }

    public void ReleaseLabel(string label)
    {
        if (labelHandles.TryGetValue(label, out var h))
        {
            Addressables.Release(h);
            labelHandles.Remove(label);
        }
        // Nếu muốn chắc chắn, có thể Clear toàn bộ cache hoặc lọc theo label.
        cache.Clear();
        // keyHandles giữ nguyên cho các key load riêng lẻ.
    }

    public void ReleaseAll()
    {
        foreach (var kv in keyHandles) Addressables.Release(kv.Value);
        keyHandles.Clear();

        foreach (var kv in labelHandles) Addressables.Release(kv.Value);
        labelHandles.Clear();

        cache.Clear();
    }
}
