using System.Collections.Generic;
using UnityEngine;

public sealed class InputMap {
    private readonly List<InputBinding> bindings = new List<InputBinding>(24);

    public InputMap() {
        RegisterDefaultBindings();
    }

    public List<InputBinding> GetBindings() {
        return bindings;
    }

    private void RegisterDefaultBindings() {
        bindings.Add(InputBinding.CreateKey(InputActionId.Cancel, KeyCode.Escape));
        bindings.Add(InputBinding.CreateKey(InputActionId.Confirm, KeyCode.Return, KeyCode.Space));
        bindings.Add(InputBinding.CreateKey(InputActionId.ToggleRoleAttrPanel, KeyCode.C));
        bindings.Add(InputBinding.CreateKey(InputActionId.ToggleDebugUI, KeyCode.F1));
        bindings.Add(InputBinding.CreateKey(InputActionId.NextStage, KeyCode.Tab));
        bindings.Add(InputBinding.CreateKey(InputActionId.ReloadScene, KeyCode.F5));
        bindings.Add(InputBinding.CreateKey(InputActionId.AddHP, KeyCode.Alpha1));
        bindings.Add(InputBinding.CreateKey(InputActionId.ReduceHP, KeyCode.Alpha2));
        bindings.Add(InputBinding.CreateKey(InputActionId.AddMP, KeyCode.Alpha3));
        bindings.Add(InputBinding.CreateKey(InputActionId.ReduceMP, KeyCode.Alpha4));
        bindings.Add(InputBinding.CreateMouseButton(InputActionId.MouseLeft, 0));
        bindings.Add(InputBinding.CreateMouseButton(InputActionId.MouseRight, 1));
        bindings.Add(InputBinding.CreateAxis(InputActionId.MoveHorizontal, "Horizontal"));
        bindings.Add(InputBinding.CreateAxis(InputActionId.MoveVertical, "Vertical"));
        bindings.Add(InputBinding.CreateAxis(InputActionId.MouseX, "Mouse X"));
        bindings.Add(InputBinding.CreateAxis(InputActionId.MouseY, "Mouse Y"));
        bindings.Add(InputBinding.CreateAxis(InputActionId.MouseScroll, "Mouse ScrollWheel"));
    }
}
