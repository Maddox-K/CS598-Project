using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public PlayerInputActions playerControls;

    public Animator animator;

    Vector2 moveDirection = Vector2.zero;
    Vector2 lookDirection = Vector2.zero;
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
        //movement
        moveDirection = move.ReadValue<Vector2>();

        //animation
        if(!Mathf.Approximately(moveDirection.x, 0.0f) || !Mathf.Approximately(moveDirection.y, 0.0f))
        {
            lookDirection.Set(moveDirection.x, moveDirection.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("look_x", lookDirection.x);
        animator.SetFloat("look_y", lookDirection.y);
        //animator.SetFloat("Speed", moveDirection.magnitude);
    }

    private void FixedUpdate() {
        //movement (rigidbody physics compoenent)
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }
}
