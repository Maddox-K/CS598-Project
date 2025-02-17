using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleZone : MonoBehaviour
{
    // world
    private Grid _grid;

    // player
    private GameObject _player;
    private Transform _playerTransform;
    private PlayerController _playerController;
    private PlayerPuzzleController _playerPuzzleController;
    [SerializeField] private Sprite _miniSprite;
    
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
    }

    void Start()
    {
        
    }

    public void SwitchToPuzzle()
    {
        _player.GetComponent<SpriteRenderer>().sprite = _miniSprite;
        _player.GetComponent<Animator>().enabled = false;

        _player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        _playerController.enabled = false;
        _playerPuzzleController.enabled = true;

        Vector3Int cellPosition = _grid.WorldToCell(_playerTransform.position);
        _playerTransform.position = _grid.CellToWorld(cellPosition);
    }
}
