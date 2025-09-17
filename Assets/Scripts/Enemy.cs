using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;

    public int damageToPlayer = 1;

    public AudioClip hitSFX;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && !audioSource.enabled)
        {
            audioSource.enabled = true;
        }
    }

    public GameObject bonePrefab;

    void Die()
    {
        Instantiate(bonePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (hitSFX != null)
        // audioSource.PlayOneShot(hitSFX);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            TakeDamage(1); // change this to change damage
            Destroy(other.gameObject); // logic to destroy the pellet projectile
        }
        else if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
            }
        }
    }    
}
