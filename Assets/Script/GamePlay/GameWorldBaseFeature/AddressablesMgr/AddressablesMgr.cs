using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Addressables 资源加载管理器。
/// 负责统一处理异步加载、缓存、实例化、释放和批量预加载逻辑。
/// </summary>
/// <remarks>
/// 建议 AI 生成内容使用 <c>artifact_id__version</c> 形式的 Key，
/// 例如 <c>quest_side_001__v1</c>，便于结合 Label 或分组进行版本管理。
/// </remarks>
/// <example>
/// <code>
/// AddressablesMgr.Instance.LoadAssetAsync&lt;Sprite&gt;("hero_icon__v1", (sprite) => {
///     Debug.Log("Sprite loaded: " + sprite.name);
/// });
///
/// AddressablesMgr.Instance.InstantiateAsync&lt;GameObject&gt;("enemy_prefab__v2", null, (instance) => {
///     Debug.Log("Prefab instantiated: " + instance.name);
/// });
/// </code>
/// </example>
public sealed class AddressablesMgr {
    private static readonly AddressablesMgr INSTANCE = new AddressablesMgr();

    private readonly object _syncRoot = new object();
    private readonly Dictionary<string, UnityEngine.Object> _loadedAssets = new Dictionary<string, UnityEngine.Object>();
    private readonly Dictionary<string, AsyncOperationHandle> _assetHandles = new Dictionary<string, AsyncOperationHandle>();
    private readonly Dictionary<string, List<Action<UnityEngine.Object>>> _pendingAssetCallbacks = new Dictionary<string, List<Action<UnityEngine.Object>>>();
    private readonly Dictionary<string, Type> _loadingAssetTypes = new Dictionary<string, Type>();
    private readonly Dictionary<string, List<AsyncOperationHandle<GameObject>>> _instanceHandles = new Dictionary<string, List<AsyncOperationHandle<GameObject>>>();

    /// <summary>
    /// 获取 Addressables 管理器单例。
    /// </summary>
    public static AddressablesMgr Instance {
        get {
            return INSTANCE;
        }
    }

    private AddressablesMgr() {
    }

    /// <summary>
    /// 异步加载指定 Key 的资源。
    /// 若资源已缓存，则直接返回缓存对象，避免重复加载。
    /// </summary>
    /// <typeparam name="T">要加载的资源类型，例如 <see cref="GameObject"/>、<see cref="Sprite"/>、<see cref="TextAsset"/>。</typeparam>
    /// <param name="key">Addressables 资源 Key 或 Label。</param>
    /// <param name="callback">加载完成后的回调。</param>
    public void LoadAssetAsync<T>(string key, Action<T> callback) where T : UnityEngine.Object {
        ValidateKey(key);

        if (TryGetAssetInternal(key, out T loadedAsset)) {
            Debug.Log($"[AddressablesMgr] Use cached asset. Key: {key}, Type: {typeof(T).Name}");
            callback?.Invoke(loadedAsset);
            return;
        }

        bool shouldStartLoad = false;
        T cachedResult = null;
        bool hasCachedResult = false;
        lock (_syncRoot) {
            if (_loadedAssets.TryGetValue(key, out UnityEngine.Object cachedAsset)) {
                cachedResult = ConvertAsset<T>(key, cachedAsset);
                hasCachedResult = true;
            } else if (_pendingAssetCallbacks.TryGetValue(key, out List<Action<UnityEngine.Object>> pendingCallbacks)) {
                ValidateLoadingType(key, typeof(T));
                pendingCallbacks.Add((asset) => {
                    callback?.Invoke(ConvertAsset<T>(key, asset));
                });
                return;
            } else {
                List<Action<UnityEngine.Object>> callbacks = new List<Action<UnityEngine.Object>>();
                callbacks.Add((asset) => {
                    callback?.Invoke(ConvertAsset<T>(key, asset));
                });

                _pendingAssetCallbacks[key] = callbacks;
                _loadingAssetTypes[key] = typeof(T);
                shouldStartLoad = true;
            }
        }

        if (hasCachedResult) {
            Debug.Log($"[AddressablesMgr] Use cached asset. Key: {key}, Type: {typeof(T).Name}");
            callback?.Invoke(cachedResult);
            return;
        }

        if (!shouldStartLoad) {
            return;
        }

        Debug.Log($"[AddressablesMgr] Start loading asset. Key: {key}, Type: {typeof(T).Name}");
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

        lock (_syncRoot) {
            _assetHandles[key] = handle;
        }

        handle.Completed += (operation) => {
            OnAssetLoaded(key, operation);
        };
    }

    /// <summary>
    /// 异步实例化指定 Key 对应的 Prefab。
    /// </summary>
    /// <typeparam name="T">
    /// 返回类型。
    /// 若传入 <see cref="GameObject"/>，则返回实例对象；
    /// 若传入组件类型，则返回实例上的对应组件。
    /// </typeparam>
    /// <param name="key">Addressables Prefab Key。</param>
    /// <param name="parent">实例化父节点，可为空。</param>
    /// <param name="callback">实例化完成后的回调。</param>
    public void InstantiateAsync<T>(string key, Transform parent, Action<T> callback) where T : UnityEngine.Object {
        ValidateKey(key);
        Debug.Log($"[AddressablesMgr] Start instantiating prefab. Key: {key}, Type: {typeof(T).Name}");

        LoadAssetAsync<GameObject>(key, (prefab) => {
            if (prefab == null) {
                callback?.Invoke(null);
                return;
            }

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, parent);
            handle.Completed += (operation) => {
                OnPrefabInstantiated(key, operation, callback);
            };
        });
    }

    /// <summary>
    /// 释放指定 Key 对应的已加载资源及其实例。
    /// </summary>
    /// <param name="key">需要释放的资源 Key。</param>
    public void ReleaseAsset(string key) {
        ValidateKey(key);

        AsyncOperationHandle assetHandle = default;
        List<AsyncOperationHandle<GameObject>> instanceHandles = null;
        bool hasAssetHandle = false;
        bool hasInstanceHandle = false;

        lock (_syncRoot) {
            if (_assetHandles.TryGetValue(key, out assetHandle)) {
                hasAssetHandle = true;
                _assetHandles.Remove(key);
            }

            if (_instanceHandles.TryGetValue(key, out instanceHandles)) {
                hasInstanceHandle = true;
                _instanceHandles.Remove(key);
            }

            _loadedAssets.Remove(key);
            _pendingAssetCallbacks.Remove(key);
            _loadingAssetTypes.Remove(key);
        }

        if (!hasAssetHandle && !hasInstanceHandle) {
            Debug.LogWarning($"[AddressablesMgr] Release skipped because no cached asset or instance was found. Key: {key}");
            return;
        }

        if (instanceHandles != null) {
            for (int i = 0; i < instanceHandles.Count; ++i) {
                AsyncOperationHandle<GameObject> instanceHandle = instanceHandles[i];
                if (instanceHandle.IsValid()) {
                    Addressables.ReleaseInstance(instanceHandle);
                }
            }
        }

        if (hasAssetHandle && assetHandle.IsValid()) {
            Addressables.Release(assetHandle);
        }

        Debug.Log($"[AddressablesMgr] Release asset finished. Key: {key}");
    }

    /// <summary>
    /// 批量预加载资源列表。
    /// 所有 Key 处理完成后触发回调，无论中途是否存在失败项。
    /// </summary>
    /// <typeparam name="T">预加载资源类型。</typeparam>
    /// <param name="keys">待预加载的 Key 列表。</param>
    /// <param name="callback">全部处理完成后的回调。</param>
    public void PreloadAssets<T>(List<string> keys, Action callback) where T : UnityEngine.Object {
        if (keys == null) {
            Debug.LogError("[AddressablesMgr] Preload keys cannot be null.");
            throw new ArgumentNullException(nameof(keys), "[AddressablesMgr] Preload keys cannot be null.");
        }

        if (keys.Count == 0) {
            Debug.Log("[AddressablesMgr] Preload skipped because key list is empty.");
            callback?.Invoke();
            return;
        }

        PreloadState preloadState = new PreloadState(keys.Count);
        Debug.Log($"[AddressablesMgr] Start preloading assets. Count: {keys.Count}, Type: {typeof(T).Name}");

        for (int i = 0; i < keys.Count; ++i) {
            string currentKey = keys[i];

            try {
                LoadAssetAsync<T>(currentKey, (_) => {
                    CompletePreload(callback, preloadState);
                });
            } catch (Exception exception) {
                Debug.LogException(exception);
                CompletePreload(callback, preloadState);
            }
        }
    }

    /// <summary>
    /// 从缓存中获取已加载资源。
    /// 若资源不存在，则返回空。
    /// </summary>
    /// <typeparam name="T">缓存资源类型。</typeparam>
    /// <param name="key">资源 Key。</param>
    /// <returns>缓存中的资源对象，不存在时返回空。</returns>
    public T GetAsset<T>(string key) where T : UnityEngine.Object {
        ValidateKey(key);

        if (TryGetAssetInternal(key, out T loadedAsset)) {
            return loadedAsset;
        }

        return null;
    }

    private void OnAssetLoaded<T>(string key, AsyncOperationHandle<T> operation) where T : UnityEngine.Object {
        List<Action<UnityEngine.Object>> pendingCallbacks = ExtractPendingCallbacks(key);

        if (operation.Status == AsyncOperationStatus.Succeeded) {
            lock (_syncRoot) {
                _loadedAssets[key] = operation.Result;
                _assetHandles[key] = operation;
            }

            Debug.Log($"[AddressablesMgr] Asset loaded successfully. Key: {key}, Type: {typeof(T).Name}");
            InvokeCallbacks(pendingCallbacks, operation.Result);
            return;
        }

        lock (_syncRoot) {
            _loadedAssets.Remove(key);
            _assetHandles.Remove(key);
        }

        Exception exception = CreateLoadException(key, typeof(T), operation.OperationException);
        Debug.LogException(exception);
        InvokeCallbacks(pendingCallbacks, null);
    }

    private void OnPrefabInstantiated<T>(string key, AsyncOperationHandle<GameObject> operation, Action<T> callback) where T : UnityEngine.Object {
        if (operation.Status != AsyncOperationStatus.Succeeded) {
            Exception exception = CreateLoadException(key, typeof(T), operation.OperationException);
            Debug.LogException(exception);
            callback?.Invoke(null);
            return;
        }

        T instanceResult = ConvertInstance<T>(key, operation.Result);
        if (instanceResult == null) {
            Debug.LogError($"[AddressablesMgr] Instantiated object does not contain requested type. Key: {key}, Type: {typeof(T).Name}");
            Addressables.ReleaseInstance(operation);
            callback?.Invoke(null);
            return;
        }

        lock (_syncRoot) {
            if (!_instanceHandles.TryGetValue(key, out List<AsyncOperationHandle<GameObject>> handles)) {
                handles = new List<AsyncOperationHandle<GameObject>>();
                _instanceHandles[key] = handles;
            }

            handles.Add(operation);
        }

        Debug.Log($"[AddressablesMgr] Prefab instantiated successfully. Key: {key}, Instance: {operation.Result.name}");
        callback?.Invoke(instanceResult);
    }

    private bool TryGetAssetInternal<T>(string key, out T asset) where T : UnityEngine.Object {
        lock (_syncRoot) {
            if (_loadedAssets.TryGetValue(key, out UnityEngine.Object loadedAsset)) {
                asset = ConvertAsset<T>(key, loadedAsset);
                return asset != null;
            }
        }

        asset = null;
        return false;
    }

    private void ValidateLoadingType(string key, Type requestedType) {
        if (_loadingAssetTypes.TryGetValue(key, out Type loadingType) && loadingType != requestedType) {
            throw new InvalidOperationException($"[AddressablesMgr] Asset is already loading with a different type. Key: {key}, CurrentType: {loadingType.Name}, RequestedType: {requestedType.Name}");
        }
    }

    private List<Action<UnityEngine.Object>> ExtractPendingCallbacks(string key) {
        lock (_syncRoot) {
            List<Action<UnityEngine.Object>> pendingCallbacks = null;
            if (_pendingAssetCallbacks.TryGetValue(key, out pendingCallbacks)) {
                _pendingAssetCallbacks.Remove(key);
            }

            _loadingAssetTypes.Remove(key);
            return pendingCallbacks;
        }
    }

    private void InvokeCallbacks(List<Action<UnityEngine.Object>> callbacks, UnityEngine.Object asset) {
        if (callbacks == null) {
            return;
        }

        for (int i = 0; i < callbacks.Count; ++i) {
            callbacks[i]?.Invoke(asset);
        }
    }

    private T ConvertAsset<T>(string key, UnityEngine.Object asset) where T : UnityEngine.Object {
        if (asset == null) {
            return null;
        }

        T typedAsset = asset as T;
        if (typedAsset != null) {
            return typedAsset;
        }

        throw new InvalidCastException($"[AddressablesMgr] Cached asset type mismatch. Key: {key}, CachedType: {asset.GetType().Name}, RequestedType: {typeof(T).Name}");
    }

    private T ConvertInstance<T>(string key, GameObject instance) where T : UnityEngine.Object {
        if (typeof(T) == typeof(GameObject)) {
            return instance as T;
        }

        UnityEngine.Object component = instance.GetComponent(typeof(T));
        if (component != null) {
            return component as T;
        }

        Debug.LogError($"[AddressablesMgr] Failed to get requested component from instantiated prefab. Key: {key}, Type: {typeof(T).Name}");
        return null;
    }

    private Exception CreateLoadException(string key, Type assetType, Exception innerException) {
        return new InvalidOperationException($"[AddressablesMgr] Failed to load Addressables resource. Key: {key}, Type: {assetType.Name}", innerException);
    }

    private void ValidateKey(string key) {
        if (string.IsNullOrWhiteSpace(key)) {
            Debug.LogError("[AddressablesMgr] Addressables key cannot be null or empty.");
            throw new ArgumentException("[AddressablesMgr] Addressables key cannot be null or empty.", nameof(key));
        }
    }

    private void CompletePreload(Action callback, PreloadState preloadState) {
        if (Interlocked.Decrement(ref preloadState.remainingCount) == 0) {
            Debug.Log("[AddressablesMgr] Preload finished.");
            callback?.Invoke();
        }
    }

    private sealed class PreloadState {
        public int remainingCount;

        public PreloadState(int count) {
            remainingCount = count;
        }
    }
}
