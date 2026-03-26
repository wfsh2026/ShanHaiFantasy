# AI 相机开发规范

## 1. 目的

本文档约束 `ShanHaiFantasy` 项目当前相机系统的 AI 开发边界。  
当前相机系统采用简化主链，业务层只能通过统一相机入口发起调用。

---

## 2. 当前合法主链

当前唯一合法主链如下：

`GameWorldClient -> ClientCameraFeatureManager -> CameraManager.Instance`

说明：

- `ClientCameraFeatureManager`
  只负责初始化、每帧驱动和退出清理
- `CameraManager.Instance`
  是唯一合法的相机调用入口
- 底层优先尝试 `Cinemachine`
- 若场景中不存在 `Cinemachine`，则自动回退到原生 `Camera`

---

## 3. 当前目录规则

相机基础脚本目录：

- `Assets/Script/GamePlay/GameWorldBaseFeature/CameraManager/Base/`

当前基础脚本包括：

- `ClientCameraFeatureManager`
- `CameraManager`
- `CameraTag`

---

## 4. 对外调用规则

业务层只允许通过以下接口调用相机：

- `CameraManager.Instance.RefreshSceneCamera()`
- `CameraManager.Instance.SetFollowTarget(target)`
- `CameraManager.Instance.SetLookAtTarget(target)`
- `CameraManager.Instance.SetFollowOffset(offset)`
- `CameraManager.Instance.SetPosition(position)`
- `CameraManager.Instance.SetRotation(rotation)`
- `CameraManager.Instance.SetFov(value)`
- `CameraManager.Instance.SwitchCamera(cameraId)`
- `CameraManager.Instance.Shake(intensity, duration)`

禁止：

- 在业务层到处直接写 `Camera.main`
- 在业务层直接持有 `Cinemachine` 组件引用
- 在业务层自己维护虚拟相机优先级

---

## 5. CameraTag 规则

当前镜头切换使用 `CameraTag` 标记。

规则：

- 需要参与切换的镜头或镜头锚点挂 `CameraTag`
- `cameraId` 必须唯一
- 业务层只传 `cameraId`，不直接传组件引用

示例：

- `MainFollow`
- `Overview`
- `Closeup`

---

## 6. 和现有模块的关系

### 6.1 和 Mode

合法链路：

`Mode -> CameraManager.Instance`

例如：

- 模式启动时绑定主目标
- HP 变化时触发震屏

### 6.2 和 SceneFlow

合法链路：

`SceneFlow -> CameraManager.Instance.RefreshSceneCamera()`

规则：

- 场景切换完成后必须刷新相机引用

### 6.3 和 UI

合法链路：

`UI -> Controller -> CameraManager.Instance`

规则：

- UI 不直接操作相机组件
- UI 若需要切换镜头，也必须通过 `CameraManager.Instance`

---

## 7. 禁止事项

未经授权，AI 禁止：

- 在业务层直接缓存 `Camera.main`
- 在业务层直接依赖 `CinemachineVirtualCamera`
- 自行新增多套相机管理入口
- 绕过 `CameraManager.Instance` 直接控制镜头

---

## 8. 冻结边界

以下内容默认冻结，AI 未经明确授权不得修改：

- `Assets/Script/GamePlay/GameWorldBaseFeature/CameraManager/Base/`

---

## 9. AI 执行要求

AI 处理相机需求时必须遵守：

1. 先阅读本文档
2. 再阅读 `Assets/AI Prompt/AI 开发规范文档.md`
3. 判断需求是否触碰 `CameraManager/Base`
4. 若触碰 `Base`，必须先得到明确授权
5. 相机能力优先继续收口到 `CameraManager.Instance`

---

## 10. 长期有效规则

从本文档生效开始，以下规则长期有效：

- 相机系统统一走 `CameraManager.Instance`
- 场景镜头切换统一使用 `CameraTag`
- 业务层不直接操作相机组件
- `Base` 默认冻结
