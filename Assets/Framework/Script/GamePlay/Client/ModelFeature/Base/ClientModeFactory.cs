public static class ClientModeFactory {
    public static void InitMode(GameWorld gameWorld, ModeType modeType) {
        if (gameWorld == null) {
            return;
        }

        switch (modeType) {
            case ModeType.Room:
                gameWorld.AddExtendFeature<ClientRoomModeManager>();
                break;
            case ModeType.Test:
                gameWorld.AddExtendFeature<ClientTestModeManager>();
                break;
        }
    }
}
