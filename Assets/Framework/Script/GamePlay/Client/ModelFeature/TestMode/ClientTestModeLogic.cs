using UnityEngine;

/// <summary>
/// 测试模式的业务逻辑层。
/// 负责属性增减规则、自动变化节奏以及基于 Data 的联动判断。
/// </summary>
public sealed class ClientTestModeLogic : AbsModeLogic {
    private const float DEFAULT_AUTO_CHANGE_INTERVAL = 2f;
    private const int DEFAULT_DRAIN_HP_DELTA = -8;
    private const int DEFAULT_DRAIN_MP_DELTA = -5;
    private const int DEFAULT_RECOVER_HP_DELTA = 6;
    private const int DEFAULT_RECOVER_MP_DELTA = 4;

    private ClientTestModeData data;
    private float logTimer;
    private float autoChangeTimer;
    private float autoChangeInterval;
    private int drainHPDelta;
    private int drainMPDelta;
    private int recoverHPDelta;
    private int recoverMPDelta;
    private bool isDrainPhase;
    private RoleAttrValue currentHPValue;
    private RoleAttrValue currentMPValue;

    public override void OnInit() {
        data = manager.GetData<ClientTestModeData>();
        logTimer = 0f;
        autoChangeTimer = 0f;
        autoChangeInterval = DEFAULT_AUTO_CHANGE_INTERVAL;
        drainHPDelta = DEFAULT_DRAIN_HP_DELTA;
        drainMPDelta = DEFAULT_DRAIN_MP_DELTA;
        recoverHPDelta = DEFAULT_RECOVER_HP_DELTA;
        recoverMPDelta = DEFAULT_RECOVER_MP_DELTA;
        isDrainPhase = true;

        if (data != null) {
            // 逻辑层也通过 Data 做联动判断，保证所有状态变化都围绕同一份权威数据。
            data.HPValue.Bind(OnHPChanged, true);
            data.MPValue.Bind(OnMPChanged, true);
        }

        ConfigManager.Instance.Load<TestModeConfig>(OnConfigLoaded);
    }

    public override void OnClear() {
        if (data != null) {
            data.HPValue.Unbind(OnHPChanged);
            data.MPValue.Unbind(OnMPChanged);
        }

        data = null;
        logTimer = 0f;
        autoChangeTimer = 0f;
        autoChangeInterval = DEFAULT_AUTO_CHANGE_INTERVAL;
        drainHPDelta = DEFAULT_DRAIN_HP_DELTA;
        drainMPDelta = DEFAULT_DRAIN_MP_DELTA;
        recoverHPDelta = DEFAULT_RECOVER_HP_DELTA;
        recoverMPDelta = DEFAULT_RECOVER_MP_DELTA;
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

        if (autoChangeTimer >= autoChangeInterval) {
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
        CameraManager.Instance.Shake(0.18f, 0.16f);
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
            data.ChangeHP(drainHPDelta);
            data.ChangeMP(drainMPDelta);
            AudioManager.Instance.PlaySfx("hp_change");
            AudioManager.Instance.PlaySfx("mp_change");
            CameraManager.Instance.Shake(0.08f, 0.1f);
        } else {
            data.ChangeHP(recoverHPDelta);
            data.ChangeMP(recoverMPDelta);
            AudioManager.Instance.PlaySfx("hp_change");
            AudioManager.Instance.PlaySfx("mp_change");
        }
    }

    private void OnConfigLoaded(TestModeConfig config) {
        if (data == null || config == null) {
            return;
        }

        data.ApplyConfig(config);
        autoChangeInterval = Mathf.Max(0.1f, config.AutoChangeInterval);
        drainHPDelta = config.DrainHPDelta;
        drainMPDelta = config.DrainMPDelta;
        recoverHPDelta = config.RecoverHPDelta;
        recoverMPDelta = config.RecoverMPDelta;
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
