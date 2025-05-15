// Description : Cam_Follow.cs : use on camera to follow the player character
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour {
    
    public Transform target;
    public float     rotationDamping = 15;        

    [Header("Player Facing Sync")]
    public Transform playerModel;   // Assign your character’s root or model here

    void LateUpdate()
{
    if (target != null)
    {
        // position still smooth
        transform.position = Vector3.Lerp(
            transform.position, 
            target.position, 
            Time.deltaTime * rotationDamping
        );

        // restore instant camera rotation (no smoothing)
        transform.rotation = target.rotation;

        // instant yaw‑sync: make player face camera direction
        if (playerModel != null)
        {
            Vector3 camForward = transform.forward;
            camForward.y = 0;
            camForward.Normalize();
            if (camForward.sqrMagnitude > 0.01f)
                playerModel.rotation = Quaternion.LookRotation(camForward);
        }
    }
}

}
