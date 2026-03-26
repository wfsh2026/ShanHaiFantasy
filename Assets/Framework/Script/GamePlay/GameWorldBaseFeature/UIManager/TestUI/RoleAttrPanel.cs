using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色属性主面板。
/// 负责测试 UI、输入、场景流转和音频的综合调用链。
/// </summary>
public sealed class RoleAttrPanel : UIPanelBase {
    private Text titleText;
    private Text openSourceText;
    private Text stageText;
    private Text runningTimeText;
    private Text hpText;
    private Text mpText;
    private Text networkDemoText;
    private Button addHPButton;
    private Button reduceHPButton;
    private Button addMPButton;
    private Button reduceMPButton;
    private Button popupAddHPButton;
    private Button popupReduceMPButton;
    private Button nextStageButton;
    private Button reloadSceneButton;
    private Button toggleEmitterButton;
    private Button spawnCubeButton;
    private Button recycleCubeButton;
    private Button networkDemoButton;
    private Button closeButton;
    private RoleAttrPanelController controller;

    protected override void OnCreate() {
        Image mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.42f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);

        Image panelBackground = UIRuntimeWidgetFactory.CreateImage("PanelBackground", mask.rectTransform, new Color(0.12f, 0.14f, 0.2f, 0.96f));
        SetAnchor(panelBackground.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(680f, 588f), Vector2.zero);

        titleText = CreateLabel(panelBackground.rectTransform, "Title", new Vector2(24f, -24f), new Vector2(630f, 32f), 28, TextAnchor.MiddleLeft);
        openSourceText = CreateLabel(panelBackground.rectTransform, "OpenSource", new Vector2(24f, -64f), new Vector2(630f, 24f), 18, TextAnchor.MiddleLeft);
        stageText = CreateLabel(panelBackground.rectTransform, "Stage", new Vector2(24f, -94f), new Vector2(630f, 24f), 18, TextAnchor.MiddleLeft);
        runningTimeText = CreateLabel(panelBackground.rectTransform, "RunningTime", new Vector2(24f, -124f), new Vector2(630f, 24f), 18, TextAnchor.MiddleLeft);
        hpText = CreateLabel(panelBackground.rectTransform, "HP", new Vector2(24f, -166f), new Vector2(260f, 28f), 24, TextAnchor.MiddleLeft);
        mpText = CreateLabel(panelBackground.rectTransform, "MP", new Vector2(24f, -202f), new Vector2(260f, 28f), 24, TextAnchor.MiddleLeft);
        networkDemoText = CreateLabel(panelBackground.rectTransform, "NetworkDemo", new Vector2(24f, -448f), new Vector2(630f, 48f), 16, TextAnchor.UpperLeft);

        addHPButton = CreateButton(panelBackground.rectTransform, "AddHPButton", "+10 HP", new Vector2(24f, -252f), new Vector2(130f, 42f), OnClickAddHP);
        reduceHPButton = CreateButton(panelBackground.rectTransform, "ReduceHPButton", "-10 HP", new Vector2(170f, -252f), new Vector2(130f, 42f), OnClickReduceHP);
        addMPButton = CreateButton(panelBackground.rectTransform, "AddMPButton", "+5 MP", new Vector2(24f, -304f), new Vector2(130f, 42f), OnClickAddMP);
        reduceMPButton = CreateButton(panelBackground.rectTransform, "ReduceMPButton", "-5 MP", new Vector2(170f, -304f), new Vector2(130f, 42f), OnClickReduceMP);
        popupAddHPButton = CreateButton(panelBackground.rectTransform, "PopupAddHPButton", "Popup +HP", new Vector2(332f, -252f), new Vector2(146f, 42f), OnClickPopupAddHP);
        popupReduceMPButton = CreateButton(panelBackground.rectTransform, "PopupReduceMPButton", "Popup -MP", new Vector2(332f, -304f), new Vector2(146f, 42f), OnClickPopupReduceMP);
        nextStageButton = CreateButton(panelBackground.rectTransform, "NextStageButton", "Next Stage", new Vector2(508f, -252f), new Vector2(146f, 42f), OnClickNextStage);
        reloadSceneButton = CreateButton(panelBackground.rectTransform, "ReloadSceneButton", "Reload Scene", new Vector2(508f, -304f), new Vector2(146f, 42f), OnClickReloadScene);
        toggleEmitterButton = CreateButton(panelBackground.rectTransform, "ToggleEmitterButton", "Toggle Emitter", new Vector2(332f, -356f), new Vector2(146f, 42f), OnClickToggleEmitter);
        spawnCubeButton = CreateButton(panelBackground.rectTransform, "SpawnCubeButton", "Spawn Cube", new Vector2(24f, -356f), new Vector2(130f, 42f), OnClickSpawnCube);
        recycleCubeButton = CreateButton(panelBackground.rectTransform, "RecycleCubeButton", "Recycle Cube", new Vector2(170f, -356f), new Vector2(130f, 42f), OnClickRecycleCube);
        networkDemoButton = CreateButton(panelBackground.rectTransform, "NetworkDemoButton", "Run Network Demo", new Vector2(332f, -408f), new Vector2(146f, 42f), OnClickRunNetworkDemo);
        closeButton = CreateButton(panelBackground.rectTransform, "CloseButton", "Close", new Vector2(508f, -408f), new Vector2(146f, 42f), OnClickClose);

        Text tipsText = CreateLabel(panelBackground.rectTransform, "Tips", new Vector2(24f, -504f), new Vector2(620f, 56f), 18, TextAnchor.UpperLeft);
        tipsText.text = "Flow Demo: UI, Input, SceneFlow, Audio, Pool.\nButtons play UI sound, HP/MP change drives mode audio, emitter object can be toggled, cubes are spawned from prefab pool.";
        ShowDefault();
    }

    protected override void OnOpen() {
        controller = new RoleAttrPanelController(this);
        controller.Bind();
    }

    protected override void OnClose() {
        if (controller != null) {
            controller.Unbind();
            controller = null;
        }
    }

    protected override void OnDestroyPanel() {
        RemoveButtonListeners();
    }

    public void ShowDefault() {
        RefreshTitle("Role Attribute Test");
        openSourceText.text = "OpenSource: FieldBinding";
        RefreshStage("None", 0);
        RefreshRunningTime(0f);
        RefreshHP(new RoleAttrValue(0, 0));
        RefreshMP(new RoleAttrValue(0, 0));
        RefreshNetworkDemoResult("Network demo not executed.");
        SetCanChangeHP(false);
        SetCanChangeMP(false);
    }

    public void RefreshTitle(string title) {
        titleText.text = title;
    }

    public void RefreshStage(string stageName, int enterCount) {
        stageText.text = "Stage: " + stageName + "  |  EnterCount: " + enterCount;
    }

    public void RefreshRunningTime(float runningTime) {
        runningTimeText.text = "RunningTime: " + runningTime.ToString("F1") + "s";
    }

    public void RefreshHP(RoleAttrValue hpValue) {
        hpText.text = "HP: " + hpValue.Current + " / " + hpValue.Max;
    }

    public void RefreshMP(RoleAttrValue mpValue) {
        mpText.text = "MP: " + mpValue.Current + " / " + mpValue.Max;
    }

    public void RefreshNetworkDemoResult(string resultText) {
        networkDemoText.text = "NetworkSync: " + (string.IsNullOrEmpty(resultText) ? "None" : resultText);
    }

    public void SetCanChangeHP(bool canChange) {
        addHPButton.interactable = canChange;
        reduceHPButton.interactable = canChange;
        popupAddHPButton.interactable = canChange;
    }

    public void SetCanChangeMP(bool canChange) {
        addMPButton.interactable = canChange;
        reduceMPButton.interactable = canChange;
        popupReduceMPButton.interactable = canChange;
    }

    private void OnClickAddHP() {
        if (controller != null) {
            controller.AddHP();
        }
    }

    private void OnClickReduceHP() {
        if (controller != null) {
            controller.ReduceHP();
        }
    }

    private void OnClickAddMP() {
        if (controller != null) {
            controller.AddMP();
        }
    }

    private void OnClickReduceMP() {
        if (controller != null) {
            controller.ReduceMP();
        }
    }

    private void OnClickPopupAddHP() {
        if (controller != null) {
            controller.OpenPopup(RoleAttrType.HP, RoleAttrOperationType.Add);
        }
    }

    private void OnClickPopupReduceMP() {
        if (controller != null) {
            controller.OpenPopup(RoleAttrType.MP, RoleAttrOperationType.Reduce);
        }
    }

    private void OnClickNextStage() {
        if (controller != null) {
            controller.NextStage();
        }
    }

    private void OnClickReloadScene() {
        if (controller != null) {
            controller.ReloadScene();
        }
    }

    private void OnClickToggleEmitter() {
        if (controller != null) {
            controller.ToggleEmitter();
        }
    }

    private void OnClickSpawnCube() {
        if (controller != null) {
            controller.SpawnCube();
        }
    }

    private void OnClickRecycleCube() {
        if (controller != null) {
            controller.RecycleCube();
        }
    }

    private void OnClickRunNetworkDemo() {
        if (controller != null) {
            controller.RunNetworkDemo();
        }
    }

    private void OnClickClose() {
        if (controller != null) {
            controller.Close();
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
        RemoveButtonListener(spawnCubeButton, OnClickSpawnCube);
        RemoveButtonListener(recycleCubeButton, OnClickRecycleCube);
        RemoveButtonListener(networkDemoButton, OnClickRunNetworkDemo);
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
