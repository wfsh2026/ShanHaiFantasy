/// <summary>
/// 客户端世界入口。
/// 负责按固定顺序挂载客户端基础模块和测试模式。
/// </summary>
public class GameWorldClient : System.IDisposable {
    private GameWorld gameWorld;
    private bool isStarted;

    public GameWorldClient(GameWorld world) {
        gameWorld = world;
    }

    public void Init() {
        if (gameWorld == null || isStarted) {
            return;
        }

        isStarted = true;
        gameWorld.GameState = GameWorld.GameStateType.Running;
        // 先配置和存档，再 UI、音频、输入、场景流转，最后进入具体模式。
        gameWorld.AddExtendFeature<ClientConfigFeatureManager>();
        gameWorld.AddExtendFeature<ClientSaveDataFeatureManager>();
        gameWorld.AddExtendFeature<ClientUIFeatureManager>();
        gameWorld.AddExtendFeature<ClientAudioFeatureManager>();
        gameWorld.AddExtendFeature<ClientInputFeatureManager>();
        gameWorld.AddExtendFeature<ClientSceneFlowFeatureManager>();
        gameWorld.AddExtendFeature<ClientCameraFeatureManager>();
        gameWorld.AddExtendFeature<ClientModeFeatureManager>();
    }

    public void Dispose() {
        if (gameWorld == null) {
            return;
        }

        isStarted = false;
        gameWorld = null;
    }
}
