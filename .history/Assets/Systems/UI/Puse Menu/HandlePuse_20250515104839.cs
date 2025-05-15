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

            if (playerController != null)
                playerController.InputEnabled = !isActive;

            // Check and update cursor visibility and lock state
            if (isActive)
            {
                if (!Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                if (Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }

    public void EnableUI()
    {
        isActive = !isActive;
        uiCanvas.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;

        if (playerController != null)
            playerController.InputEnabled = !isActive;

        if (isActive)
        {
            if (!Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
