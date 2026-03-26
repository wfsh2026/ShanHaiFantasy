# AI 音频开发规范

## 1. 文档目的

本文档用于约束 `ShanHaiFantasy` 项目中音频系统相关的 AI 开发方式。

当前音频系统已经确定为：

- 音频只属于客户端
- 主入口为 `ClientAudioFeatureManager -> AudioManager`
- `AudioManager` 允许以单例方式直接供逻辑层调用
- 挂物体脚本统一使用 `AudioEmitter`
- 音频资源加载优先复用项目现有 Addressables 封装
- 任意 `Base` 目录默认冻结

未经用户明确授权，AI 不得修改音频主链和 `Base` 目录。

---

## 2. 当前音频主链

当前唯一合法音频主链如下：

`GameWorldClient -> ClientAudioFeatureManager -> AudioManager -> AudioRegistry / AudioAssetLoaderAdapter / AudioSourcePool -> AudioEmitter / 业务调用`

职责边界如下：

- `ClientAudioFeatureManager`
  负责把音频系统接入 `GameWorldClient`
- `AudioManager`
  负责全局单例播放入口、BGM、SFX、UI 音效、3D 音效、句柄与音量组
- `AudioRegistry`
  负责音频配置注册
- `AudioAssetLoaderAdapter`
  负责统一对接现有 Addressables 音频加载能力
- `AudioSourcePool`
  负责运行时 AudioSource 复用
- `AudioEmitter`
  负责挂物体的启停/显隐触发播放

---

## 3. 当前目录约定

当前目录以项目实际结构为准：

- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Base/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/TestAudio/`

目录职责如下：

- `Base/`
  放音频系统核心框架
- `Common/`
  放可复用挂件，例如 `AudioEmitter`
- `TestAudio/`
  放测试控制器、示例入口和测试专用脚本

---

## 4. Base 冻结边界

以下目录默认冻结，AI 未经用户明确同意不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`

这意味着：

- 不允许擅自改动音频主链
- 不允许擅自改动单例入口
- 不允许擅自改动音频加载适配层
- 不允许擅自改动音量组和 AudioSource 复用主机制

---

## 5. 音频系统标准对象

音频系统统一只允许使用以下对象组织逻辑：

- `AudioBusType`
- `AudioConfig`
- `AudioHandle`
- `AudioRegistry`
- `AudioAssetLoaderAdapter`
- `AudioSourcePool`
- `AudioEmitter`

禁止在业务代码中绕过这些对象，零散管理底层 `AudioSource`。

---

## 6. 音频开发规则

### 6.1 逻辑层调用规则

逻辑层允许直接调用：

- `AudioManager.Instance.PlayBgm(...)`
- `AudioManager.Instance.PlaySfx(...)`
- `AudioManager.Instance.PlayUISfx(...)`
- `AudioManager.Instance.PlayWorldSfx(...)`

但禁止：

- 逻辑层自己创建和维护 `AudioSource`
- 逻辑层自己直接加载 `AudioClip`

### 6.2 物体挂载规则

随物体启停、显隐触发的音频，统一使用：

- `AudioEmitter`

禁止：

- 每个业务物体自己重新写一套 `OnEnable / OnDisable` 音频播放逻辑
- 在业务脚本里反复复制粘贴启停播放代码

### 6.3 资源加载规则

音频资源加载必须优先走：

- `AudioAssetLoaderAdapter`

适配层内部再对接：

- 项目现有 Addressables 封装

禁止：

- 在业务逻辑中直接写 Addressables 原生加载
- 在业务脚本中自己维护平行音频加载入口

---

## 7. 当前测试规范

当前音频测试以 `UITestScene` 为标准验证场景。

测试链路包括：

- `AudioManager` 单例直接播放
- UI 按钮点击播放 UI 音效
- HP / MP 变化驱动模式逻辑音效
- `UITestAudioDemoController` 管理测试物体
- `AudioEmitter` 挂在测试物体上，随启停触发循环音

AI 后续新增音频测试，应优先复用这条测试链路。

---

## 8. 允许 AI 修改的范围

默认允许 AI 修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Common/`
- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/TestAudio/`
- 业务模块里对 `AudioManager` 的标准调用

默认不允许 AI 修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/AudioManager/Base/`
- `Assets/Script/GamePlay/Client/GameWorld/Base/`
- 音频主链结构
- 音频加载主机制

---

## 9. AI 执行顺序要求

AI 在处理音频系统需求时，应遵循以下顺序：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 再阅读 `Assets/AI Prompt/代码规范.md`
4. 判断需求是否会触碰 `Base`
5. 如果会触碰 `Base`，必须先确认是否已获用户授权
6. 如果不触碰 `Base`，优先在 `Common/`、`TestAudio/` 或业务扩展层实现

---

## 10. 文档生效规则

从本文档开始，音频系统相关 AI 开发默认遵守以下规则：

- 音频必须统一走 `AudioManager`
- 物体挂载触发必须优先复用 `AudioEmitter`
- 音频加载必须统一走 `AudioAssetLoaderAdapter`
- 任意 `Base` 目录默认冻结
