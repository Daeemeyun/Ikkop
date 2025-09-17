using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems; //assign the start, options, quit in inspector
    public GameObject menuPanel;
    public GameObject optionsPanel;
    public TextMeshProUGUI backText;
    public TextMeshProUGUI musicLabel, sfxLabel;
    public Slider musicSlider, sfxSlider;

    public SFXPlayer sfxPlayer;

    private int selectedIndex = 0;
    private int optionsIndex = 0;
    private TextMeshProUGUI[] optionsItems;
    private bool inOptions = false;
    private bool adjustingSlider = false;

    void Start()
    {
        UpdateMenuSelection();
        optionsPanel.SetActive(false);

        optionsItems = new TextMeshProUGUI[] { musicLabel, sfxLabel, backText };

        musicSlider.value = 1.0f;
        sfxSlider.value = 1.0f;

        musicLabel.text = "Music Volume: 100%";
        sfxLabel.text = "SFX Volume: 100%";
    }

    void Update()
    {
        if (inOptions)
        {
            if (!adjustingSlider)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    optionsIndex = (optionsIndex - 1 + optionsItems.Length % optionsItems.Length);
                    sfxPlayer.PlayClick();
                    UpdateOptionsSelection();
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    optionsIndex = (optionsIndex + 1) % optionsItems.Length;
                    sfxPlayer.PlayClick();
                    UpdateOptionsSelection();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    sfxPlayer.PlayClick();
                    if (optionsIndex == 2) //backtext
                        CloseOptions();
                    else
                        adjustingSlider = true; //enters slider adjustment mode
                }
            }
            else
            {
                //using A/D keys to change value
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    AdjustSlider(Input.GetKeyDown(KeyCode.A) ? -0.1f : 0.1f);
                    sfxPlayer.PlayClick();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    sfxPlayer.PlayClick();
                    adjustingSlider = false; //exits slider adjustment mode
                }
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
            MoveSelection(-1);
        else if (Input.GetKeyDown(KeyCode.S))
            MoveSelection(1);
        else if (Input.GetKeyDown(KeyCode.Space))
            SelectItem();  

        UpdateMenuSelection();
    }

    void MoveSelection(int dir)
    {
        selectedIndex = (selectedIndex + dir + menuItems.Length) % menuItems.Length;
        sfxPlayer.PlayClick();
        UpdateMenuSelection();
    }

    private float normalFontSize = 36f;
    private float selectedFontSize = 48f;
    private float fontLerpSpeed = 40f;

    void UpdateMenuSelection()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            bool isSelected = (i == selectedIndex);

            //color transition
            menuItems[i].color = isSelected ? Color.yellow : Color.white;

            //smooth font size transition
            float targetSize = isSelected ? selectedFontSize : normalFontSize;
            float currentSize = menuItems[i].fontSize;
            menuItems[i].fontSize = Mathf.MoveTowards(currentSize, targetSize, fontLerpSpeed * Time.deltaTime);
        }
    }

    void UpdateOptionsSelection()
    {
        for (int i = 0; i < optionsItems.Length; i++)
        {
            bool isSelected = (i == optionsIndex);

            //highlighting selected label
            optionsItems[i].color = isSelected ? Color.yellow : Color.white;

            //smooth font size change (same as main menu)
            float targetSize = isSelected ? selectedFontSize : normalFontSize;
            float currentSize = optionsItems[i].fontSize;
            optionsItems[i].fontSize = Mathf.MoveTowards(currentSize, targetSize, fontLerpSpeed * Time.deltaTime);
        }
    }

    void SelectItem()
    {
        sfxPlayer.PlayClick();
        string selection = menuItems[selectedIndex].name;
        if (selection.Contains("Start"))
            SceneManager.LoadScene("CabinHub");
        else if (selection.Contains("Options"))
            OpenOptions();
        else if (selection.Contains("Quit"))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            # else
                Application.Quit();
            #endif
        }    
    }

    void OpenOptions()
    {
        inOptions = true;
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        optionsIndex = 0;
        UpdateOptionsSelection();
        sfxPlayer.PlayClick();
    }

    void CloseOptions()
    {
        inOptions = false;
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        sfxPlayer.PlayClick();
    }

    void AdjustSlider(float change)
    {
        if (optionsIndex == 0) //music
        {
            musicSlider.value = Mathf.Clamp(musicSlider.value + change, 0f, 1f);
            int percentage = Mathf.RoundToInt(musicSlider.value * 100f);
            musicLabel.text = $"Music Volume: {percentage}%";

            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        }
        else if (optionsIndex == 1) //sfx
        {
            sfxSlider.value = Mathf.Clamp(sfxSlider.value + change, 0f, 1f);
            int percentage = Mathf.RoundToInt(sfxSlider.value * 100f);
            sfxLabel.text = $"SFX Volume: {percentage}%";

            PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        }
    }
}
