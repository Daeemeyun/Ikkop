using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject faintScreen;

    private Slider healthSlider;
    
    private bool isDead = false;

    void Start()
    {
        StartCoroutine(FindUIAfterDelay());
    }

    private IEnumerator FindUIAfterDelay()
    {
        yield return null;

        currentHealth = maxHealth;

        healthSlider = GameObject.Find("HealthSlider")?.GetComponent<Slider>();
        faintScreen = GameObject.Find("FaintScreen");

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("HealthSlider not found.");
        }

        if (faintScreen != null)
        {
            faintScreen.SetActive(false);
        }
        else
        {
            Debug.LogWarning("FaintScreen not found.");
        }
    }
    
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            StartCoroutine(HandleFaint());
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
    
    IEnumerator HandleFaint()
    {
        isDead = true;

        if (faintScreen != null)
            faintScreen.SetActive(true);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("CabinHub");
    }
}
