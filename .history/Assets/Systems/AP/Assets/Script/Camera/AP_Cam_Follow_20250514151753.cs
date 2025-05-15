using System.Collections;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    // Reference to the character movement script
    public characterMovement characterMovementScript;
    
    // Target transform to follow (must be characterMovement's objCamera)
    public Transform target;
    
    // Player model to make face the camera direction
    public Transform playerModel;
    
    // Camera movement dampening
    public float positionDamping = 15f;
    
    // Mouse sensitivity (matches reference implementation)
    public float mouseSensitivity = 100f;
    
    // Camera pitch limits
    public float upperLimit = -30f;
    public float bottomLimit = 70f;
    
    // Current camera pitch
    private float _xRotation = 0f;
    
    // References
    private Rigidbody _playerRigidbody;
    private Animator _playerAnimator;

    void Start()
    {
        if (playerModel != null)
        {
            _playerRigidbody = playerModel.GetComponent<Rigidbody>();
            _playerAnimator = playerModel.GetComponent<Animator>();
        }
        
        if (characterMovementScript != null && ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (target == null) Debug.LogError("AP_Cam_Follow: Target is not assigned!");
        if (characterMovementScript == null) Debug.LogError("AP_Cam_Follow: characterMovementScript is not assigned!");
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
        // Smooth position follow
        transform.position = Vector3.Lerp(
            transform.position, 
            target.position, 
            Time.deltaTime * positionDamping);
    }
    
    void HandleCameraRotation()
    {
        // Get mouse input from character movement script
        float mouseX = characterMovementScript.GetMouseXInput();
        float mouseY = characterMovementScript.GetMouseYInput();
        
        // Handle camera pitch
        _xRotation = Mathf.Clamp(
            _xRotation - mouseY * mouseSensitivity * Time.deltaTime,
            upperLimit,
            bottomLimit);
        
        // Apply camera rotation
        transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }
    
    void UpdatePlayerFacing()
    {
        if (_playerRigidbody == null || characterMovementScript == null) return;
        
        // Get mouse input from character movement script
        float mouseX = characterMovementScript.GetMouseXInput();
        
        // Rotate player based on horizontal mouse movement (matches reference implementation)
        _playerRigidbody.MoveRotation(
            _playerRigidbody.rotation * 
            Quaternion.Euler(0, mouseX * mouseSensitivity * Time.deltaTime, 0));
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