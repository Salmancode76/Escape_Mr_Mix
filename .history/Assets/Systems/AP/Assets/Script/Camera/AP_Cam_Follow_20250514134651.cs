using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private Transform Camera;
    
    [Header("Camera Settings")]
    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float BottomLimit = 70f;
    [SerializeField] private float MouseSensitivity = 21.9f;
    
    [Header("Player Settings")]
    [SerializeField] private Transform playerModel;
    [SerializeField] private bool enableDebugDisplay = false;
    
    // Private variables
    private Rigidbody _playerRigidbody;
    private float _xRotation = 0f;
    private bool _inputEnabled = true;
    
    // Optional references for more complex functionality
    private InputManager _inputManager;
    private bool _useInputManager = false;
    
    private void Start()
    {
        // If Camera and CameraRoot aren't set, use this object and its parent
        if (Camera == null) Camera = transform;
        if (CameraRoot == null && transform.parent != null) CameraRoot = transform.parent;
        
        // Get player model and components
        if (playerModel != null)
        {
            _playerRigidbody = playerModel.GetComponent<Rigidbody>();
            
            // Try to get InputManager for more complex control
            _inputManager = playerModel.GetComponent<InputManager>();
            _useInputManager = _inputManager != null;
        }
        
        // If no player model found but CameraRoot has a parent
        if (_playerRigidbody == null && CameraRoot != null && CameraRoot.parent != null)
        {
            _playerRigidbody = CameraRoot.parent.GetComponent<Rigidbody>();
        }
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void LateUpdate()
    {
        if (!_inputEnabled) return;
        
        // If we have the complex controls with InputManager, use that
        if (_useInputManager)
        {
            CamMovementsWithInputManager();
        }
        // Otherwise use standard input
        else
        {
            CamMovements();
        }
    }
    
    private void CamMovements()
    {
        if (_playerRigidbody == null) return;
        
        // Get mouse input directly
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        
        // Update camera position to match camera root
        if (Camera != null && CameraRoot != null)
        {
            Camera.position = CameraRoot.position;
        }
        
        // Update rotation values based on mouse input
        _xRotation = Mathf.Clamp(_xRotation - my * MouseSensitivity * Time.smoothDeltaTime, 
                                UpperLimit, BottomLimit);
        
        // Apply vertical look to camera
        if (Camera != null)
        {
            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
        
        // Apply horizontal rotation to player body through rigidbody
        if (_playerRigidbody != null && Mathf.Abs(mx) > 0.01f)
        {
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * 
                                         Quaternion.Euler(0, mx * MouseSensitivity * Time.smoothDeltaTime, 0));
        }
    }
    
    private void CamMovementsWithInputManager()
    {
        if (_playerRigidbody == null || _inputManager == null) return;
        
        // Get look input from the input manager
        float mx = _inputManager.Look.x;
        float my = _inputManager.Look.y;
        
        // Update camera position to match camera root
        if (Camera != null && CameraRoot != null)
        {
            Camera.position = CameraRoot.position;
        }
        
        // Update rotation values based on input manager values
        _xRotation = Mathf.Clamp(_xRotation - my * MouseSensitivity * Time.smoothDeltaTime, 
                                UpperLimit, BottomLimit);
        
        // Apply vertical look to camera
        if (Camera != null)
        {
            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
        
        // Apply horizontal rotation to player body through rigidbody
        if (_playerRigidbody != null)
        {
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * 
                                         Quaternion.Euler(0, mx * MouseSensitivity * Time.smoothDeltaTime, 0));
        }
    }
    
    // Property to control input from external scripts
    public bool InputEnabled
    {
        get { return _inputEnabled; }
        set { _inputEnabled = value; }
    }
    
    // Public method to reset camera orientation
    public void ResetCamera()
    {
        _xRotation = 0f;
        if (Camera != null)
        {
            Camera.localRotation = Quaternion.identity;
        }
    }
    
    // Debug visualization 
    private void OnDrawGizmos()
    {
        if (!enableDebugDisplay) return;
        
        if (CameraRoot != null)
        {
            // Draw connection between camera and root
            Gizmos.color = Color.green;
            if (Camera != null)
            {
                Gizmos.DrawLine(Camera.position, CameraRoot.position);
            }
            
            // Draw camera view direction
            Gizmos.color = Color.yellow;
            if (Camera != null)
            {
                Gizmos.DrawRay(Camera.position, Camera.forward * 2f);
            }
            
            // Draw player model direction if available
            if (playerModel != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(playerModel.position, playerModel.forward * 2f);
            }
            
            // Draw up/down look limits
            if (Camera != null)
            {
                Gizmos.color = Color.red;
                Vector3 upLimitDir = Quaternion.Euler(UpperLimit, 0, 0) * Vector3.forward;
                Vector3 downLimitDir = Quaternion.Euler(BottomLimit, 0, 0) * Vector3.forward;
                Gizmos.DrawRay(Camera.position, upLimitDir * 1.5f);
                Gizmos.DrawRay(Camera.position, downLimitDir * 1.5f);
            }
        }
    }
}