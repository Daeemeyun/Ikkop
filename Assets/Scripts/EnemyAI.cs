using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 1.5f;
    public float detectionRange = 1f;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= detectionRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                Vector2 movePos = rb.position + direction * speed * Time.fixedDeltaTime;

                rb.MovePosition(movePos);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy collided with: " + collision.collider.name);

        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Hit the player!");
            
            PlayerHealth ph = collision.collider.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
        }
    }
}
