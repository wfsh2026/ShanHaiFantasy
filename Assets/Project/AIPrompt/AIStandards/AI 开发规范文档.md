# AI 开发规范文档

## 1. 目的

本文档定义当前 `ShanHaiFantasy` 项目 `GamePlay` 通用框架的 AI 开发边界。

默认原则：

- 主链冻结
- Base 冻结
- 业务从扩展层进入
- 通用模块先设计后实现
- 实现后必须补注释、补文档、补验证

---

## 2. 当前唯一合法主链

当前唯一合法主链如下：

`GameEngine -> GameWorld -> GameWorldServer / GameWorldClient -> Feature`

已废弃且禁止恢复：

- `GameWorldManager`
- `worldList`
- `StartGame`
- 旧版多 World 管理结构
- 旧版总消息流主链

---

## 3. 当前目录约定

核心目录：

- `Assets/Script/GamePlay/Host/GameWorld/Base/`
- `Assets/Script/GamePlay/Server/GameWorld/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`
- `Assets/Script/GamePlay/Host/ModelFeature/Base/`
- `Assets/Script/GamePlay/Server/ModelFeature/Base/`
- `Assets/Script/GamePlay/Client/ModelFeature/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/SceneFlow/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/ConfigManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/SaveDataManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/CameraManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/PoolManager/Base/`

业务扩展目录：

- `Assets/Script/GamePlay/Server/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/Client/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/HUD/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/TestUI/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/TestInput/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/TestAudio/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/PoolManager/TestPool/`
- `Assets/ToBundle/Configs/`

说明：

- 不允许自己发明新的框架层目录
- 新文件夹必须先得到授权
- 通用模块优先放到 `GameWorldBaseFeature`

---

## 4. 冻结边界

以下内容默认冻结，未获授权不得修改：

- `GameEngine`
- 所有 `GameWorld/Base`
- 所有 `ModelFeature/Base`
- 所有 `GameWorldBaseFeature/*/Base`

冻结含义：

- 不改主链结构
- 不改生命周期机制
- 不改注册、路由、调度核心
- 不把业务逻辑写回核心主干

---

## 5. 当前系统分层

### 5.1 Mode

当前合法链路：

`GameWorldClient / GameWorldServer -> ModeFeatureManager -> ModeManager -> Data / Logic / Stage`

### 5.2 UI

当前合法链路：

`GameWorldClient -> ClientUIFeatureManager -> UIManager.Instance -> UIPanel -> UIController -> ClientModeData / ClientModeLogic`

说明：

- `ClientUIFeatureManager` 只负责 `UIManager.Instance` 生命周期绑定
- `UIManager.Instance` 是唯一合法 UI 开关入口
- UI 长期只绑定 `Data`
- Controller 在命令发生时按需调用 `Logic`
- UI 下行刷新必须使用字段绑定

### 5.3 Input

当前合法链路：

`GameWorldClient -> ClientInputFeatureManager -> InputManager -> InputHandler -> UI / Mode`

### 5.4 SceneFlow

当前合法链路：

`GameWorldClient -> ClientSceneFlowFeatureManager -> SceneFlowManager -> LoadingPanel / Scene Runtime`

### 5.5 Audio

当前合法链路：

`GameWorldClient -> ClientAudioFeatureManager -> AudioManager -> AudioEmitter / 业务调用`

### 5.6 Config

当前合法链路：

`GameWorldClient -> ClientConfigFeatureManager -> ConfigManager.Instance -> ScriptableObject Config`

### 5.7 SaveData

当前合法链路：

`GameWorldClient -> ClientSaveDataFeatureManager -> SaveDataManager.Instance -> SettingsData / PlayerLocalData`

### 5.8 Camera

当前合法链路：

`GameWorldClient -> ClientCameraFeatureManager -> CameraManager.Instance`

### 5.9 Pool

当前合法链路：

`GameWorldClient -> ClientPoolFeatureManager -> PoolManager.Instance`

### 5.10 Addressables

当前合法链路：

`业务模块 -> ContentLoader / AddressablesMgr.Instance -> Addressables`

### 5.11 NetworkSync

当前合法链路：

`GameWorldClient -> ClientNetworkFeatureManager -> NetworkSyncClient`

`GameWorldServer -> ServerNetworkFeatureManager -> NetworkSyncServer`

### 5.12 Client 初始化顺序

当前 `GameWorldClient` 的标准初始化顺序如下：

1. `ClientConfigFeatureManager`
2. `ClientSaveDataFeatureManager`
3. `ClientUIFeatureManager`
4. `ClientAudioFeatureManager`
5. `ClientInputFeatureManager`
6. `ClientSceneFlowFeatureManager`
7. `ClientCameraFeatureManager`
8. `ClientPoolFeatureManager`
9. `ClientNetworkFeatureManager`
10. `ClientModeFeatureManager`

规则：

- 新增通用客户端模块时，必须先说明依赖关系
- 依赖配置或本地设置的模块，必须排在 `Config / SaveData` 之后
- 被 `Mode` 直接使用的模块，必须排在 `ClientModeFeatureManager` 之前
- 未经授权，不允许随意改动既有初始化顺序

### 5.13 数据归属规则

当前框架中的数据归属必须保持清晰：

- `Config`
  只保存静态只读模板数据
- `SaveData`
  只保存本地持久化数据
- `ModeData`
  只保存运行时权威状态
- `Logic`
  只负责规则和状态写入
- `UI`
  只绑定 `Data` 做显示

规则：

- 运行时状态不得写回 `Config`
- UI 不得持有权威业务状态副本
- Logic 改状态时，优先写入 `ModeData`
- 新增显示字段时，优先先补 `ModeData` 绑定值，再补 UI 刷新

### 5.14 Client / Host / Server 边界

当前跨端边界必须严格保持：

- `Host`
  放通用运行时逻辑和数据结构
- `Client`
  放 UI、输入、音频、相机、场景流转、本地存档等客户端能力
- `Server`
  放纯服务端逻辑

规则：

- `UI / Input / Audio / Camera / SceneFlow / SaveData` 只允许存在于 `Client`
- 通用数据结构优先放 `Host`
- 服务端逻辑不得依赖客户端表现层对象
- 新增模块前必须先判断它属于 `Client / Host / Server` 哪一侧

---

## 6. AI 允许修改的范围

默认情况下，AI 只允许：

- 新增业务模块
- 在允许的扩展入口挂载业务模块
- 修改明确授权范围内的 Base

正常允许扩展的内容包括：

- 新增 `Feature`
- 新增 `ModeData / ModeLogic / ModeStage / ModeManager`
- 新增业务 UI
- 新增业务 Input Handler
- 新增业务音频触发脚本
- 新增业务配置类和配置资产
- 新增业务本地存档字段和设置项
- 新增业务相机调用和场景相机标记
- 新增业务对象池测试脚本和 prefab 池使用入口
- 新增业务配置驱动的测试入口
- 新增模块对应的 AI 规范文档
- 新增 Addressables 资源使用入口

如需触碰以下内容，必须先得到明确授权：

- 任意 `Base`
- 任意主链入口
- 任意注册表、路由器、调度器
- 任意 `GameWorldClient` 初始化顺序

---

## 7. 禁止事项

未经授权，AI 禁止：

- 恢复旧版多 World 结构
- 恢复旧版消息总线主链
- 在 `GameEngine` 直接写玩法逻辑
- 在 `GameWorld` 直接写玩法逻辑
- 绕过 `UIManager.Instance` 管理业务 UI
- 绕过 `InputManager` 在业务层直接到处写 `Input.GetKeyDown`
- 绕过 `SceneFlowManager` 在 UI 或业务逻辑里直接切场景
- 绕过 `AudioManager` 在业务层到处直接管理 `AudioSource`
- 绕过 `ConfigManager.Instance` 在业务层到处直接加载配置资源
- 绕过 `SaveDataManager.Instance` 在业务层到处直接写本地文件
- 绕过 `CameraManager.Instance` 在业务层到处直接操作 `Camera.main` 或 `Cinemachine`
- 绕过 `PoolManager.Instance` 在业务层到处直接维护公共对象池
- 绕过 `ContentLoader / AddressablesMgr.Instance` 在业务层直接散写加载主链
- 在 UI 中恢复 `UIPresenter / UIService / 整包 UIState`
- 在 `Client` 模块中混入 `Server` 专属逻辑
- 在 `Server` 或 `Host` 中直接依赖客户端表现层模块
- 绕过 `ClientNetworkFeatureManager / ServerNetworkFeatureManager` 直接在业务层 new 正式网络主链
- 修改框架后不补文档、不补验证入口

---

## 8. 新增模块流程

AI 新增通用模块时，必须遵守以下流程：

1. 先给出结构设计，不直接写代码
2. 明确模块主链和依赖关系
3. 明确模块目录和是否涉及新文件夹
4. 若涉及 `Base` 或新文件夹，先得到授权
5. 先落基础层，再接测试入口
6. 接入当前标准测试链
7. 补中文注释
8. 补对应 AI 规范文档
9. 最后回写总规范文档

说明：

- 不允许只写代码不补文档
- 不允许只补文档不接验证入口
- 不允许跳过设计直接改主链

---

## 9. 统一验证规则

当前通用框架修改后，默认至少完成以下验证：

1. Unity 刷新成功
2. 新脚本编译通过
3. 控制台无新的 `error`
4. 若模块有测试入口，必须能在 `UITestScene` 或现有标准测试场景中验证
5. 若改动涉及 UI、Input、SceneFlow、Audio、Camera、Pool，优先复用 `UITestScene`

说明：

- 没有验证的交付不算完成
- 若确实无法验证，必须明确说明卡点
- 新模块优先复用现有测试主链，不重新造平行测试场景

---

## 10. 文档同步规则

当 AI 修改框架时，默认需要同步检查文档：

- `AI 开发规范文档.md`
- 模块对应的 AI 规范文档
- 如涉及通用代码规则，再检查 `代码规范.md` 和 `目录规范.md`

规则：

- 主链变化必须更新总规范
- 模块规则变化必须更新模块规范
- 新增通用模块必须新增对应模块规范
- 文档内容要以项目当前真实代码为准

---

## 11. AI 执行顺序

AI 处理 `GamePlay` 相关需求时，应遵守以下顺序：

1. 先阅读本文档
2. 若需求准备进入多 Agent 协作，再阅读 `Assets/Project/AIPrompt/AIStandards/AI Agent 协作规范.md`
3. 若本轮工作需要多 Agent 协作，先检查当前会话是否已存在标准协作包；若不存在，先重建：
   - `lider`
   - `project-2-code`
   - `CodeAgent`
   - `SkillCreateAgent`
4. 再阅读 `Assets/Project/AIPrompt/AIStandards/代码规范.md`
5. 再阅读 `Assets/Project/AIPrompt/AIStandards/目录规范.md`
6. 若需求涉及 UI，再阅读 `Assets/Project/AIPrompt/AIStandards/AI UI 开发通用规范文档.md`
7. 若需求涉及 Input，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 输入系统开发规范.md`
8. 若需求涉及 SceneFlow，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 场景流转开发规范.md`
9. 若需求涉及 Audio，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 音频开发规范.md`
10. 若需求涉及 Config，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 配置文件创建规范.md`
11. 若需求涉及 SaveData，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 存档开发规范文档.md`
12. 若需求涉及 Camera，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 相机开发规范.md`
13. 若需求涉及 Pool，再阅读 `Assets/Project/AIPrompt/AIStandards/AI 对象池开发规范.md`
14. 若需求涉及 GameWorld，再阅读 `Assets/Project/AIPrompt/AIStandards/AI GameWorld 开发规范.md`
15. 若需求涉及 Mode，再阅读 `Assets/Project/AIPrompt/AIStandards/AI Mode 开发规范.md`
16. 若需求涉及 Addressables，再阅读 `Assets/Project/AIPrompt/AIStandards/AI Addressables 开发规范.md`
17. 若需求涉及 NetworkSync，再阅读 `Assets/Project/AIPrompt/AIStandards/AI NetworkSync 开发规范.md`
18. 判断需求是否触碰主链或 Base
19. 若触碰冻结层，必须先确认已获授权

---

## 12. 长期有效规则

从本文档生效开始，以下规则长期有效：

- 核心主链默认冻结
- 所有 Base 默认冻结
- 新功能优先从扩展层进入
- 通用模块新增必须先设计后实现
- 通用模块实现后必须补注释、补文档、补验证入口
- UI 必须统一走 `UIManager.Instance`
- Input 必须统一走 `InputManager`
- SceneFlow 必须统一走 `SceneFlowManager`
- Audio 必须统一走 `AudioManager`
- Config 必须统一走 `ConfigManager.Instance`
- SaveData 必须统一走 `SaveDataManager.Instance`
- Camera 必须统一走 `CameraManager.Instance`
- Pool 必须统一走 `PoolManager.Instance`
- Addressables 必须统一走现有加载模块
- NetworkSync 正式接入必须走 `ClientNetworkFeatureManager / ServerNetworkFeatureManager`
- UI 数据刷新必须使用字段绑定
- `UITestScene` 是当前默认综合验证入口
# 总规范补充（2026-03-26）

## NetworkSync 当前执行规则
- `NetworkSync` 已正式纳入 `GameWorld` 主链。
- 修改 `NetworkSync` 时，必须同时遵守：
  - `Assets/Project/AIPrompt/AIStandards/AI NetworkSync 开发规范.md`
  - 当前总规范中的 Base 冻结与验证规则
- `NetworkSync` 公共协议层不得直接依赖 `Mirror`。
- `NetworkSync` 新增协议或同步逻辑后，必须补 `UITestScene` 本地回环测试入口或更新现有测试链。

## 当前默认测试入口补充
- `UITestScene -> RoleAttrPanel -> Run Network Demo`
  用于验证 `NetworkSync` 的本地回环同步链。
# 阅读顺序补充（2026-03-26）
- AI 开始理解项目整体结构时，先阅读：
  - `Assets/Project/AIPrompt/AIStandards/AI 整体架构设计文档.md`
- AI 开始准备实际改动时，再继续阅读：
  - `Assets/Project/AIPrompt/AIStandards/AI 开发规范文档.md`
  - 对应模块子规范文档
