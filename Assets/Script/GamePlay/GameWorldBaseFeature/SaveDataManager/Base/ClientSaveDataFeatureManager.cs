using UnityEngine.SceneManagement;

public sealed class ClientSaveDataFeatureManager : AbsExtendGameWorldFeature {
    protected override void OnInit() {
        SaveDataManager.Instance.Initialize();
        SaveDataManager.Instance.ApplySettingsToRuntime();
        SaveDataManager.Instance.SetLastScene(SceneId.None, SceneManager.GetActiveScene().path);
        gameWorld.AddUpdate(OnUpdate);
    }

    protected override void OnRemove() {
        if (gameWorld != null) {
            gameWorld.RemoveUpdate(OnUpdate);
        }

        SaveDataManager.Instance.Clear();
    }

    private void OnUpdate(float delta) {
        SaveDataManager.Instance.Tick(delta);
    }
}
