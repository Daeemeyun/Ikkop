using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int currentBones = 0;
    public TextMeshProUGUI boneText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddBones(int amount)
    {
        currentBones += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (boneText != null)
            boneText.text = "Bones: " + currentBones;
    }
}
