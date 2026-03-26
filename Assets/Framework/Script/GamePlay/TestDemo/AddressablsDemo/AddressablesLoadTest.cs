using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesLoadTest : MonoBehaviour {
    // Addressable Name
    [SerializeField] private string prefabKey = "TestPrefab";

    // 加载并实例化 prefab
    public void LoadAndInstantiatePrefab() {
        Debug.Log($"Start loading prefab: {prefabKey}");
        Addressables.LoadAssetAsync<GameObject>(prefabKey).Completed += OnPrefabLoaded;
    }

    private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj) {
        if (obj.Status == AsyncOperationStatus.Succeeded) {
            GameObject go = Instantiate(obj.Result);
            go.name = obj.Result.name;
            Debug.Log($"Prefab loaded and instantiated: {go.name}");
        } else {
            Debug.LogError($"Failed to load prefab: {prefabKey}");
        }
    }

    // 可选：释放 prefab
    public void ReleasePrefab(GameObject go) {
        Addressables.ReleaseInstance(go);
        Debug.Log($"Prefab released: {go.name}");
    }

    // Unity Start 示例调用
    private void Start() {
        LoadAndInstantiatePrefab();
    }
}