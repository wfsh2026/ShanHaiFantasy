using UnityEngine.SceneManagement;

public sealed class ClientModeFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        ClientModeFactory.InitMode(gameWorld, ResolveModeType());
    }

    protected override void OnRemove() {
    }

    private static ModeType ResolveModeType() {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "RoomEntryScene") {
            return ModeType.Room;
        }

        if (sceneName == "BattleTestScene") {
            return ModeType.None;
        }

        if (sceneName == "UITestScene") {
            return ModeType.Test;
        }

        return ModeType.None;
    }
}
