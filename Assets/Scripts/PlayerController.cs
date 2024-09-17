using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public PlayerInputActions playerControls;

    Vector2 moveDirection = Vector2.zero;
    private InputAction move;
    private InputAction interact;

    private void Awake() {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable() {
        move = playerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable() {
        move.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }
}
