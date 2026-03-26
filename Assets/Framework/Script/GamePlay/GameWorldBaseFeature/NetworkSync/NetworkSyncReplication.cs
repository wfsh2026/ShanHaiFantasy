using System;
using System.Collections.Generic;

public static class NetworkSyncBuiltInMessageIds {
    public const int SpawnSnapshot = 1;
    public const int StateDelta = 2;
    public const int Despawn = 3;
    public const int WorldSnapshotComplete = 4;
}

[Serializable]
public sealed class NetworkSyncReplicatedFieldState {
    public string fieldId;
    public string payloadJson;
    public int version;
    public int sortOrder;
    public NetworkSyncDelivery delivery = NetworkSyncDelivery.Reliable;
}

[Serializable]
public sealed class NetworkSyncReplicatedEntityState {
    public int entityId;
    public string entityType;
    public int ownerConnectionId = -1;
    public int stateVersion;
    public List<NetworkSyncReplicatedFieldState> fields = new List<NetworkSyncReplicatedFieldState>();
}

[Serializable]
public sealed class NetworkSyncSpawnSnapshotMessage : INetworkSyncSnapshot {
    public List<NetworkSyncReplicatedEntityState> entities = new List<NetworkSyncReplicatedEntityState>();
}

[Serializable]
public sealed class NetworkSyncStateDeltaMessage : INetworkSyncDelta {
    public List<NetworkSyncReplicatedEntityState> entities = new List<NetworkSyncReplicatedEntityState>();
}

[Serializable]
public sealed class NetworkSyncDespawnMessage : INetworkSyncRpc {
    public List<int> entityIds = new List<int>();
}

[Serializable]
public sealed class NetworkSyncWorldSnapshotCompleteMessage : INetworkSyncTargetRpc {
    public int snapshotVersion;
}

public sealed class NetworkSyncClientWorldState {
    private readonly Dictionary<int, NetworkSyncReplicatedEntityState> entities =
        new Dictionary<int, NetworkSyncReplicatedEntityState>();

    public bool IsSnapshotReady { get; private set; }
    public int LastSnapshotVersion { get; private set; }

    public void ApplySpawn(NetworkSyncSpawnSnapshotMessage message) {
        if (message == null || message.entities == null) {
            return;
        }

        for (int i = 0; i < message.entities.Count; i++) {
            NetworkSyncReplicatedEntityState state = CloneEntity(message.entities[i]);
            entities[state.entityId] = state;
        }
    }

    public void ApplyDelta(NetworkSyncStateDeltaMessage message) {
        if (message == null || message.entities == null) {
            return;
        }

        for (int i = 0; i < message.entities.Count; i++) {
            NetworkSyncReplicatedEntityState incoming = message.entities[i];
            if (!entities.TryGetValue(incoming.entityId, out NetworkSyncReplicatedEntityState local)) {
                entities[incoming.entityId] = CloneEntity(incoming);
                continue;
            }

            if (incoming.stateVersion < local.stateVersion) {
                continue;
            }

            local.entityType = incoming.entityType;
            local.ownerConnectionId = incoming.ownerConnectionId;
            local.stateVersion = incoming.stateVersion;
            MergeFields(local.fields, incoming.fields);
        }
    }

    public void ApplyDespawn(NetworkSyncDespawnMessage message) {
        if (message == null || message.entityIds == null) {
            return;
        }

        for (int i = 0; i < message.entityIds.Count; i++) {
            entities.Remove(message.entityIds[i]);
        }
    }

    public void MarkSnapshotReady(int snapshotVersion) {
        LastSnapshotVersion = snapshotVersion;
        IsSnapshotReady = true;
    }

    public bool TryGetEntity(int entityId, out NetworkSyncReplicatedEntityState entityState) {
        return entities.TryGetValue(entityId, out entityState);
    }

    private static void MergeFields(List<NetworkSyncReplicatedFieldState> localFields, List<NetworkSyncReplicatedFieldState> incomingFields) {
        if (incomingFields == null) {
            return;
        }

        for (int i = 0; i < incomingFields.Count; i++) {
            NetworkSyncReplicatedFieldState incoming = incomingFields[i];
            NetworkSyncReplicatedFieldState existed = FindField(localFields, incoming.fieldId);
            if (existed == null) {
                localFields.Add(CloneField(incoming));
                continue;
            }

            if (incoming.version < existed.version) {
                continue;
            }

            existed.payloadJson = incoming.payloadJson;
            existed.version = incoming.version;
            existed.sortOrder = incoming.sortOrder;
            existed.delivery = incoming.delivery;
        }
    }

    private static NetworkSyncReplicatedFieldState FindField(List<NetworkSyncReplicatedFieldState> fields, string fieldId) {
        for (int i = 0; i < fields.Count; i++) {
            if (fields[i].fieldId == fieldId) {
                return fields[i];
            }
        }
        return null;
    }

    internal static NetworkSyncReplicatedEntityState CloneEntity(NetworkSyncReplicatedEntityState source) {
        NetworkSyncReplicatedEntityState entity = new NetworkSyncReplicatedEntityState();
        entity.entityId = source.entityId;
        entity.entityType = source.entityType;
        entity.ownerConnectionId = source.ownerConnectionId;
        entity.stateVersion = source.stateVersion;
        entity.fields = new List<NetworkSyncReplicatedFieldState>(source.fields.Count);

        for (int i = 0; i < source.fields.Count; i++) {
            entity.fields.Add(CloneField(source.fields[i]));
        }
        return entity;
    }

    internal static NetworkSyncReplicatedFieldState CloneField(NetworkSyncReplicatedFieldState source) {
        return new NetworkSyncReplicatedFieldState {
            fieldId = source.fieldId,
            payloadJson = source.payloadJson,
            version = source.version,
            sortOrder = source.sortOrder,
            delivery = source.delivery
        };
    }
}

public sealed class NetworkSyncReplicationManager {
    private readonly Dictionary<int, NetworkSyncReplicatedEntityState> entities =
        new Dictionary<int, NetworkSyncReplicatedEntityState>();

    private readonly HashSet<int> dirtyEntityIds = new HashSet<int>();

    public int SnapshotVersion { get; private set; }

    public void UpsertEntity(int entityId, string entityType, int ownerConnectionId) {
        if (!entities.TryGetValue(entityId, out NetworkSyncReplicatedEntityState entity)) {
            entity = new NetworkSyncReplicatedEntityState();
            entity.entityId = entityId;
            entity.fields = new List<NetworkSyncReplicatedFieldState>();
            entities.Add(entityId, entity);
        }

        entity.entityType = entityType ?? string.Empty;
        entity.ownerConnectionId = ownerConnectionId;
        entity.stateVersion = Math.Max(entity.stateVersion, 1);
        dirtyEntityIds.Add(entityId);
    }

    public void RemoveEntity(int entityId) {
        entities.Remove(entityId);
        dirtyEntityIds.Remove(entityId);
    }

    public void SetFieldJson(
        int entityId,
        string entityType,
        string fieldId,
        string payloadJson,
        NetworkSyncDelivery delivery,
        int ownerConnectionId = -1,
        int sortOrder = 0) {
        UpsertEntity(entityId, entityType, ownerConnectionId);

        NetworkSyncReplicatedEntityState entity = entities[entityId];
        NetworkSyncReplicatedFieldState field = FindField(entity.fields, fieldId);
        if (field == null) {
            field = new NetworkSyncReplicatedFieldState();
            field.fieldId = fieldId;
            entity.fields.Add(field);
        }

        field.payloadJson = payloadJson ?? string.Empty;
        field.delivery = delivery;
        field.sortOrder = sortOrder;
        field.version++;
        entity.stateVersion++;
        dirtyEntityIds.Add(entityId);
    }

    public NetworkSyncSpawnSnapshotMessage BuildFullSnapshot(Predicate<NetworkSyncReplicatedEntityState> filter = null) {
        NetworkSyncSpawnSnapshotMessage message = new NetworkSyncSpawnSnapshotMessage();
        foreach (KeyValuePair<int, NetworkSyncReplicatedEntityState> pair in entities) {
            if (filter != null && !filter(pair.Value)) {
                continue;
            }
            message.entities.Add(NetworkSyncClientWorldState.CloneEntity(pair.Value));
        }
        SnapshotVersion++;
        return message;
    }

    public NetworkSyncStateDeltaMessage BuildDelta(Predicate<NetworkSyncReplicatedEntityState> filter = null) {
        NetworkSyncStateDeltaMessage message = new NetworkSyncStateDeltaMessage();
        foreach (int entityId in dirtyEntityIds) {
            if (!entities.TryGetValue(entityId, out NetworkSyncReplicatedEntityState entity)) {
                continue;
            }

            if (filter != null && !filter(entity)) {
                continue;
            }

            message.entities.Add(NetworkSyncClientWorldState.CloneEntity(entity));
        }
        return message;
    }

    public void ClearDirty() {
        dirtyEntityIds.Clear();
    }

    public NetworkSyncDespawnMessage CreateDespawnMessage(params int[] entityIds) {
        NetworkSyncDespawnMessage message = new NetworkSyncDespawnMessage();
        if (entityIds == null) {
            return message;
        }

        for (int i = 0; i < entityIds.Length; i++) {
            message.entityIds.Add(entityIds[i]);
        }
        return message;
    }

    private static NetworkSyncReplicatedFieldState FindField(List<NetworkSyncReplicatedFieldState> fields, string fieldId) {
        for (int i = 0; i < fields.Count; i++) {
            if (fields[i].fieldId == fieldId) {
                return fields[i];
            }
        }
        return null;
    }
}
