using System;

[Serializable]
/// <summary>
/// 玩家本地进度持久化对象。
/// 只保存本地需要恢复的轻量游戏进度。
/// </summary>
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
