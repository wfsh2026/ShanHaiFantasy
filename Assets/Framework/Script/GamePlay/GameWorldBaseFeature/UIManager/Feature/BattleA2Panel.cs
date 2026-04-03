using UnityEngine;
using UnityEngine.UI;

public sealed class BattleA2Panel : UIPanelBase {
    private Image mask;
    private Image mainPanel;
    private Text titleText;
    private Text subtitleText;
    private Text localPlayerText;
    private Text stageStateText;
    private Text sectionTitleText;
    private Text sectionBodyText;
    private Text heroProfileText;
    private Text formationText;
    private Text participantText;
    private Text progressText;
    private Text footerText;
    private Button[] candidateButtons;
    private Text[] candidateTitleTexts;
    private Text[] candidateBodyTexts;
    private BattleA2PanelController controller;

    protected override void OnCreate() {
        mask = UIRuntimeWidgetFactory.CreateImage("Mask", RectTransform, new Color(0f, 0f, 0f, 0.16f));
        UIRuntimeWidgetFactory.StretchRect(mask.rectTransform);
        mainPanel = UIRuntimeWidgetFactory.CreateImage("MainPanel", mask.rectTransform, new Color(0.08f, 0.11f, 0.16f, 0.96f));
        SetRect(mainPanel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1360f, 760f), Vector2.zero);

        titleText = CreateLabel(mainPanel.rectTransform, "Title", 34, FontStyle.Bold);
        SetRect(titleText.rectTransform, new Vector2(0f, 1f), new Vector2(460f, 40f), new Vector2(36f, -28f), new Vector2(0f, 1f));
        subtitleText = CreateLabel(mainPanel.rectTransform, "Subtitle", 18, FontStyle.Normal);
        SetRect(subtitleText.rectTransform, new Vector2(0f, 1f), new Vector2(620f, 46f), new Vector2(36f, -76f), new Vector2(0f, 1f));
        localPlayerText = CreateLabel(mainPanel.rectTransform, "Local", 18, FontStyle.Normal);
        SetRect(localPlayerText.rectTransform, new Vector2(0f, 1f), new Vector2(520f, 96f), new Vector2(36f, -134f), new Vector2(0f, 1f));
        stageStateText = CreateLabel(mainPanel.rectTransform, "State", 18, FontStyle.Normal);
        SetRect(stageStateText.rectTransform, new Vector2(0f, 1f), new Vector2(520f, 110f), new Vector2(36f, -238f), new Vector2(0f, 1f));
        sectionTitleText = CreateLabel(mainPanel.rectTransform, "SectionTitle", 22, FontStyle.Bold);
        SetRect(sectionTitleText.rectTransform, new Vector2(0f, 1f), new Vector2(320f, 30f), new Vector2(36f, -362f), new Vector2(0f, 1f));
        sectionBodyText = CreateLabel(mainPanel.rectTransform, "SectionBody", 17, FontStyle.Normal);
        SetRect(sectionBodyText.rectTransform, new Vector2(0f, 1f), new Vector2(520f, 148f), new Vector2(36f, -398f), new Vector2(0f, 1f));
        heroProfileText = CreateLabel(mainPanel.rectTransform, "HeroProfile", 16, FontStyle.Normal);
        SetRect(heroProfileText.rectTransform, new Vector2(0f, 0f), new Vector2(520f, 214f), new Vector2(36f, 56f), new Vector2(0f, 0f));

        formationText = CreateLabel(mainPanel.rectTransform, "Formation", 17, FontStyle.Normal);
        SetRect(formationText.rectTransform, new Vector2(1f, 1f), new Vector2(332f, 172f), new Vector2(-36f, -248f), new Vector2(1f, 1f));
        participantText = CreateLabel(mainPanel.rectTransform, "Participants", 17, FontStyle.Normal);
        SetRect(participantText.rectTransform, new Vector2(1f, 1f), new Vector2(332f, 196f), new Vector2(-36f, -36f), new Vector2(1f, 1f));
        progressText = CreateLabel(mainPanel.rectTransform, "Progress", 16, FontStyle.Normal);
        SetRect(progressText.rectTransform, new Vector2(1f, 0f), new Vector2(332f, 214f), new Vector2(-36f, 56f), new Vector2(1f, 0f));

        footerText = CreateLabel(mainPanel.rectTransform, "Footer", 15, FontStyle.Italic);
        footerText.color = new Color(0.88f, 0.91f, 0.95f, 0.88f);
        SetRect(footerText.rectTransform, new Vector2(0f, 0f), new Vector2(1248f, 26f), new Vector2(36f, 18f), new Vector2(0f, 0f));

        candidateButtons = new Button[4];
        candidateTitleTexts = new Text[4];
        candidateBodyTexts = new Text[4];
        for (int i = 0; i < candidateButtons.Length; i++) {
            CreateCandidateCard(i);
        }

        ShowDefault();
    }

    protected override void OnOpen() {
        controller = new BattleA2PanelController(this);
        controller.Bind();
    }

    protected override void OnClose() {
        if (controller != null) {
            controller.Unbind();
            controller = null;
        }
    }

    public void ShowDefault() {
        RefreshHeader("Battle Flow", "Waiting for authoritative battle snapshot.");
        RefreshLocalPlayer("Local player info is waiting for sync.");
        RefreshStageState("Stage: MatchInit\nWaiting to enter hero select or round loop.");
        RefreshSection("Current Stage", "No hero, cultivation, formation or result summary is available yet.");
        RefreshHeroProfile("Hero profile is waiting for sync.");
        RefreshFormation("Formation and eye info are waiting for sync.");
        RefreshParticipants("Participant list is waiting for sync.");
        RefreshProgress("Round progress is waiting for sync.");
        RefreshFooter("This panel shows hero select, cultivation, formation and match result from the server snapshot.");
        for (int i = 0; i < candidateButtons.Length; i++) {
            RefreshCandidateCard(i, "Option " + (i + 1), "Waiting for sync.", false, false, false);
        }
    }

    public void RefreshHeader(string title, string subtitle) { titleText.text = title; subtitleText.text = subtitle; }
    public void RefreshLocalPlayer(string content) { localPlayerText.text = content; }
    public void RefreshStageState(string content) { stageStateText.text = content; }
    public void RefreshSection(string title, string body) { sectionTitleText.text = title; sectionBodyText.text = body; }
    public void RefreshHeroProfile(string content) { heroProfileText.text = content; }
    public void RefreshFormation(string content) { formationText.text = content; }
    public void RefreshParticipants(string content) { participantText.text = content; }
    public void RefreshProgress(string content) { progressText.text = content; }
    public void RefreshFooter(string content) { footerText.text = content; }

    public void RefreshCandidateCard(int index, string title, string body, bool visible, bool interactable, bool selected) {
        if (index < 0 || index >= candidateButtons.Length || candidateButtons[index] == null) {
            return;
        }

        Button button = candidateButtons[index];
        button.gameObject.SetActive(visible);
        button.interactable = interactable;
        candidateTitleTexts[index].text = title;
        candidateBodyTexts[index].text = body;
        Image image = button.GetComponent<Image>();
        if (image != null) {
            image.color = selected ? new Color(0.23f, 0.39f, 0.24f, 0.97f) : new Color(0.13f, 0.18f, 0.25f, 0.97f);
        }
    }

    private void CreateCandidateCard(int index) {
        Button button = UIRuntimeWidgetFactory.CreateButton("Card_" + index, mainPanel.rectTransform, string.Empty, new Vector2(178f, 256f));
        RectTransform rect = button.GetComponent<RectTransform>();
        float x = 584f + index * 184f;
        SetRect(rect, new Vector2(0f, 0f), new Vector2(178f, 256f), new Vector2(x, 98f), new Vector2(0f, 0f));
        int buttonIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            if (controller != null) {
                controller.SubmitCandidate(buttonIndex);
            }
        });

        Text title = UIRuntimeWidgetFactory.CreateText("Title", rect, string.Empty, 22, TextAnchor.UpperLeft, Color.white);
        title.fontStyle = FontStyle.Bold;
        SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(150f, 28f), new Vector2(12f, -14f), new Vector2(0f, 1f));
        Text body = UIRuntimeWidgetFactory.CreateText("Body", rect, string.Empty, 16, TextAnchor.UpperLeft, new Color(0.88f, 0.91f, 0.95f, 1f));
        SetRect(body.rectTransform, new Vector2(0f, 1f), new Vector2(154f, 184f), new Vector2(12f, -48f), new Vector2(0f, 1f));

        candidateButtons[index] = button;
        candidateTitleTexts[index] = title;
        candidateBodyTexts[index] = body;
    }

    private static Text CreateLabel(Transform parent, string name, int fontSize, FontStyle style) {
        Text text = UIRuntimeWidgetFactory.CreateText(name, parent, string.Empty, fontSize, TextAnchor.UpperLeft, Color.white);
        text.fontStyle = style;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 size, Vector2 pos, Vector2? pivot = null) {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot ?? anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }
}
