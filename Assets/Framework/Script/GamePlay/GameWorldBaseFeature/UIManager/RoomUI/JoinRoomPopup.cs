using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加入房间弹窗。
/// 负责输入邀请码并发起加入。
/// </summary>
public sealed class JoinRoomPopup : UIPanelBase {
    private InputField inviteCodeInputField;
    private Button confirmButton;
    private Button cancelButton;
    private JoinRoomPopupController controller;

    protected override void OnCreate() {
        Image mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.55f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);

        Image popupBackground = UIRuntimeWidgetFactory.CreateImage("PopupBackground", mask.rectTransform, new Color(0.16f, 0.18f, 0.26f, 0.98f));
        SetCenterRect(popupBackground.rectTransform, Vector2.zero, new Vector2(440f, 260f));

        Text titleText = CreateLabel(popupBackground.rectTransform, "Title", "加入房间", new Vector2(0f, 84f), new Vector2(260f, 34f), 30);
        titleText.alignment = TextAnchor.MiddleCenter;

        Text tipsText = CreateLabel(popupBackground.rectTransform, "Tips", "请输入邀请码", new Vector2(0f, 32f), new Vector2(260f, 26f), 18);
        tipsText.alignment = TextAnchor.MiddleCenter;

        inviteCodeInputField = UIRuntimeWidgetFactory.CreateInputField("InviteCodeInputField", popupBackground.rectTransform, string.Empty, "邀请码", new Vector2(320f, 48f));
        SetCenterRect(inviteCodeInputField.GetComponent<RectTransform>(), new Vector2(0f, -18f), new Vector2(320f, 48f));

        confirmButton = UIRuntimeWidgetFactory.CreateButton("ConfirmButton", popupBackground.rectTransform, "确认", new Vector2(132f, 44f));
        cancelButton = UIRuntimeWidgetFactory.CreateButton("CancelButton", popupBackground.rectTransform, "取消", new Vector2(132f, 44f));
        SetCenterRect(confirmButton.GetComponent<RectTransform>(), new Vector2(-78f, -88f), new Vector2(132f, 44f));
        SetCenterRect(cancelButton.GetComponent<RectTransform>(), new Vector2(78f, -88f), new Vector2(132f, 44f));

        confirmButton.onClick.AddListener(OnClickConfirm);
        cancelButton.onClick.AddListener(OnClickCancel);
    }

    protected override void OnOpen() {
        controller = new JoinRoomPopupController(this);
        inviteCodeInputField.text = string.Empty;
    }

    protected override void OnClose() {
        controller = null;
    }

    protected override void OnDestroyPanel() {
        if (confirmButton != null) {
            confirmButton.onClick.RemoveListener(OnClickConfirm);
        }

        if (cancelButton != null) {
            cancelButton.onClick.RemoveListener(OnClickCancel);
        }
    }

    private void OnClickConfirm() {
        controller?.JoinRoom(inviteCodeInputField.text);
    }

    private void OnClickCancel() {
        controller?.Cancel();
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
