// GameStarterTrigger.cs
using UnityEngine;
using TMPro;

public class GameStarterTrigger : MonoBehaviour
{
    public TextMeshProUGUI instructionText; // 🔥 Assign in Inspector
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if (instructionText != null)
            {
                instructionText.color = Color.yellow;
                instructionText.text = "Memorize the lights. Press them in order or die.";
                instructionText.gameObject.SetActive(false);
                instructionText.gameObject.SetActive(true);
            }

            if (MemoryGameManager.instance != null)
            {
                MemoryGameManager.instance.StartCoroutine(DelayedStart());
            }
        }
    }

    private System.Collections.IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(10f);
        MemoryGameManager.instance.StartCoroutine("StartGameWithDelay");
    }
}