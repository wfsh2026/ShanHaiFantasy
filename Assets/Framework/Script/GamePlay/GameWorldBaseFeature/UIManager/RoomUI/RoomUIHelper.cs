using UnityEngine;

/// <summary>
/// 房间 UI 的轻量公共工具。
/// 负责把头像标识和参与者类型转换为可显示内容。
/// </summary>
public static class RoomUIHelper {
    public static Color GetAvatarColor(string avatarId) {
        if (string.IsNullOrEmpty(avatarId)) {
            return new Color(0.45f, 0.45f, 0.45f, 1f);
        }

        int hash = avatarId.GetHashCode();
        float red = 0.35f + (Mathf.Abs(hash % 100) / 200f);
        float green = 0.35f + (Mathf.Abs((hash / 10) % 100) / 200f);
        float blue = 0.35f + (Mathf.Abs((hash / 100) % 100) / 200f);
        return new Color(red, green, blue, 1f);
    }

    public static string GetParticipantTypeText(NetworkSyncRoomSlotData slotData) {
        if (slotData == null) {
            return "空";
        }

        switch (slotData.participantType) {
            case NetworkSyncRoomParticipantType.Host:
                return "房主";
            case NetworkSyncRoomParticipantType.Player:
                return "玩家";
            case NetworkSyncRoomParticipantType.AI:
                return "AI";
        }

        return "未知";
    }
}
