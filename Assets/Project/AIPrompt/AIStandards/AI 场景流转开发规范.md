# AI 场景流转开发规范

## 1. 文档目的

本文档用于约束 `ShanHaiFantasy` 项目中场景流转、场景重载、Loading 流程相关的 AI 开发方式。

当前场景流转系统已经确定为：

- 挂载位置：客户端常驻 `GameWorld Feature`
- 主入口：`ClientSceneFlowFeatureManager -> SceneFlowManager`
- Loading UI：统一走现有 `UIManager`
- 输入锁：统一走现有 `InputManager`
- `Base` 目录默认冻结

未经用户明确授权，AI 不得修改场景流转主链和 `Base` 目录。

---

## 2. 当前场景流转主链

当前唯一合法主链如下：

`GameWorldClient -> ClientSceneFlowFeatureManager -> SceneFlowManager -> SceneFlowRuntimeRunner -> LoadingPanel / Scene Runtime`

职责边界如下：

- `ClientSceneFlowFeatureManager`
  负责场景流转模块接入 `GameWorld`
- `SceneFlowManager`
  负责外部请求入口、场景注册表访问、切场景请求组织
- `SceneFlowRuntimeRunner`
  负责运行时切场景执行流程
- `LoadingPanel`
  负责 Loading 表现
- `UIManager`
  负责 LoadingPanel 的打开关闭和普通 UI 清理
- `InputManager`
  负责切场景期间的输入阻塞

---

## 3. 当前目录约定

当前目录以项目实际结构为准：

- `Assets/Script/GamePlay/GameWorldBaseFeature/SceneFlow/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`

目录职责如下：

- `SceneFlow/Base/`
  场景流转核心层，包括请求、配置、上下文、执行器、Feature 接入
- `UIManager/Common/`
  放 LoadingPanel 这类可复用的通用场景流转 UI

---

## 4. Base 冻结边界

以下目录默认冻结，AI 未经用户明确授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/SceneFlow/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

这意味着：

- 不允许擅自改动切场景主链
- 不允许擅自改动 Loading 接入主链
- 不允许擅自改动输入阻塞主链
- 不允许擅自改动注册表和请求模型

---

## 5. 场景流转标准对象

场景流转系统统一只允许使用以下对象组织逻辑：

- `SceneId`
- `SceneType`
- `SceneConfig`
- `SceneRequest`
- `SceneLoadingContext`
- `SceneLoadingStep`

禁止在业务脚本中绕过这些对象，直接零散写场景切换逻辑。

---

## 6. 标准切换流程

当前统一切场景流程如下：

1. 校验请求是否合法
2. 构建 `SceneLoadingContext`
3. 打开 `LoadingPanel`
4. 打开输入阻塞
5. 清理普通 UI / Popup
6. 执行目标场景加载
7. 等待场景进入完成
8. 更新 Loading 状态
9. 关闭 LoadingPanel
10. 恢复输入

AI 不允许绕过上述流程，在业务层直接自行切场景。

---

## 7. SceneFlow 与其他系统的交互规则

### 7.1 和 UI 的交互

Loading UI 必须统一走：

`SceneFlowManager / SceneFlowRuntimeRunner -> UIManager -> LoadingPanel`

禁止：

- 业务脚本直接 new Loading UI
- 业务 UI 自己管理切场景 Loading 生命周期

### 7.2 和 Input 的交互

切场景期间的输入阻塞必须统一走：

`SceneFlow -> InputManager`

禁止：

- 在业务脚本里手动零散锁输入
- 在切场景期间让普通输入链继续工作

### 7.3 和 Mode 的交互

Mode 只能发起请求，不能自己执行切场景：

`Mode / UI / Input -> SceneFlowManager.LoadScene(...)`

禁止：

- `ModeLogic` 直接调用 Unity 场景 API
- 业务 UI 直接自己 `LoadScene`

---

## 8. 当前测试示例要求

当前标准测试场景为：

- `Assets/Scenes/UITestScene.unity/UITestScene.unity`

当前标准测试链路包括：

- 点击 `Reload Scene` 按钮重载 `UITestScene`
- 按 `F5` 重载 `UITestScene`
- 重载期间显示 LoadingPanel
- 重载期间阻塞输入
- 重载前关闭普通 UI 与 Popup

AI 后续新增场景流转测试时，应优先复用这套测试链，而不是重新造一套平行验证入口。

---

## 9. 允许 AI 修改的范围

默认允许 AI 修改：

- 与具体业务关联的场景请求发起逻辑
- 非 `Base` 的测试 UI 或测试调用入口
- 业务模块中对 `SceneFlowManager` 的标准调用

默认不允许 AI 修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/SceneFlow/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`
- 场景请求模型
- Loading 主链
- 切场景执行主链

---

## 10. AI 执行顺序要求

AI 在处理场景流转相关需求时，应遵循以下顺序：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 再阅读 `Assets/AI Prompt/AI UI 开发通用规范文档.md`
4. 再阅读 `Assets/AI Prompt/AI 输入系统开发规范.md`
5. 判断需求是否会触碰 `Base`
6. 如果会触碰 `Base`，必须先确认是否已获用户授权
7. 如果不触碰 `Base`，优先在业务入口层接入 `SceneFlowManager`

---

## 11. 文档生效规则

从本文档开始，场景流转相关 AI 开发默认遵守以下规则：

- 切场景必须统一走 `SceneFlowManager`
- Loading 必须统一走 `UIManager`
- 输入阻塞必须统一走 `InputManager`
- 任意 `Base` 目录默认冻结
- `UITestScene` 是当前标准测试入口
