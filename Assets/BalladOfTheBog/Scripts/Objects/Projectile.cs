using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    public int damage;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("horizontal", rb.velocity.x);
            animator.SetFloat("vertical", rb.velocity.y);
        }
    }
}
