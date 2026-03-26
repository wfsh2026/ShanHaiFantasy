public partial class GameWorld {
    private readonly bool IS_START_SERVER;
    private readonly bool IS_START_CLIENT;
    private GameWorldServer myGameWorldServer;
    private GameWorldClient myGameWorldClient;

    public bool IsStartServer {
        get {
            return IS_START_SERVER;
        }
    }

    public bool IsStartClient {
        get {
            return IS_START_CLIENT;
        }
    }

    public bool IsStartHost {
        get {
            return IsStartServer && IsStartClient;
        }
    }

    public GameWorldServer MyGameWorldServer {
        get {
            return myGameWorldServer;
        }
        private set {
            myGameWorldServer = value;
        }
    }

    public GameWorldClient MyGameWorldClient {
        get {
            return myGameWorldClient;
        }
        private set {
            myGameWorldClient = value;
        }
    }

    public GameStateType GameState {
        get {
            return gameState;
        }
        set {
            gameState = value;
        }
    }

    public enum GameStateType {
        None = 0,
        Loading = 1,
        Running = 2,
        GameOver = 3,
        ServerClose = 4
    }

    public GameWorld(bool isStartServer, bool isStartClient) {
        IS_START_SERVER = isStartServer;
        IS_START_CLIENT = isStartClient;
    }

    public void Init() {
        OnInit();

        if (IsStartServer) {
            MyGameWorldServer = new GameWorldServer(this);
            MyGameWorldServer.Init();
        }

        if (IsStartClient) {
            MyGameWorldClient = new GameWorldClient(this);
            MyGameWorldClient.Init();
        }
    }

    public void Clear() {
        if (isDispose) {
            return;
        }

        if (MyGameWorldClient != null) {
            MyGameWorldClient.Dispose();
            MyGameWorldClient = null;
        }

        if (MyGameWorldServer != null) {
            MyGameWorldServer.Dispose();
            MyGameWorldServer = null;
        }

        Dispose();
    }
}
