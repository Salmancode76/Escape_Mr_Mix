using System.Collections;
using System.Collections.Generic;
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

        }
    }
}

