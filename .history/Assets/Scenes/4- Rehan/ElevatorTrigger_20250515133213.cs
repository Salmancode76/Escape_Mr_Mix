using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElevatorTrigger : MonoBehaviour
{
    private Animation anim;

    void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim != null)
            anim.enabled = false; // Start disabled
    }

    // Call this to enable and play the animation
    public void EnableAndPlay()
    {
        if (anim != null)
        {
            anim.enabled = true;
            anim.Play();
        }
        else
        {
            Debug.LogWarning("No Animation component found on " + gameObject.name);
        }
    }
}