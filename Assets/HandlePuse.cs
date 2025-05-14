using UnityEngine;
using UnityEngine.InputSystem;
using UnityTutorial.PlayerControl;

public class HandlePuse : MonoBehaviour
{
    public static HandlePuse instance;
    public GameObject uiCanvas;

    private bool isActive = false;
    private PlayerController playerController;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    void Start()
    {
        uiCanvas.SetActive(isActive);
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isActive = !isActive;
            uiCanvas.SetActive(isActive);
            Time.timeScale = isActive ? 0f : 1f;

            // Disable the player controller component
            if (playerController != null)
                playerController.enabled = !isActive;

            // Show or hide cursor for UI
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isActive;
        }
    }

    public void EnableUI()
    {
        isActive = !isActive;
        uiCanvas.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;
        
        // Also disable player controller when UI is enabled
        if (playerController != null)
            playerController.enabled = !isActive;
            
        // Show or hide cursor for UI
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }
}