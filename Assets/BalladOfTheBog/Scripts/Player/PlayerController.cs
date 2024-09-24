using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public Animator animator;

    //input
    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction interact;

    //movement vectors
    Vector2 moveDirection = Vector2.zero;
    Vector2 lookDirection = Vector2.zero;
    
    private bool isInInteractRange = false;
    private standard_NPC standardNPC;

    private void Awake() {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable() {
        move = playerControls.Player.Move;
        move.Enable();
        interact = playerControls.Player.Interact;
        interact.Enable();
    }

    private void OnDisable() {
        move.Disable();
        interact.Disable();
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

        if (isInInteractRange && interact.WasPressedThisFrame())
        {
            if (standardNPC != null)
            {
                //Debug.Log("Interact");
                standardNPC.Interact();
            }
        }
    }

    private void FixedUpdate() {
        //movement (rigidbody physics compoenent)
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.gameObject.transform.Find("InteractObject").gameObject.SetActive(true);
        isInInteractRange = true;
        standardNPC = collider.GetComponent<standard_NPC>();
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        collider.gameObject.transform.Find("InteractObject").gameObject.SetActive(false);
        isInInteractRange = false;
        standardNPC = null;
    }
}
