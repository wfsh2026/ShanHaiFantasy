using System.IO;
using UnityEngine;

public static class SavePathRegistry {
    private const string ROOT_FOLDER_NAME = "SaveData";
    private const string SETTINGS_FILE_NAME = "settings.json";
    private const string PLAYER_FILE_NAME = "player_local.json";

    public static string RootFolderPath {
        get {
            return Path.Combine(Application.persistentDataPath, ROOT_FOLDER_NAME);
        }
    }

    public static string SettingsFilePath {
        get {
            return Path.Combine(RootFolderPath, SETTINGS_FILE_NAME);
        }
    }

    public static string PlayerDataFilePath {
        get {
            return Path.Combine(RootFolderPath, PLAYER_FILE_NAME);
        }
    }
}
