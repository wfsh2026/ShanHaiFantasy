using System;
using System.Collections.Generic;

/// <summary>
/// NetworkSync 消息描述。
/// 用于约束 messageId、消息方向、投递方式和协议类型。
/// </summary>
public sealed class NetworkSyncMessageDescriptor {
    public int MessageId { get; private set; }
    public Type MessageType { get; private set; }
    public string ModuleName { get; private set; }
    public string ValidatorName { get; private set; }
    public NetworkSyncMessageKind MessageKind { get; private set; }
    public NetworkSyncProtocolType ProtocolType { get; private set; }
    public NetworkSyncDirection Direction { get; private set; }
    public NetworkSyncDelivery Delivery { get; private set; }
    public NetworkSyncTarget Target { get; private set; }
    public NetworkSyncAuthority Authority { get; private set; }

    private NetworkSyncMessageDescriptor() {
    }

    public static NetworkSyncMessageDescriptor Create<TMessage>(
        int messageId,
        string moduleName,
        NetworkSyncMessageKind messageKind,
        NetworkSyncProtocolType protocolType,
        NetworkSyncDirection direction,
        NetworkSyncDelivery delivery,
        NetworkSyncTarget target = NetworkSyncTarget.None,
        NetworkSyncAuthority authority = NetworkSyncAuthority.None,
        string validatorName = "") where TMessage : INetworkSyncMessage {
        return new NetworkSyncMessageDescriptor {
            MessageId = messageId,
            MessageType = typeof(TMessage),
            ModuleName = moduleName ?? string.Empty,
            MessageKind = messageKind,
            ProtocolType = protocolType,
            Direction = direction,
            Delivery = delivery,
            Target = target,
            Authority = authority,
            ValidatorName = validatorName ?? string.Empty
        };
    }
}

/// <summary>
/// NetworkSync 消息注册表。
/// 统一维护消息类型与消息 id 的映射关系。
/// </summary>
public sealed class NetworkSyncRegistry {
    private readonly Dictionary<int, NetworkSyncMessageDescriptor> descriptorsById =
        new Dictionary<int, NetworkSyncMessageDescriptor>();

    private readonly Dictionary<Type, NetworkSyncMessageDescriptor> descriptorsByType =
        new Dictionary<Type, NetworkSyncMessageDescriptor>();

    public void Register(NetworkSyncMessageDescriptor descriptor) {
        if (descriptor == null) {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (descriptorsById.TryGetValue(descriptor.MessageId, out NetworkSyncMessageDescriptor existedById)) {
            if (existedById.MessageType != descriptor.MessageType) {
                throw new InvalidOperationException(
                    "NetworkSync message id conflict: " + descriptor.MessageId +
                    ", existed type: " + existedById.MessageType.Name +
                    ", new type: " + descriptor.MessageType.Name);
            }
            return;
        }

        if (descriptorsByType.TryGetValue(descriptor.MessageType, out NetworkSyncMessageDescriptor existedByType)) {
            if (existedByType.MessageId != descriptor.MessageId) {
                throw new InvalidOperationException(
                    "NetworkSync message type conflict: " + descriptor.MessageType.Name +
                    ", existed id: " + existedByType.MessageId +
                    ", new id: " + descriptor.MessageId);
            }
            return;
        }

        descriptorsById.Add(descriptor.MessageId, descriptor);
        descriptorsByType.Add(descriptor.MessageType, descriptor);
    }

    public bool TryGetById(int messageId, out NetworkSyncMessageDescriptor descriptor) {
        return descriptorsById.TryGetValue(messageId, out descriptor);
    }

    public NetworkSyncMessageDescriptor GetById(int messageId) {
        if (TryGetById(messageId, out NetworkSyncMessageDescriptor descriptor)) {
            return descriptor;
        }

        throw new KeyNotFoundException("NetworkSync descriptor missing for message id: " + messageId);
    }

    public bool TryGetByType(Type messageType, out NetworkSyncMessageDescriptor descriptor) {
        return descriptorsByType.TryGetValue(messageType, out descriptor);
    }

    public NetworkSyncMessageDescriptor GetByType(Type messageType) {
        if (TryGetByType(messageType, out NetworkSyncMessageDescriptor descriptor)) {
            return descriptor;
        }

        throw new KeyNotFoundException("NetworkSync descriptor missing for message type: " + messageType.Name);
    }
}
