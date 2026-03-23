using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NetworkSyncConnectionRef {
    internal object NativeConnection;

    public int ConnectionId { get; private set; }
    public NetworkSyncConnectionMetadata Metadata { get; private set; }

    public NetworkSyncConnectionRef(int connectionId, object nativeConnection) {
        ConnectionId = connectionId;
        NativeConnection = nativeConnection;
        Metadata = new NetworkSyncConnectionMetadata();
    }
}

public sealed class NetworkSyncSession {
    public string SessionKey { get; set; }
    public NetworkSyncConnectionRef Connection { get; private set; }

    public NetworkSyncSession(NetworkSyncConnectionRef connection) {
        Connection = connection;
        SessionKey = "conn_" + connection.ConnectionId;
    }
}

public sealed class NetworkSyncServerContext {
    public NetworkSyncServer Network { get; private set; }
    public NetworkSyncConnectionRef Connection { get; private set; }
    public NetworkSyncSession Session { get; private set; }
    public NetworkSyncReplicationManager Replication { get; private set; }
    public int CurrentWorldId { get; internal set; }

    internal NetworkSyncServerContext(
        NetworkSyncServer network,
        NetworkSyncConnectionRef connection,
        NetworkSyncSession session,
        NetworkSyncReplicationManager replication) {
        Network = network;
        Connection = connection;
        Session = session;
        Replication = replication;
    }

    public void TargetRpc(INetworkSyncTargetRpc message) {
        Network.TargetRpc(Connection, message, CurrentWorldId);
    }

    public void Rpc(INetworkSyncRpc message, Predicate<NetworkSyncConnectionRef> filter = null) {
        Network.Rpc(message, CurrentWorldId, filter);
    }
}

public sealed class NetworkSyncServer {
    private sealed class ServerHandlerEntry {
        public NetworkSyncMessageDescriptor Descriptor;
        public Func<NetworkSyncServerContext, INetworkSyncMessage, NetworkSyncValidationResult> Validator;
        public Action<NetworkSyncServerContext, INetworkSyncMessage> Handler;
    }

    private readonly INetworkSyncServerTransport transport;
    private readonly INetworkSyncSerializer serializer;
    private readonly NetworkSyncRegistry registry;
    private readonly Dictionary<int, ServerHandlerEntry> handlers = new Dictionary<int, ServerHandlerEntry>();
    private readonly Dictionary<int, NetworkSyncSession> sessions = new Dictionary<int, NetworkSyncSession>();
    private readonly NetworkSyncReplicationManager replication = new NetworkSyncReplicationManager();

    public NetworkSyncServer(
        INetworkSyncServerTransport transport,
        INetworkSyncSerializer serializer = null,
        NetworkSyncRegistry registry = null) {
        this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
        this.serializer = serializer ?? new NetworkSyncJsonSerializer();
        this.registry = registry ?? new NetworkSyncRegistry();

        RegisterBuiltInMessages();
        this.transport.EnvelopeReceived += OnEnvelopeReceived;
        this.transport.Bind();
    }

    public NetworkSyncRegistry Registry {
        get {
            return registry;
        }
    }

    public NetworkSyncReplicationManager Replication {
        get {
            return replication;
        }
    }

    public void RegisterModule(INetworkSyncServerModule module) {
        module?.Register(this);
    }

    public void RegisterCmd<TMessage>(
        NetworkSyncMessageDescriptor descriptor,
        Action<NetworkSyncServerContext, TMessage> handler,
        Func<NetworkSyncServerContext, TMessage, NetworkSyncValidationResult> validator = null)
        where TMessage : class, INetworkSyncCmd {
        if (descriptor == null) {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (handler == null) {
            throw new ArgumentNullException(nameof(handler));
        }

        if (descriptor.ProtocolType != NetworkSyncProtocolType.Cmd) {
            throw new InvalidOperationException("Cmd descriptor required: " + descriptor.MessageType.Name);
        }

        registry.Register(descriptor);
        handlers[descriptor.MessageId] = new ServerHandlerEntry {
            Descriptor = descriptor,
            Validator = (context, message) => {
                if (validator == null) {
                    return NetworkSyncValidationResult.Ok();
                }
                return validator(context, (TMessage)message);
            },
            Handler = (context, message) => {
                handler(context, (TMessage)message);
            }
        };
    }

    public NetworkSyncSession GetOrCreateSession(NetworkSyncConnectionRef connection) {
        if (!sessions.TryGetValue(connection.ConnectionId, out NetworkSyncSession session)) {
            session = new NetworkSyncSession(connection);
            sessions.Add(connection.ConnectionId, session);
        }
        return session;
    }

    public void RemoveSession(NetworkSyncConnectionRef connection) {
        if (connection == null) {
            return;
        }
        sessions.Remove(connection.ConnectionId);
    }

    public void TargetRpc(NetworkSyncConnectionRef connection, INetworkSyncTargetRpc message, int worldId = 0) {
        if (connection == null || message == null) {
            return;
        }

        NetworkSyncMessageDescriptor descriptor = registry.GetByType(message.GetType());
        if (descriptor.ProtocolType != NetworkSyncProtocolType.TargetRpc) {
            throw new InvalidOperationException("TargetRpc descriptor required: " + descriptor.MessageType.Name);
        }

        SendEnvelope(connection, message, worldId, descriptor);
    }

    public void Rpc(INetworkSyncRpc message, int worldId = 0, Predicate<NetworkSyncConnectionRef> filter = null) {
        if (message == null) {
            return;
        }

        NetworkSyncMessageDescriptor descriptor = registry.GetByType(message.GetType());
        if (descriptor.ProtocolType != NetworkSyncProtocolType.Rpc) {
            throw new InvalidOperationException("Rpc descriptor required: " + descriptor.MessageType.Name);
        }

        NetworkSyncEnvelope envelope = new NetworkSyncEnvelope {
            worldId = worldId,
            messageId = descriptor.MessageId,
            payload = serializer.Serialize(message)
        };
        transport.Broadcast(envelope, descriptor.Delivery, filter);
    }

    private void SendMessage(NetworkSyncConnectionRef connection, INetworkSyncMessage message, int worldId = 0) {
        if (connection == null || message == null) {
            return;
        }

        NetworkSyncMessageDescriptor descriptor = registry.GetByType(message.GetType());
        SendEnvelope(connection, message, worldId, descriptor);
    }

    private void SendEnvelope(
        NetworkSyncConnectionRef connection,
        INetworkSyncMessage message,
        int worldId,
        NetworkSyncMessageDescriptor descriptor) {
        NetworkSyncEnvelope envelope = new NetworkSyncEnvelope {
            worldId = worldId,
            messageId = descriptor.MessageId,
            payload = serializer.Serialize(message)
        };
        transport.Send(connection, envelope, descriptor.Delivery);
    }

    public void SendFullSnapshot(
        NetworkSyncConnectionRef connection,
        int worldId = 0,
        Predicate<NetworkSyncReplicatedEntityState> filter = null) {
        NetworkSyncSpawnSnapshotMessage snapshot = replication.BuildFullSnapshot(filter);
        if (snapshot.entities.Count > 0) {
            SendMessage(connection, snapshot, worldId);
        }

        SendMessage(connection, new NetworkSyncWorldSnapshotCompleteMessage {
            snapshotVersion = replication.SnapshotVersion
        }, worldId);
    }

    public void FlushDelta(
        int worldId = 0,
        Predicate<NetworkSyncConnectionRef> connectionFilter = null,
        Predicate<NetworkSyncReplicatedEntityState> stateFilter = null) {
        NetworkSyncStateDeltaMessage delta = replication.BuildDelta(stateFilter);
        if (delta.entities.Count <= 0) {
            return;
        }

        NetworkSyncMessageDescriptor descriptor = registry.GetByType(typeof(NetworkSyncStateDeltaMessage));
        NetworkSyncEnvelope envelope = new NetworkSyncEnvelope {
            worldId = worldId,
            messageId = descriptor.MessageId,
            payload = serializer.Serialize(delta)
        };
        transport.Broadcast(envelope, descriptor.Delivery, connectionFilter);
        replication.ClearDirty();
    }

    private void RegisterBuiltInMessages() {
        registry.Register(NetworkSyncMessageDescriptor.Create<NetworkSyncSpawnSnapshotMessage>(
            NetworkSyncBuiltInMessageIds.SpawnSnapshot,
            "NetworkSync",
            NetworkSyncMessageKind.Snapshot,
            NetworkSyncProtocolType.Snapshot,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.ConnectionList,
            NetworkSyncAuthority.ServerOnly));

        registry.Register(NetworkSyncMessageDescriptor.Create<NetworkSyncStateDeltaMessage>(
            NetworkSyncBuiltInMessageIds.StateDelta,
            "NetworkSync",
            NetworkSyncMessageKind.Delta,
            NetworkSyncProtocolType.Delta,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.ConnectionList,
            NetworkSyncAuthority.ServerOnly));

        registry.Register(NetworkSyncMessageDescriptor.Create<NetworkSyncDespawnMessage>(
            NetworkSyncBuiltInMessageIds.Despawn,
            "NetworkSync",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.Rpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.ConnectionList,
            NetworkSyncAuthority.ServerOnly));

        registry.Register(NetworkSyncMessageDescriptor.Create<NetworkSyncWorldSnapshotCompleteMessage>(
            NetworkSyncBuiltInMessageIds.WorldSnapshotComplete,
            "NetworkSync",
            NetworkSyncMessageKind.Event,
            NetworkSyncProtocolType.TargetRpc,
            NetworkSyncDirection.ServerToClient,
            NetworkSyncDelivery.Reliable,
            NetworkSyncTarget.ConnectionList,
            NetworkSyncAuthority.ServerOnly));
    }

    private void OnEnvelopeReceived(NetworkSyncConnectionRef connection, NetworkSyncEnvelope envelope) {
        if (!registry.TryGetById(envelope.messageId, out NetworkSyncMessageDescriptor descriptor)) {
            Debug.LogError("NetworkSyncServer received unknown message id: " + envelope.messageId);
            return;
        }

        if (!handlers.TryGetValue(envelope.messageId, out ServerHandlerEntry handlerEntry)) {
            Debug.LogWarning("NetworkSyncServer received unhandled message: " + descriptor.MessageType.Name);
            return;
        }

        if (descriptor.ProtocolType != NetworkSyncProtocolType.Cmd) {
            Debug.LogError("NetworkSyncServer only accepts cmd messages from clients: " + descriptor.MessageType.Name);
            return;
        }

        NetworkSyncSession session = GetOrCreateSession(connection);
        NetworkSyncServerContext context = new NetworkSyncServerContext(this, connection, session, replication);
        context.CurrentWorldId = envelope.worldId;

        try {
            INetworkSyncMessage message = serializer.Deserialize(envelope.payload, descriptor.MessageType);
            NetworkSyncValidationResult validation = handlerEntry.Validator != null
                ? handlerEntry.Validator(context, message)
                : NetworkSyncValidationResult.Ok();

            if (!validation.IsValid) {
                Debug.LogWarning(
                    "NetworkSyncServer validation failed. Message: " + descriptor.MessageType.Name +
                    ", ErrorCode: " + validation.ErrorCode +
                    ", Message: " + validation.ErrorMessage);
                return;
            }

            handlerEntry.Handler(context, message);
        } catch (Exception exception) {
            Debug.LogError("NetworkSyncServer failed to process message: " + descriptor.MessageType.Name + "\n" + exception);
        }
    }
}
