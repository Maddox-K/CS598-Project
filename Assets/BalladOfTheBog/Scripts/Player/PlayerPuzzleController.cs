using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPuzzleController : MonoBehaviour
{
    private Transform _playerTransform;
    public PuzzleZone currentPuzzle;
    public Dictionary<Vector2Int, (Transform, bool)> currentContents = new Dictionary<Vector2Int, (Transform, bool)>();
    private Camera _camera;
    public Camera currentPuzzleCamera;

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
    private GameObject _resetMenu;

    // Audio
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _move;
    [SerializeField] private AudioClip _pushedIntoSolution;
    [SerializeField] private AudioClip _cannotMove;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        
        _playerTransform = GetComponent<Transform>();

        _resetMenu = GameObject.FindGameObjectWithTag("ResetMenu");

        _audioSource = GetComponent<AudioSource>();

        _camera = transform.GetChild(0).GetComponent<Camera>();
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
            _camera.enabled = true;
            currentPuzzleCamera.enabled = false;

            currentPuzzle = null;
            currentContents = null;
            currentPuzzleCamera = null;

            GetComponent<SpriteRenderer>().sprite = _normalSprite;
            GetComponent<Animator>().enabled = true;
            GetComponent<PlayerController>().enabled = true;

            if (_resetMenu != null)
            {
                _resetMenu.SetActive(false);
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

                if (CheckCompletion(currentPuzzle.numSolved, currentPuzzle.GetNumRequired()))
                {
                    QuestEvents.OnPuzzleCompleted?.Invoke(currentPuzzle.GetPuzzleID());
                    Debug.Log("Puzzle Solved!");
                }
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

                if (CheckCompletion(currentPuzzle.numSolved, currentPuzzle.GetNumRequired()))
                {
                    QuestEvents.OnPuzzleCompleted?.Invoke(currentPuzzle.GetPuzzleID());
                    Debug.Log("Puzzle Solved!");
                }
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

                if (CheckCompletion(currentPuzzle.numSolved, currentPuzzle.GetNumRequired()))
                {
                    QuestEvents.OnPuzzleCompleted?.Invoke(currentPuzzle.GetPuzzleID());
                    Debug.Log("Puzzle Solved!");
                }
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

                if (CheckCompletion(currentPuzzle.numSolved, currentPuzzle.GetNumRequired()))
                {
                    QuestEvents.OnPuzzleCompleted?.Invoke(currentPuzzle.GetPuzzleID());
                    Debug.Log("Puzzle Solved!");
                }
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

        Vector2Int SpaceToCheck = Vector2Int.RoundToInt(coords);
        Vector2Int AdjacentSpace = Vector2Int.RoundToInt(nextCoords);

        if (currentContents.TryGetValue(SpaceToCheck, out (Transform, bool) value)) // space you are trying to move into has either an obstacle or a wall on it
        {
            if (!value.Item2) // space contains a wall
            {
                return true;
            }
            else if (currentContents.ContainsKey(AdjacentSpace)) // space contains an obstacle and the obstacle's adjacent space is not empty
            {
                return true;
            }
            else // space contains an obstacle and the adjacent space is empty
            {
                if (currentPuzzle.solutionTiles.Contains(SpaceToCheck))
                {
                    currentPuzzle.numSolved--;
                }
                if (currentPuzzle.solutionTiles.Contains(AdjacentSpace))
                {
                    currentPuzzle.numSolved++;
                }

                _objectToPush = value.Item1;
                currentContents.Remove(SpaceToCheck);
                currentContents.Add(AdjacentSpace, (value.Item1, true));
                return false;
            }
        }
        else // space you are trying to move into is empty
        {
            return false;
        }
    }

    private bool CheckCompletion(int solved, int required)
    {
        return solved == required;
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
