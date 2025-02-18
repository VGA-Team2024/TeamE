using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : SingletonMonoBehavior<PlayerInputProvider>
{
    private GameInputs _gameInputs;
    private Vector2 _lookValue;
    private Vector2 _moveValue;
    private bool _grab;
    private readonly Subject<Unit> _jumpSubject = new();
    private readonly Subject<InputAction.CallbackContext> _aimSubject = new();
    private readonly Subject<Unit> _attackSubject = new();
    public Vector2 LookValue => _lookValue;
    public Vector2 MoveValue => _moveValue;
    public Subject<Unit> JumpSubject => _jumpSubject;
    public Subject<InputAction.CallbackContext> AimSubject => _aimSubject;
    public Subject<Unit> AttackSubject => _attackSubject;
    public bool Grab => _grab;
    public InputAction PauseAction { get; private set; }
    protected override void OnAwake()
    {
        _gameInputs = new GameInputs();
        _gameInputs.Player.Look.performed += OnLook;
        _gameInputs.Player.Look.canceled += OnLook;
        _gameInputs.Player.Move.performed += OnMove;
        _gameInputs.Player.Move.canceled += OnMove;
        _gameInputs.Player.Jump.started += OnJump;
        _gameInputs.Player.Aim.started += OnAim;
        _gameInputs.Player.Aim.canceled += OnAim;
        _gameInputs.Player.Attack.started += OnAttack;
        _gameInputs.Player.Grab.started += OnGrab;
        _gameInputs.Player.Grab.canceled += OnGrab;
        PauseAction = _gameInputs.Player.Pause;
        _gameInputs.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
            _lookValue = context.ReadValue<Vector2>();
        else
            _lookValue = Vector2.zero;
    }
    void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            _moveValue = context.ReadValue<Vector2>();
        else
            _moveValue = Vector2.zero;
    }
    void OnJump(InputAction.CallbackContext context)
    {
        _jumpSubject.OnNext(Unit.Default);
    }
    void OnAim(InputAction.CallbackContext context)
    {
        _aimSubject.OnNext(context);
    }
    void OnAttack(InputAction.CallbackContext context)
    {
        _attackSubject.OnNext(Unit.Default);
    }
    void OnGrab(InputAction.CallbackContext context)
    {
        _grab = context.started;
    }
}