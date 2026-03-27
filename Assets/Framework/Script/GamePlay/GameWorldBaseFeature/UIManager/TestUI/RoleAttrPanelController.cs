using UnityEngine;

/// <summary>
/// 角色属性主面板控制器。
/// 负责把按钮命令转给模式逻辑，并把 Data 字段变化映射到主面板局部刷新。
/// </summary>
public sealed class RoleAttrPanelController : UIControllerBase<RoleAttrPanel> {
    private const int QUICK_HP_VALUE = 10;
    private const int QUICK_MP_VALUE = 5;

    private ClientTestModeData data;
    private string currentStageName;
    private int currentStageEnterCount;

    public RoleAttrPanelController(RoleAttrPanel targetPanel) : base(targetPanel) {
    }

    public override void Bind() {
        data = ResolveModeData();
        panel.ShowDefault();
        panel.SetCanChangeHP(data != null);
        panel.SetCanChangeMP(data != null);

        if (data == null) {
            return;
        }

        data.RoleNameValue.Bind(OnRoleNameChanged, true);
        data.StageNameValue.Bind(OnStageNameChanged, true);
        data.StageEnterCountValue.Bind(OnStageEnterCountChanged, true);
        data.RunningTimeValue.Bind(OnRunningTimeChanged, true);
        data.HPValue.Bind(OnHPChanged, true);
        data.MPValue.Bind(OnMPChanged, true);
        panel.RefreshNetworkDemoResult(NetworkSyncRoleFireLocalDemo.EnsureInstance().LastDemoSummary);
    }

    public override void Unbind() {
        if (data != null) {
            data.RoleNameValue.Unbind(OnRoleNameChanged);
            data.StageNameValue.Unbind(OnStageNameChanged);
            data.StageEnterCountValue.Unbind(OnStageEnterCountChanged);
            data.RunningTimeValue.Unbind(OnRunningTimeChanged);
            data.HPValue.Unbind(OnHPChanged);
            data.MPValue.Unbind(OnMPChanged);
            data.ClearAdjustAttrRequest();
        }

        data = null;
    }

    public void AddHP() {
        PlayUIButtonSound();
        ClientTestModeLogic logic = ResolveModeLogic();
        if (logic != null) {
            logic.AddHP(QUICK_HP_VALUE);
        }
    }

    public void ReduceHP() {
        PlayUIButtonSound();
        ClientTestModeLogic logic = ResolveModeLogic();
        if (logic != null) {
            logic.ReduceHP(QUICK_HP_VALUE);
        }
    }

    public void AddMP() {
        PlayUIButtonSound();
        ClientTestModeLogic logic = ResolveModeLogic();
        if (logic != null) {
            logic.AddMP(QUICK_MP_VALUE);
        }
    }

    public void ReduceMP() {
        PlayUIButtonSound();
        ClientTestModeLogic logic = ResolveModeLogic();
        if (logic != null) {
            logic.ReduceMP(QUICK_MP_VALUE);
        }
    }

    public void OpenPopup(RoleAttrType attrType, RoleAttrOperationType operationType) {
        PlayUIButtonSound();
        if (data != null) {
            // Popup 请求数据仍然先写回 Data，再由弹窗自己读取。
            int defaultValue = attrType == RoleAttrType.HP ? QUICK_HP_VALUE : QUICK_MP_VALUE;
            data.SetAdjustAttrRequest(GetOperationText(attrType, operationType), attrType, operationType, defaultValue);
        }

        OpenPanelForResult<AdjustAttrPopup, AdjustAttrPopupResult>(OnPopupResult);
    }

    public void NextStage() {
        PlayUIButtonSound();
        ClientTestModeManager modeManager = ResolveModeManager();
        if (modeManager != null) {
            modeManager.NextStage();
        }
    }

    public void ReloadScene() {
        AudioManager.Instance.PlayUISfx("scene_reload");
        if (gameWorld == null) {
            return;
        }

        ClientSceneFlowFeatureManager sceneFlowFeatureManager = gameWorld.GetExtendFeature<ClientSceneFlowFeatureManager>();
        if (sceneFlowFeatureManager == null || sceneFlowFeatureManager.SceneFlowManager == null) {
            return;
        }

        sceneFlowFeatureManager.SceneFlowManager.ReloadCurrentScene("ReloadFromUI");
    }

    public void ToggleEmitter() {
        PlayUIButtonSound();
        UITestAudioDemoController audioController = Object.FindObjectOfType<UITestAudioDemoController>();
        if (audioController != null) {
            audioController.ToggleDemoEmitterObject();
        }
    }

    public void SpawnCube() {
        PlayUIButtonSound();
        UITestPoolDemoController poolController = UITestPoolDemoController.EnsureInstance();
        if (poolController != null) {
            poolController.SpawnDemoCube();
        }
    }

    public void RecycleCube() {
        PlayUIButtonSound();
        UITestPoolDemoController poolController = UITestPoolDemoController.EnsureInstance();
        if (poolController != null) {
            poolController.RecycleLastCube();
        }
    }

    public void Close() {
        PlayUIButtonSound();
        ClosePanel();
    }

    public void RunNetworkDemo() {
        AudioManager.Instance.PlayUISfx("ui_click");
        NetworkSyncRoomLocalDemo demo = NetworkSyncRoomLocalDemo.EnsureInstance();
        if (demo == null) {
            panel.RefreshNetworkDemoResult("Create demo failed.");
            return;
        }

        string result = demo.RunRoomLifecycleDemoOnce();
        panel.RefreshNetworkDemoResult(result);
    }

    private void OnPopupResult(AdjustAttrPopupResult result) {
        if (data != null) {
            data.ClearAdjustAttrRequest();
        }

        if (result == null || !result.IsConfirm) {
            return;
        }

        ClientTestModeLogic logic = ResolveModeLogic();
        if (logic == null) {
            return;
        }

        switch (result.AttrType) {
            case RoleAttrType.HP:
                if (result.OperationType == RoleAttrOperationType.Add) {
                    logic.AddHP(result.Value);
                } else {
                    logic.ReduceHP(result.Value);
                }
                break;
            case RoleAttrType.MP:
                if (result.OperationType == RoleAttrOperationType.Add) {
                    logic.AddMP(result.Value);
                } else {
                    logic.ReduceMP(result.Value);
                }
                break;
        }
    }

    private void OnRoleNameChanged(string roleName) {
        panel.RefreshTitle(string.IsNullOrEmpty(roleName) ? "Role Attribute Test" : roleName + " Attribute Test");
    }

    private void OnStageNameChanged(string stageName) {
        currentStageName = string.IsNullOrEmpty(stageName) ? "None" : stageName;
        panel.RefreshStage(currentStageName, currentStageEnterCount);
    }

    private void OnStageEnterCountChanged(int enterCount) {
        currentStageEnterCount = enterCount;
        panel.RefreshStage(currentStageName, currentStageEnterCount);
    }

    private void OnRunningTimeChanged(float runningTime) {
        panel.RefreshRunningTime(runningTime);
    }

    private void OnHPChanged(RoleAttrValue hpValue) {
        panel.RefreshHP(hpValue);
        panel.SetCanChangeHP(hpValue.Max > 0);
    }

    private void OnMPChanged(RoleAttrValue mpValue) {
        panel.RefreshMP(mpValue);
        panel.SetCanChangeMP(mpValue.Max > 0);
    }

    private ClientTestModeManager ResolveModeManager() {
        if (gameWorld == null) {
            return null;
        }

        return gameWorld.GetExtendFeature<ClientTestModeManager>();
    }

    private ClientTestModeData ResolveModeData() {
        ClientTestModeManager modeManager = ResolveModeManager();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetData<ClientTestModeData>();
    }

    private ClientTestModeLogic ResolveModeLogic() {
        ClientTestModeManager modeManager = ResolveModeManager();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetLogic<ClientTestModeLogic>();
    }

    private static string GetOperationText(RoleAttrType attrType, RoleAttrOperationType operationType) {
        string attrName = attrType == RoleAttrType.HP ? "HP" : "MP";
        string actionName = operationType == RoleAttrOperationType.Add ? "Add" : "Reduce";
        return actionName + " " + attrName;
    }

    private static void PlayUIButtonSound() {
        AudioManager.Instance.PlayUISfx("ui_click");
    }
}
