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

        public bool HasUnlocked { get; private set; } = false;

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

        private void HandlePadlockOpened()
        {
            HasUnlocked = true;

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
            if (canvasUI != null)
            {
                canvasUI.SetActive(false);
            }

            canUse = false;
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
                HandlePadlockOpened();
            }
        }

    }
}


