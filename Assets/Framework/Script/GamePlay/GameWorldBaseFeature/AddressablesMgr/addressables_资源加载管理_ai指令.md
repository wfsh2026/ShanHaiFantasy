项目名称：ShanHaiFantasy
目标目录：Assets/Scripts/GamePlay/AddressablesMgr/

目标：
生成一个 Unity C# 框架，用于管理 ShanHaiFantasy 2D 游戏项目中的 Addressables 资源加载。
框架应提供统一接口，用于加载 Prefab、Sprite、UI 元素和 JSON 数据，
支持异步加载、缓存、实例管理和批量预加载。同时必须兼容 AI 生成内容，通过 Key 和版本号管理动态资源。

目录结构：
Assets/
└── Scripts/
    └── GamePlay/
        └── AddressablesMgr/
            ├── AddressablesMgr.cs      # 核心管理类，封装所有 Addressables API 调用
            └── ContentLoader.cs        # 封装类，供游戏系统调用 AddressablesMgr

设计要求：

1. AddressablesMgr.cs
   - 使用单例模式
   - 职责：
     * 异步加载任意类型的资源
     * 异步实例化 Prefab
     * 管理已加载资源缓存
     * 释放已加载资源
     * 批量预加载资源列表
     * 可选：从缓存获取已加载资源
   - 公共方法：
     * LoadAssetAsync<T>(string key, Action<T> callback)
     * InstantiateAsync<T>(string key, Transform parent, Action<T> callback)
     * ReleaseAsset(string key)
     * PreloadAssets<T>(List<string> keys, Action callback)
     * GetAsset<T>(string key)
   - 私有成员：
     * Dictionary<string, UnityEngine.Object> _loadedAssets

2. ContentLoader.cs
   - AddressablesMgr 的封装，供游戏系统和 AI 生成内容调用
   - 职责：
     * 简化 Prefab、Sprite、JSON 的加载调用
     * 抽象 AddressablesMgr，系统无需直接调用 Addressables API
   - 公共方法：
     * LoadPrefab(string key, Action<GameObject> callback)
     * LoadSprite(string key, Action<Sprite> callback)
     * LoadJson(string key, Action<TextAsset> callback)
   - 内部可使用泛型实现

3. 加载策略
   - 所有加载操作必须为异步
   - 缓存已加载资源以避免重复加载
   - Prefab 自动实例化，可指定父 Transform
   - 支持批量预加载资源列表
   - 释放资源实例时需调用释放方法，避免内存泄漏

4. AI 集成要求
   - Addressables Key 格式建议：artifact_id__version
   - AI 生成的内容可动态加载，保持接口统一
   - 支持 Label 或分组管理不同版本和资源类型

5. 示例使用

- 加载 Prefab：
ContentLoader.LoadPrefab("TestLoad", (go) => {
    Debug.Log("Prefab 加载完成: " + go.name);
});

- 加载 JSON 数据：
ContentLoader.LoadJson("quest_side_001__v1", (json) => {
    Debug.Log("JSON 加载完成: " + json.text);
});

- 批量预加载资源：
AddressablesMgr.Instance.PreloadAssets<GameObject>(
    new List<string>{"TestLoad1","TestLoad2"},
    () => { Debug.Log("批量预加载完成"); }
);

6. 实现注意事项
   - 确保 Key 不存在时能抛出异常并记录日志
   - 加载成功和失败都应输出调试日志
   - 可结合 Addressables Label 或元数据支持 AI 内容版本管理
   - 异步方法应保证线程安全
   - 遵循 Unity C# 编码规范，兼容 Unity 2021.3 LTS+

交付内容：
- 两个 C# 文件：AddressablesMgr.cs 与 ContentLoader.cs
- 放置路径：Assets/Scripts/GamePlay/AddressablesMgr/
- 方法和类需有完整注释，描述方法功能、参数及使用示例
- 框架可直接在 Unity 中编译运行

结束

