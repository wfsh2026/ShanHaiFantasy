using System;
using UnityEngine;

/// <summary>
/// 游戏系统与 AI 内容调用的统一资源加载入口。
/// 通过封装 <see cref="AddressablesMgr"/>，避免业务侧直接依赖 Addressables API。
/// </summary>
/// <example>
/// <code>
/// ContentLoader.LoadPrefab("TestLoad", (go) => {
///     Debug.Log("Prefab 加载完成: " + go.name);
/// });
///
/// ContentLoader.LoadJson("quest_side_001__v1", (json) => {
///     Debug.Log("JSON 加载完成: " + json.text);
/// });
/// </code>
/// </example>
public static class ContentLoader {
    /// <summary>
    /// 异步加载并实例化 Prefab。
    /// </summary>
    /// <param name="key">Prefab 的 Addressables Key。</param>
    /// <param name="callback">实例化完成后的回调。</param>
    public static void LoadPrefab(string key, Action<GameObject> callback) {
        LoadPrefab(key, null, callback);
    }

    /// <summary>
    /// 异步加载并实例化 Prefab，可指定父节点。
    /// </summary>
    /// <param name="key">Prefab 的 Addressables Key。</param>
    /// <param name="parent">实例对象父节点。</param>
    /// <param name="callback">实例化完成后的回调。</param>
    public static void LoadPrefab(string key, Transform parent, Action<GameObject> callback) {
        AddressablesMgr.Instance.InstantiateAsync<GameObject>(key, parent, callback);
    }

    /// <summary>
    /// 异步加载 Sprite 资源。
    /// </summary>
    /// <param name="key">Sprite 的 Addressables Key。</param>
    /// <param name="callback">加载完成后的回调。</param>
    public static void LoadSprite(string key, Action<Sprite> callback) {
        LoadAsset(key, callback);
    }

    /// <summary>
    /// 异步加载 JSON 文本资源。
    /// 建议 AI 生成内容使用版本化 Key，例如 <c>quest_side_001__v1</c>。
    /// </summary>
    /// <param name="key">JSON 的 Addressables Key。</param>
    /// <param name="callback">加载完成后的回调。</param>
    public static void LoadJson(string key, Action<TextAsset> callback) {
        LoadAsset(key, callback);
    }

    private static void LoadAsset<T>(string key, Action<T> callback) where T : UnityEngine.Object {
        AddressablesMgr.Instance.LoadAssetAsync<T>(key, callback);
    }
}
