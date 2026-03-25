using System;

[Serializable]
public sealed class PlayerLocalData {
    public int lastSceneId = 0;
    public string lastScenePath = string.Empty;
    public string lastModeId = string.Empty;
    public bool isFirstLaunch = true;
    public bool isTutorialFinished = false;
    public int highestUnlockedStage = 0;
    public string lastLoginDate = string.Empty;
    public string playerDisplayName = "Player";
}
