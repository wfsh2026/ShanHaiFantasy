# AI NetworkSync 开发规范

## 1. 目的

本文档约束当前 `NetworkSync` 模块的 AI 开发边界。  
当前 `NetworkSync` 更接近预研和保留模块，未经授权不应被当作正式主链随意接入现有框架。

---

## 2. 当前模块定位

当前 `NetworkSync` 模块位于：

- `Assets/Script/GamePlay/GameWorldBaseFeature/NetworkSync/`

当前包含：

- `NetworkSyncClient`
- `NetworkSyncServer`
- `NetworkSyncRegistry`
- `NetworkSyncReplication`
- `Mirror` 传输适配
- 本地演示模块

---

## 3. 当前合法使用边界

当前允许的使用方式：

- 作为独立预研模块阅读和扩展
- 在明确授权下做网络同步验证
- 作为未来正式网络框架的候选基础

当前默认不允许：

- 未经授权直接接入现有 `GameWorld` 主链
- 未经授权将业务玩法硬接到 `NetworkSync`
- 将其视为当前项目唯一正式网络框架

---

## 4. 目录与冻结边界

以下内容默认冻结，未获授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/NetworkSync/`

说明：

- 当前模块整体按保留模块处理
- 若后续决定正式启用，再单独做主链设计和规则收口

---

## 5. 当前规则

- 网络命令与同步消息必须先走统一注册
- 业务层不得直接散写底层传输细节
- 若需与现有 `Mode / UI / SceneFlow` 接轨，必须先出设计方案

---

## 6. 长期有效规则

- `NetworkSync` 当前按预研保留模块处理
- 未经授权，不得把它直接并入正式框架主链
- 后续若正式启用，必须补完整主链设计和对应 AI 规范
# NetworkSync 补充规范（2026-03-26）

## 当前模块定位
- `NetworkSync` 当前仍按独立预研模块管理，不直接并入正式 `GameWorld` 主链。
- 允许保留在 `Assets/Script/GamePlay/GameWorldBaseFeature/NetworkSync/` 下独立演进，但所有新改动必须遵守当前框架的生命周期和测试规则。

## 当前合法主链
- `NetworkSyncServer`
- `NetworkSyncClient`
- `NetworkSyncReplicationManager`
- `NetworkSyncRegistry`
- `INetworkSyncSerializer`
- `INetworkSyncClientTransport / INetworkSyncServerTransport`

## 传输层规则
- 公共协议层不得直接依赖 `Mirror`。
- `NetworkSyncEnvelope` 只保留协议公共字段。
- `Mirror` 相关消息结构和转换逻辑只能写在 `NetworkSyncMirrorTransport.cs`。
- 本地回环测试传输继续放在 `Assets/Script/GamePlay/TestDemo/NetWorkDemo/`，仅用于测试，不允许当作正式线上传输实现。

## 生命周期规则
- `NetworkSyncClient` 与 `NetworkSyncServer` 必须支持显式 `Dispose()`。
- 构造时允许完成默认注册，但销毁时必须解除 transport 事件绑定并清理内部状态。
- 模块新增时优先通过 `RegisterModule()` 接入，不允许把业务逻辑直接写进 transport。

## 测试规则
- 当前标准测试入口是 `UITestScene` 中的 `RoleAttrPanel -> Run Network Demo`。
- 本地回环测试必须覆盖：
  - `Cmd` 上行
  - `Server` 校验与处理
  - `Rpc/Delta` 下行
  - 客户端世界状态更新
- 新增网络协议后，至少补一个本地回环测试样例。

## 禁止事项
- 不允许把 `Mirror` 类型扩散到 `NetworkSyncEnvelope`、消息定义、复制状态层。
- 不允许绕过 `NetworkSyncRegistry` 直接用硬编码分发处理器。
- 不允许把正式业务逻辑写进 `NetworkSyncRoleFireLocalDemo` 一类测试脚本。
