using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime); // destroy automatically after some time
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check for normal enemies
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // damage = 1
            }
            Destroy(gameObject); // destroy pellet
        }
        // Check for boss
        else if (other.CompareTag("Boss"))
        {
            BossEnemy boss = other.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        // Breakables
        else if (other.CompareTag("Breakable"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        // Walls
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
