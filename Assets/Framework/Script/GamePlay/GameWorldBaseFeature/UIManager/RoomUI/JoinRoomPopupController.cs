/// <summary>
/// 加入房间弹窗控制器。
/// </summary>
public sealed class JoinRoomPopupController : UIControllerBase<JoinRoomPopup> {
    public JoinRoomPopupController(JoinRoomPopup targetPanel) : base(targetPanel) {
    }

    public void JoinRoom(string inviteCode) {
        AudioManager.Instance.PlayUISfx("ui_click");
        ClientRoomModeLogic logic = ResolveLogic();
        if (logic != null) {
            logic.JoinRoom(inviteCode);
        }

        ClosePanel();
    }

    public void Cancel() {
        AudioManager.Instance.PlayUISfx("ui_click");
        ClosePanel();
    }

    private ClientRoomModeLogic ResolveLogic() {
        if (gameWorld == null) {
            return null;
        }

        ClientRoomModeManager modeManager = gameWorld.GetExtendFeature<ClientRoomModeManager>();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetLogic<ClientRoomModeLogic>();
    }
}
