using System;

/// <summary>
/// NetworkSync 统一消息包。
/// 这一层只保留与同步协议相关的公共字段，不再直接依赖具体网络库。
/// </summary>
[Serializable]
public struct NetworkSyncEnvelope {
    public int worldId;
    public int messageId;
    public byte flags;
    public byte[] payload;

    /// <summary>
    /// 生成一个安全可发送的消息包副本，避免外部传入空 payload。
    /// </summary>
    public static NetworkSyncEnvelope Create(int worldId, int messageId, byte[] payload, byte flags = 0) {
        return new NetworkSyncEnvelope {
            worldId = worldId,
            messageId = messageId,
            flags = flags,
            payload = payload ?? Array.Empty<byte>()
        };
    }
}
