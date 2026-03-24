using UnityEngine;
using UnityEngine.UI;

public sealed class AdjustAttrPopup : UIPanelBase {
    private Text titleText;
    private Text descriptionText;
    private InputField valueInputField;
    private Button confirmButton;
    private Button cancelButton;
    private AdjustAttrPopupPresenter presenter;

    protected override void OnCreate() {
        presenter = GetPresenter<AdjustAttrPopupPresenter>();

        Image mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.5f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);

        Image popupBackground = UIRuntimeWidgetFactory.CreateImage("PopupBackground", mask.rectTransform, new Color(0.14f, 0.16f, 0.24f, 0.98f));
        SetAnchor(popupBackground.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(420f, 260f), Vector2.zero);

        titleText = CreateLabel(popupBackground.rectTransform, "Title", new Vector2(24f, -24f), new Vector2(360f, 30f), 28);
        descriptionText = CreateLabel(popupBackground.rectTransform, "Description", new Vector2(24f, -72f), new Vector2(360f, 28f), 20);
        valueInputField = UIRuntimeWidgetFactory.CreateInputField("ValueInputField", popupBackground.rectTransform, string.Empty, "请输入数值", new Vector2(360f, 48f));
        SetAnchor(valueInputField.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(360f, 48f), new Vector2(0f, -12f));

        confirmButton = UIRuntimeWidgetFactory.CreateButton("ConfirmButton", popupBackground.rectTransform, "确认", new Vector2(140f, 42f));
        cancelButton = UIRuntimeWidgetFactory.CreateButton("CancelButton", popupBackground.rectTransform, "取消", new Vector2(140f, 42f));
        SetAnchor(confirmButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(140f, 42f), new Vector2(-84f, 24f));
        SetAnchor(cancelButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(140f, 42f), new Vector2(84f, 24f));

        confirmButton.onClick.AddListener(OnClickConfirm);
        cancelButton.onClick.AddListener(OnClickCancel);
    }

    public override void Refresh(UIStateBase state) {
        AdjustAttrPopupUIState uiState = state as AdjustAttrPopupUIState;
        if (uiState == null) {
            return;
        }

        titleText.text = uiState.Title;
        descriptionText.text = uiState.Description;
        valueInputField.text = uiState.DefaultValue;
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
        if (presenter != null) {
            presenter.OnClickConfirm(valueInputField.text);
        }
    }

    private void OnClickCancel() {
        if (presenter != null) {
            presenter.OnClickCancel();
        }
    }

    private static Text CreateLabel(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, string.Empty, fontSize, TextAnchor.MiddleLeft, Color.white);
        SetAnchor(text.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), sizeDelta, anchoredPosition);
        return text;
    }

    private static void SetAnchor(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Vector2 anchoredPosition) {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}
