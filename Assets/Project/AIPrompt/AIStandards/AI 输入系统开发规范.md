# AI 输入系统开发规范

## 1. 文档目的

本文档用于约束 `ShanHaiFantasy` 项目中输入系统相关的 AI 开发方式。

当前输入系统已经确定为：

- 运行环境：`Unity`
- 输入方案：旧版 `UnityEngine.Input`
- 挂载位置：客户端常驻 `GameWorld Feature`
- 主入口：`ClientInputFeatureManager -> InputManager`
- `Base` 目录默认冻结

未经用户明确授权，AI 不得修改输入系统核心主链和 `Base` 目录。

---

## 2. 当前输入主链

当前唯一合法输入主链如下：

`GameWorldClient -> ClientInputFeatureManager -> InputManager -> InputContext / InputRouter / InputHandler -> UI / Mode`

职责边界如下：

- `ClientInputFeatureManager`
  负责输入系统初始化、生命周期接入、常驻持有
- `InputManager`
  负责每帧采样、状态更新、上下文管理、输入分发
- `InputDeviceAdapter`
  负责唯一的底层 `UnityEngine.Input` 采样
- `InputRouter`
  负责按上下文与优先级路由输入
- `IInputHandler`
  负责把输入动作转换为业务意图

---

## 3. 当前目录约定

输入系统当前目录以项目实际结构为准：

- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/TestInput/`

目录职责如下：

- `Base/`
  输入框架核心层，包括输入管理器、状态、动作枚举、上下文、路由器、基础接口
- `TestInput/`
  测试输入 Handler、测试输入 Service、示例输入映射行为

---

## 4. Base 冻结边界

以下目录默认冻结，AI 未经用户明确同意不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

这意味着：

- 不允许擅自改动输入主链
- 不允许擅自改动输入上下文模型
- 不允许擅自改动路由优先级
- 不允许擅自改动动作查询接口

如果要改：

- `ClientInputFeatureManager`
- `InputManager`
- `InputRouter`
- `InputContextManager`
- `InputActionId`

都必须先得到用户明确授权。

---

## 5. 输入系统标准对象

输入系统统一只允许使用以下对象组织逻辑：

- `InputActionId`
  统一动作定义
- `InputBinding`
  动作到按键/轴的映射
- `InputState`
  当前帧输入状态快照
- `InputContextType`
  输入上下文
- `IInputHandler`
  输入处理接口

禁止在业务代码中绕过这套对象，直接到处写原生 `Input.GetKeyDown`。

---

## 6. 输入上下文规则

当前上下文体系如下：

- `Global`
- `UI`
- `Popup`
- `Mode`
- `Block`

当前默认优先级如下：

`Block -> Popup -> UI -> Global -> Mode`

规则如下：

- `Block`
  用于加载、演出、强制锁输入
- `Popup`
  用于弹窗确认、取消、优先消费
- `UI`
  用于普通界面关闭、快捷导航
- `Global`
  用于全局快捷键
- `Mode`
  用于玩法输入

AI 未经授权不得擅自修改上下文优先级。

---

## 7. 输入系统开发规则

### 7.1 输入采样规则

所有底层输入采样只能出现在 `InputDeviceAdapter` 中。

禁止：

- 在 `UI` 中直接写 `Input.GetKeyDown`
- 在 `ModeLogic` 中直接写 `Input.GetKey`
- 在任意业务脚本中直接写轴输入采样

### 7.2 输入分发规则

输入必须通过：

`InputManager -> InputRouter -> IInputHandler`

进行分发。

禁止：

- Handler 之间直接互调
- UI 直接持有 InputManager 并自行采样
- Mode 直接注册 Unity 原生输入监听

### 7.3 业务输入规则

业务输入必须通过 Handler 和 Service 进入模式逻辑：

`InputHandler -> InputService -> ModeLogic / ModeManager`

禁止：

- `InputHandler` 直接修改权威业务数据
- `InputHandler` 直接改 UI 控件
- `InputHandler` 直接持有复杂业务对象并跨层操作

---

## 8. 和 UI 的交互规则

输入系统与 UI 的交互必须走统一入口：

- 关闭普通界面：`UIManager.CloseTop()`
- 打开业务界面：`UIManager.Open<T>()`
- 关闭弹窗：通过 `PopupInputHandler`

输入系统不允许：

- 直接改具体按钮状态
- 直接操作某个 Panel 的内部显示
- 直接替代 UI 的业务逻辑

---

## 9. 和 Mode 的交互规则

玩法输入必须通过：

`InputHandler -> InputService -> ModeLogic / ModeManager`

当前示例要求如下：

- `1`：`AddHP`
- `2`：`ReduceHP`
- `3`：`AddMP`
- `4`：`ReduceMP`
- `Tab`：`NextStage`
- `F5`：`ReloadScene`
- `C`：开关属性主面板
- `Esc`：优先关闭弹窗，再关闭普通界面

如果新增新的玩法输入，AI 应优先：

1. 扩展业务 Handler / Service
2. 复用现有 `InputManager`

而不是修改输入主链。

---

## 10. 当前测试示例要求

当前输入系统以 `UITestScene` 为标准测试场景。

验证范围包括：

- 输入打开/关闭属性面板
- 输入修改 HP / MP
- 输入切换 Stage
- 输入触发场景重载
- 输入与 UI、Mode、SceneFlow 的联动是否正确

AI 后续新增输入示例时，应优先复用当前测试链路，而不是另外起一套测试主线。

---

## 11. 允许 AI 修改的范围

默认允许 AI 修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/TestInput/`
- 业务模式中的输入 Service / 输入适配脚本
- 与业务直接相关的非 `Base` 输入扩展

默认不允许 AI 修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/InputManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`
- 输入上下文优先级
- 输入路由核心机制
- 动作查询主接口

---

## 12. AI 执行顺序要求

AI 在处理输入系统需求时，应遵循以下顺序：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 再阅读 `Assets/AI Prompt/代码规范.md`
4. 判断需求是否会触碰 `Base`
5. 如果会触碰 `Base`，必须先确认是否已获用户授权
6. 如果不触碰 `Base`，优先在 `TestInput/` 或业务扩展层实现

---

## 13. 文档生效规则

从本文档开始，输入系统相关 AI 开发默认遵守以下规则：

- 旧版 Unity Input 仍是当前合法输入方案
- 输入必须统一走 `InputManager`
- 输入采样必须统一走 `InputDeviceAdapter`
- 输入与 UI、Mode、SceneFlow 的交互必须走标准主链
- 任意 `Base` 目录默认冻结
