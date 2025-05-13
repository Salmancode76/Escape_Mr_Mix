using UnityEngine;

namespace PadlockSystem
{
    public class PadlockTrigger : MonoBehaviour
    {
        [Header("Padlock Controller Object")]
        [SerializeField] private PadlockController padlockController = null;

        [Header("UI Prompt")]
        [SerializeField] public GameObject interactPrompt;

        [Header("Canvas UI")]
        [SerializeField] private GameObject canvasUI; // New reference

        private const string playerTag = "Player";
        private bool canUse;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                canUse = true;
                interactPrompt.SetActive(true);

                if (canvasUI != null)
                {
                    canvasUI.SetActive(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                canUse = false;
                interactPrompt.SetActive(false);

                if (canvasUI != null)
                {
                    canvasUI.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (canUse && Input.GetKeyDown(PLInputManager.instance.triggerInteractKey))
            {
                padlockController.ShowPadlock();
            }
        }
    }
}


