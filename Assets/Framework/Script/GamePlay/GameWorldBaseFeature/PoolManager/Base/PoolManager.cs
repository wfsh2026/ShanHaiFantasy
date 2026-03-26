using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 预制对象池统一入口。
/// 第一版只负责 GameObject 级别的 Spawn、Recycle 和 Prewarm。
/// </summary>
public sealed class PoolManager {
    private const string POOL_ROOT_NAME = "PoolManagerRoot";

    private static readonly PoolManager INSTANCE = new PoolManager();

    private readonly Dictionary<string, GameObjectPool> poolDict;
    private Transform rootTransform;

    public static PoolManager Instance {
        get {
            return INSTANCE;
        }
    }

    private PoolManager() {
        poolDict = new Dictionary<string, GameObjectPool>();
    }

    /// <summary>
    /// 初始化对象池系统。
    /// 每次进入场景时重新校验根节点和运行时缓存。
    /// </summary>
    public void Initialize() {
        ClearRuntimeIfRootLost();
        EnsureRoot();
    }

    /// <summary>
    /// 按指定 key 和 prefab 生成实例。
    /// </summary>
    public GameObject Spawn(string poolKey, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) {
        if (string.IsNullOrEmpty(poolKey) || prefab == null) {
            return null;
        }

        EnsureRoot();
        GameObjectPool pool = GetOrCreatePool(poolKey, prefab);
        return pool.Spawn(position, rotation, parent);
    }

    /// <summary>
    /// 预热指定对象池。
    /// </summary>
    public void Prewarm(string poolKey, GameObject prefab, int count, Transform parent = null) {
        if (string.IsNullOrEmpty(poolKey) || prefab == null || count <= 0) {
            return;
        }

        EnsureRoot();
        GameObjectPool pool = GetOrCreatePool(poolKey, prefab, parent);
        pool.Prewarm(count);
    }

    /// <summary>
    /// 回收实例到所属对象池。
    /// 若实例不属于对象池，则直接销毁。
    /// </summary>
    public bool Recycle(GameObject instance) {
        if (instance == null) {
            return false;
        }

        PoolIdentity identity = instance.GetComponent<PoolIdentity>();
        if (identity == null || string.IsNullOrEmpty(identity.PoolKey)) {
            Object.Destroy(instance);
            return false;
        }

        GameObjectPool pool;
        if (!poolDict.TryGetValue(identity.PoolKey, out pool) || pool == null) {
            Object.Destroy(instance);
            return false;
        }

        return pool.Recycle(instance);
    }

    /// <summary>
    /// 判断实例是否由对象池系统管理。
    /// </summary>
    public bool Contains(GameObject instance) {
        if (instance == null) {
            return false;
        }

        PoolIdentity identity = instance.GetComponent<PoolIdentity>();
        if (identity == null || string.IsNullOrEmpty(identity.PoolKey)) {
            return false;
        }

        return poolDict.ContainsKey(identity.PoolKey);
    }

    /// <summary>
    /// 清理指定 key 的对象池。
    /// </summary>
    public void Clear(string poolKey) {
        if (string.IsNullOrEmpty(poolKey)) {
            return;
        }

        GameObjectPool pool;
        if (!poolDict.TryGetValue(poolKey, out pool) || pool == null) {
            return;
        }

        pool.Clear();
        poolDict.Remove(poolKey);
    }

    /// <summary>
    /// 清理全部对象池和对象池根节点。
    /// </summary>
    public void ClearAll() {
        foreach (GameObjectPool pool in poolDict.Values) {
            if (pool != null) {
                pool.Clear();
            }
        }

        poolDict.Clear();

        if (rootTransform != null) {
            Object.Destroy(rootTransform.gameObject);
            rootTransform = null;
        }
    }

    private GameObjectPool GetOrCreatePool(string poolKey, GameObject prefab, Transform parent = null) {
        GameObjectPool pool;
        if (poolDict.TryGetValue(poolKey, out pool) && pool != null) {
            return pool;
        }

        Transform poolRoot = CreatePoolRoot(poolKey, parent);
        pool = new GameObjectPool(poolKey, prefab, poolRoot);
        poolDict[poolKey] = pool;
        return pool;
    }

    private void EnsureRoot() {
        if (rootTransform != null) {
            return;
        }

        GameObject rootObject = new GameObject(POOL_ROOT_NAME);
        rootTransform = rootObject.transform;
    }

    private Transform CreatePoolRoot(string poolKey, Transform parent) {
        Transform actualParent = parent != null ? parent : rootTransform;
        GameObject poolRootObject = new GameObject(poolKey + "_Pool");
        Transform poolRootTransform = poolRootObject.transform;
        poolRootTransform.SetParent(actualParent, false);
        return poolRootTransform;
    }

    private void ClearRuntimeIfRootLost() {
        if (rootTransform == null && poolDict.Count > 0) {
            poolDict.Clear();
        }
    }
}
