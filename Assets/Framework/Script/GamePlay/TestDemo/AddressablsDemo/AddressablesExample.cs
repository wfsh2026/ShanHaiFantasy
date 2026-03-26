using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesExample : MonoBehaviour {
    // 加载 Prefab
    public void LoadPrefab(string key) {
        Addressables.LoadAssetAsync<GameObject>(key).Completed += OnPrefabLoaded;
    }

    private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj) {
        if (obj.Status == AsyncOperationStatus.Succeeded) {
            GameObject go = Instantiate(obj.Result);
            Debug.Log("Prefab Loaded: " + go.name);
        }
    }

    // 加载 Sprite
    public void LoadSprite(string key) {
        Addressables.LoadAssetAsync<Sprite>(key).Completed += (op) => {
            if (op.Status == AsyncOperationStatus.Succeeded) {
                Sprite s = op.Result;
                Debug.Log("Sprite Loaded: " + s.name);
            }
        };
    }

    // 加载文本数据（AI JSON）
    public void LoadTextAsset(string key) {
        Addressables.LoadAssetAsync<TextAsset>(key).Completed += (op) => {
            if (op.Status == AsyncOperationStatus.Succeeded) {
                string json = op.Result.text;
                Debug.Log("TextAsset Loaded: " + json);
            }
        };
    }
}