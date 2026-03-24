# AI 开发规范文档

## 1. 文档目的

本文件用于约束 ShanHaiFantasy 项目中 `GameWorld` 主干框架的 AI 开发边界。

从本文件生效开始，AI 在处理 `GamePlay` 主干框架时，默认只能新增或修改 `Feature` 模块，不允许擅自改动核心主干代码。

如用户没有明确提出“允许调整核心框架”或“允许重构主干”，则 AI 必须把核心代码视为冻结代码。

---

## 2. 当前框架主结构

当前框架的唯一合法主链如下：

`GameEngine -> GameWorld -> GameWorldServer / GameWorldClient -> Feature 模块`

当前设计明确约束如下：

- 不允许恢复 `GameWorldManager`
- 不允许恢复 `worldList`
- 不允许恢复 `StartGame`
- 不允许恢复旧消息流
- 不允许恢复 `GameWorldNetworkServer`
- 不允许恢复 `GameWorldNetworkClient`
- 不允许引入第二套并行 world 管理结构

---

## 3. 当前启动与运行流程

### 3.1 启动流程

1. `GameEngine.Start()` 创建 `GameWorld`
2. `GameWorld.Init()` 初始化运行时容器
3. 如果开启服务端，则创建 `GameWorldServer`
4. 如果开启客户端，则创建 `GameWorldClient`
5. `GameWorldServer` 和 `GameWorldClient` 只负责装配各自的 `FeatureManager`

### 3.2 帧驱动流程

1. `GameEngine.Update()` 调用 `GameWorld.OnUpdate()`
2. `GameEngine.FixedUpdate()` 调用 `GameWorld.OnFixedUpdate()`
3. `GameEngine.LateUpdate()` 调用 `GameWorld.OnLateUpdate()`
4. `GameWorld` 内部通过 `TimerRegister`、`UpdateRegister`、`LateUpdateRegister` 驱动功能模块

### 3.3 Feature 挂载流程

1. `GameWorldServer.Init()` 负责挂载服务端功能管理模块
2. `GameWorldClient.Init()` 负责挂载客户端功能管理模块
3. 后续新增业务功能，必须由对应的 `FeatureManager` 统一挂载
4. 不允许为了接业务功能而直接修改 `GameEngine` 或 `GameWorld` 主干逻辑

---

## 4. 核心冻结代码

以下文件属于核心主干代码。

除非用户明确要求“修改核心框架”或“重构主干”，否则 AI 不允许修改以下文件：

- `Assets/Script/GamePlay/GameEngine.cs`
- `Assets/Script/GamePlay/Host/GameWorld/Base/GameWorld.cs`
- `Assets/Script/GamePlay/Host/GameWorld/Base/GameWorld_Logic.cs`
- `Assets/Script/GamePlay/Host/GameWorld/Base/GameWorldFrameworkBase.cs`
- `Assets/Script/GamePlay/Host/GameWorld/Base/GameWorldPlaceholders.cs`
- `Assets/Script/GamePlay/Server/GameWorld/Base/GameWorldServer.cs`
- `Assets/Script/GamePlay/Server/GameWorld/Base/ServerSceneFeatureManager.cs`
- `Assets/Script/GamePlay/Client/GameWorld/Base/GameWorldClient.cs`
- `Assets/Script/GamePlay/Server/ModelFeature/Base/ServerModeFeatureManager.cs`
- `Assets/Script/GamePlay/Client/ModelFeature/Base/ClientModeFeatureManager.cs`
- `Assets/Script/GamePlay/Host/ModelFeature/Base/`
- `Assets/Script/GamePlay/Server/ModelFeature/Base/`
- `Assets/Script/GamePlay/Client/ModelFeature/Base/`

这些文件的职责边界如下：

- `GameEngine.cs`
  只负责 Unity 生命周期入口，以及唯一 `GameWorld` 的创建、更新、销毁。
- `Host/GameWorld/Base/GameWorld.cs`
  只负责 `GameWorld` 的初始化、清理、Server/Client 入口持有。
- `Host/GameWorld/Base/GameWorld_Logic.cs`
  只负责 `GameWorld` 的运行时容器能力，包括 Update、FixedUpdate、LateUpdate、Feature 容器。
- `Host/GameWorld/Base/GameWorldFrameworkBase.cs`
  只负责基础注册器和 Feature 基类，不承载业务逻辑。
- `Server/GameWorld/Base/GameWorldServer.cs`
  只负责服务端功能管理模块入口，不直接承载具体业务功能。
- `Client/GameWorld/Base/GameWorldClient.cs`
  只负责客户端功能管理模块入口，不直接承载具体业务功能。
- `Host/GameWorld/Base/GameWorldPlaceholders.cs`
  当前仅保留基础枚举定义，不承载业务功能。
- `*/Base/`
  所有 Base 目录下的文件都属于基础框架层，AI 未经用户明确同意不得修改。

---

## 5. 允许 AI 修改的范围

后续 AI 默认只允许修改以下两类内容：

- 新增具体业务 `Feature` 文件
- 修改现有 `FeatureManager` 文件，用于挂载新的 `Feature`

当前允许作为扩展入口的目录如下：

- `Assets/Script/GamePlay/Server/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/Client/ModelFeature/TestMode/`

以下目录虽然属于模式系统，但因为位于 `Base` 下，修改前必须先得到用户同意：

- `Assets/Script/GamePlay/Host/ModelFeature/Base/`
- `Assets/Script/GamePlay/Server/ModelFeature/Base/`
- `Assets/Script/GamePlay/Client/ModelFeature/Base/`
- `Assets/Script/GamePlay/Host/GameWorld/Base/`
- `Assets/Script/GamePlay/Server/GameWorld/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

后续 AI 可以新增的文件类型如下：

- `XXXFeature.cs`
- `XXXFeatureManager.cs`
- `XXXModeManager.cs`
- `XXXModeData.cs`
- `XXXModeLogic.cs`
- `XXXModeStage.cs`
- 与 `Feature` 强关联的局部辅助类文件

如果 AI 要新增功能，应优先在现有 `FeatureManager` 中挂载，不允许跳过管理层直接把业务逻辑塞回核心主干。

---

## 6. Feature 开发规则

### 6.1 基本原则

- 一个业务功能对应一个明确的 `Feature` 类
- `Feature` 必须挂到 `GameWorld` 的 `baseFeatures` 或 `extendFeatures`
- 默认业务功能使用 `AbsExtendGameWorldFeature`
- 除非用户明确要求，不要新增第二套生命周期系统
- Feature 必须明确区分 server 和 Clietnt 逻辑，不允许在同一个 Feature 中混写两套逻辑

### 6.2 生命周期要求

Feature 应通过 `OnInit()` 和 `OnRemove()` 管理自己的注册与反注册。

如果需要每帧更新，应在 `OnInit()` 中注册 `OnUpdate`，在 `OnRemove()` 中移除 `OnUpdate`。

### 6.3 推荐写法

```csharp
public sealed class DemoFeature : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        gameWorld.AddUpdate(OnUpdate);
    }

    protected override void OnRemove() {
        if (gameWorld != null) {
            gameWorld.RemoveUpdate(OnUpdate);
        }
    }

    private void OnUpdate(float delta) {
    }
}
```

### 6.4 推荐挂载写法

```csharp
public sealed class ServerModeFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        gameWorld.AddExtendFeature<DemoFeature>();
    }
}
```

---

## 7. AI 禁止事项

如果用户没有明确授权，AI 禁止进行以下操作：

- 修改核心冻结代码
- 修改任意 `Base` 目录下的文件而未得到用户明确同意
- 改动主链结构
- 恢复旧版管理器或 world 容器
- 新增消息系统替代当前直接调用链
- 在核心文件中直接写业务逻辑
- 将具体玩法逻辑写入 `GameEngine`
- 将具体玩法逻辑写入 `GameWorld`
- 将具体玩法逻辑写入 `GameWorldServer`
- 将具体玩法逻辑写入 `GameWorldClient`
- 为了接功能而绕过 `FeatureManager`

---

## 8. AI 执行顺序要求

AI 在执行与 `GameWorld` 框架相关的任务时，必须遵循以下顺序：

1. 先阅读本文件
2. 再阅读 `Assets/AI Prompt/代码规范.md`
3. 再阅读 `Assets/AI Prompt/目录规范.md`
4. 判断需求是否属于核心框架修改
5. 如果不是核心框架修改，只能在 `Feature` 层工作
6. 如果需求会触碰核心冻结代码，必须先明确得到用户授权

---

## 9. 当前目录约定

当前这套 `GameWorld` 主干的实际代码路径，以项目现状为准：

- `Assets/Script/GamePlay/`
- `Assets/Script/GamePlay/Host/GameWorld/Base/`
- `Assets/Script/GamePlay/Server/GameWorld/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`
- `Assets/Script/GamePlay/Host/ModelFeature/Base/`
- `Assets/Script/GamePlay/Server/ModelFeature/Base/`
- `Assets/Script/GamePlay/Server/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/Client/ModelFeature/Base/`
- `Assets/Script/GamePlay/Client/ModelFeature/TestMode/`

虽然部分历史规范文档中写有 `Assets/Scripts/GamePlay/`，但在本项目当前阶段，AI 必须优先遵守项目实际目录现状，不允许擅自迁移整套主干目录。

如果后续需要统一目录命名，必须由用户明确下达迁移指令后再执行。

---

## 10. 文档生效规则

从本文件创建开始，凡是 AI 处理 `GameWorld` 主干框架相关需求时，都必须默认遵守以下规则：

- 核心主干默认冻结
- 所有 `Base` 目录默认冻结
- 新增功能默认走 `Feature` 模块
- 新增功能默认走 `FeatureManager` 挂载
- 未经授权不得修改主链结构

本文件与 `代码规范.md`、`目录规范.md` 一起构成当前项目的 AI 开发约束基础。
