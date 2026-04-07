using UnityEngine;
using static InputManager;

public class InputManager : Singleton<InputManager>
{
    InputSystem_Actions _input;
    public delegate void InputDelegate();
    public delegate void InputGenericDelegate<T>(T param);
    public static event InputDelegate OnAttackDown;
    public static event InputDelegate OnAttackUp;
    protected override void Awake()
    {
        base.Awake();
        _input = new InputSystem_Actions();

        _input.Player.Attack.started += ctx => OnAttackDown?.Invoke();
        _input.Player.Attack.canceled += ctx => OnAttackUp?.Invoke();


    }
    #region Enable/Disable Input Actions
    private void OnEnable()
    {
        _input?.Enable();
    }
    private void OnDisable()
    {
        _input?.Disable();
    }
    #endregion
}
