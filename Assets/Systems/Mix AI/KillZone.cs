using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Kill Zone - Instant Death!");

            // Reference to your MixAIController
            MixAIController mixAI = GetComponentInParent<MixAIController>();
            if (mixAI != null)
            {
                mixAI.InstantKillPlayer();
            }
        }
    }
}

