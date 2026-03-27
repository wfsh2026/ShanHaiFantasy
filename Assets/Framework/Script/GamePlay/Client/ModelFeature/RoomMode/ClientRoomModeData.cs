public sealed class ClientRoomModeData : AbsModeData {
    private readonly BindableValue<bool> hasRoomValue = new BindableValue<bool>(false);
    private readonly BindableValue<string> inviteCodeValue = new BindableValue<string>(string.Empty);
    private readonly BindableValue<int> playerCountValue = new BindableValue<int>(0);
    private readonly BindableValue<int> aiCountValue = new BindableValue<int>(0);
    private readonly BindableValue<string> roomStatusValue = new BindableValue<string>("请选择创建房间或加入房间");
    private readonly BindableValue<NetworkSyncRoomSlotData[]> slotListValue =
        new BindableValue<NetworkSyncRoomSlotData[]>(BuildDefaultSlots());
    private readonly BindableValue<bool> isHostValue = new BindableValue<bool>(false);
    private readonly BindableValue<bool> canStartValue = new BindableValue<bool>(false);
    private readonly BindableValue<int> noticeVersionValue = new BindableValue<int>(0);

    private string noticeTitle;
    private string noticeMessage;
    private bool shouldReturnToEntryOnNoticeConfirm;

    public BindableValue<bool> HasRoomValue {
        get {
            return hasRoomValue;
        }
    }

    public BindableValue<string> InviteCodeValue {
        get {
            return inviteCodeValue;
        }
    }

    public BindableValue<int> PlayerCountValue {
        get {
            return playerCountValue;
        }
    }

    public BindableValue<int> AICountValue {
        get {
            return aiCountValue;
        }
    }

    public BindableValue<string> RoomStatusValue {
        get {
            return roomStatusValue;
        }
    }

    public BindableValue<NetworkSyncRoomSlotData[]> SlotListValue {
        get {
            return slotListValue;
        }
    }

    public BindableValue<bool> IsHostValue {
        get {
            return isHostValue;
        }
    }

    public BindableValue<bool> CanStartValue {
        get {
            return canStartValue;
        }
    }

    public BindableValue<int> NoticeVersionValue {
        get {
            return noticeVersionValue;
        }
    }

    public bool HasNoticeRequest {
        get {
            return !string.IsNullOrEmpty(noticeMessage);
        }
    }

    public string NoticeTitle {
        get {
            return noticeTitle;
        }
    }

    public string NoticeMessage {
        get {
            return noticeMessage;
        }
    }

    public bool ShouldReturnToEntryOnNoticeConfirm {
        get {
            return shouldReturnToEntryOnNoticeConfirm;
        }
    }

    public override void OnInit() {
        SetEntryState();
    }

    public override void OnClear() {
        hasRoomValue.ClearListeners();
        inviteCodeValue.ClearListeners();
        playerCountValue.ClearListeners();
        aiCountValue.ClearListeners();
        roomStatusValue.ClearListeners();
        slotListValue.ClearListeners();
        isHostValue.ClearListeners();
        canStartValue.ClearListeners();
        noticeVersionValue.ClearListeners();
    }

    public void SetEntryState() {
        hasRoomValue.SetValue(false);
        inviteCodeValue.SetValue(string.Empty);
        playerCountValue.SetValue(0);
        aiCountValue.SetValue(0);
        roomStatusValue.SetValue("请选择创建房间或加入房间");
        slotListValue.SetValue(BuildDefaultSlots());
        isHostValue.SetValue(false);
        canStartValue.SetValue(false);
    }

    public void SetWaitingStatus(string statusText) {
        roomStatusValue.SetValue(string.IsNullOrWhiteSpace(statusText) ? string.Empty : statusText);
    }

    public void SetRoomState(NetworkSyncRoomStateRpc roomState, string localPlayerId) {
        if (roomState == null) {
            return;
        }

        hasRoomValue.SetValue(true);
        inviteCodeValue.SetValue(roomState.inviteCode ?? string.Empty);
        playerCountValue.SetValue(roomState.playerCount);
        aiCountValue.SetValue(roomState.aiCount);
        slotListValue.SetValue(CloneSlots(roomState.slots));

        bool isLocalHost = false;
        if (!string.IsNullOrEmpty(localPlayerId) && roomState.slots != null) {
            for (int i = 0; i < roomState.slots.Length; i++) {
                NetworkSyncRoomSlotData slot = roomState.slots[i];
                if (slot != null && slot.playerId == localPlayerId) {
                    isLocalHost = slot.isHost;
                    break;
                }
            }
        }

        isHostValue.SetValue(isLocalHost);
        canStartValue.SetValue(isLocalHost && !roomState.hasStarted);
        roomStatusValue.SetValue(roomState.hasStarted ? "正在进入战斗场景..." : "等待玩家加入");
    }

    public void SetNoticeRequest(string title, string message, bool returnToEntry) {
        noticeTitle = string.IsNullOrWhiteSpace(title) ? "提示" : title;
        noticeMessage = string.IsNullOrWhiteSpace(message) ? "连接失败" : message;
        shouldReturnToEntryOnNoticeConfirm = returnToEntry;
        noticeVersionValue.SetValue(noticeVersionValue.Value + 1);
    }

    public void ClearNoticeRequest() {
        noticeTitle = string.Empty;
        noticeMessage = string.Empty;
        shouldReturnToEntryOnNoticeConfirm = false;
    }

    private static NetworkSyncRoomSlotData[] BuildDefaultSlots() {
        NetworkSyncRoomSlotData[] slots = new NetworkSyncRoomSlotData[8];
        for (int i = 0; i < slots.Length; i++) {
            slots[i] = new NetworkSyncRoomSlotData {
                slotIndex = i,
                displayName = "空槽位",
                avatarId = string.Empty,
                participantType = NetworkSyncRoomParticipantType.AI,
                playerId = string.Empty,
                isHost = false
            };
        }

        return slots;
    }

    private static NetworkSyncRoomSlotData[] CloneSlots(NetworkSyncRoomSlotData[] sourceSlots) {
        if (sourceSlots == null || sourceSlots.Length == 0) {
            return BuildDefaultSlots();
        }

        NetworkSyncRoomSlotData[] slots = new NetworkSyncRoomSlotData[sourceSlots.Length];
        for (int i = 0; i < sourceSlots.Length; i++) {
            NetworkSyncRoomSlotData source = sourceSlots[i];
            if (source == null) {
                slots[i] = new NetworkSyncRoomSlotData();
                continue;
            }

            slots[i] = new NetworkSyncRoomSlotData {
                slotIndex = source.slotIndex,
                displayName = source.displayName,
                avatarId = source.avatarId,
                participantType = source.participantType,
                playerId = source.playerId,
                isHost = source.isHost
            };
        }

        return slots;
    }
}
