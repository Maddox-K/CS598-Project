using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPuzzleController : MonoBehaviour
{
    public PlayerInputActions playerControls;
    private Transform _playerTransform;
    public InputAction move;
    private Rigidbody2D _playerRigidBody;
    private Vector2 _rawMove = Vector2.zero;
    private Vector2 _moveDirection = Vector2.zero;
    private float _absX;
    private float _absY;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        _playerRigidBody = GetComponent<Rigidbody2D>();
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

    // Update is called once per frame
    void Update()
    {
        /* _rawMove = move.ReadValue<Vector2>().normalized;
        
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
        } */
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        /* if (collider.gameObject.CompareTag("Obstacle"))
        {

        } */
    }

    private void MoveLeft()
    {
        _playerTransform.position += new Vector3(-1, 0, 0);
    }

    private void MoveRight()
    {
        _playerTransform.position += new Vector3(1, 0, 0);
    }

    private void MoveDown()
    {
        _playerTransform.position += new Vector3(0, -1, 0);
    }

    private void MoveUp()
    {
        _playerTransform.position += new Vector3(0, 1, 0);
    }
}
