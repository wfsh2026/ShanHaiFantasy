# AI Mode 开发规范

## 1. 目的

本文档约束当前 `Mode` 框架的 AI 开发边界。  
当前 `Mode` 已经是玩法组织主链，后续新增玩法必须沿用这套结构。

---

## 2. 当前合法主链

当前唯一合法主链如下：

`GameWorldClient / GameWorldServer -> ModeFeatureManager -> ModeManager -> Data / Logic / Stage`

说明：

- `ModeFeatureManager`
  负责选择并初始化当前 Mode
- `ModeManager`
  是当前玩法根对象
- `ModeData`
  保存运行时权威状态
- `ModeLogic`
  负责规则和状态写入
- `ModeStage`
  负责阶段切换和阶段行为

---

## 3. 当前目录约定

核心目录：

- `Assets/Script/GamePlay/Host/ModelFeature/Base/`
- `Assets/Script/GamePlay/Client/ModelFeature/Base/`
- `Assets/Script/GamePlay/Server/ModelFeature/Base/`

业务扩展目录：

- `Assets/Script/GamePlay/Client/ModelFeature/TestMode/`
- `Assets/Script/GamePlay/Server/ModelFeature/TestMode/`

---

## 4. Base 冻结边界

以下内容默认冻结，未获授权不得修改：

- `AbsModeManager`
- `AbsModeData`
- `AbsModeLogic`
- `AbsModeStage`
- `ModeStageCollection`
- `ModeLogicCollection`
- `ModeDataCollection`
- `ClientModeFeatureManager`
- `ClientModeFactory`
- `ClientModeManager`

---

## 5. 数据归属规则

`Mode` 内部数据归属必须保持清晰：

- `ModeData`
  保存权威运行时状态
- `ModeLogic`
  只负责规则和状态写入
- `ModeStage`
  只负责阶段切换与阶段生命周期

禁止：

- 把权威状态写在 `Logic`
- 把业务规则写在 `Data`
- 让 UI 直接操作 `ModeData`

---

## 6. AI 允许修改的范围

默认允许：

- 新增业务 `ModeManager`
- 新增业务 `ModeData / ModeLogic / ModeStage`
- 新增业务配置驱动的 Mode 字段

默认不允许：

- 改 `Base`
- 改通用 `Mode` 容器规则
- 恢复旧版 `StartGame` 或旧消息主链

---

## 7. 长期有效规则

- Mode 必须沿用 `Data / Logic / Stage` 结构
- 运行时权威状态只放在 `ModeData`
- 业务规则只放在 `ModeLogic`
- UI 只绑定 `ModeData`
