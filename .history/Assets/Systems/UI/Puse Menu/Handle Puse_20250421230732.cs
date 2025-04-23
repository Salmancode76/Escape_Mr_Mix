using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePuse : MonoBehaviour
{
    public GameObject uiCanvas; // Assign your Canvas GameObject here

    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActive = !isActive;
            uiCanvas.SetActive(isActive);

            // Optional: pause the game when menu is active
            Time.timeScale = isActive ? 0f : 1f;
        }
    }
}
