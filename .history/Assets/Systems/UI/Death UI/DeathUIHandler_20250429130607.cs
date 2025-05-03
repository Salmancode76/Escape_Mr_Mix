using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUIHandler : MonoBehaviour
{
    public AudioClip ClickClip;

    public void OnRetry()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        // Unfreeze time before reload
        Time.timeScale = 1f;

        // Reload current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }


    public void OnMainMenu()
    {
        SoundFXManager.instance.playSoundFXClip(ClickClip, transform, 1f);

        // Load scene index 0 (main menu)
        SceneManager.LoadScene(0);
    }


}
