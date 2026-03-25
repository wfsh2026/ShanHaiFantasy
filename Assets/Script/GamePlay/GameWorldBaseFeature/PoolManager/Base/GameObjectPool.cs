using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个预制对象池。
/// 负责某一类 prefab 的生成、缓存和回收。
/// </summary>
public sealed class GameObjectPool {
    private readonly string poolKey;
    private readonly GameObject prefab;
    private readonly Transform poolRoot;
    private readonly Stack<GameObject> inactiveInstances;
    private readonly HashSet<GameObject> activeInstances;

    public GameObjectPool(string targetPoolKey, GameObject targetPrefab, Transform targetPoolRoot) {
        poolKey = targetPoolKey;
        prefab = targetPrefab;
        poolRoot = targetPoolRoot;
        inactiveInstances = new Stack<GameObject>();
        activeInstances = new HashSet<GameObject>();
    }

    /// <summary>
    /// 预热对象池。
    /// 预先创建指定数量的实例并放入缓存栈。
    /// </summary>
    public void Prewarm(int count) {
        if (prefab == null || count <= 0) {
            return;
        }

        for (int i = 0; i < count; i++) {
            GameObject instance = CreateNewInstance();
            Recycle(instance);
        }
    }

    /// <summary>
    /// 从对象池中取出实例。
    /// 若缓存为空则新建一个实例。
    /// </summary>
    public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent) {
        if (prefab == null) {
            return null;
        }

        GameObject instance = inactiveInstances.Count > 0 ? inactiveInstances.Pop() : CreateNewInstance();
        if (instance == null) {
            return null;
        }

        activeInstances.Add(instance);
        Transform instanceTransform = instance.transform;
        instanceTransform.SetParent(parent, false);
        instanceTransform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    /// <summary>
    /// 回收实例到缓存栈。
    /// </summary>
    public bool Recycle(GameObject instance) {
        if (instance == null) {
            return false;
        }

        activeInstances.Remove(instance);
        instance.SetActive(false);
        instance.transform.SetParent(poolRoot, false);
        inactiveInstances.Push(instance);
        return true;
    }

    /// <summary>
    /// 判断实例是否属于当前对象池。
    /// </summary>
    public bool Owns(GameObject instance) {
        if (instance == null) {
            return false;
        }

        return activeInstances.Contains(instance) || inactiveInstances.Contains(instance);
    }

    /// <summary>
    /// 清理当前对象池中的全部实例和池根节点。
    /// </summary>
    public void Clear() {
        foreach (GameObject instance in activeInstances) {
            if (instance != null) {
                Object.Destroy(instance);
            }
        }

        foreach (GameObject instance in inactiveInstances) {
            if (instance != null) {
                Object.Destroy(instance);
            }
        }

        activeInstances.Clear();
        inactiveInstances.Clear();

        if (poolRoot != null) {
            Object.Destroy(poolRoot.gameObject);
        }
    }

    private GameObject CreateNewInstance() {
        GameObject instance = Object.Instantiate(prefab, poolRoot);
        instance.name = prefab.name + "_Pooled";

        PoolIdentity identity = instance.GetComponent<PoolIdentity>();
        if (identity == null) {
            identity = instance.AddComponent<PoolIdentity>();
        }

        identity.SetPoolKey(poolKey);
        return instance;
    }
}
