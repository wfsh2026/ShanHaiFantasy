/// <summary>
/// 房间主界面控制器。
/// 负责绑定房间 Data，并把开始 / 离开行为转给房间逻辑。
/// </summary>
public sealed class RoomPanelController : UIControllerBase<RoomPanel> {
    private ClientRoomModeData data;
    private int currentPlayerCount;
    private int currentAICount;

    public RoomPanelController(RoomPanel targetPanel) : base(targetPanel) {
    }

    public override void Bind() {
        data = ResolveData();
        if (data == null) {
            return;
        }

        data.InviteCodeValue.Bind(OnInviteCodeChanged, true);
        data.PlayerCountValue.Bind(OnPlayerCountChanged, true);
        data.AICountValue.Bind(OnAICountChanged, true);
        data.RoomStatusValue.Bind(OnRoomStatusChanged, true);
        data.SlotListValue.Bind(OnSlotListChanged, true);
        data.IsHostValue.Bind(OnHostChanged, true);
        data.CanStartValue.Bind(OnCanStartChanged, true);
    }

    public override void Unbind() {
        if (data == null) {
            return;
        }

        data.InviteCodeValue.Unbind(OnInviteCodeChanged);
        data.PlayerCountValue.Unbind(OnPlayerCountChanged);
        data.AICountValue.Unbind(OnAICountChanged);
        data.RoomStatusValue.Unbind(OnRoomStatusChanged);
        data.SlotListValue.Unbind(OnSlotListChanged);
        data.IsHostValue.Unbind(OnHostChanged);
        data.CanStartValue.Unbind(OnCanStartChanged);
        data = null;
    }

    public void StartRoom() {
        AudioManager.Instance.PlayUISfx("ui_click");
        ClientRoomModeLogic logic = ResolveLogic();
        if (logic != null) {
            logic.StartRoom();
        }
    }

    public void LeaveRoom() {
        AudioManager.Instance.PlayUISfx("ui_click");
        ClientRoomModeLogic logic = ResolveLogic();
        if (logic != null) {
            logic.LeaveRoom();
        }
    }

    private void OnInviteCodeChanged(string inviteCode) {
        panel.RefreshInviteCode(inviteCode);
    }

    private void OnPlayerCountChanged(int playerCount) {
        currentPlayerCount = playerCount;
        panel.RefreshPlayerCount(currentPlayerCount, currentAICount);
    }

    private void OnAICountChanged(int aiCount) {
        currentAICount = aiCount;
        panel.RefreshPlayerCount(currentPlayerCount, currentAICount);
    }

    private void OnRoomStatusChanged(string statusText) {
        panel.RefreshRoomStatus(statusText);
    }

    private void OnSlotListChanged(NetworkSyncRoomSlotData[] slots) {
        panel.RefreshSlots(slots);
    }

    private void OnHostChanged(bool isHost) {
        bool canStart = data != null && data.CanStartValue.Value;
        panel.RefreshStartState(isHost, canStart);
    }

    private void OnCanStartChanged(bool canStart) {
        bool isHost = data != null && data.IsHostValue.Value;
        panel.RefreshStartState(isHost, canStart);
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
