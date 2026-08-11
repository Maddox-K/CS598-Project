using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInitializer : MonoBehaviour
{
    [SerializeField] private PlayerInputActions _playerControls;
    private InputAction _interact;
    private InputAction _move;
    private InputAction _dash;

    // Properties
    public InputAction Interact
    {
        get => _interact;
        private set => _interact = value;
    }
    public InputAction Move
    {
        get => _move;
        private set => _move = value;
    }
    public InputAction Dash
    {
        get => _dash;
        private set => _dash = value;
    }

    // Lifecycle Methods
    void Awake()
    {
        _playerControls = new PlayerInputActions();
    }

    void OnEnable()
    {
        _interact = _playerControls.Player.Interact;
        _interact.Enable();

        _move = _playerControls.Player.Move;
        _move.Enable();

        _dash = _playerControls.Player.Dash;
        _dash.Enable();

        PlayerEvents.ActivateControls += ActivateControls;
        PlayerEvents.DeactivateControls += DeactivateControls;
    }

    void OnDisable()
    {
        _interact?.Disable();
        _move?.Disable();
        _dash?.Disable();

        PlayerEvents.ActivateControls -= ActivateControls;
        PlayerEvents.DeactivateControls -= DeactivateControls;
    }

    // Methods
    private void ActivateControls(int type)
    {
        switch (type)
        {
            case 0:
                _interact?.Enable();
                break;
            case 1:
                _move?.Enable();
                break;
            case 2:
                _move?.Enable();
                _interact?.Enable();
                break;
            case 3:
                _dash?.Enable();
                break;
        }
    }

    private void DeactivateControls(int type)
    {
        switch (type)
        {
            case 0:
                _interact?.Disable();
                break;
            case 1:
                _move?.Disable();
                break;
            case 2:
                _move?.Disable();
                _interact?.Disable();
                break;
            case 3:
                _dash?.Disable();
                break;
        }
    }
}
