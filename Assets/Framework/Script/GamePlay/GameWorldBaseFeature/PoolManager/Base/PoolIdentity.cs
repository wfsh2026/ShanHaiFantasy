using UnityEngine;

/// <summary>
/// 对象池实例标记。
/// 用于在回收实例时快速找到所属对象池。
/// </summary>
public sealed class PoolIdentity : MonoBehaviour {
    [SerializeField]
    private string poolKey;

    public string PoolKey {
        get {
            return poolKey;
        }
    }

    public void SetPoolKey(string targetPoolKey) {
        poolKey = targetPoolKey;
    }
}
