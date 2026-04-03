# B5_AI补位与AI决策深化_开发文档_V1

## 1. 文档信息

- 文档名称：B5_AI补位与AI决策深化_开发文档_V1
- 文档路径：`Assets/Project/ProjectPrompt/B5_AI补位与AI决策深化_开发文档_V1.md`
- 对应策划案：`Assets/Project/ProjectPrompt/B5_AI补位与AI决策深化_策划规格_V1.md`
- 当前状态：已完成实现并通过基础编译验证

## 2. 目录落点

- AI 决策入口：`Assets/Framework/Script/GamePlay/Host/ModelFeature/BattleMode/BattleA2Content.cs`
- 触发与超时处理：`Assets/Framework/Script/GamePlay/Server/ModelFeature/BattleMode/ServerBattleModeLogic.cs`
- AI 结果承载：`Assets/Framework/Script/GamePlay/Host/ModelFeature/BattleMode/BattleModeData.cs`

## 3. 实现约束

- AI 不新增独立客户端逻辑。
- 继续复用现有 `AutoSelectPending...` 系列入口。

## 4. 计划修改

- 深化主角自动选择、养成自动选择和站位自动确认。
- 在超时和全 AI 对局下保持回合主链稳定推进。

## 5. 验证入口

- `BattleTestScene`
- `CultivationTestScene`
- `BattleQuickSetupPanel`

## 6. 进入实现判断

- 建议进入实现：是
- 原因：A 组已有 AI 自动补主链，B5 只需补内容感知决策。
