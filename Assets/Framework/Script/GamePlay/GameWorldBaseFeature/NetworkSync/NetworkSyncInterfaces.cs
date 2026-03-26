using System;
using System.Collections.Generic;

public interface INetworkSyncMessage {
}

public interface INetworkSyncCommand : INetworkSyncMessage {
}

public interface INetworkSyncCmd : INetworkSyncCommand {
}

public interface INetworkSyncEvent : INetworkSyncMessage {
}

public interface INetworkSyncRpc : INetworkSyncEvent {
}

public interface INetworkSyncTargetRpc : INetworkSyncEvent {
}

public interface INetworkSyncSnapshot : INetworkSyncMessage {
}

public interface INetworkSyncDelta : INetworkSyncMessage {
}

public interface INetworkSyncSerializer {
    byte[] Serialize(INetworkSyncMessage message);
    INetworkSyncMessage Deserialize(byte[] payload, Type messageType);
}

public interface INetworkSyncClientTransport {
    event Action<NetworkSyncEnvelope> EnvelopeReceived;
    bool IsConnected { get; }
    void Bind();
    void Unbind();
    void Send(NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery);
}

public interface INetworkSyncServerTransport {
    event Action<NetworkSyncConnectionRef, NetworkSyncEnvelope> EnvelopeReceived;
    void Bind();
    void Unbind();
    void Send(NetworkSyncConnectionRef connection, NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery);
    void Broadcast(NetworkSyncEnvelope envelope, NetworkSyncDelivery delivery, Predicate<NetworkSyncConnectionRef> filter);
}

public interface INetworkSyncClientModule {
    void Register(NetworkSyncClient client);
}

public interface INetworkSyncServerModule {
    void Register(NetworkSyncServer server);
}

public interface INetworkSyncValidator {
    NetworkSyncValidationResult Validate(NetworkSyncServerContext context, INetworkSyncMessage message);
}

public sealed class NetworkSyncConnectionMetadata {
    private readonly Dictionary<string, string> values = new Dictionary<string, string>();

    public void Set(string key, string value) {
        if (string.IsNullOrWhiteSpace(key)) {
            return;
        }

        values[key] = value ?? string.Empty;
    }

    public bool TryGet(string key, out string value) {
        return values.TryGetValue(key, out value);
    }

    public IReadOnlyDictionary<string, string> Values {
        get {
            return values;
        }
    }
}
