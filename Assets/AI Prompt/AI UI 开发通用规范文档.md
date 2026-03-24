# AI UI 开发通用规范文档

## 1. 文档目的

本文档用于约束 `ShanHaiFantasy` 项目中 UI 框架相关的 AI 开发边界。
从本文档生效开始，AI 在处理 UI 需求时，必须优先遵守本文档、`Assets/AI Prompt/代码规范.md`、`Assets/AI Prompt/目录规范.md`。

UI 框架当前已经确定为：

- 表现层使用 `UGUI`
- 资源加载使用项目现有的 `Addressables` UI 加载封装
- `UIManager` 作为客户端常驻 `GameWorld Feature`
- `HUD` 使用独立层级
- 弹窗必须支持结果返回机制
- 所有 `Base` 目录默认冻结，修改前必须先得到用户明确同意

---

## 2. 当前 UI 主链

当前唯一合法 UI 主链如下：

`GameWorldClient -> ClientUIFeatureManager -> UIManager -> UIPanel / UIPresenter -> UIService -> ClientMode`

其中：

- `GameWorldClient`
  只负责挂载客户端常驻 UI 功能
- `ClientUIFeatureManager`
  负责初始化 UI 根节点、层级、注册表、打开关闭入口
- `UIPanel`
  只负责显示和用户输入转发
- `UIPresenter`
  负责 UI 逻辑组织和状态刷新
- `UIService`
  负责 UI 与模式数据、模式逻辑交互
- `ClientMode`
  负责权威业务数据和业务执行

禁止 AI 恢复以下旧式写法：

- UI 直接修改模式数据
- 逻辑层直接操作具体 Panel 控件
- 一个 UI 直接持有另一个 UI 的业务引用并相互调用
- 绕过 `UIManager` 直接实例化、显示、关闭界面

---

## 3. 当前目录约定

当前 UI 代码以项目实际目录为准：

- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/HUD/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/TestUI/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Feature/`

目录职责如下：

- `Base/`
  放 UI 框架基础层、接口、层级、注册表、面板基类、Presenter 基类、加载适配器
- `Common/`
  放通用弹窗、通用提示、通用返回结果型 UI
- `HUD/`
  放常驻 HUD 类界面
- `TestUI/`
  放测试 UI、示例 UI、模式专属 UI 示例
- `Feature/`
  放 UI 相关功能入口或桥接 Feature

---

## 4. 冻结边界

以下目录和文件属于 UI 核心冻结层，AI 未经用户明确授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

特别说明：

- `Base/` 下所有 UI 框架文件默认冻结
- 如需新增 UI 功能，优先在 `Common/`、`HUD/`、`TestUI/` 下新增
- 如需改变 UI 主链、注册机制、层级机制、缓存机制、结果回传机制，必须先得到用户同意

---

## 5. UI 标准对象

UI 层统一只允许使用以下对象组织交互：

- `UIOpenData`
  只负责界面打开参数
- `UIState`
  只负责界面显示状态
- `UIResult`
  只负责弹窗或子界面的返回结果
- `UICommand`
  只负责描述用户意图
- `UIService`
  负责 UI 与模式逻辑交互

规则如下：

- `UIOpenData` 不允许直接传 `GameWorld`、`ModeData`、`ModeLogic`
- `UIState` 不允许持有复杂业务对象引用
- `UIResult` 不允许直接执行业务逻辑
- `UICommand` 不允许直接改权威数据

---

## 6. 三种交互规则

### 6.1 UI -> 模式逻辑

标准链路：

`Panel -> Presenter -> UIService -> ClientModeLogic / ClientModeManager`

适用场景：

- 点击按钮
- 输入数值
- 选择页签
- 触发业务行为

禁止：

- Panel 直接改 `ClientModeData`
- Panel 直接调 `GameWorld`
- Panel 直接写业务判断

### 6.2 模式逻辑 -> UI

标准链路：

`ClientModeData / ClientModeLogic -> Presenter -> UIState -> Panel.Refresh(state)`

适用场景：

- HP / MP 变化
- 阶段变化
- 倒计时变化
- 分数变化

禁止：

- Logic 直接操作 Text、Image、Button
- Data 层直接引用具体 Panel

### 6.3 UI -> UI

标准链路：

- 导航型：`UIManager.Open / UIManager.Close`
- 结果型：`UIManager.OpenForResult -> UIResult`
- 共享状态型：共同监听同一份业务数据源

适用场景：

- 主界面打开子界面
- 主界面打开确认弹窗
- 弹窗返回用户选择结果
- HUD 与主界面共同显示同一份业务状态

禁止：

- PanelA 直接持有 PanelB 并改动其业务状态
- 两个 UI 之间直接传递权威业务数据

---

## 7. UI 分类规则

### 7.1 普通界面

特点：

- 一般位于 `Normal` 层
- 支持返回栈
- 可以缓存

推荐：

- `UICacheMode.HideOnClose`
- `UIOpenMode.Single`

### 7.2 弹窗

特点：

- 位于 `Popup` 层
- 支持结果返回
- 一般关闭即销毁

推荐：

- `UICacheMode.DestroyOnClose`
- `UIOpenMode.Multi`

### 7.3 HUD

特点：

- 位于 `HUD` 层
- 常驻
- 不进返回栈
- 不从其他 UI 取数据

推荐：

- `UICacheMode.Permanent`
- `UIOpenMode.Single`

---

## 8. Addressables 规则

UI 资源加载只能走当前项目已有的封装入口，不允许在业务 UI 中直接写原生 Addressables 调用。

当前约定：

- 通过 `UIAssetLoaderAdapter` 统一接入
- `UIAssetLoaderAdapter` 内部调用现有 `ContentLoader`
- 若未配置 prefab key，可使用运行时构建 UI 作为兜底测试方案

禁止：

- 在具体 Panel 中直接写 Addressables 加载代码
- 在业务 UI 中自行维护另一套加载入口

---

## 9. 示例规则

当前 UI 示例以角色属性为准，包含：

- `HP`
- `MP`
- `RoleAttrHUDPanel`
- `RoleAttrPanel`
- `AdjustAttrPopup`

示例必须覆盖三类交互：

1. `UI -> 模式逻辑`
   例如主面板点击 `+HP`、`-MP`
2. `模式逻辑 -> UI`
   例如模式内自动损耗或恢复 HP / MP 后通知 HUD 和主面板刷新
3. `UI -> UI`
   例如主面板打开属性调整弹窗，弹窗返回结果后再转给模式逻辑

---

## 10. AI 允许修改的范围

后续 AI 默认只允许修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/HUD/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/UIManager/TestUI/`

以及：

- 新增具体业务 UI 脚本
- 新增具体业务 Presenter
- 新增具体业务 UIService
- 新增具体业务 OpenData / State / Result

AI 默认不允许：

- 擅自修改 `Base/`
- 擅自修改 UI 主链
- 擅自修改层级和注册核心机制
- 擅自引入第二套 UI 管理结构

---

## 11. AI 执行顺序要求

AI 在执行 UI 相关需求时，必须遵守以下顺序：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/代码规范.md`
3. 再阅读 `Assets/AI Prompt/目录规范.md`
4. 判断需求是否会触碰 `Base/`
5. 如果会触碰 `Base/`，必须先确认是否已获得用户授权
6. 若未触碰 `Base/`，优先在 `Common/`、`HUD/`、`TestUI/` 中实现
7. 所有业务 UI 必须优先复用现有 `UIManager` 主链

---

## 12. 文档生效规则

从本文档创建开始，凡是 AI 处理 UI 框架、UI 示例、UI 扩展时，都必须默认遵守以下规则：

- UI 只属于客户端
- UI 主链默认冻结
- `Base/` 默认冻结
- 所有打开关闭必须统一走 `UIManager`
- 所有弹窗必须支持结果返回机制
- 所有业务数据交互必须遵守：
  - `UI -> Logic`
  - `Logic -> UI`
  - `UI -> UI`
  三条标准链路

本文档与 `AI 开发规范文档.md`、`代码规范.md`、`目录规范.md` 共同构成当前项目的 AI UI 开发约束基础。
