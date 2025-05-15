using System.Collections;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    // Reference to the character movement script
    public characterMovement characterMovementScript;
    
    // Target transform to follow (should be the same as characterMovement's objCamera or a child of the player)
    public Transform target;
    
    // Player model to make face the camera direction
    public Transform playerModel;
    
    // Camera movement dampening
    public float positionDamping = 15f;
    
    // How quickly the player model turns to face camera direction
    public float playerTurnSpeed = 8f;
    
    // References
    private Rigidbody playerRigidbody;

    void Start()
    {
        // Get rigidbody if player model is assigned
        if (playerModel != null)
        {
            playerRigidbody = playerModel.GetComponent<Rigidbody>();
        }
        
        // Lock and hide cursor for desktop inputs
        if (characterMovementScript != null && ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void LateUpdate()
    {
        // Follow the target position
        FollowTarget();
        
        // Update player facing direction to align with camera
        UpdatePlayerFacing();
        
        // Synchronize camera rotation with characterMovement's rotation
        SyncCameraRotation();
    }
    
    void FollowTarget()
    {
        // Follow target position if target exists
        if (target != null)
        {
            // Smoothly move to target position
            transform.position = Vector3.Lerp(transform.position, 
                                             target.position, 
                                             Time.deltaTime * positionDamping);
        }
    }
    
    void UpdatePlayerFacing()
    {
        if (playerModel == null || characterMovementScript == null || characterMovementScript.objCamera == null)
            return;

        // Get the direction from player to camera (ignoring vertical component)
        Vector3 cameraForward = characterMovementScript.objCamera.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        // Use Rigidbody if available
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
        if (characterMovementScript == null || characterMovementScript.objCamera == null) return;
        
        // Sync this camera's rotation with the characterMovement's objCamera rotation
        transform.rotation = characterMovementScript.objCamera.rotation;
    }
    
    // Public method to reset camera orientation
    public void ResetCamera()
    {
        if (characterMovementScript != null)
        {
            characterMovementScript.mouseY = 0f; // Reset vertical rotation
            if (playerModel != null)
            {
                // Reset to player's current Y rotation
                characterMovementScript.objCamera.localEulerAngles = new Vector3(
                    characterMovementScript.mouseY,
                    playerModel.eulerAngles.y,
                    0f);
            }
        }
        
        // Apply the reset rotation to this camera
        SyncCameraRotation();
    }
}