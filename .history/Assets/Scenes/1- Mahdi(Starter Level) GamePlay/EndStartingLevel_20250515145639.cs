using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndStartingLevel : MonoBehaviour
{
    public PuseMenu PuseMenuInstance;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Debug.Log("Player has entered the trigger zone.");

            
            PuseMenuInstance.LoadNextLevel();


        }
    }
}

