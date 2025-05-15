using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public characterMovement characterMovementScript;
    public Transform target;
    public Transform playerModel;
    public float positionDamping = 15f;
    public float rotationSpeed = 100f;
    
    // Camera rotation limits
    public float upperLimit = -30f;
    public float bottomLimit = 70f;
    
    private float _xRotation = 0f;
    private Rigidbody _playerRigidbody;

    void Start()
    {
        if (playerModel != null)
        {
            _playerRigidbody = playerModel.GetComponent<Rigidbody>();
        }
        
        if (characterMovementScript != null && ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void LateUpdate()
    {
        if (characterMovementScript == null || target == null) return;
        
        FollowTarget();
        HandleCameraRotation();
        UpdatePlayerFacing();
    }
    
    void FollowTarget()
    {
        transform.position = Vector3.Lerp(
            transform.position, 
            target.position, 
            Time.deltaTime * positionDamping);
    }
    
    void HandleCameraRotation()
    {
        if (!ingameGlobalManager.instance.b_DesktopInputs) return;
        
        // Get mouse input
        float mouseX = characterMovementScript.GetMouseXInput();
        float mouseY = characterMovementScript.GetMouseYInput();
        
        // Rotate the camera horizontally (Y-axis rotation)
        transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);
        
        // Handle vertical rotation (X-axis)
        _xRotation -= mouseY * rotationSpeed * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, upperLimit, bottomLimit);
        
        // Apply the rotation
        transform.localEulerAngles = new Vector3(
            _xRotation,
            transform.localEulerAngles.y,
            0);
    }
    
    void UpdatePlayerFacing()
    {
        if (_playerRigidbody == null || characterMovementScript == null) return;
        
        // Get the camera's forward direction (ignoring pitch)
        Vector3 cameraForward = transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        if (cameraForward.sqrMagnitude > 0.1f)
        {
            // Create target rotation based on camera forward
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            
            // Smoothly rotate the player model
            _playerRigidbody.MoveRotation(Quaternion.Slerp(
                _playerRigidbody.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed));
        }
    }
    
    public void ResetCamera()
    {
        _xRotation = 0f;
        if (_playerRigidbody != null)
        {
            transform.localEulerAngles = new Vector3(0, _playerRigidbody.rotation.eulerAngles.y, 0);
        }
    }
}