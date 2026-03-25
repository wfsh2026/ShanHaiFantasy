using UnityEngine;

/// <summary>
/// 测试模式的单阶段实现。
/// 当前主要用于验证模式切换、阶段计数和 UI 刷新链路。
/// </summary>
public sealed class ClientTestModeStage : AbsModeStage {
    public override void OnEnter() {
        ClientTestModeData data = manager.GetData<ClientTestModeData>();
        if (data != null) {
            data.MarkStageEnter();
            data.SetStageName(GetType().Name);
        }

        Debug.Log("ClientTestModeStage Enter");
    }

    public override void OnUpdate(float delta) {
    }

    public override void OnQuit() {
        Debug.Log("ClientTestModeStage Quit");
    }
}
