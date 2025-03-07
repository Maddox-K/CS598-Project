using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PuzzleZone : MonoBehaviour
{
    // world
    private Grid _grid;
    public Dictionary<Vector2Int, (Transform, bool)> puzzleContents = new Dictionary<Vector2Int, (Transform, bool)>();
    public Dictionary<Vector2Int, (Transform, bool)> startContents = new Dictionary<Vector2Int, (Transform, bool)>();
    [SerializeField] public Vector2Int[] solutionTiles;
    [SerializeField] private int _numRequiredToSolve;
    public int numSolved = 0;

    // player
    private GameObject _player;
    private Transform _playerTransform;
    private PlayerController _playerController;
    private PlayerPuzzleController _playerPuzzleController;
    [SerializeField] private Sprite _miniSprite;
    [SerializeField] private Vector3 _initialPlayerPosition;
    private Vector3Int _initialPlayerCellPosition;

    // UI
    private Button _resetMenuButton;
    
    void Awake()
    {
        _grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player != null)
        {
            _playerTransform = _player.transform;
            _playerController = _player.GetComponent<PlayerController>();
            _playerPuzzleController = _player.GetComponent<PlayerPuzzleController>();
        }

        GameObject resetMenu = GameObject.FindGameObjectWithTag("ResetMenu");
        if (resetMenu != null)
        {
            _resetMenuButton = resetMenu.transform.GetChild(0).gameObject.GetComponent<Button>();
        }

        _initialPlayerCellPosition = _grid.WorldToCell(_initialPlayerPosition);
    }

    void Start()
    {
        foreach (Vector2Int key in startContents.Keys)
        {
            if (startContents[key].Item2)
            {
                Debug.Log(key);
            }
        }
        if (_resetMenuButton != null)
        {
            _resetMenuButton.onClick.AddListener(ResetPuzzle);
        }
    }

    public void SwitchToPuzzle()
    {
        _player.GetComponent<SpriteRenderer>().sprite = _miniSprite;
        _player.GetComponent<Animator>().enabled = false;

        _player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        _playerController.enabled = false;
        _playerPuzzleController.enabled = true;
        _playerPuzzleController.currentPuzzle = this;
        _playerPuzzleController.currentContents = puzzleContents;

        Vector3Int cellPosition = _grid.WorldToCell(_playerTransform.position);
        _playerTransform.position = _grid.CellToWorld(cellPosition);

        if (_resetMenuButton != null)
        {
            _resetMenuButton.gameObject.SetActive(true);
            _resetMenuButton.Select();
        }
    }

    private void ResetPuzzle()
    {
        numSolved = 0;

        puzzleContents = new Dictionary<Vector2Int, (Transform, bool)>(startContents);
        _playerPuzzleController.currentContents = puzzleContents;

        _playerTransform.position = _grid.CellToWorld(_initialPlayerCellPosition);

        foreach ((Transform, bool) value in puzzleContents.Values)
        {
            if (value.Item2)
            {
                Vector3 initPos = value.Item1.gameObject.GetComponent<Obstacle>().GetInitialPosition();
                value.Item1.position = initPos;
            }
        }
    }

    public int GetNumRequired()
    {
        return _numRequiredToSolve;
    }
}
