using UnityEngine;
using UnityEngine.InputSystem;
using UnityTutorial.PlayerControl; // Make sure this matches your namespace

public class HandlePuse : MonoBehaviour
{
    public GameObject uiCanvas;

    private bool isActive = false;
    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isActive = !isActive;
            uiCanvas.SetActive(isActive);
            Time.timeScale = isActive ? 0f : 1f;

            if (playerController != null)
                playerController.InputEnabled = !isActive;

            // Show or hide cursor for UI
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isActive;
        }
    }
}
