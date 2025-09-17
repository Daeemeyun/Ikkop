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
        // logic to find player either pre-spawned or after the dungeon generation script
        StartCoroutine(FindPlayerAfterDelay());
        
        if (ShouldShowPowerupMenu())
        {
            ShowPowerupMenu();
        }
    }

    private System.Collections.IEnumerator FindPlayerAfterDelay()
    {
        // waits for a frame to let dungeon generator spawn the player
        yield return null;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            
            // applies already chosen powerups to the newly spawned player
            ApplyChosenPowerups();
        }
    }

    private bool ShouldShowPowerupMenu()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        // shows the menu at start of each stage if there are powerups left to choose (rest will be red button, unavailable to be clicked)
        return (sceneName == "stage1" || sceneName == "stage2" || sceneName == "stage3") 
               && chosenPowerups.Count < 3;
    }

    private void ShowPowerupMenu()
    {
        Time.timeScale = 0f;
        powerupMenuUI.SetActive(true);
        
        // updates the button states
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

    // this is called when player is returning to cabin to reset for new run
    public static void ResetPowerups()
    {
        chosenPowerups.Clear();
        Debug.Log("Powerups reset for new run");
    }

    // checking if player has specific powerup
    public static bool HasPowerup(PowerupType powerupType)
    {
        return chosenPowerups.Contains(powerupType);
    }
}