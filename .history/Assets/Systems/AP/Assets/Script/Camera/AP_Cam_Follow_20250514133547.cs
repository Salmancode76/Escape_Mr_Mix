using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour 
{
    [Header("Target Settings")]
    public Transform target;
    public float positionSmoothTime = 0.1f;
    
    [Header("Camera Look Settings")]
    public float mouseSensitivity = 21.9f;
    public float upperLookLimit = -40f;
    public float bottomLookLimit = 70f;
    
    [Header("Player Rotation Settings")]
    public Transform playerModel;
    public float rotationSmoothSpeed = 8.9f;
    public bool enableDebugDisplay = false;
    
    // Private variables for camera control
    private float xRotation = 0f;
    private Vector3 positionVelocity = Vector3.zero;
    private Rigidbody playerRigidbody;
    
    private void Start()
    {
        // Cache references 
        if (target != null && target.GetComponent<Rigidbody>() != null)
        {
            playerRigidbody = target.GetComponent<Rigidbody>();
        }
        
        // Lock and hide cursor - common for FPS games
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Move camera to follow target position with smooth damping
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            target.position, 
            ref positionVelocity, 
            positionSmoothTime
        );
        
        // Handle camera rotation based on mouse input (like in your PlayerController)
        HandleCameraRotation();
    }
    
    private void HandleCameraRotation()
    {
        if (playerRigidbody == null) return;
        
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Calculate new rotation values
        xRotation = Mathf.Clamp(xRotation - mouseY * mouseSensitivity * Time.smoothDeltaTime, 
                               upperLookLimit, bottomLookLimit);
        
        // Apply vertical rotation to camera (looking up/down)
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        
        // Apply horizontal rotation to player body
        if (Mathf.Abs(mouseX) > 0.01f) 
        {
            // Rotate the player's rigidbody horizontally (like in PlayerController)
            playerRigidbody.MoveRotation(playerRigidbody.rotation * 
                Quaternion.Euler(0, mouseX * mouseSensitivity * Time.smoothDeltaTime, 0));
            
            // Also update player model if it's different from the rigidbody owner
            if (playerModel != null && playerModel.transform != target)
            {
                Vector3 targetForward = playerRigidbody.rotation * Vector3.forward;
                targetForward.y = 0f;
                
                if (targetForward.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetForward.normalized);
                    playerModel.rotation = Quaternion.Slerp(
                        playerModel.rotation,
                        targetRotation,
                        rotationSmoothSpeed * Time.deltaTime
                    );
                }
            }
        }
    }
    
    // Public method to reset camera position and rotation
    public void ResetCamera()
    {
        if (target == null) return;
        
        // Reset position
        transform.position = target.position;
        
        // Reset rotation
        xRotation = 0f;
        transform.localRotation = Quaternion.identity;
        
        // Reset player model if necessary
        if (playerModel != null && playerRigidbody != null)
        {
            Vector3 forward = playerRigidbody.rotation * Vector3.forward;
            forward.y = 0f;
            
            if (forward.sqrMagnitude > 0.001f)
            {
                playerModel.rotation = Quaternion.LookRotation(forward.normalized);
            }
        }
    }
    
    // Debug visualization 
    private void OnDrawGizmos()
    {
        if (!enableDebugDisplay) return;
        
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            
            // Draw camera view direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
            
            // Draw player model direction if available
            if (playerModel != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(playerModel.position, playerModel.forward * 2f);
            }
            
            // Draw up/down look limits
            Gizmos.color = Color.red;
            Vector3 upLimitDir = Quaternion.Euler(upperLookLimit, 0, 0) * Vector3.forward;
            Vector3 downLimitDir = Quaternion.Euler(bottomLookLimit, 0, 0) * Vector3.forward;
            Gizmos.DrawRay(transform.position, upLimitDir * 1.5f);
            Gizmos.DrawRay(transform.position, downLimitDir * 1.5f);
        }
    }
}