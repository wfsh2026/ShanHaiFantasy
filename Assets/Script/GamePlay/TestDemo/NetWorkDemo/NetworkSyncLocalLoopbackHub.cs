using System.Collections.Generic;

internal sealed class NetworkSyncLocalLoopbackHub {
    private readonly Dictionary<int, NetworkSyncLocalLoopbackClientTransport> clientTransports =
        new Dictionary<int, NetworkSyncLocalLoopbackClientTransport>();

    private readonly Dictionary<int, NetworkSyncConnectionRef> connections =
        new Dictionary<int, NetworkSyncConnectionRef>();

    private NetworkSyncLocalLoopbackServerTransport serverTransport;

    public NetworkSyncLocalLoopbackServerTransport CreateServerTransport() {
        serverTransport = new NetworkSyncLocalLoopbackServerTransport(this);
        return serverTransport;
    }

    public NetworkSyncLocalLoopbackClientTransport CreateClientTransport(int connectionId) {
        NetworkSyncLocalLoopbackClientTransport clientTransport =
            new NetworkSyncLocalLoopbackClientTransport(this, connectionId);
        clientTransports[connectionId] = clientTransport;
        GetConnection(connectionId);
        return clientTransport;
    }

    public NetworkSyncConnectionRef GetConnection(int connectionId) {
        if (!connections.TryGetValue(connectionId, out NetworkSyncConnectionRef connection)) {
            connection = new NetworkSyncConnectionRef(connectionId, null);
            connections.Add(connectionId, connection);
        }
        return connection;
    }

    public IEnumerable<NetworkSyncConnectionRef> GetConnections() {
        return connections.Values;
    }

    public void DeliverToServer(int connectionId, NetworkSyncEnvelope envelope) {
        serverTransport?.Receive(GetConnection(connectionId), envelope);
    }

    public void DeliverToClient(int connectionId, NetworkSyncEnvelope envelope) {
        if (clientTransports.TryGetValue(connectionId, out NetworkSyncLocalLoopbackClientTransport clientTransport)) {
            clientTransport.Receive(envelope);
        }
    }
}
