using System.Collections;
using UnityEngine;

public class FireflyProjectile : Projectile
{
    private GameObject _player;
    private Transform _target;
    private const float speed = 2.5f;
    private const float playerBias = 0.6f;
    private Vector2 _move;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _target = _player.transform;
    }

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(RandomMove());
    }

    protected override void Update()
    {
        
    }

    private IEnumerator RandomMove()
    {
        while (true)
        {
            rb.linearVelocity = Vector2.zero;

            yield return new WaitForSeconds(0.15f);

            Vector2 direction = (_target.position - transform.position).normalized;

            Vector2 random = Random.insideUnitCircle.normalized;
        
            _move = ((direction * playerBias) + (random * (1 - playerBias))).normalized;

            rb.linearVelocity = _move * speed;

            yield return new WaitForSeconds(0.4f);
        }
        
    }
}
