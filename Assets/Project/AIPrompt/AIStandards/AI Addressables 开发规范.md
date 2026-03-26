# AI Addressables 开发规范

## 1. 目的

本文档约束当前 `Addressables` 资源加载模块的 AI 开发边界。  
当前项目已存在可用加载模块，后续 AI 应在此基础上扩展，而不是重复造轮子。

---

## 2. 当前合法主链

当前唯一合法主链如下：

`业务模块 -> ContentLoader / AddressablesMgr.Instance -> Addressables`

说明：

- `AddressablesMgr.Instance`
  是底层统一加载入口
- `ContentLoader`
  是业务层优先使用的轻量封装入口

---

## 3. 当前目录约定

模块目录：

- `Assets/Script/GamePlay/GameWorldBaseFeature/AddressablesMgr/`

运行时资源目录：

- `Assets/ToBundle/`

---

## 4. 当前规则

- 所有 Addressables 加载默认应保持异步
- 业务层优先使用 `ContentLoader`
- 直接调用 `AddressablesMgr.Instance` 时，必须有明确理由
- AI 生成的运行时资源优先放到 `Assets/ToBundle/`

建议：

- Key 使用稳定命名
- 版本化资源可使用 `artifact_id__version` 风格

---

## 5. 冻结边界

以下内容默认冻结，未获授权不得修改：

- `AddressablesMgr.cs`
- `ContentLoader.cs`

默认不允许：

- 再造一套平行资源加载器
- 绕过现有模块在业务层直接散写 Addressables API

---

## 6. 与其他模块的关系

- `Config`
  配置资产可通过当前加载模块管理
- `UI`
  UI 资源加载优先走现有封装
- `Pool`
  第一版对象池不强绑 Addressables，如需联动应新增轻量桥接层

---

## 7. 长期有效规则

- Addressables 统一走现有加载模块
- 业务层优先使用 `ContentLoader`
- 运行时可加载资源优先放在 `Assets/ToBundle/`
