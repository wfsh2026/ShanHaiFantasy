using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A1 房间主界面。
/// 显示邀请码、房间状态和固定 8 个槽位。
/// </summary>
public sealed class RoomPanel : UIPanelBase {
    private sealed class RoomSlotWidget {
        public Image AvatarImage;
        public Text SlotIndexText;
        public Text DisplayNameText;
        public Text RoleText;
    }

    private Text inviteCodeText;
    private Text playerCountText;
    private Text roomStatusText;
    private Button startButton;
    private Button leaveButton;
    private RoomSlotWidget[] slotWidgets;
    private RoomPanelController controller;

    protected override void OnCreate() {
        Image background = UIRuntimeWidgetFactory.CreateImage("Background", RectTransform, new Color(0.07f, 0.09f, 0.14f, 1f));
        UIRuntimeWidgetFactory.StretchRect(background.rectTransform);

        Text titleText = CreateLabel(background.rectTransform, "Title", "房间界面", new Vector2(24f, -24f), new Vector2(220f, 34f), 30, TextAnchor.MiddleLeft);
        inviteCodeText = CreateLabel(background.rectTransform, "InviteCode", "邀请码：--", new Vector2(24f, -68f), new Vector2(280f, 26f), 18, TextAnchor.MiddleLeft);
        playerCountText = CreateLabel(background.rectTransform, "PlayerCount", "当前人数：0 / 8", new Vector2(24f, -100f), new Vector2(280f, 26f), 18, TextAnchor.MiddleLeft);
        roomStatusText = CreateLabel(background.rectTransform, "RoomStatus", "状态：等待玩家加入", new Vector2(24f, -132f), new Vector2(420f, 26f), 18, TextAnchor.MiddleLeft);

        slotWidgets = new RoomSlotWidget[8];
        for (int i = 0; i < slotWidgets.Length; i++) {
            slotWidgets[i] = CreateSlotWidget(background.rectTransform, i);
        }

        startButton = UIRuntimeWidgetFactory.CreateButton("StartButton", background.rectTransform, "开始", new Vector2(180f, 46f));
        leaveButton = UIRuntimeWidgetFactory.CreateButton("LeaveButton", background.rectTransform, "离开房间", new Vector2(180f, 46f));
        SetBottomRect(startButton.GetComponent<RectTransform>(), new Vector2(-100f, 26f), new Vector2(180f, 46f));
        SetBottomRect(leaveButton.GetComponent<RectTransform>(), new Vector2(100f, 26f), new Vector2(180f, 46f));
        startButton.onClick.AddListener(OnClickStart);
        leaveButton.onClick.AddListener(OnClickLeave);

        RefreshInviteCode(string.Empty);
        RefreshPlayerCount(0, 0);
        RefreshRoomStatus("等待玩家加入");
        RefreshSlots(null);
        RefreshStartState(false, false);
    }

    protected override void OnOpen() {
        controller = new RoomPanelController(this);
        controller.Bind();
    }

    protected override void OnClose() {
        if (controller != null) {
            controller.Unbind();
            controller = null;
        }
    }

    protected override void OnDestroyPanel() {
        if (startButton != null) {
            startButton.onClick.RemoveListener(OnClickStart);
        }

        if (leaveButton != null) {
            leaveButton.onClick.RemoveListener(OnClickLeave);
        }
    }

    public void RefreshInviteCode(string inviteCode) {
        inviteCodeText.text = "邀请码：" + (string.IsNullOrWhiteSpace(inviteCode) ? "--" : inviteCode);
    }

    public void RefreshPlayerCount(int playerCount, int aiCount) {
        playerCountText.text = "当前人数：" + playerCount + " 真人 / " + aiCount + " AI";
    }

    public void RefreshRoomStatus(string statusText) {
        roomStatusText.text = "状态：" + (string.IsNullOrWhiteSpace(statusText) ? "等待玩家加入" : statusText);
    }

    public void RefreshSlots(NetworkSyncRoomSlotData[] slots) {
        for (int i = 0; i < slotWidgets.Length; i++) {
            NetworkSyncRoomSlotData slotData = null;
            if (slots != null && i < slots.Length) {
                slotData = slots[i];
            }

            RefreshSlot(slotWidgets[i], i, slotData);
        }
    }

    public void RefreshStartState(bool isHost, bool canStart) {
        startButton.gameObject.SetActive(isHost);
        startButton.interactable = isHost && canStart;
    }

    private void OnClickStart() {
        controller?.StartRoom();
    }

    private void OnClickLeave() {
        controller?.LeaveRoom();
    }

    private static RoomSlotWidget CreateSlotWidget(Transform parent, int slotIndex) {
        int column = slotIndex % 2;
        int row = slotIndex / 2;

        Image slotBackground = UIRuntimeWidgetFactory.CreateImage(
            "Slot_" + slotIndex,
            parent,
            new Color(0.16f, 0.19f, 0.28f, 0.98f));
        RectTransform slotRect = slotBackground.rectTransform;
        slotRect.anchorMin = new Vector2(0f, 1f);
        slotRect.anchorMax = new Vector2(0f, 1f);
        slotRect.pivot = new Vector2(0f, 1f);
        slotRect.sizeDelta = new Vector2(314f, 82f);
        slotRect.anchoredPosition = new Vector2(24f + column * 326f, -176f - row * 92f);
        slotRect.localScale = Vector3.one;

        Image avatarImage = UIRuntimeWidgetFactory.CreateImage("Avatar", slotRect, Color.white);
        RectTransform avatarRect = avatarImage.rectTransform;
        avatarRect.anchorMin = new Vector2(0f, 0.5f);
        avatarRect.anchorMax = new Vector2(0f, 0.5f);
        avatarRect.pivot = new Vector2(0f, 0.5f);
        avatarRect.sizeDelta = new Vector2(46f, 46f);
        avatarRect.anchoredPosition = new Vector2(16f, 0f);

        Text slotIndexText = CreateLabel(slotRect, "SlotIndex", "槽位 " + (slotIndex + 1), new Vector2(74f, -16f), new Vector2(86f, 20f), 16, TextAnchor.MiddleLeft);
        Text displayNameText = CreateLabel(slotRect, "DisplayName", "空槽位", new Vector2(74f, -40f), new Vector2(200f, 22f), 18, TextAnchor.MiddleLeft);
        Text roleText = CreateLabel(slotRect, "RoleText", "AI", new Vector2(230f, -40f), new Vector2(66f, 22f), 16, TextAnchor.MiddleRight);

        RoomSlotWidget widget = new RoomSlotWidget();
        widget.AvatarImage = avatarImage;
        widget.SlotIndexText = slotIndexText;
        widget.DisplayNameText = displayNameText;
        widget.RoleText = roleText;
        return widget;
    }

    private static void RefreshSlot(RoomSlotWidget slotWidget, int slotIndex, NetworkSyncRoomSlotData slotData) {
        if (slotWidget == null) {
            return;
        }

        slotWidget.SlotIndexText.text = "槽位 " + (slotIndex + 1);
        if (slotData == null) {
            slotWidget.AvatarImage.color = new Color(0.38f, 0.38f, 0.38f, 1f);
            slotWidget.DisplayNameText.text = "空槽位";
            slotWidget.RoleText.text = "空";
            return;
        }

        slotWidget.AvatarImage.color = RoomUIHelper.GetAvatarColor(slotData.avatarId);
        slotWidget.DisplayNameText.text = string.IsNullOrWhiteSpace(slotData.displayName) ? "空槽位" : slotData.displayName;
        slotWidget.RoleText.text = RoomUIHelper.GetParticipantTypeText(slotData);
    }

    private static Text CreateLabel(Transform parent, string name, string content, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAnchor anchor) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, content, fontSize, anchor, Color.white);
        RectTransform rectTransform = text.rectTransform;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        return text;
    }

    private static void SetBottomRect(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 sizeDelta) {
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}
