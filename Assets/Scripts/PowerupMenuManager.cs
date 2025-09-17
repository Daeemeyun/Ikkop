using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerupMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject powerupMenuUI;
    public Button doubleShotButton;
    public Button speedBoostButton; 
    public Button healthBoostButton;
    
    [Header("Button Colors")]
    public Color availableColor = Color.white;
    public Color chosenColor = Color.gray;
    public Color disabledColor = Color.red;

    private PlayerController playerController;
    private static HashSet<PowerupType> chosenPowerups = new HashSet<PowerupType>();
    
    public enum PowerupType
    {
        DoubleShot,
        SpeedBoost,
        HealthBoost
    }

    private void Start()
    {
        // Find player - either pre-spawned or wait for dungeon generation
        StartCoroutine(FindPlayerAfterDelay());
        
        if (ShouldShowPowerupMenu())
        {
            ShowPowerupMenu();
        }
    }

    private System.Collections.IEnumerator FindPlayerAfterDelay()
    {
        // Wait a frame to let dungeon generator spawn the player
        yield return null;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            
            // Apply already chosen powerups to the newly spawned player
            ApplyChosenPowerups();
        }
    }

    private bool ShouldShowPowerupMenu()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // Show menu at start of each stage if there are powerups left to choose
        return (sceneName == "stage1" || sceneName == "stage2" || sceneName == "stage3") 
               && chosenPowerups.Count < 3;
    }

    private void ShowPowerupMenu()
    {
        Time.timeScale = 0f;
        powerupMenuUI.SetActive(true);
        
        // Update button states
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        UpdateButton(doubleShotButton, PowerupType.DoubleShot);
        UpdateButton(speedBoostButton, PowerupType.SpeedBoost);
        UpdateButton(healthBoostButton, PowerupType.HealthBoost);
    }

    private void UpdateButton(Button button, PowerupType powerupType)
    {
        bool isChosen = chosenPowerups.Contains(powerupType);
        
        button.interactable = !isChosen;
        
        ColorBlock colors = button.colors;
        if (isChosen)
        {
            colors.normalColor = chosenColor;
            colors.disabledColor = disabledColor;
        }
        else
        {
            colors.normalColor = availableColor;
        }
        button.colors = colors;
    }

    public void ChooseDoubleShot()
    {
        if (!chosenPowerups.Contains(PowerupType.DoubleShot))
        {
            chosenPowerups.Add(PowerupType.DoubleShot);
            if (playerController != null)
                playerController.EnableDoubleShot();
            
            ClosePowerupMenu();
        }
    }

    public void ChooseSpeedBoost()
    {
        if (!chosenPowerups.Contains(PowerupType.SpeedBoost))
        {
            chosenPowerups.Add(PowerupType.SpeedBoost);
            if (playerController != null)
                playerController.IncreaseSpeed();
            
            ClosePowerupMenu();
        }
    }

    public void ChooseHealthBoost()
    {
        if (!chosenPowerups.Contains(PowerupType.HealthBoost))
        {
            chosenPowerups.Add(PowerupType.HealthBoost);
            if (playerController != null)
                playerController.IncreaseHealth(2);
            
            ClosePowerupMenu();
        }
    }

    private void ClosePowerupMenu()
    {
        powerupMenuUI.SetActive(false);
        Time.timeScale = 1f;
        
        Debug.Log($"Powerups chosen so far: {chosenPowerups.Count}/3");
    }

    private void ApplyChosenPowerups()
    {
        if (playerController == null) return;
        
        foreach (PowerupType powerup in chosenPowerups)
        {
            switch (powerup)
            {
                case PowerupType.DoubleShot:
                    playerController.EnableDoubleShot();
                    break;
                case PowerupType.SpeedBoost:
                    playerController.IncreaseSpeed();
                    break;
                case PowerupType.HealthBoost:
                    playerController.IncreaseHealth(2);
                    break;
            }
        }
    }

    // Call this when returning to CabinHub to reset for new run
    public static void ResetPowerups()
    {
        chosenPowerups.Clear();
        Debug.Log("Powerups reset for new run");
    }

    // Optional: Method to check if player has specific powerup
    public static bool HasPowerup(PowerupType powerupType)
    {
        return chosenPowerups.Contains(powerupType);
    }
}