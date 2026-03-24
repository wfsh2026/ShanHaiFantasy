using UnityEngine;

public class GameEngine : MonoBehaviour {
    [SerializeField] private bool startClient = true;
    [SerializeField] private bool startServer;

    private GameWorld gameWorld;

    private void Start() {
        gameWorld = new GameWorld(startServer, startClient);
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
}
