using System;

/// <summary>
/// 角色属性值结构。
/// 同时保存当前值和上限值，便于 UI 和逻辑统一处理。
/// </summary>
public struct RoleAttrValue : IEquatable<RoleAttrValue> {
    public int Current;
    public int Max;

    public RoleAttrValue(int current, int max) {
        Current = current;
        Max = max;
    }

    public bool Equals(RoleAttrValue other) {
        return Current == other.Current && Max == other.Max;
    }

    public override bool Equals(object obj) {
        if (!(obj is RoleAttrValue)) {
            return false;
        }

        return Equals((RoleAttrValue)obj);
    }

    public override int GetHashCode() {
        return (Current * 397) ^ Max;
    }

    public static bool operator ==(RoleAttrValue left, RoleAttrValue right) {
        return left.Equals(right);
    }

    public static bool operator !=(RoleAttrValue left, RoleAttrValue right) {
        return !left.Equals(right);
    }
}
