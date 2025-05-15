using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElevatorTrigger : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "Play";

    // Call this to start the animation
    public void PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning("Animator not assigned on " + gameObject.name);
        }
    }
}
