public sealed class RoleAttrHUDController : UIControllerBase<RoleAttrHUDPanel> {
    private ClientTestModeData data;

    public RoleAttrHUDController(RoleAttrHUDPanel targetPanel) : base(targetPanel) {
    }

    public override void Bind() {
        data = ResolveModeData();
        panel.ShowDefault();

        if (data == null) {
            return;
        }

        data.RoleNameValue.Bind(OnRoleNameChanged, true);
        data.StageNameValue.Bind(OnStageNameChanged, true);
        data.RunningTimeValue.Bind(OnRunningTimeChanged, true);
        data.HPValue.Bind(OnHPChanged, true);
        data.MPValue.Bind(OnMPChanged, true);
    }

    public override void Unbind() {
        if (data != null) {
            data.RoleNameValue.Unbind(OnRoleNameChanged);
            data.StageNameValue.Unbind(OnStageNameChanged);
            data.RunningTimeValue.Unbind(OnRunningTimeChanged);
            data.HPValue.Unbind(OnHPChanged);
            data.MPValue.Unbind(OnMPChanged);
        }

        data = null;
    }

    public void OpenMainPanel() {
        OpenPanel<RoleAttrPanel>();
    }

    private void OnRoleNameChanged(string roleName) {
        panel.RefreshRoleName(string.IsNullOrEmpty(roleName) ? "Role HUD" : roleName);
    }

    private void OnStageNameChanged(string stageName) {
        panel.RefreshStage(string.IsNullOrEmpty(stageName) ? "None" : stageName);
    }

    private void OnRunningTimeChanged(float runningTime) {
        panel.RefreshRunningTime(runningTime);
    }

    private void OnHPChanged(RoleAttrValue hpValue) {
        panel.RefreshHP(hpValue);
    }

    private void OnMPChanged(RoleAttrValue mpValue) {
        panel.RefreshMP(mpValue);
    }

    private ClientTestModeData ResolveModeData() {
        if (gameWorld == null) {
            return null;
        }

        ClientTestModeManager modeManager = gameWorld.GetExtendFeature<ClientTestModeManager>();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetData<ClientTestModeData>();
    }
}
