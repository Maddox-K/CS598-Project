using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [System.Serializable]
    public class Bounds
    {
        public Vector2 minimumBound;
        public Vector2 maximumBound;
    }

    [SerializeField] private Bounds wander;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float changeDirection = 1f;
    private Vector2 currentDirection;
    private float changeDirectionTimer;

    private void Start()
    {
        OnRandomDirection();
        changeDirectionTimer = changeDirection;
    }

    private void Update()
    {
        NPCMove();
        OnDirectionChange();
    }

    private void NPCMove()
    {
        Vector2 newPos = (Vector2)transform.position + currentDirection * speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, wander.minimumBound.x, wander.maximumBound.x);
        newPos.y = Mathf.Clamp(newPos.y, wander.minimumBound.y, wander.maximumBound.y);

        transform.position = newPos;
    }

    private void OnDirectionChange()
    {
        changeDirectionTimer -= Time.deltaTime;
        if (changeDirectionTimer <= 0f)
        {
            OnRandomDirection();
            changeDirectionTimer = changeDirection;
        }


    }

    private void OnRandomDirection()
    {
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);

        currentDirection = new Vector2(randX, randY).normalized;
    }

      

}
