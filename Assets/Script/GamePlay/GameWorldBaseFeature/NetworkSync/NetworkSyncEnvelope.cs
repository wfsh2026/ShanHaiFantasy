using System;
using Mirror;

[Serializable]
public struct NetworkSyncEnvelope {
    public int worldId;
    public int messageId;
    public byte flags;
    public byte[] payload;
}

public struct NetworkSyncMirrorEnvelopeMessage : NetworkMessage {
    public int worldId;
    public int messageId;
    public byte flags;
    public byte[] payload;
}

public static class NetworkSyncEnvelopeConverter {
    public static NetworkSyncMirrorEnvelopeMessage ToMirrorMessage(NetworkSyncEnvelope envelope) {
        return new NetworkSyncMirrorEnvelopeMessage {
            worldId = envelope.worldId,
            messageId = envelope.messageId,
            flags = envelope.flags,
            payload = envelope.payload ?? new byte[0]
        };
    }

    public static NetworkSyncEnvelope ToEnvelope(NetworkSyncMirrorEnvelopeMessage message) {
        return new NetworkSyncEnvelope {
            worldId = message.worldId,
            messageId = message.messageId,
            flags = message.flags,
            payload = message.payload ?? new byte[0]
        };
    }
}
