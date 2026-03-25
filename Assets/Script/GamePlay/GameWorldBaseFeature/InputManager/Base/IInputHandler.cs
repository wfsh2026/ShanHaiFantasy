/// <summary>
/// 输入处理器接口。
/// 返回 true 表示本次输入已被消费，不再继续向后路由。
/// </summary>
public interface IInputHandler {
    bool HandleInput(InputManager inputManager, InputState inputState);
}
