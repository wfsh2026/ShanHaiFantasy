# AI 配置文件创建规范

## 1. 目的

本文档约束 `ShanHaiFantasy` 项目当前配置文件的 AI 创建方式。  
当前项目的配置系统只允许使用 `ScriptableObject`，不允许 AI 再引入额外的表解析层。

---

## 2. 当前合法配置主链

当前唯一合法配置主链如下：

`GameWorldClient -> ClientConfigFeatureManager -> ConfigManager.Instance -> ScriptableObject Config`

说明：

- `ClientConfigFeatureManager`
  只负责配置系统初始化和预加载。
- `ConfigManager.Instance`
  是唯一合法的配置读取入口。
- `ScriptableObject`
  是唯一合法的配置载体。

禁止恢复或新增以下结构：

- Excel 解析器
- JSON 配置主链
- CSV 配置主链
- 额外代码生成表系统
- 运行时可写配置对象

---

## 3. 配置目录规则

### 3.1 脚本目录

配置管理脚本放在：

- `Assets/Script/GamePlay/GameWorldBaseFeature/ConfigManager/Base/`

业务配置脚本放在对应业务目录：

- 示例：
  `Assets/Script/GamePlay/Client/ModelFeature/TestMode/TestModeConfig.cs`

### 3.2 资源目录

运行时配置资产放在：

- `Assets/ToBundle/Configs/`

规则：

- 配置资源属于运行时可加载资源
- 不放到 `Assets/Art`
- 不放到散乱的业务目录

---

## 4. 配置类规则

配置类必须继承 `ScriptableObject`。

推荐写法：

- 使用 `[CreateAssetMenu(...)]`
- Unity 面板字段使用 `[SerializeField] private`
- 运行时读取使用只读属性

推荐结构：

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "TestModeConfig", menuName = "ShanHaiFantasy/Config/TestModeConfig")]
public sealed class TestModeConfig : ScriptableObject {
    [SerializeField] private int maxHP = 100;

    public int MaxHP {
        get {
            return maxHP;
        }
    }
}
```

禁止：

- 公开可随意修改的运行时配置字段作为业务状态
- 在配置类里写运行时业务逻辑
- 在配置类里缓存运行时对象引用

---

## 5. 配置读取规则

标准读取方式：

`ConfigManager.Instance.Load<TConfig>(callback)`
或
`ConfigManager.Instance.Get<TConfig>()`

规则：

- 配置第一次使用可以异步加载
- 已缓存后直接从 `ConfigManager` 取
- 业务层不直接访问 Addressables API
- 业务层不直接自己写资源 key

禁止：

- 到处直接写 `Addressables.LoadAssetAsync`
- UI、Mode、Audio 各自维护自己的配置缓存

---

## 6. 配置与运行时数据边界

必须明确区分：

### 配置

只放静态只读模板数据，例如：

- 默认 HP / MP
- 自动变化间隔
- 音频 key
- 场景默认策略

### 运行时数据

只放运行时状态，例如：

- 当前 HP
- 当前 MP
- 当前阶段
- 当前计时

规则：

- 配置只负责提供初始值和只读参数
- 运行时状态必须写入 `Data`
- 不允许直接修改配置对象当作运行时状态

---

## 7. 当前推荐接法

以测试模式为例：

1. `ClientConfigFeatureManager` 启动时预加载 `TestModeConfig`
2. `ClientTestModeLogic.OnInit()` 调用 `ConfigManager.Instance.Load<TestModeConfig>(...)`
3. 配置加载成功后：
   - `data.ApplyConfig(config)`
   - `logic` 自己读取自动变化参数
4. 后续运行时只使用 `Data`，不直接修改配置

---

## 8. 冻结边界

以下内容默认冻结，AI 未经明确授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/ConfigManager/Base/`

业务配置类和配置资产允许扩展，但必须遵守本文档规则。

---

## 9. AI 执行要求

AI 处理配置相关需求时必须遵守：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 判断是否触碰 `ConfigManager/Base`
4. 若触碰 `Base`，必须先得到明确授权
5. 业务配置优先放到对应业务目录
6. 配置资产统一放到 `Assets/ToBundle/Configs/`

---

## 10. 长期有效规则

从本文档生效开始，以下规则长期有效：

- 配置文件统一使用 `ScriptableObject`
- 配置读取统一使用 `ConfigManager.Instance`
- 运行时配置资产统一放在 `Assets/ToBundle/Configs/`
- 配置只读，运行时状态仍然落在 `Data`
- 不再允许额外引入复杂表系统
