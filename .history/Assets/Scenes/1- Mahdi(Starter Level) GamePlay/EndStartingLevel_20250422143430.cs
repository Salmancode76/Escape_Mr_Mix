using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class EndStartingLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PuseMenu.instance != null)
            {
                PuseMenu.instance.LoadNextLevel();
            }
            else
            {
                Debug.LogError("PuseMenu.instance is null. Make sure PuseMenu is in the scene and has an Awake() method setting the instance.");
            }
        }
    }
}

