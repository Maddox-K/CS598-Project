using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, I_DataPersistence
{
    public PlayerData playerData;

    //physics
    public Rigidbody2D rb;
    public float speed = 5f;
    public Vector2 moveDirection = Vector2.zero;
    public Vector2 lookDirection = Vector2.zero;
    public bool isDashing;
    private bool canDash = true;
    private const float dashDuration = .4f;
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
    private GameObject _interactSprite;
    private List<Collider2D> _thingsInRange = new List<Collider2D>();
    private GameObject _closestObject;


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
        }

        animator.SetFloat("look_x", lookDirection.x);
        animator.SetFloat("look_y", lookDirection.y);
        animator.SetFloat("Speed", moveDirection.magnitude);

        // interaction
        if (_thingsInRange.Count > 0)
        {
            _closestObject = GetClosestObject();
        }
        else
        {
            _closestObject = null;
            if (_interactSprite != null)
            {
                _interactSprite.SetActive(false);
                _interactSprite = null;
            }
        }

        if (_closestObject != null && interact.WasPressedThisFrame())
        {
            switch (_closestObject.tag)
            {
                case "SNPC":
                    _closestObject.GetComponent<standard_NPC>().Interact();
                    break;
                case "Enemy":
                    _closestObject.GetComponent<Enemy>().Interact();
                    break;
            }
        }
    }

    private IEnumerator Dash()
    {
        animator.SetTrigger("DashTrigger");

        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(moveDirection.x * dashSpeed, moveDirection.y * dashSpeed);
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCoolDown);

        canDash = true;
    }

    GameObject GetClosestObject()
    {
        Collider2D closest = null;
        float closestDistanceSqr = float.MaxValue;
        Vector3 playerPosition = transform.position;

        foreach (Collider2D obj in _thingsInRange)
        {
            float sqrDistance = (obj.transform.position - playerPosition).sqrMagnitude;
            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closest = obj;
            }
        }

        GameObject closestObj = closest.gameObject;

        if (closestObj.CompareTag("SNPC") || closestObj.CompareTag("Enemy"))
        {
            if (closestObj.transform.childCount > 0)
            {
                GameObject thisInteract = closestObj.transform.GetChild(0).gameObject;
                if (_interactSprite == null)
                {
                    _interactSprite = thisInteract;
                    _interactSprite.SetActive(true);
                }
                else if (_interactSprite != thisInteract)
                {
                    _interactSprite.SetActive(false);
                    _interactSprite = thisInteract;
                    _interactSprite.SetActive(true);
                }
            }
        }
        else if (_interactSprite != null)
        {
            _interactSprite.SetActive(false);
            _interactSprite = null;
        }

        return closestObj;
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
        _thingsInRange.Add(collider);

        GameObject thisCollided = collider.gameObject;

        if (thisCollided.CompareTag("Projectile"))
        {
            playerData.TakeDamage(thisCollided.GetComponent<Projectile>());
        }
        else if (thisCollided.CompareTag("Currency"))
        {
            thisCollided.GetComponent<Coin>().Collect();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        _thingsInRange.Remove(collider);

        if (_thingsInRange.Count == 0)
        {
            _closestObject = null;
        }
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
        //transform.position = data.playerPosition;
        Vector3 temp = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            temp[i] = data.playerPosition[i];
        }
        transform.position = temp;

        //lookDirection = data.playerRotation;
        for (int i = 0; i < 2; i++)
        {
            temp[i] = data.playerRotation[i];
        }
        lookDirection = temp;
    }

    public void SaveData(GameData data)
    {
        //data.playerPosition = transform.position;
        Vector3 currPos = transform.position;
        data.playerPosition[0] = currPos.x;
        data.playerPosition[1] = currPos.y;
        data.playerPosition[2] = currPos.z;

        //data.playerRotation = lookDirection;
        data.playerRotation[0] = lookDirection.x;
        data.playerRotation[1] = lookDirection.y;
    }
}