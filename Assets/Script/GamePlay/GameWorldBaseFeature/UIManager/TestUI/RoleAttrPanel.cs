using UnityEngine;
using UnityEngine.UI;

public sealed class RoleAttrPanel : UIPanelBase {
    private Text titleText;
    private Text openSourceText;
    private Text stageText;
    private Text runningTimeText;
    private Text hpText;
    private Text mpText;
    private Button addHPButton;
    private Button reduceHPButton;
    private Button addMPButton;
    private Button reduceMPButton;
    private Button popupAddHPButton;
    private Button popupReduceMPButton;
    private Button nextStageButton;
    private Button reloadSceneButton;
    private Button toggleEmitterButton;
    private Button closeButton;
    private RoleAttrPanelPresenter presenter;

    protected override void OnCreate() {
        presenter = GetPresenter<RoleAttrPanelPresenter>();

        Image mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.42f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);

        Image panelBackground = UIRuntimeWidgetFactory.CreateImage("PanelBackground", mask.rectTransform, new Color(0.12f, 0.14f, 0.2f, 0.96f));
        SetAnchor(panelBackground.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(680f, 528f), Vector2.zero);

        titleText = CreateLabel(panelBackground.rectTransform, "Title", new Vector2(24f, -24f), new Vector2(630f, 32f), 28, TextAnchor.MiddleLeft);
        openSourceText = CreateLabel(panelBackground.rectTransform, "OpenSource", new Vector2(24f, -64f), new Vector2(630f, 24f), 18, TextAnchor.MiddleLeft);
        stageText = CreateLabel(panelBackground.rectTransform, "Stage", new Vector2(24f, -94f), new Vector2(630f, 24f), 18, TextAnchor.MiddleLeft);
        runningTimeText = CreateLabel(panelBackground.rectTransform, "RunningTime", new Vector2(24f, -124f), new Vector2(630f, 24f), 18, TextAnchor.MiddleLeft);
        hpText = CreateLabel(panelBackground.rectTransform, "HP", new Vector2(24f, -166f), new Vector2(260f, 28f), 24, TextAnchor.MiddleLeft);
        mpText = CreateLabel(panelBackground.rectTransform, "MP", new Vector2(24f, -202f), new Vector2(260f, 28f), 24, TextAnchor.MiddleLeft);

        addHPButton = CreateButton(panelBackground.rectTransform, "AddHPButton", "+10 HP", new Vector2(24f, -252f), new Vector2(130f, 42f), OnClickAddHP);
        reduceHPButton = CreateButton(panelBackground.rectTransform, "ReduceHPButton", "-10 HP", new Vector2(170f, -252f), new Vector2(130f, 42f), OnClickReduceHP);
        addMPButton = CreateButton(panelBackground.rectTransform, "AddMPButton", "+5 MP", new Vector2(24f, -304f), new Vector2(130f, 42f), OnClickAddMP);
        reduceMPButton = CreateButton(panelBackground.rectTransform, "ReduceMPButton", "-5 MP", new Vector2(170f, -304f), new Vector2(130f, 42f), OnClickReduceMP);
        popupAddHPButton = CreateButton(panelBackground.rectTransform, "PopupAddHPButton", "Popup +HP", new Vector2(332f, -252f), new Vector2(146f, 42f), OnClickPopupAddHP);
        popupReduceMPButton = CreateButton(panelBackground.rectTransform, "PopupReduceMPButton", "Popup -MP", new Vector2(332f, -304f), new Vector2(146f, 42f), OnClickPopupReduceMP);
        nextStageButton = CreateButton(panelBackground.rectTransform, "NextStageButton", "Next Stage", new Vector2(508f, -252f), new Vector2(146f, 42f), OnClickNextStage);
        reloadSceneButton = CreateButton(panelBackground.rectTransform, "ReloadSceneButton", "Reload Scene", new Vector2(508f, -304f), new Vector2(146f, 42f), OnClickReloadScene);
        toggleEmitterButton = CreateButton(panelBackground.rectTransform, "ToggleEmitterButton", "Toggle Emitter", new Vector2(332f, -356f), new Vector2(146f, 42f), OnClickToggleEmitter);
        closeButton = CreateButton(panelBackground.rectTransform, "CloseButton", "Close", new Vector2(508f, -356f), new Vector2(146f, 42f), OnClickClose);

        Text tipsText = CreateLabel(panelBackground.rectTransform, "Tips", new Vector2(24f, -460f), new Vector2(620f, 52f), 18, TextAnchor.UpperLeft);
        tipsText.text = "Flow Demo: UI, Input, SceneFlow, Audio.\nButtons play UI sound, HP/MP change drives mode audio, emitter object can be toggled.";
    }

    public override void Refresh(UIStateBase state) {
        RoleAttrPanelUIState uiState = state as RoleAttrPanelUIState;
        if (uiState == null) {
            return;
        }

        titleText.text = uiState.Title;
        openSourceText.text = uiState.OpenSourceText;
        stageText.text = uiState.StageName;
        runningTimeText.text = uiState.RunningTimeText;
        hpText.text = uiState.HPText;
        mpText.text = uiState.MPText;
        addHPButton.interactable = uiState.CanChangeHP;
        reduceHPButton.interactable = uiState.CanChangeHP;
        popupAddHPButton.interactable = uiState.CanChangeHP;
        addMPButton.interactable = uiState.CanChangeMP;
        reduceMPButton.interactable = uiState.CanChangeMP;
        popupReduceMPButton.interactable = uiState.CanChangeMP;
    }

    protected override void OnDestroyPanel() {
        RemoveButtonListeners();
    }

    private void OnClickAddHP() {
        if (presenter != null) {
            presenter.OnClickAddHP();
        }
    }

    private void OnClickReduceHP() {
        if (presenter != null) {
            presenter.OnClickReduceHP();
        }
    }

    private void OnClickAddMP() {
        if (presenter != null) {
            presenter.OnClickAddMP();
        }
    }

    private void OnClickReduceMP() {
        if (presenter != null) {
            presenter.OnClickReduceMP();
        }
    }

    private void OnClickPopupAddHP() {
        if (presenter != null) {
            presenter.OnClickPopupChange(RoleAttrType.HP, RoleAttrOperationType.Add);
        }
    }

    private void OnClickPopupReduceMP() {
        if (presenter != null) {
            presenter.OnClickPopupChange(RoleAttrType.MP, RoleAttrOperationType.Reduce);
        }
    }

    private void OnClickNextStage() {
        if (presenter != null) {
            presenter.OnClickNextStage();
        }
    }

    private void OnClickReloadScene() {
        if (presenter != null) {
            presenter.OnClickReloadScene();
        }
    }

    private void OnClickToggleEmitter() {
        if (presenter != null) {
            presenter.OnClickToggleEmitter();
        }
    }

    private void OnClickClose() {
        if (presenter != null) {
            presenter.OnClickClose();
        }
    }

    private void RemoveButtonListeners() {
        RemoveButtonListener(addHPButton, OnClickAddHP);
        RemoveButtonListener(reduceHPButton, OnClickReduceHP);
        RemoveButtonListener(addMPButton, OnClickAddMP);
        RemoveButtonListener(reduceMPButton, OnClickReduceMP);
        RemoveButtonListener(popupAddHPButton, OnClickPopupAddHP);
        RemoveButtonListener(popupReduceMPButton, OnClickPopupReduceMP);
        RemoveButtonListener(nextStageButton, OnClickNextStage);
        RemoveButtonListener(reloadSceneButton, OnClickReloadScene);
        RemoveButtonListener(toggleEmitterButton, OnClickToggleEmitter);
        RemoveButtonListener(closeButton, OnClickClose);
    }

    private static void RemoveButtonListener(Button button, UnityEngine.Events.UnityAction action) {
        if (button != null) {
            button.onClick.RemoveListener(action);
        }
    }

    private static Text CreateLabel(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAnchor anchor) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, string.Empty, fontSize, anchor, Color.white);
        SetAnchor(text.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), sizeDelta, anchoredPosition);
        return text;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 sizeDelta, UnityEngine.Events.UnityAction callback) {
        Button button = UIRuntimeWidgetFactory.CreateButton(name, parent, label, sizeDelta);
        SetAnchor(button.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), sizeDelta, anchoredPosition);
        button.onClick.AddListener(callback);
        return button;
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
