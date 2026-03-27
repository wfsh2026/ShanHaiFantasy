# AI NetworkSync 开发规范

## 1. 当前定位

`NetworkSync` 已经正式收口为当前项目的基础网络组件。  
它的目标不是直接承载全部业务逻辑，而是提供：

- `Client / Server` 同步内核
- 协议注册与分发
- 复制与快照能力
- 业务网络模块接入入口

当前代码位置：

- `Assets/Framework/Script/GamePlay/GameWorldBaseFeature/NetworkSync/`

---

## 2. 当前合法主链

正式主链如下：

- `GameWorldClient -> ClientNetworkFeatureManager -> NetworkSyncClient`
- `GameWorldServer -> ServerNetworkFeatureManager -> NetworkSyncServer`

业务接入链如下：

- `UI / Controller / Logic -> ClientRoomNetProxy -> NetworkSyncRoomClientModule -> NetworkSyncClient`
- `NetworkSyncServer -> NetworkSyncRoomServerModule -> ServerRoomNetHandler -> 房间业务数据`

---

## 3. 核心保持不变的部分

以下 `NetworkSync` 内核结构默认保持稳定：

- `NetworkSyncClient`
- `NetworkSyncServer`
- `NetworkSyncRegistry`
- `NetworkSyncReplicationManager`
- `INetworkSyncSerializer`
- `INetworkSyncClientTransport`
- `INetworkSyncServerTransport`
- `NetworkSyncEnvelope`

规则：

- 新增业务能力时，优先新增 `Module / Proxy / Handler`
- 不得把业务玩法逻辑直接写进 `NetworkSyncClient / NetworkSyncServer`

---

## 4. 允许新增的内容

允许新增：

- 新的业务协议消息
- 新的 `INetworkSyncClientModule`
- 新的 `INetworkSyncServerModule`
- 新的业务代理层，例如 `ClientRoomNetProxy`
- 新的服务端门面层，例如 `ServerRoomNetHandler`
- 新的本地回环测试脚本

默认不建议直接改动：

- `NetworkSyncEnvelope`
- `NetworkSyncRegistry`
- `NetworkSyncReplicationManager`
- `NetworkSyncJsonSerializer`

如需修改上述内核，必须先说明原因。

---

## 5. 传输层规则

- 公共协议层不得直接依赖 `Mirror`
- `Mirror` 相关实现只能留在 `NetworkSyncMirrorTransport.cs`
- 本地回环传输只允许用于测试
- 正式业务调用不得直接操作 transport

当前本地回环测试传输位置：

- `Assets/Framework/Script/GamePlay/TestDemo/NetWorkDemo/`

---

## 6. Feature 接入规则

当前正式入口：

- `ClientNetworkFeatureManager`
- `ServerNetworkFeatureManager`

规则：

- `NetworkSync` 必须通过 `GameWorld Feature` 接入
- 业务层不得自行 new `NetworkSyncClient / NetworkSyncServer` 作为正式主链
- 运行时 transport 必须通过 Feature 显式绑定
- 模块注册优先通过 `RegisterModule()` 完成

---

## 7. 协议设计规则

协议必须：

- 先定义消息 id
- 再定义消息体
- 再定义 `NetworkSyncMessageDescriptor`
- 最后在 `ClientModule / ServerModule` 中注册

推荐按模块组织协议，例如：

- `Room_Create_Cmd`
- `Room_Join_Cmd`
- `Room_State_Rpc`
- `Room_Disband_Rpc`

不要把不同业务模块的消息混写在一个文件里。

---

## 8. 业务层规则

业务层要分清三层：

- `ClientModule`
  负责客户端发送命令、接收服务端消息
- `ServerModule`
  负责服务端注册命令与执行服务端权威逻辑
- `Proxy / Handler`
  负责给 UI、Logic、业务数据提供稳定门面

禁止：

- UI 直接使用 `NetworkSyncClient.SendCmd(...)`
- 业务直接操作 transport
- 把房间、战斗等业务规则写进 `NetworkSyncClient / Server`

---

## 9. 生命周期规则

- `NetworkSyncClient` 与 `NetworkSyncServer` 必须支持显式 `Dispose()`
- Feature 卸载时必须解除运行时绑定
- 本地测试脚本销毁时必须释放 client / server / transport 绑定

---

## 10. 测试规则

当前标准测试入口：

- `UITestScene -> RoleAttrPanel -> Run Room Demo`

当前房间网络测试必须覆盖：

- Host 创建房间
- 客户端加入房间
- 服务端同步房间状态
- 房主开始游戏
- 开始后禁止继续加入
- 房主离开解散房间

新增协议后，至少补一个本地回环测试样例。

---

## 11. 禁止事项

禁止：

- 把 `Mirror` 类型扩散到公共协议层
- 绕过 `NetworkSyncRegistry` 硬编码分发
- 把正式业务逻辑写进本地演示脚本
- 让 UI 直接持有底层 `NetworkSyncClient / Server`
- 在 `Client / Server Feature` 里塞具体房间或战斗业务逻辑

---

## 12. 长期有效规则

- `NetworkSync` 已是正式基础组件
- 核心内核尽量保持稳定
- 新业务优先通过 `Module + Proxy / Handler` 扩展
- 测试优先走本地回环验证
- 公共协议层继续保持与具体网络库解耦
