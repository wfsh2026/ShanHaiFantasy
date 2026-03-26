using System;

/// <summary>
/// NetworkSync 本地回环服务端传输。
/// 用于在不启动 Mirror 连接的情况下验证 server 处理链。
/// </summary>
internal sealed class NetworkSyncLocalLoopbackServerTransport : INetworkSyncServerTransport {
    private readonly NetworkSyncLocalLoopbackHub hub;

    public event Action<NetworkSyncConnectionRef, NetworkSyncEnvelope> EnvelopeReceived;

    public NetworkSyncLocalLoopbackServerTransport(NetworkSyncLocalLoopbackHub hub) {
        this.hub = hub;
    }

    public void Bind() {
    }

    public void Unbind() {
    }

    public void Send(NetworkSyncConnectionRef connection, NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery) {
        if (connection == null) {
            return;
        }

        hub.DeliverToClient(connection.ConnectionId, envelope);
    }

    public void Broadcast(NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery, Predicate<NetworkSyncConnectionRef> filter) {
        foreach (NetworkSyncConnectionRef connection in hub.GetConnections()) {
            if (filter != null && !filter(connection)) {
                continue;
            }

            hub.DeliverToClient(connection.ConnectionId, envelope);
        }
    }

    internal void Receive(NetworkSyncConnectionRef connection, NetworkSyncEnvelope envelope) {
        EnvelopeReceived?.Invoke(connection, envelope);
    }
}
