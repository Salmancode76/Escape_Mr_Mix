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
            // 1) Original follow logic (unchanged)
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping); 
            // 2) Instant yaw‑sync: make player face camera direction
            if (playerModel != null)
            {
                Vector3 camForward = transform.forward;
                // camForward.y = 0;                // keep them upright
                // camForward.Normalize();
                if (camForward.sqrMagnitude > 0.01f)
                    playerModel.rotation = Quaternion.LookRotation(camForward);
            }
        }
    }
}
