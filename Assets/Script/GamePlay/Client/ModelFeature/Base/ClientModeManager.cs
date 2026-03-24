using UnityEngine;

public abstract class ClientModeManager : AbsModeManager {
    protected override void OnInit() {
        base.OnInit();
    }

    protected override void OnRemove() {
        base.OnRemove();
    }

    protected override void OnChangeStage(string stageName) {
        Debug.Log("Client mode stage change: " + stageName);
    }
}
