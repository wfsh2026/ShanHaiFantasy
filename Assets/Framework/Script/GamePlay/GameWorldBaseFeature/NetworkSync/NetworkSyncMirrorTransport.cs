using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Mirror 网络层专用消息结构。
/// 仅在 Mirror 适配层内使用，避免把 Mirror 依赖扩散到协议公共层。
/// </summary>
public struct NetworkSyncMirrorEnvelopeMessage : NetworkMessage {
    public int worldId;
    public int messageId;
    public byte flags;
    public byte[] payload;
}

/// <summary>
/// NetworkSync 公共消息包与 Mirror 消息之间的转换工具。
/// </summary>
public static class NetworkSyncEnvelopeConverter {
    public static NetworkSyncMirrorEnvelopeMessage ToMirrorMessage(NetworkSyncEnvelope envelope) {
        return new NetworkSyncMirrorEnvelopeMessage {
            worldId = envelope.worldId,
            messageId = envelope.messageId,
            flags = envelope.flags,
            payload = envelope.payload ?? Array.Empty<byte>()
        };
    }

    public static NetworkSyncEnvelope ToEnvelope(NetworkSyncMirrorEnvelopeMessage message) {
        return NetworkSyncEnvelope.Create(message.worldId, message.messageId, message.payload, message.flags);
    }
}

/// <summary>
/// Mirror 客户端传输适配。
/// </summary>
public sealed class NetworkSyncMirrorClientTransport : INetworkSyncClientTransport {
    public event Action<NetworkSyncEnvelope> EnvelopeReceived;

    private bool isBound;

    public bool IsConnected {
        get {
            return NetworkClient.isConnected;
        }
    }

    public void Bind() {
        if (isBound) {
            return;
        }

        NetworkClient.RegisterHandler<NetworkSyncMirrorEnvelopeMessage>(OnMirrorEnvelopeReceived, false);
        isBound = true;
    }

    public void Unbind() {
        isBound = false;
    }

    public void Send(NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery) {
        if (!NetworkClient.active) {
            Debug.LogWarning("NetworkSyncMirrorClientTransport send skipped because client is not active.");
            return;
        }

        NetworkClient.Send(NetworkSyncEnvelopeConverter.ToMirrorMessage(envelope), GetChannel(delivery));
    }

    private void OnMirrorEnvelopeReceived(NetworkSyncMirrorEnvelopeMessage message) {
        if (!isBound) {
            return;
        }

        EnvelopeReceived?.Invoke(NetworkSyncEnvelopeConverter.ToEnvelope(message));
    }

    private static int GetChannel(NetworkSyncDelivery delivery) {
        return delivery == NetworkSyncDelivery.Unreliable ? Channels.Unreliable : Channels.Reliable;
    }
}

/// <summary>
/// Mirror 服务端传输适配。
/// </summary>
public sealed class NetworkSyncMirrorServerTransport : INetworkSyncServerTransport {
    public event Action<NetworkSyncConnectionRef, NetworkSyncEnvelope> EnvelopeReceived;

    private readonly Dictionary<int, NetworkSyncConnectionRef> connectionCache =
        new Dictionary<int, NetworkSyncConnectionRef>();

    private bool isBound;

    public void Bind() {
        if (isBound) {
            return;
        }

        NetworkServer.RegisterHandler<NetworkSyncMirrorEnvelopeMessage>(OnMirrorEnvelopeReceived, false);
        isBound = true;
    }

    public void Unbind() {
        connectionCache.Clear();
        isBound = false;
    }

    public void Send(NetworkSyncConnectionRef connection, NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery) {
        if (connection == null) {
            return;
        }

        NetworkConnectionToClient mirrorConnection = connection.NativeConnection as NetworkConnectionToClient;
        if (mirrorConnection == null || !mirrorConnection.isReady) {
            return;
        }

        mirrorConnection.Send(NetworkSyncEnvelopeConverter.ToMirrorMessage(envelope), GetChannel(delivery));
    }

    public void Broadcast(NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery, Predicate<NetworkSyncConnectionRef> filter) {
        if (!NetworkServer.active) {
            return;
        }

        foreach (NetworkConnectionToClient mirrorConnection in NetworkServer.connections.Values) {
            if (mirrorConnection == null || !mirrorConnection.isReady) {
                continue;
            }

            NetworkSyncConnectionRef connection = GetOrCreateConnection(mirrorConnection);
            if (filter != null && !filter(connection)) {
                continue;
            }

            mirrorConnection.Send(NetworkSyncEnvelopeConverter.ToMirrorMessage(envelope), GetChannel(delivery));
        }
    }

    private void OnMirrorEnvelopeReceived(NetworkConnectionToClient connection, NetworkSyncMirrorEnvelopeMessage message) {
        if (!isBound || connection == null) {
            return;
        }

        NetworkSyncConnectionRef connectionRef = GetOrCreateConnection(connection);
        EnvelopeReceived?.Invoke(connectionRef, NetworkSyncEnvelopeConverter.ToEnvelope(message));
    }

    private NetworkSyncConnectionRef GetOrCreateConnection(NetworkConnectionToClient mirrorConnection) {
        if (!connectionCache.TryGetValue(mirrorConnection.connectionId, out NetworkSyncConnectionRef connection)) {
            connection = new NetworkSyncConnectionRef(mirrorConnection.connectionId, mirrorConnection);
            connectionCache.Add(mirrorConnection.connectionId, connection);
        } else {
            connection.NativeConnection = mirrorConnection;
        }
        return connection;
    }

    private static int GetChannel(NetworkSyncDelivery delivery) {
        return delivery == NetworkSyncDelivery.Unreliable ? Channels.Unreliable : Channels.Reliable;
    }
}
