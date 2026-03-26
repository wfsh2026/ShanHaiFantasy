public class GameWorldServer : System.IDisposable {
    private GameWorld gameWorld;
    private bool isStarted;

    public GameWorldServer(GameWorld world) {
        gameWorld = world;
    }

    public void Init() {
        if (gameWorld == null || isStarted) {
            return;
        }

        isStarted = true;
        gameWorld.AddExtendFeature<ServerSceneFeatureManager>();
        gameWorld.AddExtendFeature<ServerModeFeatureManager>();
        gameWorld.GameState = GameWorld.GameStateType.Running;
    }

    public void Dispose() {
        if (gameWorld == null) {
            return;
        }

        isStarted = false;
        gameWorld = null;
    }
}
