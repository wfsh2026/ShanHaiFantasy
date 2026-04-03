# C4_AI站位与对局决策修正_开发文档_V1

## 1. 文档信息

- 文档名称：C4_AI站位与对局决策修正_开发文档_V1
- 文档路径：`Assets/Project/ProjectPrompt/C4_AI站位与对局决策修正_开发文档_V1.md`
- 文档版本：V1
- 文档状态：可进入实现前审阅
- 负责角色：主程
- 更新时间：2026-03-30

## 2. 需求来源

- 对应总策划案：`Assets/Project/ProjectPrompt/宗门大比总策划案.md`
- 对应 Task 策划案：`Assets/Project/ProjectPrompt/C4_AI站位与对局决策修正_策划规格_V1.md`
- 对应任务 ID：C4
- 对应版本目标：让 AI 形成可识别的职业倾向、养成倾向和站位倾向

## 3. 对齐文档

- `Assets/Project/AIPrompt/AIStandards/AI 整体架构设计文档.md`
- `Assets/Project/AIPrompt/AIStandards/AI 开发规范文档.md`
- `Assets/Project/AIPrompt/AIStandards/目录规范.md`
- `Assets/Project/AIPrompt/AIStandards/AI Mode 开发规范.md`
- `Assets/Project/AIPrompt/AIStandards/AI NetworkSync 开发规范.md`
- `Assets/Project/AIPrompt/AIStandards/通用代码审核规范.md`
- `Assets/Project/AIPrompt/AIStandards` 目录已做全量核对，本次实际重点使用与 Mode / NetworkSync 直接相关的规范
- 其他直接参考文档：
  - `Assets/Project/ProjectPrompt/B5_AI补位与AI决策深化_开发文档_V1.md`
  - `Assets/Project/ProjectPrompt/C4_AI站位与对局决策修正_策划规格_V1.md`

## 4. 当前项目现状核对

- 当前实际参考的代码目录：
  - `Assets/Framework/Script/GamePlay/Host/ModelFeature/BattleMode`
  - `Assets/Framework/Script/GamePlay/Server/ModelFeature/BattleMode`
  - `Assets/Framework/Script/GamePlay/GameWorldBaseFeature/NetworkSync`
- 当前可复用内容：
  - `BattleA2ContentCatalog.ResolveAutoCultivationSelectionIndex`
  - `BattleA2ContentCatalog.ResolveAutoSecondarySelectionIndex`
  - `BattleA2ContentCatalog.ResolveAutoFormationPosition`
  - `BattleBContentCatalog.ResolveAutoFormationPosition`
  - `BattleModeData.AutoSelectPendingHeroChoices`
  - `BattleModeData.AutoSelectPendingCultivationChoices`
  - `BattleModeData.AutoConfirmPendingFormationChoices`
- 当前已知问题：
  - AI 能走完主角、养成和站位流程，但“对手差异感”仍不足
  - 站位确认链中存在“计算时考虑阵眼、落地时又忽略阵眼”的偏差风险
  - AI 结果更多体现在流程完成，而不是体现在玩家可感知的选择差异
- 当前测试入口：
  - `BattleTestScene`
  - `CultivationTestScene`
  - 调试入口可辅助验证 AI 倾向变化

## 5. 功能目标

- 修正 AI 在站位确认与阵眼联动中的当前偏差
- 让 AI 主角选择、养成选择、站位确认三处形成一致的职业和构筑倾向
- 让全 AI 对局也能稳定跑完整局，并且表现出最小差异
- 不追求：
  - 长期学习型 AI
  - 高级博弈树
  - 动态难度系统

## 6. 架构归属

- 本需求涉及层级：
  - `Mode`
  - `Feature`
  - `Test`
- 归属原因：
  - AI 决策规则属于 `BattleMode` 逻辑
  - battle 同步校验仍在 `NetworkSyncBattle`
  - 测试通过 battle 测试场景和调试入口验证
- 不修改：
  - `Base`
  - 独立新增 AI 客户端

## 7. 目录落点

- 修改代码目录：
  - `Assets/Framework/Script/GamePlay/Host/ModelFeature/BattleMode`
  - `Assets/Framework/Script/GamePlay/Server/ModelFeature/BattleMode`
  - `Assets/Framework/Script/GamePlay/GameWorldBaseFeature/NetworkSync`
- 修改文档目录：
  - `Assets/Project/ProjectPrompt`
- 测试入口：
  - `Assets/Content/Scene/Test/BattleTestScene.unity`
  - `Assets/Content/Scene/Test/CultivationTestScene.unity`

## 8. 模块拆分

- 模块名称：AI 主角与养成决策扩展
  - 模块类型：Mode Logic
  - 模块职责：让 AI 主角与养成选择更贴合职业偏好和已有承载
  - 关系：依赖 `BattleA2ContentCatalog`
  - 是否新增：否，扩展现有自动选择逻辑
- 模块名称：AI 站位确认修正
  - 模块类型：Mode Data / Logic
  - 模块职责：修正 AI 站位确认对阵眼的使用，保证计算与落地一致
  - 关系：依赖 `BattleBContentCatalog.ResolveAutoFormationPosition`
  - 是否新增：否
- 模块名称：AI 结果可读性辅助
  - 模块类型：Mode Data / UI 配合
  - 模块职责：让 AI 的倾向能在结果里被看出
  - 关系：与 C1 结果摘要联动
  - 是否新增：扩展现有结果字段

## 9. 数据流与状态流

- `Config`
  - 本任务不新起独立 AI 配置体系，首版继续使用现有内容池与偏好字段
- `Data`
  - 继续使用参与者的职业偏好、五线成长、推荐站位、阵眼位置
  - 若要表达 AI 倾向，首版优先补最小“本轮选择来源 / 倾向标签”
- `Logic`
  - AI 选主角时参考模板偏好
  - AI 选养成时参考职业偏好、已有承载与当前局势
  - AI 站位时真实参考阵眼与职业偏好
  - 站位计算与写回必须使用同一套结果
- `UI`
  - 不做独立 AI 面板
  - 通过结果摘要或测试入口观察 AI 倾向

## 10. Client / Server 责任分配

- 客户端负责：
  - 展示 AI 已经做出的结果
  - 测试场景中辅助观察 AI 倾向
- 服务端负责：
  - 主角自动选择
  - 养成自动选择
  - 站位自动确认
  - 权威决定 AI 本轮行为结果
- 需要同步的状态：
  - AI 已选主角
  - AI 已选养成
  - AI 已确认站位
  - 必要时的 AI 行为来源标签
- 同步方式：
  - 继续使用现有 battle 快照广播

## 11. 关键流程

1. 进入 `HeroSelect`
2. AI 按模板偏好选择主角
3. 进入 `Cultivation`
4. AI 按职业偏好、已有承载和当前局势选养成
5. 进入 `FormationConfirm`
6. AI 按阵眼位置与职业倾向确认站位
7. 进入 `Battle` / `BattleResult`
8. 玩家能从结果看出 AI 的选择倾向

### 异常与回退流程

- 若 AI 当前没有明确优势倾向：
  - 使用默认保底倾向，不允许卡住阶段
- 若阵眼与职业偏好冲突：
  - 按文档中约定的优先顺序统一处理
- 若自动站位计算与写回不一致：
  - 必须以最终写回逻辑为唯一权威，禁止“双算两次”

## 12. 对外接口与交互边界

- 继续复用：
  - `AutoSelectPending...` 系列入口
  - `BattleBContentCatalog.ResolveAutoFormationPosition`
  - `BattleA2ContentCatalog` 自动评分函数
- 不允许跨越：
  - 新增独立 AI 客户端
  - 在 UI 层直接替 AI 做选择
  - 在调试入口里绕过权威 AI 逻辑替代正式决策

## 13. 进入条件、完成出口与回退策略

- 进入实现前条件：
  - B5 已有基础 AI 自动流程
  - 当前站位确认链路已定位到真实代码目录
- 最小完成出口：
  - 全 AI 房间能稳定跑完整局
  - AI 在不同职业偏好和阵眼条件下能出现稳定差异
  - 玩家能从结果中感知至少 3 类最小 AI 倾向
- 回退策略：
  - 若首版 AI 倾向过多导致结果不可读，优先收敛为激进 / 稳态 / 阵眼联动三类最小标签

## 14. 测试与验证方案

- 测试入口：
  - `BattleTestScene`
  - `CultivationTestScene`
- 验证步骤：
  1. 启动全 AI 房间
  2. 观察 AI 主角选择是否有差异
  3. 观察 AI 养成选择是否有差异
  4. 观察阵眼切换时 AI 站位是否变化
  5. 观察结果摘要能否看出 AI 倾向
- 预期结果：
  - AI 不再只是“把流程跑完”
  - AI 倾向能被观察

## 15. 风险与待确认项

- 风险点：当前自动站位链中可能存在“双重计算”
  - 影响：AI 行为看起来随机或不稳定
  - 建议：站位计算与落地统一走一处逻辑
- 风险点：AI 倾向过多会让首版调试困难
  - 影响：很难判断修正是否真的生效
  - 建议：首版先收敛到最小分类
- 待确认项：
  - 首版 AI 倾向是否要在测试入口显式显示标签

## 16. 不做范围

- 不做长期学习 AI
- 不做高级难度系统
- 不提前并入 C5 的大内容扩容

## 17. 是否建议进入实现

- 结论：可以进入实现
- 最小实现闭环：
  - 修正 AI 站位逻辑
  - 强化主角 / 养成 / 站位三段决策一致性
  - 在结果中留下最小 AI 倾向痕迹

