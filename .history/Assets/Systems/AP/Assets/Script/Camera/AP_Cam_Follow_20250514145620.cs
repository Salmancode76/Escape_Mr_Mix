using System.Collections;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{ // Reference to the character movement script public characterMovement characterMovementScript;

// Target transform to follow (should be the same as characterMovement's objCamera or a child of the player)

characterMovement characterMovementScript;

// Target transform to follow (should be the same as characterMovement's objCamera or a child of the player)
public Transform target;

// Player model to make face the camera direction (optional, only if different from rbBodyCharacter)
public Transform playerModel;

// Camera movement dampening
public float positionDamping = 15f;

// How quickly the player model turns to face camera direction (if playerModel is separate)
public float playerTurnSpeed = 8f;

// References
private Rigidbody playerRigidbody;
private Animator playerAnimator; // To check for Animator interference

void Start()
{
    // Get rigidbody and animator if player model is assigned
    if (playerModel != null)
    {
        playerRigidbody = playerModel.GetComponent<Rigidbody>();
        playerAnimator = playerModel.GetComponent<Animator>();
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
    
    // Update player facing direction (only for separate playerModel)
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
    if (characterMovementScript == null || characterMovementScript.objCamera == null) return;
    
    // Get the camera's Y rotation from characterMovement
    float yRotation = characterMovementScript.objCamera.localEulerAngles.y;
    Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
    
    // If playerModel is the same as rbBodyCharacter, let characterMovement handle rotation
    if (playerModel != null && playerRigidbody != null && playerRigidbody == characterMovementScript.rbBodyCharacter)
    {
        // Debug check for rotation mismatch
        if (Mathf.Abs(playerRigidbody.rotation.eulerAngles.y - yRotation) > 5f)
        {
            Debug.LogWarning($"Player rotation ({playerRigidbody.rotation.eulerAngles.y}) does not match camera Y ({yRotation})");
        }
        return; // Skip manual rotation, as characterMovement handles it via bodyRotation or bodyRotationMobile
    }
    
    // If playerModel is a separate transform (e.g., visual mesh), align it with camera
    if (playerModel != null)
    {
        // Check if Animator is overriding rotation
        if (playerAnimator != null && playerAnimator.applyRootMotion)
        {
            Debug.LogWarning("Animator applyRootMotion is enabled, which may override player rotation");
        }
        
        // Apply rotation to playerModel transform
        playerModel.rotation = Quaternion.Lerp(playerModel.rotation, 
                                             targetRotation, 
                                             Time.deltaTime * playerTurnSpeed);
        
        // Debug current rotation
        Debug.Log($"PlayerModel Y Rotation: {playerModel.eulerAngles.y}, Target Y Rotation: {yRotation}");
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