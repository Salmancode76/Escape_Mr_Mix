// Description : Cam_Follow.cs : use on camera to follow the player character
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public Transform target;
    public float rotationDamping = 15f;

    [Header("Player Facing Sync")]
    public Transform playerModel;

    [Header("Mouse Control")]
    public float mouseSensitivity = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // 1) Smooth follow position only
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);

        // 2) Mouse input for camera rotation (not from target)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -60f, 60f);

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraRotation, Time.deltaTime * rotationDamping);

        // 3) Make player face camera direction instantly (yaw only)
        if (playerModel != null)
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            if (forward.sqrMagnitude > 0.01f)
                playerModel.rotation = Quaternion.LookRotation(forward);
        }
    }
}
