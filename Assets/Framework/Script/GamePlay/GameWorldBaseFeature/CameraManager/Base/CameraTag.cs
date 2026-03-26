using UnityEngine;

/// <summary>
/// 场景相机标记。
/// 用于给虚拟相机或普通场景锚点提供可切换的 cameraId。
/// </summary>
[DisallowMultipleComponent]
public sealed class CameraTag : MonoBehaviour {
    [SerializeField] private string cameraId = "MainFollow";

    public string CameraId {
        get {
            return cameraId;
        }
    }
}
