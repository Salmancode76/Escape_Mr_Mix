using UnityEngine;
using TMPro;

public class Collect_Keys : MonoBehaviour
{
    public int totalKeys = 6;
    private int collectedKeys = 0;

    public TMP_Text keyScoreText;
    public GameObject doorObject;  // Drag door GameObject here

    public float openRotationY = 90f; // How much to rotate (door opening angle)
    public float openSpeed = 2f;

    private bool doorShouldOpen = false;
    private Quaternion targetRotation;

    void Update()
    {
        // Smoothly rotate door when triggered
        if (doorShouldOpen && doorObject != null)
        {
            doorObject.transform.rotation = Quaternion.Lerp(
                doorObject.transform.rotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );
        }
    }

    public void AddKey()
    {
        collectedKeys++;
        UpdateUI();

        if (collectedKeys >= totalKeys)
        {
            OpenDoor();
        }
    }

    void UpdateUI()
    {
        if (keyScoreText != null)
        {
            keyScoreText.text = collectedKeys >= totalKeys
                ? "All Keys Collected!"
                : $"Keys: {collectedKeys} / {totalKeys}";
        }
    }

    void OpenDoor()
    {
        Debug.Log("✅ All keys collected — rotating the door open");
        if (doorObject != null)
        {
            targetRotation = Quaternion.Euler(0, openRotationY, 0);
            doorShouldOpen = true;
        }
        else
        {
            Debug.LogWarning("🚪 Door GameObject not assigned!");
        }
    }
}
