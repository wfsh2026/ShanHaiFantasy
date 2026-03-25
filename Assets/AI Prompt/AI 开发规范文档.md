# AI 开发规范文档

## 1. 文档目的

本文档用于约束 `ShanHaiFantasy` 项目中 `GamePlay` 主干框架的 AI 开发边界。

从本文档生效开始，AI 在处理 `GamePlay` 相关需求时，默认只能：

- 新增业务模块
- 在允许的扩展入口挂载业务模块
- 在用户明确授权后，才允许修改核心主链或任意 `Base` 目录

如果用户没有明确提出“允许修改核心框架”或“允许修改 Base”，则 AI 必须将主链代码和所有 `Base` 目录视为冻结代码。

---

## 2. 当前唯一合法主链

当前项目的唯一合法主链如下：

`GameEngine -> GameWorld -> GameWorldServer / GameWorldClient -> Feature 模块`

当前已经明确废弃且禁止恢复的旧结构包括：

- `GameWorldManager`
- `worldList`
- `StartGame`
- 旧消息总线驱动主链
- `GameWorldNetworkServer`
- `GameWorldNetworkClient`
- 第二套并行 world 管理结构

---

## 3. 当前目录约定

当前项目以实际目录为准，禁止 AI 擅自迁移主干目录。

有效主干目录如下：

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

允许扩展的业务目录如下：

- `Assets/Script/GamePlay/Server/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/Client/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/HUD/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/TestUI/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/TestInput/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/TestAudio/`

---

## 4. 核心冻结范围

以下内容默认冻结，AI 未经用户明确同意不得修改：

- `Assets/Script/GamePlay/GameEngine.cs`
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

冻结范围的含义是：

- 不允许擅自改主链结构
- 不允许擅自换生命周期机制
- 不允许擅自换注册/调度/路由机制
- 不允许擅自把业务逻辑写回核心主干

---

## 5. 当前系统分层

### 5.1 GameWorld

`GameWorld` 负责：

- 运行时容器
- Update / FixedUpdate / LateUpdate 驱动
- Feature 挂载与移除
- Client / Server 侧入口持有

`GameWorld` 不负责：

- 具体玩法逻辑
- 具体 UI 逻辑
- 具体输入逻辑
- 具体场景业务逻辑
- 具体音频业务逻辑

### 5.2 Mode

Mode 相关逻辑统一走：

`GameWorldClient / GameWorldServer -> ModeFeatureManager -> ModeManager -> Data / Logic / Stage`

### 5.3 UI

UI 相关逻辑统一走：

`GameWorldClient -> ClientUIFeatureManager -> UIManager -> UIPanel / UIPresenter / UIService`

### 5.4 Input

输入相关逻辑统一走：

`GameWorldClient -> ClientInputFeatureManager -> InputManager -> InputHandler -> UI / Mode`

### 5.5 SceneFlow

场景流转相关逻辑统一走：

`GameWorldClient -> ClientSceneFlowFeatureManager -> SceneFlowManager -> LoadingPanel / Scene Runtime`

### 5.6 Audio

音频相关逻辑统一走：

`GameWorldClient -> ClientAudioFeatureManager -> AudioManager -> AudioEmitter / 业务调用`

---

## 6. 允许 AI 修改的范围

默认情况下，AI 只允许修改以下两类内容：

- 新增业务模块文件
- 在允许的扩展入口挂载新业务模块

允许 AI 正常扩展的内容包括：

- 新增 `Feature`
- 新增 `FeatureManager` 下的业务挂载
- 新增 `ModeData / ModeLogic / ModeStage / ModeManager`
- 新增业务 UI、Presenter、UIService
- 新增业务输入 Handler / Service
- 新增业务音频触发脚本、测试脚本和业务调用入口

如果需求会触碰以下内容，必须先获得用户明确授权：

- 任意 `Base` 目录
- 任意主链入口
- 任意生命周期机制
- 任意注册表、路由器、调度器

---

## 7. 禁止事项

未经授权，AI 禁止进行以下行为：

- 恢复旧版多 World 结构
- 恢复旧版消息主链
- 恢复旧版 StartGame 结构
- 在 `GameEngine` 中直接写玩法逻辑
- 在 `GameWorld` 中直接写玩法逻辑
- 在 `GameWorldServer` 或 `GameWorldClient` 中直接写具体业务逻辑
- 绕过 `FeatureManager` 直接塞业务逻辑到主链
- 绕过 `UIManager` 直接管理业务 UI
- 绕过 `InputManager` 直接在业务层到处写 `Input.GetKeyDown`
- 绕过 `SceneFlowManager` 直接在业务 UI 或业务逻辑中切场景
- 绕过 `AudioManager` 直接在业务层到处管理 `AudioSource`

---

## 8. AI 执行顺序要求

AI 在处理 `GamePlay` 相关需求时，应遵循以下顺序：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/代码规范.md`
3. 再阅读 `Assets/AI Prompt/目录规范.md`
4. 如果需求涉及 UI，再阅读 `Assets/AI Prompt/AI UI 开发通用规范文档.md`
5. 如果需求涉及输入，再阅读 `Assets/AI Prompt/AI 输入系统开发规范.md`
6. 如果需求涉及场景流转，再阅读 `Assets/AI Prompt/AI 场景流转开发规范.md`
7. 如果需求涉及音频，再阅读 `Assets/AI Prompt/AI 音频开发规范.md`
8. 判断需求是否会触碰主链或 `Base`
9. 如果会触碰冻结层，必须先获得用户明确授权

---

## 9. 文档生效规则

从本文档开始，以下规则默认长期生效：

- 核心主链默认冻结
- 所有 `Base` 目录默认冻结
- 新增功能优先走 Feature 扩展
- UI 必须统一走 `UIManager`
- 输入必须统一走 `InputManager`
- 场景流转必须统一走 `SceneFlowManager`
- 音频必须统一走 `AudioManager`

本文档与以下文档共同构成当前项目的 AI 开发约束基础：

- `Assets/AI Prompt/代码规范.md`
- `Assets/AI Prompt/目录规范.md`
- `Assets/AI Prompt/AI UI 开发通用规范文档.md`
- `Assets/AI Prompt/AI 输入系统开发规范.md`
- `Assets/AI Prompt/AI 场景流转开发规范.md`
- `Assets/AI Prompt/AI 音频开发规范.md`
