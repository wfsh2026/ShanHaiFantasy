# AI UI 开发通用规范文档

## 1. 目的

本文档约束 `ShanHaiFantasy` 项目当前 UI 架构的 AI 开发边界。  
当前 UI 已经明确采用简化调用链，后续 AI 不允许再按旧的 `Presenter / UIService / 整包 UIState` 方式继续扩写。

---

## 2. 当前合法 UI 主链

当前唯一合法 UI 主链如下：

`GameWorldClient -> ClientUIFeatureManager -> UIManager.Instance -> UIPanel -> UIController -> ClientModeData / ClientModeLogic`

说明：

- `ClientUIFeatureManager`
  只负责 `UIManager.Instance` 的绑定和解绑，不承担业务调用入口。
- `UIManager.Instance`
  是唯一合法的 UI 开关入口。
- `UIPanel`
  只负责控件创建、显示、按钮转发、局部刷新。
- `UIController`
  负责业务命令调用和字段绑定。
- `ClientModeData / ClientModeLogic`
  负责权威业务数据和业务行为。

禁止恢复以下旧链路：

- `UIPresenter`
- `UIService`
- `DataChanged` 整包广播
- `BuildState / BuildDefaultState` 整页拼装刷新

---

## 3. 当前数据通信规则

### 3.1 UI -> Logic

标准链路：

`Panel -> Controller -> Logic`

适用场景：

- 按钮点击
- 输入确认
- 打开弹窗
- 切换阶段
- 切场景

禁止：

- Panel 直接修改 `ClientModeData`
- Panel 直接操作 `GameWorld` 业务逻辑
- Panel 自己写业务判断
- Controller 长期缓存 `Logic` 作为显示依赖

### 3.2 Logic -> UI

标准链路：

`BindableValue<T> -> Controller 回调 -> Panel 局部刷新`

适用场景：

- HP 变化
- MP 变化
- StageName 变化
- RunningTime 变化

规则：

- 逻辑层按字段更新绑定值
- Controller 只绑定自己关心的字段
- Panel 只刷新对应字段控件
- 不允许整页重刷替代字段刷新
- UI 侧长期只持有 `Data` 绑定，不长期持有 `Logic`

### 3.3 UI -> UI

标准链路：

- 打开 / 关闭：
  `UIManager.Instance.Open<TPanel>()`
  `UIManager.Instance.Close<TPanel>()`
- 弹窗返回结果：
  `UIManager.Instance.OpenForResult<TPanel, TResult>()`

规则：

- UI 间不直接持有彼此业务引用
- Popup 只返回 `UIResult`
- 调用方收到结果后再决定是否调用逻辑层

---

## 4. 字段绑定规则

当前 UI 下行刷新必须使用字段绑定。

推荐结构：

- `BindableValue<RoleAttrValue> HPValue`
- `BindableValue<RoleAttrValue> MPValue`
- `BindableValue<string> StageNameValue`
- `BindableValue<float> RunningTimeValue`

规则：

- `Controller.Bind()` 时注册字段回调
- 注册时使用立即回推当前值
- `Controller.Unbind()` 时解除字段绑定
- 只更新变更字段，不触发无关 UI 赋值

禁止：

- 使用单个 `DataChanged` 驱动全部 UI
- 用一个大 `UIState` 承担所有显示字段
- 某个字段变化时整页重新拼装字符串再赋值

---

## 5. Panel 规则

`UIPanel` 只允许承担这些职责：

- 创建 UI 控件
- 注册按钮点击
- 生命周期管理
- 提供局部刷新方法

推荐写法：

- `RefreshHP(RoleAttrValue value)`
- `RefreshMP(RoleAttrValue value)`
- `RefreshStage(string stageName, int enterCount)`
- `RefreshRunningTime(float runningTime)`

禁止：

- Panel 直接查找 `ClientModeLogic`
- Panel 直接查找 `ClientModeData`
- Panel 直接决定业务是否合法

---

## 6. Controller 规则

`UIController` 是当前业务 UI 唯一合法的中间层。

职责：

- 绑定业务字段
- 接收按钮命令
- 在命令发生时按需转发到逻辑层
- 驱动 Panel 局部刷新

推荐写法：

- `AddHP()`
- `ReduceHP()`
- `OpenPopup(...)`
- `ReloadScene()`
- `Bind()`
- `Unbind()`

禁止：

- 再套一层 `Service`
- 再套一层 `Presenter`
- 在 Controller 里缓存整页 `UIState`
- 把 `Logic` 当成长期显示数据源

---

## 7. Popup 规则

Popup 不使用字段持续绑定，使用一次性请求数据 + 结果返回。

标准链路：

1. 调用方把请求数据写入逻辑层
2. `UIManager.Instance.OpenForResult<Popup, Result>()`
3. Popup 打开时读取当前请求数据
4. Popup 关闭时返回 `UIResult`
5. 调用方根据结果再调用逻辑层

禁止：

- Popup 自己直接改业务数据
- Popup 自己长期监听字段

---

## 8. 目录约定

当前 UI 相关目录如下：

- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/HUD/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/TestUI/`

目录职责：

- `Base/`
  UI 核心冻结层
- `Common/`
  通用弹窗和系统界面
- `HUD/`
  常驻 HUD
- `TestUI/`
  测试界面和示例界面

---

## 9. 冻结边界

以下内容默认冻结，AI 未经明确授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

但本轮结构已经明确：

- `Base` 的当前合法结构是 `UIManager + UIPanelBase + UIControllerBase`
- 不允许再恢复 `UIPresenterBase`
- 不允许再恢复 `UIService`

---

## 10. 当前示例标准

当前测试示例以角色属性 UI 为准：

- `RoleAttrHUDPanel`
- `RoleAttrPanel`
- `AdjustAttrPopup`

示例必须覆盖：

1. `UI -> Logic`
   例如 `+HP / -HP / +MP / -MP`
2. `Logic -> UI`
   例如 HP / MP / Stage / Time 字段变化
3. `UI -> UI`
   例如主面板打开属性调整弹窗并接收结果

---

## 11. AI 执行要求

AI 处理 UI 需求时必须遵守：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 判断需求是否触碰 `Base`
4. 若触碰 `Base`，必须先得到明确授权
5. 若不触碰 `Base`，优先在 `Common / HUD / TestUI` 扩展

---

## 12. 长期有效规则

从本文档生效开始，以下规则长期有效：

- UI 只属于 Client
- UIManager 单例是唯一 UI 开关入口
- UI 与 Logic 的上行走直接命令调用
- UI 长期只绑定 Data，不长期持有 Logic
- Logic 只修改 Data，必要时也从 Data 绑定做联动
- Logic 与 UI 的下行走字段绑定
- UI 只做局部刷新
- Popup 走一次性请求 + 结果返回
- 不再允许恢复 `Presenter / UIService / 整包 UIState` 主链
