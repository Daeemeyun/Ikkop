using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public GameObject exitPortalPrefab; // Assign this in Inspector
    public GameObject healthBarCanvas; // Child canvas with slider
    private Slider healthBar;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBarCanvas != null)
        {
            healthBar = healthBarCanvas.GetComponentInChildren<Slider>();
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = currentHealth;
            }
            else
            {
                Debug.LogWarning("Slider not found in healthBarCanvas.");
            }
        }
        else
        {
            Debug.LogWarning("healthBarCanvas is not assigned.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Pellet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (exitPortalPrefab != null)
        {
            Instantiate(exitPortalPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Portal prefab not assigned on BossEnemy.");
        }

        Destroy(gameObject);
    }
}
