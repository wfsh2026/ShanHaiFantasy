/// <summary>
/// 启动界面控制器。
/// 只负责把创建房间和加入房间入口转给房间逻辑。
/// </summary>
public sealed class RoomEntryPanelController : UIControllerBase<RoomEntryPanel> {
    public RoomEntryPanelController(RoomEntryPanel targetPanel) : base(targetPanel) {
    }

    public void CreateRoom() {
        AudioManager.Instance.PlayUISfx("ui_click");
        ClientRoomModeLogic logic = ResolveLogic();
        if (logic != null) {
            logic.CreateRoom();
        }
    }

    public void OpenJoinRoomPopup() {
        AudioManager.Instance.PlayUISfx("ui_click");
        OpenPanel<JoinRoomPopup>();
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
