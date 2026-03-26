# AI GameWorld 开发规范

## 1. 目的

本文档约束 `GameWorld` 主链相关的 AI 开发边界。  
`GameWorld` 是当前框架的核心运行容器，默认冻结。

---

## 2. 当前合法主链

当前唯一合法主链如下：

`GameEngine -> GameWorld -> GameWorldServer / GameWorldClient -> Feature`

说明：

- `GameEngine`
  只负责 Unity 生命周期入口
- `GameWorld`
  只负责世界初始化、Update 驱动和 Feature 容器
- `GameWorldServer / GameWorldClient`
  分别负责服务端和客户端系统接入

---

## 3. 当前目录约定

核心目录：

- `Assets/Script/GamePlay/Host/GameWorld/Base/`
- `Assets/Script/GamePlay/Server/GameWorld/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

---

## 4. Base 冻结边界

以下内容默认冻结，未获授权不得修改：

- `GameWorld.cs`
- `GameWorld_Logic.cs`
- `GameWorldFrameworkBase.cs`
- `GameWorldServer.cs`
- `GameWorldClient.cs`
- `GameEngine.cs`

冻结含义：

- 不修改 `GameWorld` 主链
- 不恢复 `GameWorldManager`
- 不恢复 `worldList`
- 不恢复 `StartGame`

---

## 5. GameWorld 规则

当前 `GameWorld` 只允许承担以下职责：

- 持有 `GameWorldServer / GameWorldClient`
- 持有 Feature 容器
- 驱动 `Update / FixedUpdate / LateUpdate`
- 负责世界级清理

禁止：

- 在 `GameWorld` 直接写玩法逻辑
- 在 `GameWorld` 直接写业务 UI
- 在 `GameWorld` 直接写具体模式逻辑

---

## 6. Feature 接入规则

当前合法方式：

- `GameWorldClient` 负责挂客户端通用模块
- `GameWorldServer` 负责挂服务端通用模块
- 业务模块通过 `AddExtendFeature<T>()` 进入

禁止：

- 绕过 `GameWorldClient / GameWorldServer` 直接改世界主链
- 把业务模块硬写回 `GameWorld.cs`

---

## 7. AI 允许修改的范围

默认允许：

- 在 `GameWorldClient / GameWorldServer` 已授权范围内接业务模块
- 新增业务 `Feature`
- 新增客户端或服务端测试入口

默认不允许：

- 改 `GameWorld` 主链
- 改世界生命周期
- 改核心 Feature 容器规则

---

## 8. 长期有效规则

- `GameWorld` 主链默认冻结
- 业务逻辑必须从 Feature 层进入
- 客户端系统只从 `GameWorldClient` 接入
- 服务端系统只从 `GameWorldServer` 接入
