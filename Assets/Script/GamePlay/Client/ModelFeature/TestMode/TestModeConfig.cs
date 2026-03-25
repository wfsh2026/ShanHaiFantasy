using UnityEngine;

[CreateAssetMenu(fileName = "TestModeConfig", menuName = "ShanHaiFantasy/Config/TestModeConfig")]
public sealed class TestModeConfig : ScriptableObject {
    [SerializeField] private string roleName = "Test Hero";
    [SerializeField] private string defaultStageName = "ClientTestModeStage";
    [SerializeField] private int maxHP = 100;
    [SerializeField] private int maxMP = 60;
    [SerializeField] private float autoChangeInterval = 2f;
    [SerializeField] private int drainHPDelta = -8;
    [SerializeField] private int drainMPDelta = -5;
    [SerializeField] private int recoverHPDelta = 6;
    [SerializeField] private int recoverMPDelta = 4;

    public string RoleName {
        get {
            return roleName;
        }
    }

    public string DefaultStageName {
        get {
            return defaultStageName;
        }
    }

    public int MaxHP {
        get {
            return maxHP;
        }
    }

    public int MaxMP {
        get {
            return maxMP;
        }
    }

    public float AutoChangeInterval {
        get {
            return autoChangeInterval;
        }
    }

    public int DrainHPDelta {
        get {
            return drainHPDelta;
        }
    }

    public int DrainMPDelta {
        get {
            return drainMPDelta;
        }
    }

    public int RecoverHPDelta {
        get {
            return recoverHPDelta;
        }
    }

    public int RecoverMPDelta {
        get {
            return recoverMPDelta;
        }
    }
}
