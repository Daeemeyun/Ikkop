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
        // checking for normal enemies
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // damage = 1, can change
            }
            Destroy(gameObject); // destroy pellet
        }
        // checking for boss enemy
        else if (other.CompareTag("Boss"))
        {
            BossEnemy boss = other.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(1);
            }
            Destroy(gameObject);
        }
        // for destroyables/pots
        else if (other.CompareTag("Breakable"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        // for walls
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
