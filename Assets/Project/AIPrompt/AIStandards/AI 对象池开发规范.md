# AI 对象池开发规范

## 1. 目的

本文档定义 `PoolManager` 的合法开发方式。  
第一版对象池只负责 `GameObject` 级别的预制缓存、生成和回收。

---

## 2. 当前合法主链

当前唯一合法主链如下：

`GameWorldClient -> ClientPoolFeatureManager -> PoolManager.Instance`

说明：

- `ClientPoolFeatureManager` 只负责初始化和清理 `PoolManager.Instance`
- `PoolManager.Instance` 是唯一合法对象池入口
- `GameObjectPool` 负责单个 prefab 的缓存
- `PoolIdentity` 负责实例归属标记

---

## 3. 当前目录约定

核心目录：

- `Assets/Script/GamePlay/GameWorldBaseFeature/PoolManager/Base/`

测试与业务扩展目录：

- `Assets/Script/GamePlay/GameWorldBaseFeature/PoolManager/TestPool/`

---

## 4. Base 冻结边界

以下内容默认冻结，未获授权不得修改：

- `ClientPoolFeatureManager`
- `PoolManager`
- `GameObjectPool`
- `PoolIdentity`

冻结含义：

- 不修改对象池主链
- 不修改池实例归属规则
- 不把业务逻辑写回对象池基础层

---

## 5. 对象池设计规则

- 对外统一使用 `PoolManager.Instance`
- 第一版只允许缓存 `GameObject`
- 业务层不直接维护自己的实例缓存列表
- 从对象池生成的对象，优先使用 `PoolManager.Instance.Recycle(...)` 回收
- `PoolIdentity` 只用于记录池归属，不承载业务字段
- 对象池默认不直接绑定 Addressables

---

## 6. Addressables 联动规则

第一版规则：

- `PoolManager` 不直接依赖 Addressables
- 业务可以先拿到 prefab，再调用 `PoolManager.Instance.Spawn(...)`

后续如需联动：

- 允许新增一层轻量加载适配器
- 不允许把 Addressables 逻辑直接写进 `GameObjectPool`

---

## 7. AI 允许修改的范围

默认允许：

- 新增业务测试池脚本
- 新增具体 prefab 池使用入口
- 新增业务对象的生成和回收调用

如需修改以下内容，必须先得到授权：

- 任意 `Base`
- 对象池主链入口
- 对象池回收规则

---

## 8. 禁止事项

未经授权，AI 禁止：

- 绕过 `PoolManager.Instance` 自行维护公共对象池
- 在业务层到处直接 `Instantiate / Destroy` 本应复用的临时对象
- 把 Addressables 强耦合进 `PoolManager` 第一版主链
- 在 `PoolIdentity` 中混入业务状态

---

## 9. 长期有效规则

- 对象池统一走 `PoolManager.Instance`
- 第一版对象池保持 prefab 直连方案
- Addressables 如需联动，只能新增轻量桥接层
- `Base` 默认冻结
