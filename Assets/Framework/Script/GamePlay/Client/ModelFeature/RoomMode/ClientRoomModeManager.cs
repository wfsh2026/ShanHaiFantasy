/// <summary>
/// A1 房间模式管理器。
/// 负责组装房间 Data / Logic，并根据房间状态切换启动界面与房间界面。
/// </summary>
public sealed class ClientRoomModeManager : ClientModeManager {
    private ClientRoomModeData data;

    protected override void OnInit() {
        base.OnInit();

        data = AddData<ClientRoomModeData>();
        AddLogic<ClientRoomModeLogic>();
        AddStage<ClientRoomModeStage>();
        ChangeStage<ClientRoomModeStage>();
        BindUIState();
        OpenEntryPanel();
    }

    protected override void OnRemove() {
        UnbindUIState();
        CloseRoomPanels();
        data = null;
        base.OnRemove();
    }

    private void BindUIState() {
        if (data == null) {
            return;
        }

        data.HasRoomValue.Bind(OnRoomStateChanged, true);
        data.NoticeVersionValue.Bind(OnNoticeVersionChanged, false);
    }

    private void UnbindUIState() {
        if (data == null) {
            return;
        }

        data.HasRoomValue.Unbind(OnRoomStateChanged);
        data.NoticeVersionValue.Unbind(OnNoticeVersionChanged);
    }

    private void OnRoomStateChanged(bool hasRoom) {
        if (hasRoom) {
            UIManager.Instance.Close<RoomEntryPanel>();
            UIManager.Instance.Close<JoinRoomPopup>();
            UIManager.Instance.Open<RoomPanel>();
            return;
        }

        UIManager.Instance.Close<RoomPanel>();
        UIManager.Instance.Close<JoinRoomPopup>();
        OpenEntryPanel();
    }

    private void OnNoticeVersionChanged(int version) {
        if (data == null || version <= 0 || !data.HasNoticeRequest) {
            return;
        }

        UIManager.Instance.Open<RoomNoticePopup>();
    }

    private static void OpenEntryPanel() {
        if (!UIManager.Instance.IsOpen<RoomEntryPanel>()) {
            UIManager.Instance.Open<RoomEntryPanel>();
        }
    }

    private static void CloseRoomPanels() {
        UIManager.Instance.Close<RoomEntryPanel>();
        UIManager.Instance.Close<JoinRoomPopup>();
        UIManager.Instance.Close<RoomPanel>();
        UIManager.Instance.Close<RoomNoticePopup>();
    }
}
