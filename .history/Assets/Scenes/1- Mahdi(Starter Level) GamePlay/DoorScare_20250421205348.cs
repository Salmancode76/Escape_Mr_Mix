using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScare : MonoBehaviour
{
    public AudioSource audioSource; // Assign in Inspector
    public string playerTag = "Player"; // Make sure your player GameObject is tagged as "Player"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (audioSource != null && !audioSource.enabled)
            {
                audioSource.enabled = true;
            }
        }
    }
}
