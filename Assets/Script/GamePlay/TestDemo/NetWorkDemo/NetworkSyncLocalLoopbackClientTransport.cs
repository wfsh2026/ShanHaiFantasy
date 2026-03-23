using System;

internal sealed class NetworkSyncLocalLoopbackClientTransport : INetworkSyncClientTransport {
    private readonly NetworkSyncLocalLoopbackHub hub;

    public event Action<NetworkSyncEnvelope> EnvelopeReceived;

    public bool IsConnected {
        get {
            return true;
        }
    }

    public int ConnectionId { get; private set; }

    public NetworkSyncLocalLoopbackClientTransport(NetworkSyncLocalLoopbackHub hub, int connectionId) {
        this.hub = hub;
        ConnectionId = connectionId;
    }

    public void Bind() {
    }

    public void Unbind() {
    }

    public void Send(NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery) {
        hub.DeliverToServer(ConnectionId, envelope);
    }

    internal void Receive(NetworkSyncEnvelope envelope) {
        EnvelopeReceived?.Invoke(envelope);
    }
}
