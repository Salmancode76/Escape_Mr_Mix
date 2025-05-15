using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public Transform target; // The player GameObject
    public float rotationDamping = 15f;
    
    [Header("Player Model")]
    public Transform playerModel; // The model with the Animator
    
    // Sensitivity for mouse movement
    public float horizontalSensitivity = 21.9f; // Horizontal mouse sensitivity (for the player rotation)
    public float verticalSensitivity = 21.9f;   // Vertical mouse sensitivity (for the camera rotation)
    
    // Limits for camera rotation (vertical axis)
    public float upperLimit = -40f;
    public float bottomLimit = 70f;
    
    private float _xRotation = 0f;  // Vertical rotation of the camera
    private float _yRotation = 0f;  // Horizontal rotation tracking
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Mouse Input for Camera and Player Rotation
        float mx = Input.GetAxis("Mouse X") * horizontalSensitivity; // Horizontal mouse movement (left/right)
        float my = Input.GetAxis("Mouse Y") * verticalSensitivity; // Vertical mouse movement (up/down)
        
        // Update rotation values
        _yRotation += mx;
        _xRotation -= my;
        _xRotation = Mathf.Clamp(_xRotation, upperLimit, bottomLimit); // Clamp vertical rotation
        
        // Set camera's rotation directly (instant vertical rotation)
        transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        
        // Set player model rotation instantly to match camera's horizontal direction
        if (playerModel != null)
        {
            playerModel.rotation = Quaternion.Euler(0f, _yRotation, 0f);
        }
        
        // Smooth follow position for camera
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);
    }
}