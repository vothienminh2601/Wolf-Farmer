using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrefabManager : Singleton<PrefabManager>
{

    private readonly Dictionary<string, GameObject> _cache = new();

    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _keyHandles = new();
    private readonly Dictionary<string, AsyncOperationHandle<IList<GameObject>>> _labelHandles = new();

    private readonly Dictionary<string, HashSet<string>> _labelKeys = new(); // label -> keys

    void Start()
    {
        StartCoroutine(PreloadByLabel("Prefab"));
    }

    // ---------- LOAD 1 PREFAB THEO KEY ----------
    public void LoadPrefabAsync(string key, Action<GameObject> callback)
    {
        if (_cache.TryGetValue(key, out var prefab)) { callback?.Invoke(prefab); return; }

        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        _keyHandles[key] = handle;

        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                _cache[key] = h.Result;
                callback?.Invoke(h.Result);
            }
            else
            {
                Debug.LogWarning($"PrefabManager: Load thất bại key={key}");
                _keyHandles.Remove(key);
                callback?.Invoke(null);
            }
        };
    }

    // ---------- PRELOAD NHIỀU KEY ----------
    public IEnumerator PreloadKeys(IEnumerable<string> keys, Action<float> onProgress = null)
    {
        var list = new List<string>(keys);
        int done = 0;

        foreach (var k in list)
        {
            if (_cache.ContainsKey(k))
            {
                done++;
                onProgress?.Invoke((float)done / list.Count);
                continue;
            }

            var h = Addressables.LoadAssetAsync<GameObject>(k);
            _keyHandles[k] = h;

            yield return h;

            if (h.Status == AsyncOperationStatus.Succeeded)
                _cache[k] = h.Result;
            else
                Debug.LogWarning($"PrefabManager: Preload key thất bại {k}");

            done++;
            onProgress?.Invoke((float)done / list.Count);
        }
    }

    // ---------- PRELOAD THEO LABEL ----------
    public IEnumerator PreloadByLabel(string label, Action<float> onProgress = null)
    {
        if (_labelHandles.ContainsKey(label)) yield break;

        if (!_labelKeys.ContainsKey(label)) _labelKeys[label] = new HashSet<string>();

        var handle = Addressables.LoadAssetsAsync<GameObject>(
            label,
            go =>
            {
                if (go == null) return;
                var key = GetStableKey(go); // quy ước key
                _cache[key] = go;
                _labelKeys[label].Add(key);
            },
            releaseDependenciesOnFailure: true
        );

        while (!handle.IsDone)
        {
            onProgress?.Invoke(handle.PercentComplete);
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
            _labelHandles[label] = handle;
        else
            Debug.LogError($"PrefabManager: Preload label thất bại: {label}");
    }

    // ---------- TRUY XUẤT ----------
    public GameObject GetCached(string key) => _cache.TryGetValue(key, out var go) ? go : null;

    // Tạo instance trực tiếp từ key (không cần chờ load trước)
    public void InstantiateAsync(string key, Vector3 pos, Quaternion rot, Transform parent, Action<GameObject> callback)
    {
        var handle = Addressables.InstantiateAsync(key, pos, rot, parent);
        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded) callback?.Invoke(h.Result);
            else
            {
                Debug.LogWarning($"PrefabManager: Instantiate thất bại key={key}");
                callback?.Invoke(null);
            }
        };
    }

    // Tạo instance từ cache nếu có, fallback sang Addressables.InstantiateAsync nếu chưa có
    public void InstantiateFromCacheOrKey(string key, Vector3 pos, Quaternion rot, Transform parent, Action<GameObject> callback)
    {
        if (_cache.TryGetValue(key, out var prefab))
        {
            var inst = UnityEngine.Object.Instantiate(prefab, pos, rot, parent);
            callback?.Invoke(inst);
            return;
        }
        InstantiateAsync(key, pos, rot, parent, callback);
    }

    private static string GetStableKey(GameObject go)
    {
        // Mặc định dùng name; nên thay bằng Address hoặc tự map nếu có nguy cơ trùng tên
        return go.name;
    }

    // ---------- GIẢI PHÓNG ----------
    public void ReleaseKey(string key)
    {
        if (_keyHandles.TryGetValue(key, out var h))
        {
            Addressables.Release(h);
            _keyHandles.Remove(key);
        }
        _cache.Remove(key);

        // loại key khỏi mọi label track
        foreach (var set in _labelKeys.Values) set.Remove(key);
    }

    public void ReleaseLabel(string label)
    {
        if (_labelHandles.TryGetValue(label, out var h))
        {
            Addressables.Release(h);
            _labelHandles.Remove(label);
        }

        if (_labelKeys.TryGetValue(label, out var keys))
        {
            foreach (var k in keys) _cache.Remove(k);
            _labelKeys.Remove(label);
        }
    }

    public void ReleaseAll()
    {
        foreach (var kv in _keyHandles) Addressables.Release(kv.Value);
        _keyHandles.Clear();

        foreach (var kv in _labelHandles) Addressables.Release(kv.Value);
        _labelHandles.Clear();

        _cache.Clear();
        _labelKeys.Clear();
    }
}
