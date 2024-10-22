using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float patrolX, patrolY;

    [SerializeField] private int facingDirection = -1;

    private bool isWalking;

    private void Update()
    {
        if (transform.position.x > patrolX || transform.position.y > patrolY)
        {
            Flip();
        }

        rb.velocity = Vector2.right * facingDirection * speed;
    }

    void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingDirection *= -1;
    }
}
