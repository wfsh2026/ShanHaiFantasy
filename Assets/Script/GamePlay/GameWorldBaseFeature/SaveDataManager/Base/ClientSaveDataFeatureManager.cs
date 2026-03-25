using UnityEngine.SceneManagement;

/// <summary>
/// 本地存档系统的客户端挂载入口。
/// 负责初始化存档、驱动自动保存以及退出时收口。
/// </summary>
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
