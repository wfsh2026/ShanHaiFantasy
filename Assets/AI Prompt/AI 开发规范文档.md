# AI 开发规范文档

## 1. 目的

本文档定义当前 `ShanHaiFantasy` 项目 `GamePlay` 主干的 AI 开发边界。  
默认原则很简单：

- 主链冻结
- Base 冻结
- 业务从扩展层进入

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

业务扩展目录：

- `Assets/Script/GamePlay/Server/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/Client/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/HUD/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/TestUI/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/TestInput/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/TestAudio/`
- `Assets/ToBundle/Configs/`

---

## 4. 冻结边界

以下内容默认冻结，未获用户明确授权不得修改：

- `GameEngine`
- 所有 `GameWorld/Base`
- 所有 `ModelFeature/Base`
- 所有 `GameWorldBaseFeature/*/Base`

冻结的含义：

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
- UI 下行刷新必须使用字段绑定，不再使用 `Presenter / UIService / 整包 UIState`

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

如果需求触碰以下内容，必须先得到明确授权：

- 任意 `Base`
- 任意主链入口
- 任意注册表、路由器、调度器

---

## 7. 禁止事项

未经授权，AI 禁止：

- 恢复旧版多 World 结构
- 恢复旧版消息总线主链
- 在 `GameEngine` 直接写玩法逻辑
- 在 `GameWorld` 直接写玩法逻辑
- 绕过 `UIManager.Instance` 管理业务 UI
- 绕过 `InputManager` 在业务层直接到处写 `Input.GetKeyDown`
- 绕过 `SceneFlowManager` 在业务 UI 或业务逻辑里直接切场景
- 绕过 `AudioManager` 在业务层到处直接管理 `AudioSource`
- 绕过 `ConfigManager.Instance` 在业务层到处直接加载配置资源
- 绕过 `SaveDataManager.Instance` 在业务层到处直接写本地文件
- 在 UI 中恢复 `UIPresenter / UIService / 整包 UIState`

---

## 8. AI 执行顺序

AI 处理 `GamePlay` 相关需求时，应遵守以下顺序：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/代码规范.md`
3. 再阅读 `Assets/AI Prompt/目录规范.md`
4. 若需求涉及 UI，再阅读 `Assets/AI Prompt/AI UI 开发通用规范文档.md`
5. 若需求涉及 Input，再阅读 `Assets/AI Prompt/AI 输入系统开发规范.md`
6. 若需求涉及 SceneFlow，再阅读 `Assets/AI Prompt/AI 场景流转开发规范.md`
7. 若需求涉及 Audio，再阅读 `Assets/AI Prompt/AI 音频开发规范.md`
8. 若需求涉及 Config，再阅读 `Assets/AI Prompt/AI 配置文件创建规范.md`
9. 若需求涉及 SaveData，再阅读 `Assets/AI Prompt/AI 存档开发规范文档.md`
10. 判断需求是否触碰主链或 Base
11. 若触碰冻结层，必须先确认已获授权

---

## 9. 长期有效规则

从本文档生效开始，以下规则长期有效：

- 核心主链默认冻结
- 所有 Base 默认冻结
- 新功能优先从扩展层进入
- UI 必须统一走 `UIManager.Instance`
- Input 必须统一走 `InputManager`
- SceneFlow 必须统一走 `SceneFlowManager`
- Audio 必须统一走 `AudioManager`
- Config 必须统一走 `ConfigManager.Instance`
- SaveData 必须统一走 `SaveDataManager.Instance`
- UI 数据刷新必须使用字段绑定
