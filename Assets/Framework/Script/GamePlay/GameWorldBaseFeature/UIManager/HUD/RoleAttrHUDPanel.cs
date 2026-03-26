using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色属性 HUD。
/// 常驻显示角色名、阶段、时间和 HP/MP 条。
/// </summary>
public sealed class RoleAttrHUDPanel : UIPanelBase {
    private Text roleNameText;
    private Text stageNameText;
    private Text runningTimeText;
    private Text hpText;
    private Text mpText;
    private Image hpFillImage;
    private Image mpFillImage;
    private Button detailButton;
    private RoleAttrHUDController controller;

    protected override void OnCreate() {
        Image rootBackground = UIRuntimeWidgetFactory.CreateImage("HUDBackground", RectTransform, new Color(0.08f, 0.1f, 0.16f, 0.76f));
        SetAnchor(rootBackground.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(360f, 156f), new Vector2(18f, -18f));

        roleNameText = CreateText(rootBackground.rectTransform, "RoleName", new Vector2(16f, -12f), new Vector2(320f, 28f), 22);
        stageNameText = CreateText(rootBackground.rectTransform, "StageName", new Vector2(16f, -42f), new Vector2(320f, 24f), 18);
        runningTimeText = CreateText(rootBackground.rectTransform, "RunningTime", new Vector2(16f, -68f), new Vector2(320f, 24f), 18);

        hpText = CreateBar(rootBackground.rectTransform, "HP", new Vector2(16f, -98f), new Color(0.78f, 0.2f, 0.24f, 1f), out hpFillImage);
        mpText = CreateBar(rootBackground.rectTransform, "MP", new Vector2(16f, -126f), new Color(0.2f, 0.45f, 0.92f, 1f), out mpFillImage);

        detailButton = UIRuntimeWidgetFactory.CreateButton("OpenDetailButton", rootBackground.rectTransform, "Detail", new Vector2(92f, 32f));
        SetAnchor(detailButton.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(92f, 32f), new Vector2(-12f, -12f));
        detailButton.onClick.AddListener(OnClickOpenDetail);
        ShowDefault();
    }

    protected override void OnOpen() {
        controller = new RoleAttrHUDController(this);
        controller.Bind();
    }

    protected override void OnClose() {
        if (controller != null) {
            controller.Unbind();
            controller = null;
        }
    }

    protected override void OnDestroyPanel() {
        if (detailButton != null) {
            detailButton.onClick.RemoveListener(OnClickOpenDetail);
        }
    }

    public void ShowDefault() {
        RefreshRoleName("Role HUD");
        RefreshStage("None");
        RefreshRunningTime(0f);
        RefreshHP(new RoleAttrValue(0, 0));
        RefreshMP(new RoleAttrValue(0, 0));
    }

    public void RefreshRoleName(string roleName) {
        roleNameText.text = roleName;
    }

    public void RefreshStage(string stageName) {
        stageNameText.text = "Stage: " + stageName;
    }

    public void RefreshRunningTime(float runningTime) {
        runningTimeText.text = "Time: " + runningTime.ToString("F1") + "s";
    }

    public void RefreshHP(RoleAttrValue hpValue) {
        hpText.text = "HP  " + hpValue.Current + " / " + hpValue.Max;
        hpFillImage.fillAmount = hpValue.Max <= 0 ? 0f : (float)hpValue.Current / hpValue.Max;
    }

    public void RefreshMP(RoleAttrValue mpValue) {
        mpText.text = "MP  " + mpValue.Current + " / " + mpValue.Max;
        mpFillImage.fillAmount = mpValue.Max <= 0 ? 0f : (float)mpValue.Current / mpValue.Max;
    }

    private void OnClickOpenDetail() {
        if (controller != null) {
            controller.OpenMainPanel();
        }
    }

    private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, string.Empty, fontSize, TextAnchor.MiddleLeft, Color.white);
        SetAnchor(text.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), sizeDelta, anchoredPosition);
        return text;
    }

    private static Text CreateBar(Transform parent, string labelPrefix, Vector2 anchoredPosition, Color fillColor, out Image fillImage) {
        Image background = UIRuntimeWidgetFactory.CreateImage(labelPrefix + "BarBackground", parent, new Color(0f, 0f, 0f, 0.42f));
        SetAnchor(background.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(328f, 20f), anchoredPosition);

        fillImage = UIRuntimeWidgetFactory.CreateImage(labelPrefix + "BarFill", background.rectTransform, fillColor);
        UIRuntimeWidgetFactory.StretchRect(fillImage.rectTransform);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;

        Text text = UIRuntimeWidgetFactory.CreateText(labelPrefix + "Text", background.rectTransform, string.Empty, 16, TextAnchor.MiddleLeft, Color.white);
        UIRuntimeWidgetFactory.StretchRect(text.rectTransform);
        text.rectTransform.offsetMin = new Vector2(10f, 0f);
        text.rectTransform.offsetMax = new Vector2(-10f, 0f);
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
