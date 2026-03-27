# AI 整体架构设计文档

## 1. 文档目标
- 本文档用于帮助 AI 快速理解当前项目的整体架构设计。
- 本文档描述的是“为什么这样设计、各模块负责什么、模块之间如何连接”。
- 本文档不是开发规范替代品。实际改动时，仍然必须同时阅读：
  - `Assets/AI Prompt/AI 开发规范文档.md`
  - 对应模块的子规范文档

## 2. 当前架构目标
- 保持主链简单，避免多层 manager 嵌套。
- 所有基础模块都走统一入口，避免业务代码到处直接操作底层实现。
- `Base` 层尽量冻结，业务扩展优先走 Feature、Mode、Controller、测试模块。
- 数据归属明确：
  - 配置数据归 `Config`
  - 运行时状态归 `Data`
  - 规则处理归 `Logic`
  - 显示归 `UI`

## 3. 当前主链

### 3.1 运行主链
`GameEngine -> GameWorld -> Client / Server Feature -> Mode -> 业务逻辑`

### 3.2 客户端当前常驻模块主链
`GameWorldClient`
-> `ClientConfigFeatureManager`
-> `ClientSaveDataFeatureManager`
-> `ClientUIFeatureManager`
-> `ClientAudioFeatureManager`
-> `ClientInputFeatureManager`
-> `ClientSceneFlowFeatureManager`
-> `ClientCameraFeatureManager`
-> `ClientPoolFeatureManager`
-> `ClientModeFeatureManager`

说明：
- 该顺序体现依赖关系。
- 配置、存档优先初始化，因为后续多个模块会读取它们。
- `UI / Audio / Input / SceneFlow / Camera / Pool` 都是客户端常驻系统层。
- `Mode` 最后初始化，因为它要消费前面的基础能力。

## 4. GameWorld 设计

### 4.1 核心思想
- 当前项目已经移除旧的 `GameWorldManager`、`worldList`、`StartGame`。
- 当前只保留单一 `GameWorld` 主体。

### 4.2 当前主结构
`GameEngine -> GameWorld`

`GameWorld` 内部再分：
- `GameWorldClient`
- `GameWorldServer`

### 4.3 职责
- `GameEngine`
  - Unity 生命周期入口
  - 创建并持有唯一 `GameWorld`
- `GameWorld`
  - 世界上下文
  - 统一驱动 `Update / FixedUpdate / LateUpdate`
  - 管理基础 Feature 与扩展 Feature
- `GameWorldClient`
  - 挂客户端所有常驻系统与 Mode
- `GameWorldServer`
  - 挂服务端逻辑入口

## 5. Mode 设计

### 5.1 目标
- 把玩法模式组织成可扩展、可替换的结构。
- 保持 `Data / Logic / Stage` 职责分离。

### 5.2 当前主链
`ClientModeFeatureManager / ServerModeFeatureManager`
-> `XXXModeManager`
-> `ModeData / ModeLogic / ModeStage`

### 5.3 角色
- `ModeManager`
  - 当前模式根节点
  - 组装 Data、Logic、Stage
- `ModeData`
  - 只保存运行时状态
- `ModeLogic`
  - 只处理规则与状态修改
- `ModeStage`
  - 只处理流程阶段

### 5.4 当前测试模式
- 当前已有 `ClientTestModeManager`
- 已用于承接：
  - HP / MP 测试
  - UI 测试
  - 输入测试
  - 音频测试
  - 场景流转测试
  - 相机测试

## 6. UI 设计

### 6.1 当前核心思想
- UI 调用链必须短。
- UI 不直接持有 `Logic`。
- UI 长期绑定 `Data`，逻辑修改 `Data` 后驱动 UI 局部刷新。

### 6.2 当前主链
`UIManager.Instance -> UIPanel -> UIController -> Data / Logic`

### 6.3 当前规则
- `UI -> Logic`
  - `Panel -> Controller -> Logic`
- `Logic -> UI`
  - `Logic` 改 `Data`
  - `Data` 的 `BindableValue<T>` 回调给 `Controller`
  - `Controller` 调 `Panel` 局部刷新

### 6.4 当前特征
- `UIManager` 是全局单例入口。
- `ClientUIFeatureManager` 只负责单例存在与生命周期绑定。
- 当前不走复杂 `Presenter / UIService / 整包 UIState` 链。
- 常驻 UI 采用字段绑定。
- Popup 采用“打开一次 + 返回结果”的形式。

## 7. Input 设计

### 7.1 当前目标
- 统一旧版 Unity 输入系统入口。
- 不让业务层直接写 `Input.GetKeyDown`。

### 7.2 当前主链
`ClientInputFeatureManager -> InputManager -> InputHandler -> 业务`

### 7.3 当前职责
- `InputManager`
  - 收集输入
  - 维护上下文
  - 分发给对应 handler
- `InputHandler`
  - 把输入转成业务命令

### 7.4 当前测试能力
- `UITestScene` 中已接入：
  - `C` 打开/关闭属性面板
  - `Esc` 关闭顶部 UI
  - `1/2/3/4` 调整 HP / MP
  - `Tab` 切 Stage
  - `F5` 重载场景

## 8. SceneFlow 设计

### 8.1 当前目标
- 统一场景加载、重载、Loading UI、输入阻塞。

### 8.2 当前主链
`ClientSceneFlowFeatureManager -> SceneFlowManager`

### 8.3 当前职责
- `SceneFlowManager`
  - 场景加载入口
  - Loading 管理
  - 输入阻塞
  - UI 清理
- `SceneFlowRuntimeRunner`
  - 实际执行场景切换流程

### 8.4 当前测试入口
- `UITestScene`
- 主面板 `Reload Scene`
- 快捷键 `F5`

## 9. Audio 设计

### 9.1 当前目标
- 逻辑层可直接触发音频。
- 同时支持物体挂载式触发。

### 9.2 当前主链
`ClientAudioFeatureManager -> AudioManager.Instance`

### 9.3 当前职责
- `AudioManager`
  - 全局单例入口
  - 播放 BGM / SFX / UI 音
- `AudioEmitter`
  - 挂在物体上，随显隐或启停触发播放

### 9.4 当前测试入口
- `UITestScene`
- 按钮音效
- HP / MP 变化音效
- `Toggle Emitter` 测试挂物体触发

## 10. Config 设计

### 10.1 当前目标
- 所有配置统一走 `ScriptableObject`
- 不引入额外表系统

### 10.2 当前主链
`ClientConfigFeatureManager -> ConfigManager.Instance -> ScriptableObject Config`

### 10.3 当前规则
- 配置是只读模板
- 运行时状态不得直接改配置对象
- 业务从 `ConfigManager.Instance.Get<TConfig>()` 取配置

### 10.4 当前测试
- `TestModeConfig.asset`
- 当前测试模式默认 HP / MP 与节奏由配置驱动

## 11. SaveData 设计

### 11.1 当前目标
- 系统设置与玩家本地进度统一管理

### 11.2 当前主链
`ClientSaveDataFeatureManager -> SaveDataManager.Instance -> SettingsData / PlayerLocalData`

### 11.3 当前职责
- `SettingsData`
  - 音量、语言、帧率等系统设置
- `PlayerLocalData`
  - 本地进度、上次场景、上次模式等

### 11.4 当前规则
- 业务层不直接写文件
- 业务层只改内存数据
- 统一由 `SaveDataManager` 负责落盘

## 12. Camera 设计

### 12.1 当前目标
- 调用链简单
- 上层只认一个入口
- 底层兼容原生 Camera 和 `Cinemachine`

### 12.2 当前主链
`ClientCameraFeatureManager -> CameraManager.Instance`

### 12.3 当前职责
- `CameraManager`
  - 设置 Follow 目标
  - 设置 LookAt 目标
  - 切换镜头
  - 调整 FOV
  - 触发震屏
  - 场景切换后刷新引用

### 12.4 当前测试
- `ClientTestModeManager` 会为测试目标绑定相机
- HP 变化时会触发轻震屏

## 13. Pool 设计

### 13.1 当前目标
- 统一 prefab 级对象复用
- 当前先不与 Addressables 强耦合

### 13.2 当前主链
`ClientPoolFeatureManager -> PoolManager.Instance`

### 13.3 当前职责
- `PoolManager`
  - 统一 `Spawn / Recycle / Prewarm / Clear`
- `GameObjectPool`
  - 单个池实例
- `PoolIdentity`
  - 记录对象归属池

### 13.4 当前测试
- `UITestScene`
- 主面板 `Spawn Cube / Recycle Cube`

## 14. NetworkSync 设计

### 14.1 当前定位
- 当前已是正式基础网络组件
- 通过 `ClientNetworkFeatureManager / ServerNetworkFeatureManager` 接入 `GameWorld`
- 业务协议通过 `Module + Proxy / Handler` 扩展，不直接写进网络内核

### 14.2 当前主链
`GameWorldClient -> ClientNetworkFeatureManager -> NetworkSyncClient`

`GameWorldServer -> ServerNetworkFeatureManager -> NetworkSyncServer`

`业务层 -> ClientRoomNetProxy -> NetworkSyncRoomClientModule -> NetworkSyncClient`

### 14.3 当前原则
- 协议公共层不直接依赖 `Mirror`
- `Mirror` 依赖只留在 `NetworkSyncMirrorTransport.cs`
- client/server 支持显式 `Dispose()`
- 本地回环传输只用于测试，不作为正式线上传输实现

### 14.4 当前测试
- `UITestScene`
- `RoleAttrPanel -> Run Room Demo`
- 使用本地回环传输验证：
  - Host 创建房间
  - 客户端加入房间
  - 房主开始游戏
  - 开始后禁止继续加入
  - 房主离开解散房间

## 15. 当前最重要的数据归属规则

### 15.1 配置数据
- 放 `Config`
- 只读

### 15.2 运行时状态
- 放 `Data`

### 15.3 规则与状态修改
- 放 `Logic`

### 15.4 显示与交互
- 放 `UI`

### 15.5 场景、音频、相机、输入等基础能力
- 放各自系统层单例

## 16. 当前整体测试场景
- 当前默认综合验证场景为：
  - `Assets/Scenes/UITestScene.unity/UITestScene.unity`

### 该场景当前承担的验证职责
- UI 开关与字段绑定
- Input 快捷键链
- SceneFlow 重载
- Audio 播放与挂物体触发
- Camera 跟随与震屏
- Pool 生成与回收
- NetworkSync 本地回环测试
- TestMode 数据联动

## 17. 当前设计原则总结
- 主链优先简单，不做无意义层级拆分
- 单例入口统一，Feature 负责生命周期托管
- Base 尽量冻结
- 业务扩展优先放到扩展层，不污染主链
- UI 不直接持有 Logic，长期围绕 Data 工作
- 先保证链路清楚，再考虑抽象
- 先保证可测试，再考虑扩展

## 18. AI 使用建议
- 新任务开始前，先判断要改的是：
  - 主链
  - 基础模块
  - Mode
  - 测试模块
- 如果触碰 `Base` 或主链，先检查是否需要用户确认
- 如果只是新增功能，优先扩展，不要回头破坏当前稳定主链
- 修改后默认在 `UITestScene` 中补验证入口，除非该模块天然不适合在此场景验证
