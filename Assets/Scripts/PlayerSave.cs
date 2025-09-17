using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSave : MonoBehaviour
{
    private bool isNearSavePoint = false;
    private TextMeshProUGUI saveText;
    
    public SFXPlayer sfxPlayer;

    void Update()
    {
        if (isNearSavePoint && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Game Saved!");
            sfxPlayer.PlaySave();
            ShowSaveMessage("Game Saved!");

            // hide the message after 2 seconds
            Invoke("ClearMessage", 2f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SavePoint"))
        {
            isNearSavePoint = true;
            
            // auto-locates the floating SaveText inside the save portal (want to make it a dog bed in the future)
            saveText = other.GetComponentInChildren<TextMeshProUGUI>();

            if (saveText != null)
                ShowSaveMessage("Press E to save");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("SavePoint"))
        {
            isNearSavePoint = false;
            ClearMessage();
        }
    }

    void ShowSaveMessage(string message)
    {
        if (saveText != null)
            saveText.text = message;
    }

    void ClearMessage()
    {
        if (saveText != null)
            saveText.text = "";
    }
}
