using UnityEngine.SceneManagement;

public sealed class ClientModeFeatureManager : AbsExtendGameWorldFeature {
    private AbsExtendGameWorldFeature currentModeFeature;
    private ModeType currentModeType = ModeType.None;

    protected override void OnInit() {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        RefreshModeForCurrentScene();
    }

    protected override void OnRemove() {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        RemoveCurrentMode();
    }

    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene previousScene, UnityEngine.SceneManagement.Scene currentScene) {
        RefreshModeForCurrentScene();
    }

    private void RefreshModeForCurrentScene() {
        ModeType targetModeType = ResolveModeType();
        if (targetModeType == currentModeType && currentModeFeature != null) {
            return;
        }

        RemoveCurrentMode();
        currentModeType = targetModeType;
        currentModeFeature = CreateMode(targetModeType);
    }

    private static ModeType ResolveModeType() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "RoomEntryScene") {
            return ModeType.Room;
        }

        if (sceneName == "BattleTestScene" || sceneName == "CultivationTestScene") {
            return ModeType.Battle;
        }

        if (sceneName == "UITestScene") {
            return ModeType.Test;
        }

        return ModeType.None;
    }

    private AbsExtendGameWorldFeature CreateMode(ModeType modeType) {
        switch (modeType) {
            case ModeType.Room:
                gameWorld.AddExtendFeature<ClientRoomModeManager>();
                return gameWorld.GetExtendFeature<ClientRoomModeManager>();
            case ModeType.Battle:
                gameWorld.AddExtendFeature<ClientBattleModeManager>();
                return gameWorld.GetExtendFeature<ClientBattleModeManager>();
            case ModeType.Test:
                gameWorld.AddExtendFeature<ClientTestModeManager>();
                return gameWorld.GetExtendFeature<ClientTestModeManager>();
        }

        return null;
    }

    private void RemoveCurrentMode() {
        if (currentModeFeature == null) {
            return;
        }

        gameWorld.RemoveExtendFeature(currentModeFeature);
        currentModeFeature = null;
    }
}
