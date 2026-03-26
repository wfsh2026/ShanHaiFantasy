using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 客户端上下文。
/// 处理回调时统一提供当前 worldId 和客户端世界快照。
/// </summary>
public sealed class NetworkSyncClientContext {
    public NetworkSyncClient Network { get; private set; }
    public NetworkSyncClientWorldState WorldState { get; private set; }
    public int CurrentWorldId { get; internal set; }

    internal NetworkSyncClientContext(NetworkSyncClient network, NetworkSyncClientWorldState worldState) {
        Network = network;
        WorldState = worldState;
    }
}

/// <summary>
/// NetworkSync 客户端入口。
/// 负责注册客户端消息处理器、发送 Cmd、维护客户端世界快照。
/// </summary>
public sealed class NetworkSyncClient {
    private sealed class ClientHandlerEntry {
        public NetworkSyncMessageDescriptor Descriptor;
        public Action<NetworkSyncClientContext, INetworkSyncMessage> Handler;
    }

    private readonly INetworkSyncClientTransport transport;
    private readonly INetworkSyncSerializer serializer;
    private readonly NetworkSyncRegistry registry;
    private readonly Dictionary<int, ClientHandlerEntry> handlers = new Dictionary<int, ClientHandlerEntry>();
    private readonly NetworkSyncClientWorldState worldState = new NetworkSyncClientWorldState();
    private readonly NetworkSyncClientContext context;
    private bool isDisposed;

    public NetworkSyncClient(
        INetworkSyncClientTransport transport,
        INetworkSyncSerializer serializer = null,
        NetworkSyncRegistry registry = null) {
        this.transport = transport ?? throw new ArgumentNullException(nameof(transport));
        this.serializer = serializer ?? new NetworkSyncJsonSerializer();
        this.registry = registry ?? new NetworkSyncRegistry();
        context = new NetworkSyncClientContext(this, worldState);

        RegisterBuiltInHandlers();
        this.transport.EnvelopeReceived += OnEnvelopeReceived;
        this.transport.Bind();
    }

    public NetworkSyncRegistry Registry {
        get {
            return registry;
        }
    }

    public NetworkSyncClientWorldState WorldState {
        get {
            return worldState;
        }
    }

    public void RegisterModule(INetworkSyncClientModule module) {
        module?.Register(this);
    }

    /// <summary>
    /// 释放客户端绑定。
    /// 主要用于本地测试链和显式生命周期收口。
    /// </summary>
    public void Dispose() {
        if (isDisposed) {
            return;
        }

        transport.EnvelopeReceived -= OnEnvelopeReceived;
        transport.Unbind();
        handlers.Clear();
        isDisposed = true;
    }

    public void RegisterRpc<TMessage>(
        NetworkSyncMessageDescriptor descriptor,
        Action<NetworkSyncClientContext, TMessage> handler) where TMessage : class, INetworkSyncRpc {
        if (descriptor.ProtocolType != NetworkSyncProtocolType.Rpc) {
            throw new InvalidOperationException("Rpc descriptor required: " + descriptor.MessageType.Name);
        }

        RegisterClientHandler(descriptor, handler);
    }

    public void RegisterTargetRpc<TMessage>(
        NetworkSyncMessageDescriptor descriptor,
        Action<NetworkSyncClientContext, TMessage> handler) where TMessage : class, INetworkSyncTargetRpc {
        if (descriptor.ProtocolType != NetworkSyncProtocolType.TargetRpc) {
            throw new InvalidOperationException("TargetRpc descriptor required: " + descriptor.MessageType.Name);
        }

        RegisterClientHandler(descriptor, handler);
    }

    public void RegisterSnapshot<TMessage>(
        NetworkSyncMessageDescriptor descriptor,
        Action<NetworkSyncClientContext, TMessage> handler) where TMessage : class, INetworkSyncSnapshot {
        if (descriptor.ProtocolType != NetworkSyncProtocolType.Snapshot) {
            throw new InvalidOperationException("Snapshot descriptor required: " + descriptor.MessageType.Name);
        }

        RegisterClientHandler(descriptor, handler);
    }

    public void RegisterDelta<TMessage>(
        NetworkSyncMessageDescriptor descriptor,
        Action<NetworkSyncClientContext, TMessage> handler) where TMessage : class, INetworkSyncDelta {
        if (descriptor.ProtocolType != NetworkSyncProtocolType.Delta) {
            throw new InvalidOperationException("Delta descriptor required: " + descriptor.MessageType.Name);
        }

        RegisterClientHandler(descriptor, handler);
    }

    public void SendCmd<TMessage>(TMessage message, int worldId = 0) where TMessage : class, INetworkSyncCmd {
        if (message == null) {
            throw new ArgumentNullException(nameof(message));
        }

        NetworkSyncMessageDescriptor descriptor = registry.GetByType(typeof(TMessage));
        if (descriptor.ProtocolType != NetworkSyncProtocolType.Cmd) {
            throw new InvalidOperationException("Only cmd messages can be sent from the client: " + typeof(TMessage).Name);
        }

        NetworkSyncEnvelope envelope = new NetworkSyncEnvelope {
            worldId = worldId,
            messageId = descriptor.MessageId,
            payload = serializer.Serialize(message)
        };
        transport.Send(envelope, descriptor.Delivery);
    }

    private void RegisterClientHandler<TMessage>(
        NetworkSyncMessageDescriptor descriptor,
        Action<NetworkSyncClientContext, TMessage> handler) where TMessage : class, INetworkSyncMessage {
        if (descriptor == null) {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (handler == null) {
            throw new ArgumentNullException(nameof(handler));
        }

        registry.Register(descriptor);
        handlers[descriptor.MessageId] = new ClientHandlerEntry {
            Descriptor = descriptor,
            Handler = (clientContext, message) => {
                handler(clientContext, (TMessage)message);
            }
        };
    }

    private void RegisterBuiltInHandlers() {
        RegisterSnapshot<NetworkSyncSpawnSnapshotMessage>(
            NetworkSyncMessageDescriptor.Create<NetworkSyncSpawnSnapshotMessage>(
                NetworkSyncBuiltInMessageIds.SpawnSnapshot,
                "NetworkSync",
                NetworkSyncMessageKind.Snapshot,
                NetworkSyncProtocolType.Snapshot,
                NetworkSyncDirection.ServerToClient,
                NetworkSyncDelivery.Reliable,
                NetworkSyncTarget.ConnectionList,
                NetworkSyncAuthority.ServerOnly),
            (clientContext, message) => {
                clientContext.WorldState.ApplySpawn(message);
            });

        RegisterDelta<NetworkSyncStateDeltaMessage>(
            NetworkSyncMessageDescriptor.Create<NetworkSyncStateDeltaMessage>(
                NetworkSyncBuiltInMessageIds.StateDelta,
                "NetworkSync",
                NetworkSyncMessageKind.Delta,
                NetworkSyncProtocolType.Delta,
                NetworkSyncDirection.ServerToClient,
                NetworkSyncDelivery.Reliable,
                NetworkSyncTarget.ConnectionList,
                NetworkSyncAuthority.ServerOnly),
            (clientContext, message) => {
                clientContext.WorldState.ApplyDelta(message);
            });

        RegisterRpc<NetworkSyncDespawnMessage>(
            NetworkSyncMessageDescriptor.Create<NetworkSyncDespawnMessage>(
                NetworkSyncBuiltInMessageIds.Despawn,
                "NetworkSync",
                NetworkSyncMessageKind.Event,
                NetworkSyncProtocolType.Rpc,
                NetworkSyncDirection.ServerToClient,
                NetworkSyncDelivery.Reliable,
                NetworkSyncTarget.ConnectionList,
                NetworkSyncAuthority.ServerOnly),
            (clientContext, message) => {
                clientContext.WorldState.ApplyDespawn(message);
            });

        RegisterTargetRpc<NetworkSyncWorldSnapshotCompleteMessage>(
            NetworkSyncMessageDescriptor.Create<NetworkSyncWorldSnapshotCompleteMessage>(
                NetworkSyncBuiltInMessageIds.WorldSnapshotComplete,
                "NetworkSync",
                NetworkSyncMessageKind.Event,
                NetworkSyncProtocolType.TargetRpc,
                NetworkSyncDirection.ServerToClient,
                NetworkSyncDelivery.Reliable,
                NetworkSyncTarget.ConnectionList,
                NetworkSyncAuthority.ServerOnly),
            (clientContext, message) => {
                clientContext.WorldState.MarkSnapshotReady(message.snapshotVersion);
            });
    }

    private void OnEnvelopeReceived(NetworkSyncEnvelope envelope) {
        if (isDisposed) {
            return;
        }

        context.CurrentWorldId = envelope.worldId;

        if (!registry.TryGetById(envelope.messageId, out NetworkSyncMessageDescriptor descriptor)) {
            Debug.LogError("NetworkSyncClient received unknown message id: " + envelope.messageId);
            return;
        }

        if (!handlers.TryGetValue(envelope.messageId, out ClientHandlerEntry handlerEntry)) {
            Debug.LogWarning("NetworkSyncClient received unhandled message: " + descriptor.MessageType.Name);
            return;
        }

        if (descriptor.ProtocolType == NetworkSyncProtocolType.Cmd) {
            Debug.LogError("NetworkSyncClient rejected cmd message: " + descriptor.MessageType.Name);
            return;
        }

        try {
            INetworkSyncMessage message = serializer.Deserialize(envelope.payload, descriptor.MessageType);
            handlerEntry.Handler(context, message);
        } catch (Exception exception) {
            Debug.LogError("NetworkSyncClient failed to deserialize message: " + descriptor.MessageType.Name + "\n" + exception);
        }
    }
}
