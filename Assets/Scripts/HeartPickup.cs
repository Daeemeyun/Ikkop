using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    public int healAmount = 1;
    public AudioClip pickupSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.Heal(healAmount);
                Destroy(gameObject);
            }
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
        }
    }
}
