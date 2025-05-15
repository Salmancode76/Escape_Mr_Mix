using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPipSolved : MonoBehaviour
{
    public actionsWhenPuzzleIsSolved puzzleScript; // Reference to the puzzle script
    public Light pointLightToEnable; // Point Light to enable
    public GameObject objectToEnable; // Object to enable
    public AudioClip openLightSound; // Sound to play when the light is enabled

    private void Update()
    {
        if (puzzleScript != null && puzzleScript.b_actionsWhenPuzzleIsSolved)
        {
            if (pointLightToEnable != null && !pointLightToEnable.enabled)
            {
                pointLightToEnable.enabled = true;
                objectToEnable.SetActive(true);

                SoundFXManager.instance.playSoundFXClip(openLightSound, transform, 1f);
            }
        }
    }
}
