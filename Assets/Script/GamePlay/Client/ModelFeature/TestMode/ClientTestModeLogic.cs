using UnityEngine;

public sealed class ClientTestModeLogic : AbsModeLogic {
    private ClientTestModeData data;
    private float logTimer;
    private float autoChangeTimer;
    private bool isDrainPhase;
    private RoleAttrValue currentHPValue;
    private RoleAttrValue currentMPValue;

    public override void OnInit() {
        data = manager.GetData<ClientTestModeData>();
        logTimer = 0f;
        autoChangeTimer = 0f;
        isDrainPhase = true;

        if (data != null) {
            data.HPValue.Bind(OnHPChanged, true);
            data.MPValue.Bind(OnMPChanged, true);
        }
    }

    public override void OnClear() {
        if (data != null) {
            data.HPValue.Unbind(OnHPChanged);
            data.MPValue.Unbind(OnMPChanged);
        }

        data = null;
        logTimer = 0f;
        autoChangeTimer = 0f;
        isDrainPhase = true;
        currentHPValue = default(RoleAttrValue);
        currentMPValue = default(RoleAttrValue);
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
        AudioManager.Instance.PlaySfx("hp_change");
    }

    public void ReduceHP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeHP(-value);
        AudioManager.Instance.PlaySfx("hp_change");
    }

    public void AddMP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeMP(value);
        AudioManager.Instance.PlaySfx("mp_change");
    }

    public void ReduceMP(int value) {
        if (data == null || value <= 0) {
            return;
        }

        data.ChangeMP(-value);
        AudioManager.Instance.PlaySfx("mp_change");
    }

    private void ApplyAutoChange() {
        if (data == null) {
            return;
        }

        if (isDrainPhase) {
            data.ChangeHP(-8);
            data.ChangeMP(-5);
            AudioManager.Instance.PlaySfx("hp_change");
            AudioManager.Instance.PlaySfx("mp_change");
        } else {
            data.ChangeHP(6);
            data.ChangeMP(4);
            AudioManager.Instance.PlaySfx("hp_change");
            AudioManager.Instance.PlaySfx("mp_change");
        }
    }

    private void OnHPChanged(RoleAttrValue hpValue) {
        currentHPValue = hpValue;
        RefreshDrainPhase();
    }

    private void OnMPChanged(RoleAttrValue mpValue) {
        currentMPValue = mpValue;
        RefreshDrainPhase();
    }

    private void RefreshDrainPhase() {
        if (currentHPValue.Current <= 25 || currentMPValue.Current <= 15) {
            isDrainPhase = false;
        } else if (currentHPValue.Max > 0 && currentMPValue.Max > 0 &&
                   currentHPValue.Current >= currentHPValue.Max &&
                   currentMPValue.Current >= currentMPValue.Max) {
            isDrainPhase = true;
        }
    }
}
