using System.IO;
using UnityEngine;

/// <summary>
/// 本地文件读写适配层。
/// 只负责 JSON 读写和目录存在性处理，不承担业务规则。
/// </summary>
public sealed class SaveFileAdapter {
    public T Load<T>(string filePath) where T : class, new() {
        EnsureRootFolder();

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) {
            return new T();
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(json)) {
            return new T();
        }

        T data = JsonUtility.FromJson<T>(json);
        if (data != null) {
            return data;
        }

        return new T();
    }

    public void Save<T>(string filePath, T data) where T : class, new() {
        if (string.IsNullOrEmpty(filePath) || data == null) {
            return;
        }

        EnsureRootFolder();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    private void EnsureRootFolder() {
        string rootFolderPath = SavePathRegistry.RootFolderPath;
        if (!Directory.Exists(rootFolderPath)) {
            Directory.CreateDirectory(rootFolderPath);
        }
    }
}
