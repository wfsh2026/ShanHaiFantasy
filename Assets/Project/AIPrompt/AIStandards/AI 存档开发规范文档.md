# AI 存档开发规范文档

## 1. 目的

本文档约束 `ShanHaiFantasy` 项目当前本地存档与设置系统的 AI 开发边界。  
当前存档系统只允许使用简单 JSON 落盘，不允许 AI 额外引入复杂存档框架。

---

## 2. 当前合法主链

当前唯一合法主链如下：

`GameWorldClient -> ClientSaveDataFeatureManager -> SaveDataManager.Instance -> SettingsData / PlayerLocalData`

说明：

- `ClientSaveDataFeatureManager`
  只负责初始化、自动保存驱动和退出保存。
- `SaveDataManager.Instance`
  是唯一合法的本地数据读写入口。
- `SettingsData`
  保存系统设置。
- `PlayerLocalData`
  保存本地玩家进度。

禁止新增以下结构：

- 多套并行存档入口
- 各业务模块各自直接写文件
- 复杂数据库方案
- 自定义二进制协议主链
- 云存档主链

---

## 3. 目录规则

存档脚本目录：

- `Assets/Script/GamePlay/GameWorldBaseFeature/SaveDataManager/Base/`

本地文件目录：

- `Application.persistentDataPath/SaveData/`

当前文件约定：

- `settings.json`
- `player_local.json`

---

## 4. 数据划分规则

### 4.1 SettingsData

只保存系统设置，例如：

- `masterVolume`
- `bgmVolume`
- `sfxVolume`
- `uiVolume`
- `voiceVolume`
- `ambientVolume`
- `isMute`
- `isVibration`
- `language`
- `qualityLevel`
- `targetFrameRate`

### 4.2 PlayerLocalData

只保存本地进度，例如：

- `lastSceneId`
- `lastScenePath`
- `lastModeId`
- `isFirstLaunch`
- `isTutorialFinished`
- `highestUnlockedStage`
- `lastLoginDate`
- `playerDisplayName`

规则：

- 系统设置和玩家进度必须分开
- 运行时状态不直接写回配置文件
- 本地持久化对象只保存需要跨会话保留的数据

---

## 5. 读写规则

合法读写方式：

- `SaveDataManager.Instance.Initialize()`
- `SaveDataManager.Instance.LoadAll()`
- `SaveDataManager.Instance.SaveAll()`
- `SaveDataManager.Instance.MarkDirty()`

规则：

- 业务层只修改内存中的 `SettingsData / PlayerLocalData`
- 统一由 `SaveDataManager` 负责落盘
- 文件格式统一使用 JSON

禁止：

- 在 UI、Mode、Audio、SceneFlow 里直接 `File.WriteAllText`
- 在业务层自己维护另一份本地缓存

---

## 6. 和现有模块的合法关系

### 6.1 Audio

合法链路：

`UI / Logic -> SaveDataManager.Instance.SetBusVolume(...) -> AudioManager`

规则：

- 音量设置先写 `SettingsData`
- 再统一应用到 `AudioManager`

### 6.2 SceneFlow

合法链路：

`SceneFlow -> SaveDataManager.Instance.SetLastScene(...)`

规则：

- 场景切换完成后更新上次场景记录

### 6.3 Mode

合法链路：

`Mode -> SaveDataManager.Instance.SetLastMode(...)`

规则：

- 只记录需要跨会话的模式信息

---

## 7. 当前设计限制

第一版存档系统默认不做：

- 多存档槽位
- 加密
- 压缩
- 复杂版本迁移
- 云同步
- 大而全事件广播

后续如果需要扩展，必须先确认。

---

## 8. 冻结边界

以下内容默认冻结，AI 未经明确授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/SaveDataManager/Base/`

---

## 9. AI 执行要求

AI 处理存档需求时必须遵守：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 判断需求是否触碰 `SaveDataManager/Base`
4. 若触碰 `Base`，必须先得到明确授权
5. 不得绕过 `SaveDataManager.Instance`

---

## 10. 长期有效规则

从本文档生效开始，以下规则长期有效：

- 本地存档统一走 `SaveDataManager.Instance`
- 系统设置和玩家进度必须分开
- 本地持久化统一使用 JSON
- 业务层不直接写文件
- `Base` 默认冻结
