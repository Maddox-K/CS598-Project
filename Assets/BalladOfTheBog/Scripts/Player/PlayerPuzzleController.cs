using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPuzzleController : MonoBehaviour
{
    private Transform _playerTransform;
    public PuzzleZone currentPuzzle;
    public Dictionary<Vector2Int, (Transform, bool)> currentContents = new Dictionary<Vector2Int, (Transform, bool)>();

    // Input
    public PlayerInputActions playerControls;
    public InputAction move;
    
    // Movement
    private Vector2 _rawMove = Vector2.zero;
    private float _absX;
    private float _absY;

    // Sprites and Animation
    [SerializeField] private Sprite _normalSprite;

    // moving objects
    private Transform _objectToPush = null;

    // UI
    private Button _resetMenuButton;

    // Audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _move;
    [SerializeField] private AudioClip _pushedIntoSolution;
    [SerializeField] private AudioClip _cannotMove;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        
        _playerTransform = GetComponent<Transform>();

        GameObject resetMenu = GameObject.FindGameObjectWithTag("ResetMenu");
        if (resetMenu != null)
        {
            _resetMenuButton = resetMenu.transform.GetChild(0).gameObject.GetComponent<Button>();
        }

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
        move.performed += context => MovePlayer(context);
    }

    private void OnDisable()
    {
        move.Disable();
        move.performed -= context => MovePlayer(context);
    }

    private void MovePlayer(InputAction.CallbackContext context)
    {
        _rawMove = context.ReadValue<Vector2>().normalized;

        _absX = Mathf.Abs(_rawMove.x);
        _absY = Mathf.Abs(_rawMove.y);

        if (_absX > _absY)
        {
            if (_rawMove.x < 0)
            {
                MoveLeft();
            }
            else
            {
                MoveRight();
            }
        }
        else if (_absX < _absY)
        {
            if (_rawMove.y < 0)
            {
                MoveDown();
            }
            else
            {
                MoveUp();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("PuzzleZone"))
        {
            currentPuzzle = null;
            currentContents = null;

            GetComponent<SpriteRenderer>().sprite = _normalSprite;
            GetComponent<Animator>().enabled = true;
            GetComponent<PlayerController>().enabled = true;

            if (_resetMenuButton != null)
            {
                _resetMenuButton.gameObject.SetActive(false);
            }

            this.enabled = false;
        }
    }

    private void MoveLeft()
    {
        if (!CheckCollision(1))
        {
            PlayMoveAudio();

            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.left;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.left;
        }
        else
        {
            PlayBlockedAudio();
        }
    }

    private void MoveRight()
    {
        if (!CheckCollision(2))
        {
            PlayMoveAudio();

            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.right;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.right;
        }
        else
        {
            PlayBlockedAudio();
        }
    }

    private void MoveDown()
    {
        if (!CheckCollision(3))
        {
            PlayMoveAudio();

            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.down;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.down;
        }
        else
        {
            PlayBlockedAudio();
        }
    }

    private void MoveUp()
    {
        if (!CheckCollision(4))
        {
            PlayMoveAudio();

            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.up;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.up;
        }
        else
        {
            PlayBlockedAudio();
        }
    }

    private bool CheckCollision(int direction)
    {
        Vector3 coords = _playerTransform.position;
        Vector3 nextCoords = Vector3.zero;

        switch (direction)
        {
            case 1:
                coords += Vector3.left;
                nextCoords += (coords + Vector3.left);
                break;
            case 2:
                coords += Vector3.right;
                nextCoords += (coords + Vector3.right);
                break;
            case 3:
                coords += Vector3.down;
                nextCoords += (coords + Vector3.down);
                break;
            case 4:
                coords += Vector3.up;
                nextCoords += (coords + Vector3.up);
                break;
        }

        if (currentContents.TryGetValue(Vector2Int.RoundToInt(coords), out (Transform, bool) value))
        {
            if (!value.Item2)
            {
                return true;
            }
            else if (currentContents.ContainsKey(Vector2Int.RoundToInt(nextCoords)))
            {
                return true;
            }
            else
            {
                _objectToPush = value.Item1;
                currentContents.Remove(Vector2Int.RoundToInt(coords));
                currentContents.Add(Vector2Int.RoundToInt(nextCoords), (value.Item1, true));
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void PlayMoveAudio()
    {
        _audioSource.PlayOneShot(_move);
    }

    private void PlayBlockedAudio()
    {
        _audioSource.PlayOneShot(_cannotMove);
    }
}
