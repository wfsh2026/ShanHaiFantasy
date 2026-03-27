/// <summary>
/// 通知弹窗控制器。
/// </summary>
public sealed class RoomNoticePopupController : UIControllerBase<RoomNoticePopup> {
    private ClientRoomModeData data;

    public RoomNoticePopupController(RoomNoticePopup targetPanel) : base(targetPanel) {
    }

    public override void Bind() {
        data = ResolveData();
        if (data == null || !data.HasNoticeRequest) {
            panel.RefreshNotice("提示", "连接失败");
            return;
        }

        panel.RefreshNotice(data.NoticeTitle, data.NoticeMessage);
    }

    public override void Unbind() {
        data = null;
    }

    public void Confirm() {
        AudioManager.Instance.PlayUISfx("ui_click");
        ClientRoomModeLogic logic = ResolveLogic();
        if (logic != null) {
            logic.ConfirmNotice();
        }

        ClosePanel();
    }

    private ClientRoomModeData ResolveData() {
        if (gameWorld == null) {
            return null;
        }

        ClientRoomModeManager modeManager = gameWorld.GetExtendFeature<ClientRoomModeManager>();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetData<ClientRoomModeData>();
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
