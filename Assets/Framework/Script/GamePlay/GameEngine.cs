using UnityEngine;

public class GameEngine : MonoBehaviour {
    [SerializeField] private bool startClient = true;
    [SerializeField] private bool startServer;

    private GameWorld gameWorld;

    private void Start() {
        bool resolvedStartServer = ResolveStartServer();
        bool resolvedStartClient = ResolveStartClient();
        gameWorld = new GameWorld(resolvedStartServer, resolvedStartClient);
        gameWorld.Init();
    }

    private void Update() {
        OnUpdate(Time.deltaTime);
    }

    private void OnDestroy() {
        if (gameWorld != null) {
            gameWorld.Clear();
        }
        gameWorld = null;
    }

    private void OnUpdate(float deltaTime) {
        if (gameWorld != null) {
            gameWorld.OnUpdate(deltaTime);
        }
    }

    private void LateUpdate() {
        if (gameWorld != null) {
            gameWorld.OnLateUpdate();
        }
    }

    private void FixedUpdate() {
        if (gameWorld != null) {
            gameWorld.OnFixedUpdate(Time.fixedDeltaTime);
        }
    }

    private bool ResolveStartServer() {
        if (NetworkSyncMirrorRoomRuntime.HasActiveServerRuntime) {
            return true;
        }

        return startServer;
    }

    private bool ResolveStartClient() {
        if (NetworkSyncMirrorRoomRuntime.HasActiveClientRuntime) {
            return true;
        }

        return startClient;
    }
}
