using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A1 通用通知弹窗。
/// 负责显示房间失败、解散等提示。
/// </summary>
public sealed class RoomNoticePopup : UIPanelBase {
    private Text titleText;
    private Text messageText;
    private Button confirmButton;
    private RoomNoticePopupController controller;

    protected override void OnCreate() {
        Image mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.6f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);

        Image popupBackground = UIRuntimeWidgetFactory.CreateImage("PopupBackground", mask.rectTransform, new Color(0.15f, 0.16f, 0.22f, 0.98f));
        SetCenterRect(popupBackground.rectTransform, Vector2.zero, new Vector2(420f, 240f));

        titleText = CreateLabel(popupBackground.rectTransform, "Title", "提示", new Vector2(0f, 74f), new Vector2(280f, 34f), 28);
        titleText.alignment = TextAnchor.MiddleCenter;

        messageText = CreateLabel(popupBackground.rectTransform, "Message", "连接失败", new Vector2(0f, 10f), new Vector2(320f, 60f), 20);
        messageText.alignment = TextAnchor.MiddleCenter;

        confirmButton = UIRuntimeWidgetFactory.CreateButton("ConfirmButton", popupBackground.rectTransform, "确认", new Vector2(132f, 44f));
        SetCenterRect(confirmButton.GetComponent<RectTransform>(), new Vector2(0f, -74f), new Vector2(132f, 44f));
        confirmButton.onClick.AddListener(OnClickConfirm);
    }

    protected override void OnOpen() {
        controller = new RoomNoticePopupController(this);
        controller.Bind();
    }

    protected override void OnClose() {
        if (controller != null) {
            controller.Unbind();
            controller = null;
        }
    }

    protected override void OnDestroyPanel() {
        if (confirmButton != null) {
            confirmButton.onClick.RemoveListener(OnClickConfirm);
        }
    }

    public void RefreshNotice(string title, string message) {
        titleText.text = string.IsNullOrWhiteSpace(title) ? "提示" : title;
        messageText.text = string.IsNullOrWhiteSpace(message) ? "连接失败" : message;
    }

    private void OnClickConfirm() {
        controller?.Confirm();
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
