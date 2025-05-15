using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour 
{
    // Target transform to follow (like a camera root)
    public Transform target;
    
    // Player model to make face the camera direction
    public Transform playerModel;
    
    // Camera movement dampening
    public float rotationDamping = 15f;
    
    // How quickly the player model turns to face camera direction
    public float playerTurnSpeed = 8f;
    
    // Mouse sensitivity settings
    [SerializeField] private float mouseSensitivity = 21.9f;
    [SerializeField] private float upperLookLimit = -40f;
    [SerializeField] private float bottomLookLimit = 70f;
    
    // Rotation tracking
    private float xRotation = 0f;
    private float yRotation = 0f;
    
    // References
    private Rigidbody playerRigidbody;
    
    void Start()
    {
        // Setup initial rotation values if target exists
        if (target != null)
        {
            Vector3 angles = target.eulerAngles;
            xRotation = angles.x;
            yRotation = angles.y;
        }
        
        // Get rigidbody if player model is assigned
        if (playerModel != null)
        {
            playerRigidbody = playerModel.GetComponent<Rigidbody>();
        }
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void LateUpdate()
    {
        // First handle mouse input for camera rotation
        HandleMouseLook();
        
        // Then follow the target if one is assigned
        FollowTarget();
        
        // Make player model face camera direction
        UpdatePlayerFacing();
    }
    
    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Calculate rotation values
        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        yRotation += mouseX * mouseSensitivity * Time.deltaTime;
        
        // Clamp vertical rotation
        xRotation = Mathf.Clamp(xRotation, upperLookLimit, bottomLookLimit);
        
        // Apply rotation directly to camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
    
    void FollowTarget()
    {
        // Follow target position if target exists
        if (target != null)
        {
            // Smoothly move to target position
            transform.position = Vector3.Lerp(transform.position, 
                                             target.position, 
                                             Time.deltaTime * rotationDamping);
        }
    }
    
    void UpdatePlayerFacing()
    {
        if (playerModel == null) return;
        
        // Make player face the same horizontal direction as the camera
        Quaternion targetRotation = Quaternion.Euler(0f, yRotation, 0f);
        
        // Use rigidbody for rotation if available (physics-based movement)
        if (playerRigidbody != null)
        {
            playerRigidbody.MoveRotation(Quaternion.Lerp(playerRigidbody.rotation, 
                                                       targetRotation, 
                                                       Time.deltaTime * playerTurnSpeed));
        }
        // Otherwise use transform directly
        else
        {
            playerModel.rotation = Quaternion.Lerp(playerModel.rotation, 
                                                 targetRotation, 
                                                 Time.deltaTime * playerTurnSpeed);
        }
    }
    
    // Public method to reset camera orientation
    public void ResetCamera()
    {
        xRotation = 0f;
        
        if (playerModel != null)
        {
            yRotation = playerModel.eulerAngles.y;
        }
        
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}