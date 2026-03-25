using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UITestScene 对象池演示控制器。
/// 负责构建测试 prefab，并提供简单的生成和回收入口。
/// </summary>
public sealed class UITestPoolDemoController : MonoBehaviour {
    private const string DEMO_ROOT_NAME = "UITestPoolDemoRoot";
    private const string DEMO_TEMPLATE_NAME = "PoolDemoCubeTemplate";
    private const string DEMO_POOL_KEY = "UITestDemoCube";
    private const int PREWARM_COUNT = 3;

    private static UITestPoolDemoController instance;

    private readonly List<GameObject> spawnedList = new List<GameObject>();

    private Transform demoRoot;
    private GameObject demoPrefab;
    private int spawnIndex;

    public static UITestPoolDemoController EnsureInstance() {
        if (instance != null) {
            return instance;
        }

        instance = FindObjectOfType<UITestPoolDemoController>();
        if (instance != null) {
            instance.EnsureSetup();
            return instance;
        }

        GameObject controllerObject = new GameObject(nameof(UITestPoolDemoController));
        instance = controllerObject.AddComponent<UITestPoolDemoController>();
        instance.EnsureSetup();
        return instance;
    }

    /// <summary>
    /// 生成一个测试立方体。
    /// </summary>
    public void SpawnDemoCube() {
        EnsureSetup();
        Vector3 position = GetNextSpawnPosition();
        GameObject instanceObject = PoolManager.Instance.Spawn(DEMO_POOL_KEY, demoPrefab, position, Quaternion.identity, demoRoot);
        if (instanceObject == null) {
            return;
        }

        Renderer renderer = instanceObject.GetComponent<Renderer>();
        if (renderer != null && renderer.material != null) {
            renderer.material.color = Color.Lerp(new Color(0.25f, 0.8f, 1f), new Color(1f, 0.55f, 0.2f), (spawnIndex % 6) / 5f);
        }

        spawnedList.Add(instanceObject);
        spawnIndex++;
    }

    /// <summary>
    /// 回收最近一次生成的测试立方体。
    /// </summary>
    public void RecycleLastCube() {
        EnsureSetup();
        if (spawnedList.Count <= 0) {
            return;
        }

        int lastIndex = spawnedList.Count - 1;
        GameObject instanceObject = spawnedList[lastIndex];
        spawnedList.RemoveAt(lastIndex);
        PoolManager.Instance.Recycle(instanceObject);
    }

    private void Awake() {
        instance = this;
        EnsureSetup();
    }

    private void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    private void EnsureSetup() {
        if (demoRoot == null) {
            GameObject rootObject = GameObject.Find(DEMO_ROOT_NAME);
            if (rootObject == null) {
                rootObject = new GameObject(DEMO_ROOT_NAME);
            }

            demoRoot = rootObject.transform;
        }

        if (demoPrefab == null) {
            demoPrefab = CreateDemoPrefab();
            PoolManager.Instance.Prewarm(DEMO_POOL_KEY, demoPrefab, PREWARM_COUNT, demoRoot);
        }
    }

    private GameObject CreateDemoPrefab() {
        GameObject prefabObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prefabObject.name = DEMO_TEMPLATE_NAME;
        prefabObject.transform.SetParent(demoRoot, false);
        prefabObject.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
        prefabObject.transform.localPosition = new Vector3(0f, -100f, 0f);

        Renderer renderer = prefabObject.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = new Color(0.25f, 0.8f, 1f);
        }

        prefabObject.SetActive(false);
        return prefabObject;
    }

    private Vector3 GetNextSpawnPosition() {
        float x = -2.5f + (spawnIndex % 5) * 1.25f;
        float z = 2f + (spawnIndex / 5) * 1.15f;
        return new Vector3(x, 0.35f, z);
    }
}
