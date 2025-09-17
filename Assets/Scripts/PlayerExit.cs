using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerExit : MonoBehaviour
{
    private bool isNearExit = false;
    private TextMeshProUGUI advanceText;

    public SFXPlayer sfxPlayer;

    void Update()
    {
        if (isNearExit && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Loading dungeon...");
            sfxPlayer.PlayDoor();
            SceneManager.LoadScene("stage1"); //to be changed to dungeon scene name
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            isNearExit = true;

            //floating text for ExitDoor
            advanceText = other.GetComponentInChildren<TextMeshProUGUI>();
            if (advanceText != null)
                advanceText.text = "Press E to advance";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            isNearExit = false;

            if (advanceText != null)
                advanceText.text = "";
        }
    }
}
