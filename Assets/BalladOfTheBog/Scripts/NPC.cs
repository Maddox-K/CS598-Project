using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour, I_Interactible
{
    /* [SerializeField] private SpriteRenderer interact_sprite;

    private player_trans;

    public InputAction dialogueAction;

    private const float interact_distance = 5F; */
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    private void Update()
    {
        /* if (dialogueAction.WasPressedThisFrame() && isWithinInteractDist()) {
            interact();
        }

        if (interact_sprite.gameObject.activeSelf && !isWithinInteractDist())
        {
            interact_sprite.gameObject.SetActive(false);
        }

        else if (interact_sprite.gameObject.activeSelf && isWithinInteractDist())
        {
            interact_sprite.gameObject.SetActive(true);
        } */
    }

    public abstract void Interact();

    /* private bool isWithinInteractDist()
    {
        if (Vector2.Distance(player_trans.position, transform.position) < interact_distance) {
            return true;
        }
        else {
            return false;
        }
    } */
}