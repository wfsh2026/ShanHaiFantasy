using System;
using System.Text;
using UnityEngine;

/// <summary>
/// 基于 JsonUtility 的轻量序列化器。
/// 当前 NetworkSync 作为预研模块，优先保证可读性和测试链闭环。
/// </summary>
public sealed class NetworkSyncJsonSerializer : INetworkSyncSerializer {
    private static readonly Encoding UTF8 = new UTF8Encoding(false);

    public byte[] Serialize(INetworkSyncMessage message) {
        if (message == null) {
            return new byte[0];
        }

        string json = JsonUtility.ToJson(message);
        if (string.IsNullOrEmpty(json)) {
            json = "{}";
        }
        return UTF8.GetBytes(json);
    }

    public INetworkSyncMessage Deserialize(byte[] payload, Type messageType) {
        if (messageType == null) {
            throw new ArgumentNullException(nameof(messageType));
        }

        string json = payload == null || payload.Length == 0 ? "{}" : UTF8.GetString(payload);
        object message = JsonUtility.FromJson(json, messageType);
        if (message == null) {
            message = Activator.CreateInstance(messageType);
        }
        return (INetworkSyncMessage)message;
    }
}
