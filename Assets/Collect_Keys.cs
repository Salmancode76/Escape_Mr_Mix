using UnityEngine;
using TMPro;

public class Collect_Keys : MonoBehaviour
{
    [Header("Key Collection Settings")]
    public int totalKeys = 6;
    private int collectedKeys = 0;

    [Header("UI")]
    public TMP_Text keyScoreText; // Assign this in the Inspector


    public void AddKey()
    {
        collectedKeys++;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (keyScoreText == null)
        {
            Debug.LogWarning("⚠️ keyScoreText is null! Can't update UI.");
            return;
        }

        if (collectedKeys >= totalKeys)
        {
            keyScoreText.text = "All Keys Collected!";
        }
        else
        {
            keyScoreText.text = $"Keys: {collectedKeys} / {totalKeys}";
        }
    }
}
