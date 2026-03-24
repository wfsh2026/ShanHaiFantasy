public sealed class RoleAttrHUDPresenter : UIPresenterBase {
    private RoleAttrUIService uiService;

    protected override void OnOpen(UIOpenDataBase openData) {
        uiService = new RoleAttrUIService(gameWorld);
        uiService.AddDataListener(OnDataChanged);
        RefreshState();
    }

    protected override void OnClose() {
        Cleanup();
    }

    protected override void OnDispose() {
        Cleanup();
    }

    public void OnClickOpenMainPanel() {
        OpenPanel<RoleAttrPanel, RoleAttrPanelOpenData>(new RoleAttrPanelOpenData {
            OpenSource = "RoleAttrHUD",
        });
    }

    private void OnDataChanged() {
        RefreshState();
    }

    private void RefreshState() {
        if (uiService == null) {
            return;
        }

        ClientTestModeData data = uiService.GetModeData();
        if (data == null) {
            return;
        }

        RoleAttrHUDUIState state = new RoleAttrHUDUIState();
        state.RoleName = data.RoleName;
        state.StageName = data.StageName;
        state.RunningTimeText = "Time: " + data.RunningTime.ToString("F1") + "s";
        state.HPText = "HP  " + data.HP + " / " + data.MaxHP;
        state.MPText = "MP  " + data.MP + " / " + data.MaxMP;
        state.HPPercent = data.MaxHP <= 0 ? 0f : (float) data.HP / data.MaxHP;
        state.MPPercent = data.MaxMP <= 0 ? 0f : (float) data.MP / data.MaxMP;
        Refresh(state);
    }

    private void Cleanup() {
        if (uiService != null) {
            uiService.RemoveDataListener(OnDataChanged);
            uiService = null;
        }
    }
}
