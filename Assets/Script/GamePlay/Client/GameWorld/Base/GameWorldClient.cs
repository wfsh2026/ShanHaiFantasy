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
        gameWorld.AddExtendFeature<ClientUIFeatureManager>();
        gameWorld.AddExtendFeature<ClientInputFeatureManager>();
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
