using System.Collections;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    // Reference to the character movement script
    public characterMovement characterMovementScript;
    
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
        if (characterMovementScript != null && ingameGlobalManager.instance != null && characterMovementScript.ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogWarning("characterMovementScript or ingameGlobalManager is null or b_DesktopInputs not set. Cursor lock skipped.");
        }

        // Debug initial setup
        if (target == null) Debug.LogError("AP_Cam_Follow: Target is not assigned!");
        if (characterMovementScript == null) Debug.LogError("AP_Cam_Follow: characterMovementScript is not assigned!");
        if (characterMovementScript != null && characterMovementScript.objCamera == null) Debug.LogError("AP_Cam_Follow: characterMovementScript.objCamera is not assigned!");
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
            Vector3 previousPosition = transform.position;
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * positionDamping);
            
            // Debug position change
            if ((transform.position - previousPosition).magnitude < 0.001f)
            {
                Debug.LogWarning($"Camera not moving! Current pos: {transform.position}, Target pos: {target.position}, DeltaTime: {Time.deltaTime}, Damping: {positionDamping}");
            }
        }
        else
        {
            Debug.LogWarning("AP_Cam_Follow: Target is null, cannot follow position!");
        }
    }
    
    void UpdatePlayerFacing()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null || playerModel == null) 
        {
            Debug.LogWarning("AP_Cam_Follow: Skipping UpdatePlayerFacing due to null references (characterMovementScript, objCamera, or playerModel)");
            return;
        }
        
        // Get the camera's Y rotation from characterMovement
        float yRotation = characterMovementScript.objCamera.localEulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        
        // If playerModel is the same as rbBodyCharacter, let characterMovement handle rotation
        if (playerRigidbody != null && playerRigidbody == characterMovementScript.rbBodyCharacter)
        {
            // Debug check for rotation mismatch
            float playerY = playerRigidbody.rotation.eulerAngles.y;
            if (Mathf.Abs(playerY - yRotation) > 5f)
            {
                Debug.LogWarning($"Player rotation ({playerY}) does not match camera Y ({yRotation})");
            }
            return; // Skip manual rotation, as characterMovement handles it
        }
        
        // If playerModel is a separate transform (e.g., visual mesh), align it with camera
        if (playerModel != null)
        {
            // Check if Animator is overriding rotation
            if (playerAnimator != null && playerAnimator.applyRootMotion)
            {
                Debug.LogWarning("Animator applyRootMotion is enabled, which may override player rotation");
            }
            
            // Apply rotation to playerModel transform, accounting for parent rotation if any
            Quaternion parentRotation = playerModel.parent != null ? playerModel.parent.rotation : Quaternion.identity;
            Quaternion finalRotation = parentRotation * targetRotation;
            playerModel.rotation = Quaternion.Lerp(playerModel.rotation, finalRotation, Time.deltaTime * playerTurnSpeed);
            
            // Debug current rotation
            Debug.Log($"PlayerModel Y Rotation: {playerModel.eulerAngles.y}, Target Y Rotation: {yRotation}, Parent Y Rotation: {(playerModel.parent != null ? playerModel.parent.eulerAngles.y : 0)}");
        }
    }
    
    void SyncCameraRotation()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null) 
        {
            Debug.LogWarning("AP_Cam_Follow: Skipping SyncCameraRotation due to null characterMovementScript or objCamera");
            return;
        }
        
        // Sync this camera's rotation with the characterMovement's objCamera rotation
        Quaternion previousRotation = transform.rotation;
        transform.rotation = characterMovementScript.objCamera.rotation;
        
        // Debug rotation change
        if (Quaternion.Angle(previousRotation, transform.rotation) < 0.001f)
        {
            Debug.LogWarning($"Camera not rotating! Current rot: {transform.rotation.eulerAngles}, objCamera rot: {characterMovementScript.objCamera.rotation.eulerAngles}");
        }
    }
    
    // Public method to reset camera orientation
    public void ResetCamera()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null) 
        {
            Debug.LogWarning("AP_Cam_Follow: Cannot reset camera due to null characterMovementScript or objCamera");
            return;
        }
        
        characterMovementScript.mouseY = 0f; // Reset vertical rotation
        if (playerModel != null)
        {
            // Reset to player's current Y rotation
            characterMovementScript.objCamera.localEulerAngles = new Vector3(
                characterMovementScript.mouseY,
                playerModel.eulerAngles.y,
                0f);
        }
        
        // Apply the reset rotation to this camera
        SyncCameraRotation();
    }
}