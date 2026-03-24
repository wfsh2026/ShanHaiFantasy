using UnityEngine;

public sealed class ClientTestModeLogic : AbsModeLogic {
    private ClientTestModeData data;
    private float logTimer;
    private float autoChangeTimer;
    private bool isDrainPhase;

    public override void OnInit() {
        data = manager.GetData<ClientTestModeData>();
        logTimer = 0f;
        autoChangeTimer = 0f;
        isDrainPhase = true;
    }

    public override void OnClear() {
        data = null;
        logTimer = 0f;
        autoChangeTimer = 0f;
        isDrainPhase = true;
    }

    public void Tick(float delta) {
        if (data == null) {
            return;
        }

        data.Tick(delta);
        logTimer += delta;
        autoChangeTimer += delta;

        if (autoChangeTimer >= 2f) {
            autoChangeTimer = 0f;
            ApplyAutoChange();
        }

        if (logTimer < 1f) {
            return;
        }

        logTimer = 0f;
        Debug.Log("ClientTestModeLogic Tick, RunningTime = " + data.RunningTime);
    }

    public void AddHP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeHP(value);
    }

    public void ReduceHP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeHP(-value);
    }

    public void AddMP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeMP(value);
    }

    public void ReduceMP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeMP(-value);
    }

    private void ApplyAutoChange() {
        if (data == null) {
            return;
        }

        if (isDrainPhase) {
            data.ChangeHP(-8);
            data.ChangeMP(-5);
        } else {
            data.ChangeHP(6);
            data.ChangeMP(4);
        }

        if (data.HP <= 25 || data.MP <= 15) {
            isDrainPhase = false;
        } else if (data.HP >= data.MaxHP && data.MP >= data.MaxMP) {
            isDrainPhase = true;
        }
    }
}
