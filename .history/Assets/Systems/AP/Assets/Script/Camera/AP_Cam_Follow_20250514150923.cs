﻿using System.Collections;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    // Reference to the character movement script
    public characterMovement characterMovementScript;
    
    // Target transform to follow (must be characterMovement's objCamera)
    public Transform target;
    
    // Player model to make face the camera direction (optional, only if different from rbBodyCharacter)
    public Transform playerModel;
    
    // Camera movement dampening
    public float positionDamping = 15f;
    
    // How quickly the player model turns to face camera direction (if playerModel is separate)
    public float playerTurnSpeed = 8f;
    
    // References
    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    void Start()
    {
        // Get rigidbody and animator if player model is assigned
        if (playerModel != null)
        {
            playerRigidbody = playerModel.GetComponent<Rigidbody>();
            playerAnimator = playerModel.GetComponent<Animator>();
        }
        
        // Lock and hide cursor for desktop inputs
        if (characterMovementScript != null  && ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogWarning("AP_Cam_Follow: Cursor lock skipped. Check characterMovementScript or ingameGlobalManager.");
        }

        // Validate assignments
        if (target == null) Debug.LogError("AP_Cam_Follow: Target is not assigned!");
        if (characterMovementScript == null) Debug.LogError("AP_Cam_Follow: characterMovementScript is not assigned!");
        if (characterMovementScript != null && characterMovementScript.objCamera == null) Debug.LogError("AP_Cam_Follow: characterMovementScript.objCamera is not assigned!");
        if (target != characterMovementScript.objCamera) Debug.LogWarning("AP_Cam_Follow: Target should match characterMovementScript.objCamera for consistent behavior.");
    }
    
    void LateUpdate()
    {
        // Follow the target position
        FollowTarget();
        
        // Update player facing direction (only for separate playerModel)
        UpdatePlayerFacing();
        
        // Synchronize camera rotation with characterMovement's rotation
        SyncCameraRotation();
    }
    
    void FollowTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("AP_Cam_Follow: Target is null, cannot follow position!");
            return;
        }

        // Smoothly move to target position
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * positionDamping);
        
        // Debug position change
        if ((transform.position - previousPosition).magnitude < 0.001f)
        {
            Debug.LogWarning($"AP_Cam_Follow: Camera not moving! Current pos: {transform.position}, Target pos: {target.position}, DeltaTime: {Time.deltaTime}, Damping: {positionDamping}");
        }
        else
        {
            Debug.Log($"AP_Cam_Follow: Camera moved to {transform.position}, Target pos: {target.position}");
        }
    }
    
    void UpdatePlayerFacing()
{
    if (playerModel == null || characterMovementScript == null || characterMovementScript.objCamera == null)
        return;

    // Get Y rotation from camera using Transform.eulerAngles
    float yRotation = characterMovementScript.objCamera.eulerAngles.y;

    Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);

    if (playerRigidbody != null)
    {
        playerRigidbody.MoveRotation(Quaternion.Lerp(playerRigidbody.rotation,
                                                     targetRotation,
                                                     Time.deltaTime * playerTurnSpeed));
    }
    else
    {
        playerModel.rotation = Quaternion.Lerp(playerModel.rotation,
                                               targetRotation,
                                               Time.deltaTime * playerTurnSpeed);
    }
}

    
    void SyncCameraRotation()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null) 
        {
            Debug.LogWarning("AP_Cam_Follow: Skipping SyncCameraRotation due to null references");
            return;
        }
        
        // Sync camera rotation
        Quaternion previousRotation = transform.rotation;
        transform.rotation = characterMovementScript.objCamera.rotation;
        
        // Debug rotation change
        if (Quaternion.Angle(previousRotation, transform.rotation) < 0.001f)
        {
            Debug.LogWarning($"AP_Cam_Follow: Camera not rotating! Current rot: {transform.rotation.eulerAngles}, objCamera rot: {characterMovementScript.objCamera.rotation.eulerAngles}");
        }
        else
        {
            Debug.Log($"AP_Cam_Follow: Camera rotated to {transform.rotation.eulerAngles}, objCamera rot: {characterMovementScript.objCamera.rotation.eulerAngles}");
        }
    }
    
    public void ResetCamera()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null) 
        {
            Debug.LogWarning("AP_Cam_Follow: Cannot reset camera due to null references");
            return;
        }
        
        characterMovementScript.mouseY = 0f;
        if (playerModel != null)
        {
            characterMovementScript.objCamera.localEulerAngles = new Vector3(
                characterMovementScript.mouseY,
                playerModel.eulerAngles.y,
                0f);
        }
        
        SyncCameraRotation();
    }
}