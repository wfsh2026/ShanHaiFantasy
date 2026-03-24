using System;

public sealed class RoleAttrUIService {
    private readonly GameWorld gameWorld;

    public RoleAttrUIService(GameWorld world) {
        gameWorld = world;
    }

    public void AddDataListener(Action listener) {
        ClientTestModeData data = GetModeData();
        if (data == null || listener == null) {
            return;
        }

        data.DataChanged -= listener;
        data.DataChanged += listener;
    }

    public void RemoveDataListener(Action listener) {
        ClientTestModeData data = GetModeData();
        if (data == null || listener == null) {
            return;
        }

        data.DataChanged -= listener;
    }

    public ClientTestModeData GetModeData() {
        ClientTestModeManager modeManager = GetModeManager();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetData<ClientTestModeData>();
    }

    public void RequestAttrChange(RoleAttrType attrType, RoleAttrOperationType operationType, int value) {
        ClientTestModeLogic logic = GetModeLogic();
        if (logic == null || value <= 0) {
            return;
        }

        switch (attrType) {
            case RoleAttrType.HP:
                if (operationType == RoleAttrOperationType.Add) {
                    logic.AddHP(value);
                } else {
                    logic.ReduceHP(value);
                }
                break;
            case RoleAttrType.MP:
                if (operationType == RoleAttrOperationType.Add) {
                    logic.AddMP(value);
                } else {
                    logic.ReduceMP(value);
                }
                break;
        }
    }

    public void RequestNextStage() {
        ClientTestModeManager modeManager = GetModeManager();
        if (modeManager != null) {
            modeManager.NextStage();
        }
    }

    public string GetOperationText(RoleAttrType attrType, RoleAttrOperationType operationType) {
        string attrName = attrType == RoleAttrType.HP ? "HP" : "MP";
        string actionName = operationType == RoleAttrOperationType.Add ? "增加" : "减少";
        return actionName + " " + attrName;
    }

    private ClientTestModeManager GetModeManager() {
        if (gameWorld == null) {
            return null;
        }

        return gameWorld.GetExtendFeature<ClientTestModeManager>();
    }

    private ClientTestModeLogic GetModeLogic() {
        ClientTestModeManager modeManager = GetModeManager();
        if (modeManager == null) {
            return null;
        }

        return modeManager.GetLogic<ClientTestModeLogic>();
    }
}
