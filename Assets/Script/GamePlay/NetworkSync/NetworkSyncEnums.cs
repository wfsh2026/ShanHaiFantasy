using System;

public enum NetworkSyncMessageKind {
    Command = 0,
    Event = 1,
    Snapshot = 2,
    Delta = 3
}

public enum NetworkSyncProtocolType {
    None = 0,
    Cmd = 1,
    Rpc = 2,
    TargetRpc = 3,
    Snapshot = 4,
    Delta = 5
}

public enum NetworkSyncDirection {
    ClientToServer = 0,
    ServerToClient = 1
}

public enum NetworkSyncDelivery {
    Reliable = 0,
    Unreliable = 1
}

public enum NetworkSyncTarget {
    None = 0,
    Owner = 1,
    Broadcast = 2,
    Observers = 3,
    ConnectionList = 4
}

public enum NetworkSyncAuthority {
    None = 0,
    AnyClient = 1,
    OwnerOnly = 2,
    HostOnly = 3,
    ServerOnly = 4
}

[Serializable]
public struct NetworkSyncValidationResult {
    public bool IsValid;
    public string ErrorCode;
    public string ErrorMessage;

    public static NetworkSyncValidationResult Ok() {
        return new NetworkSyncValidationResult {
            IsValid = true,
            ErrorCode = string.Empty,
            ErrorMessage = string.Empty
        };
    }

    public static NetworkSyncValidationResult Fail(string errorCode, string errorMessage) {
        return new NetworkSyncValidationResult {
            IsValid = false,
            ErrorCode = errorCode ?? string.Empty,
            ErrorMessage = errorMessage ?? string.Empty
        };
    }
}
