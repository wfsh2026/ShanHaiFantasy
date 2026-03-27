using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A1 启动界面。
/// 只提供创建房间和加入房间两个入口。
/// </summary>
public sealed class RoomEntryPanel : UIPanelBase {
    private Button createRoomButton;
    private Button joinRoomButton;
    private RoomEntryPanelController controller;

    protected override void OnCreate() {
        Image background = UIRuntimeWidgetFactory.CreateImage("Background", RectTransform, new Color(0.08f, 0.1f, 0.16f, 1f));
        UIRuntimeWidgetFactory.StretchRect(background.rectTransform);

        Text titleText = CreateLabel(background.rectTransform, "Title", "山海幻想", new Vector2(0f, 170f), new Vector2(420f, 48f), 34);
        titleText.alignment = TextAnchor.MiddleCenter;

        Text subTitleText = CreateLabel(background.rectTransform, "SubTitle", "A1 房间与开局准备", new Vector2(0f, 116f), new Vector2(420f, 30f), 20);
        subTitleText.alignment = TextAnchor.MiddleCenter;

        createRoomButton = UIRuntimeWidgetFactory.CreateButton("CreateRoomButton", background.rectTransform, "创建房间", new Vector2(320f, 56f));
        joinRoomButton = UIRuntimeWidgetFactory.CreateButton("JoinRoomButton", background.rectTransform, "加入房间", new Vector2(320f, 56f));
        SetCenterRect(createRoomButton.GetComponent<RectTransform>(), new Vector2(0f, 20f), new Vector2(320f, 56f));
        SetCenterRect(joinRoomButton.GetComponent<RectTransform>(), new Vector2(0f, -58f), new Vector2(320f, 56f));

        createRoomButton.onClick.AddListener(OnClickCreateRoom);
        joinRoomButton.onClick.AddListener(OnClickJoinRoom);
    }

    protected override void OnOpen() {
        controller = new RoomEntryPanelController(this);
    }

    protected override void OnClose() {
        controller = null;
    }

    protected override void OnDestroyPanel() {
        if (createRoomButton != null) {
            createRoomButton.onClick.RemoveListener(OnClickCreateRoom);
        }

        if (joinRoomButton != null) {
            joinRoomButton.onClick.RemoveListener(OnClickJoinRoom);
        }
    }

    private void OnClickCreateRoom() {
        controller?.CreateRoom();
    }

    private void OnClickJoinRoom() {
        controller?.OpenJoinRoomPopup();
    }

    private static Text CreateLabel(Transform parent, string name, string content, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, content, fontSize, TextAnchor.MiddleCenter, Color.white);
        SetCenterRect(text.rectTransform, anchoredPosition, sizeDelta);
        return text;
    }

    private static void SetCenterRect(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 sizeDelta) {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}
