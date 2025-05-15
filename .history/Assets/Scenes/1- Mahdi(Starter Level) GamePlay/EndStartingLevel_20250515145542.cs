using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndStartingLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log("Player has entered the trigger zone.");

            
                PuseMenu.instance.LoadNextLevel();


        }
    }
}

