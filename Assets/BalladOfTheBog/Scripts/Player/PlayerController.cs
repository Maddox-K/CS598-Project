using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerData))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInputInitializer))]
public class PlayerController : MonoBehaviour, IDataPersistence
{
    private PlayerData _playerData;

    // Physics
    private Rigidbody2D _playerRigidBody;
    private float _speed = 5f;
    private Vector2 _moveDirection = Vector2.zero;
    private Vector2 _lookDirection = Vector2.zero;

    // Dash
    private bool _isDashing;
    private bool _canDash = true;
    private const float DashDuration = .4f;
    private const float DashSpeed = 10f;
    private const float DashCoolDown = 3f;

    // Animation
    private Animator _animator;
    private static readonly int LookXHash = Animator.StringToHash("look_x");
    private static readonly int LookYHash = Animator.StringToHash("look_y");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int DashTriggerHash = Animator.StringToHash("DashTrigger");

    // Input
    private PlayerInputInitializer _inputInitializer;
    private InputAction _move;
    private InputAction _dash;

    // Properties
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    public Vector2 LookDirection
    {
        get => _lookDirection;
        set => _lookDirection = value;
    }
    public bool IsDashing
    {
        get => _isDashing;
        set => _isDashing = value;
    }

    // Lifecycle Methods
    private void Awake()
    {
        _playerData = GetComponent<PlayerData>();
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _inputInitializer = GetComponent<PlayerInputInitializer>();
    }

    private void OnEnable()
    {
        PlayerEvents.OnDoorOpened += OnDoorOpened;
        PlayerEvents.OnEncounterStarted += OnEncounterStarted;
    }

    private void Start()
    {
        _move = _inputInitializer.Move;
        _dash = _inputInitializer.Dash;
    }

    void Update()
    {
        if (_isDashing)
        {
            return;
        }

        //movement
        _moveDirection = _move.ReadValue<Vector2>();

        if (_dash.WasPressedThisFrame() && _canDash)
        {
            StartCoroutine(Dash());
        }

        //animation
        if(!Mathf.Approximately(_moveDirection.x, 0.0f) || !Mathf.Approximately(_moveDirection.y, 0.0f))
        {
            _lookDirection.Set(_moveDirection.x, _moveDirection.y);
            _lookDirection.Normalize();
        }

        _animator.SetFloat(LookXHash, _lookDirection.x);
        _animator.SetFloat(LookYHash, _lookDirection.y);
        _animator.SetFloat(SpeedHash, _moveDirection.magnitude);
    }

    private void FixedUpdate() {
        // rigidbody movement
        if (_isDashing)
        {
            return;
        }

        _playerRigidBody.linearVelocity = new Vector2(_moveDirection.x * _speed, _moveDirection.y * _speed);
    }

    private void OnDisable()
    {
        PlayerEvents.OnDoorOpened -= OnDoorOpened;
        PlayerEvents.OnEncounterStarted -= OnEncounterStarted;
    }

    // Methods
    private void OnDoorOpened(Door door)
    {
        StartCoroutine(DoorTransition(door));
    }

    private IEnumerator DoorTransition(Door door)
    {
        yield return new WaitForSeconds(0.42f);

        transform.position = door.GetTeleport();
        _lookDirection = door.GetDirection();
    }

    private void OnEncounterStarted()
    {
        transform.position = new Vector3(0, 0, 0);
        _lookDirection = new Vector2(0, -1);
    }

    private IEnumerator Dash()
    {
        _animator.SetTrigger(DashTriggerHash);

        _canDash = false;
        _isDashing = true;
        _playerRigidBody.linearVelocity = new Vector2(_moveDirection.x * DashSpeed, _moveDirection.y * DashSpeed);
        yield return new WaitForSeconds(DashDuration);

        _isDashing = false;

        yield return new WaitForSeconds(DashCoolDown);

        _canDash = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject thisCollided = collider.gameObject;

        if (thisCollided.CompareTag("Projectile"))
        {
            _playerData.TakeDamage(thisCollided.GetComponent<Projectile>());
        }
        else if (thisCollided.CompareTag("Currency"))
        {
            thisCollided.GetComponent<Coin>().Collect();
        }
        else if (thisCollided.CompareTag("PuzzleZone"))
        {
            thisCollided.GetComponent<PuzzleZone>().SwitchToPuzzle();
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (_playerData.CanTakeDamage && collider.gameObject.CompareTag("Projectile"))
        {
            _playerData.TakeDamage(collider.gameObject.GetComponent<Projectile>());
        }
    }

    public void LoadData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            return;
        }

        Vector3 temp = new Vector3();

        for (int i = 0; i < 3; i++)
        {
            temp[i] = data.playerPosition[i];
        }
        transform.position = temp;

        for (int i = 0; i < 2; i++)
        {
            temp[i] = data.playerRotation[i];
        }
        _lookDirection = temp;
    }

    public void SaveData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == "BattleTest")
        {
            return;
        }

        Vector3 currPos = transform.position;
        data.playerPosition[0] = currPos.x;
        data.playerPosition[1] = currPos.y;
        data.playerPosition[2] = currPos.z;

        data.playerRotation[0] = _lookDirection.x;
        data.playerRotation[1] = _lookDirection.y;
    }
}