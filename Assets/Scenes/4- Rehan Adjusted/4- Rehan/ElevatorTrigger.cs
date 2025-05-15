using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElevatorTrigger : MonoBehaviour
{    public actionsWhenPuzzleIsSolved puzzleScript; // Reference to the puzzle script
    public Animation animToEnable; // Animation component to enable

    private void Update()
    {
        if (puzzleScript != null && puzzleScript.b_actionsWhenPuzzleIsSolved)
        {
            if (animToEnable != null && !animToEnable.enabled)
            {
                animToEnable.enabled = true;
                animToEnable.Play();
            }
        }
    }
}
