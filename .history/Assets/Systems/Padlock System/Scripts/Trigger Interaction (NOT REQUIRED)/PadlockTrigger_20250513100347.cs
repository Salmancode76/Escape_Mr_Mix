using UnityEngine;

namespace PadlockSystem 
{
    public class PadlockTrigger : MonoBehaviour
    {
        [Header("Padlock Controller Object")]
        [SerializeField] private PadlockController padlockController = null;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private GameObject canvasUI; // New reference for canvas UI
        
        private const string playerTag = "Player";
        private bool canUse;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                canUse = true;
                interactPrompt.SetActive(true);
                
                // Enable the canvas UI when player enters trigger
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
                
                // Disable the canvas UI when player exits trigger
                if (canvasUI != null)
                {
                    canvasUI.SetActive(false);
                }
            }
        }
        
        private void Update()
        {
            if (canUse)
            {
                if (Input.GetKeyDown(PLInputManager.instance.triggerInteractKey))
                {
                    padlockController.ShowPadlock();
                }
            }
        }
    }
}