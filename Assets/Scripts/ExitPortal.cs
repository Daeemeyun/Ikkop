using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortal : MonoBehaviour
{
    public string returnSceneName = "CabinHub";
    public GameObject promptPrefab;

    private GameObject promptInstance;
    private bool playerInRange = false;

    void Start()
    {
        if (promptPrefab != null)
        {
            Vector3 offsetPos = transform.position + new Vector3(0, 2f, 0);
            promptInstance = Instantiate(promptPrefab, offsetPos, Quaternion.identity);
            promptInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // when run is finished/returning to cabin, resets the powerups
            if (returnSceneName == "CabinHub")
            {
                PowerupMenuManager.ResetPowerups();
                
                // logic to also reset player powerups if player exists
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.ResetPowerups();
                    }
                }
            }
            
            SceneManager.LoadScene(returnSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptInstance != null)
        {
            playerInRange = true;
            promptInstance.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptInstance != null)
        {
            playerInRange = false;
            promptInstance.SetActive(false);
        }
    }
}