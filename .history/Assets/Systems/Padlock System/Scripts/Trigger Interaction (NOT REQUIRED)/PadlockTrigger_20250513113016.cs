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
        [SerializeField] private GameObject canvasUI;

        private const string playerTag = "Player";
        private bool canUse;

        public bool HasUnlocked { get; private set; } = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                canUse = true;
                interactPrompt?.SetActive(true);
                canvasUI?.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                canUse = false;
                interactPrompt?.SetActive(false);
                canvasUI?.SetActive(false);
            }
        }

        private void Update()
        {
            if (canUse && Input.GetKeyDown(PLInputManager.instance.triggerInteractKey))
            {
                padlockController.ShowPadlock();
                // DO NOT call HandlePadlockOpened here
            }
        }

        // Called by PadlockController when the lock is solved
        public void HandlePadlockOpened()
        {
            HasUnlocked = true;

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            interactPrompt?.SetActive(false);
            canvasUI?.SetActive(false);
            canUse = false;
        }
    }
}
