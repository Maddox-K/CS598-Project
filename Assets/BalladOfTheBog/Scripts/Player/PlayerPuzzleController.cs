using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        
        _playerTransform = GetComponent<Transform>();
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
            this.enabled = false;
        }
    }

    private void MoveLeft()
    {
        if (!CheckCollision("left"))
        {
            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.left;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.left;
        }
    }

    private void MoveRight()
    {
        if (!CheckCollision("right"))
        {
            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.right;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.right;
        }
    }

    private void MoveDown()
    {
        if (!CheckCollision("down"))
        {
            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.down;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.down;
        }
    }

    private void MoveUp()
    {
        if (!CheckCollision("up"))
        {
            if (_objectToPush != null)
            {
                _objectToPush.position += Vector3.up;
                _objectToPush = null;
            }

            _playerTransform.position += Vector3.up;
        }
    }

    private bool CheckCollision(string direction)
    {
        Vector3 coords = _playerTransform.position;
        Vector3 nextCoords = Vector3.zero;

        switch (direction)
        {
            case "left":
                coords += Vector3.left;
                nextCoords += (coords + Vector3.left);
                break;
            case "right":
                coords += Vector3.right;
                nextCoords += (coords + Vector3.right);
                break;
            case "down":
                coords += Vector3.down;
                nextCoords += (coords + Vector3.down);
                break;
            case "up":
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
}
