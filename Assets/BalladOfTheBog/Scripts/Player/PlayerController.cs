using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour, I_DataPersistence
{
    public PlayerData playerData;

    //physics
    public Rigidbody2D rb;
    public float speed = 5f;
    Vector2 moveDirection = Vector2.zero;
    public Vector2 lookDirection = Vector2.zero;
    public bool isDashing;
    private bool canDash = true;
    private const float dashDuration = .25f;
    private const float dashSpeed = 10f;
    private const float dashCoolDown = 3f;

    //animation
    public Animator animator;

    //input
    public PlayerInputActions playerControls;
    public InputAction move;
    public InputAction interact;
    public InputAction dash;
    
    //interaction
    private GameObject collided;
    private bool isInInteractRange = false;
    private bool interact_sprite_active;


    private void Awake() {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable() {
        move = playerControls.Player.Move;
        move.Enable();
        interact = playerControls.Player.Interact;
        interact.Enable();
        dash = playerControls.Player.Dash;
    }

    private void OnDisable() {
        move.Disable();
        interact.Disable();
        dash.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        //movement
        moveDirection = move.ReadValue<Vector2>();

        if (dash.WasPressedThisFrame() && canDash)
        {
            StartCoroutine(Dash());
        }

        //animation
        if(!Mathf.Approximately(moveDirection.x, 0.0f) || !Mathf.Approximately(moveDirection.y, 0.0f))
        {
            lookDirection.Set(moveDirection.x, moveDirection.y);
            lookDirection.Normalize();

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }


        animator.SetFloat("look_x", lookDirection.x);
        animator.SetFloat("look_y", lookDirection.y);
        //animator.SetFloat("Speed", moveDirection.magnitude);

        // interaction
        if (isInInteractRange && interact.WasPressedThisFrame())
        {
            switch (collided.tag)
            {
                case "Currency":
                    //collided.SetActive(false);
                    collided.GetComponent<Coin>().Collect();
                    break;
                case "SNPC":
                    collided.GetComponent<standard_NPC>().Interact();
                    break;
                case "Enemy":
                    collided.GetComponent<Enemy>().Interact();
                    break;
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        //playerData.canTakeDamage = false;
        rb.velocity = new Vector2(moveDirection.x * dashSpeed, moveDirection.y * dashSpeed);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        //playerData.canTakeDamage = true;

        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    private void FixedUpdate() {
        // rigidbody movement
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        isInInteractRange = true;
        collided = collider.gameObject;

        if (collided.CompareTag("Projectile"))
        {
            playerData.TakeDamage(collided.GetComponent<Projectile>());
        }
        else if (collided.CompareTag("SNPC"))
        {
            collided.transform.Find("InteractObject").gameObject.SetActive(true);
            interact_sprite_active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        isInInteractRange = false;

        // turn off indicator sprite
        if (interact_sprite_active == true)
        {
            collided.transform.Find("InteractObject").gameObject.SetActive(false);
            interact_sprite_active = false;
        }

        collided = null;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (playerData.canTakeDamage && collider.gameObject.CompareTag("Projectile"))
        {
            playerData.TakeDamage(collider.gameObject.GetComponent<Projectile>());
        }
    }

    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
        lookDirection = data.playerRotation;
    }

    public void SaveData(GameData data)
    {
        data.playerPosition = transform.position;
        data.playerRotation = lookDirection;
    }
}