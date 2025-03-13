using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;
    public int damage;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("horizontal", rb.linearVelocity.x);
            animator.SetFloat("vertical", rb.linearVelocity.y);
        }
    }
}
