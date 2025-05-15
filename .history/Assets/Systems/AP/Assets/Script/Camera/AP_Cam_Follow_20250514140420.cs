using System;
using System.Collections;
using System.Collections.Generic;
using UnityTutorial.Manager;
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
    [SerializeField] private float RotationDamping = 15f;
    [SerializeField] private bool RotatePlayerWithCamera = true;
    
    [Header("Player Settings")]
    [SerializeField] private Transform playerModel;
    [SerializeField] private bool enableDebugDisplay = false;
    
    // Private variables
    private Rigidbody _playerRigidbody;
    private float _xRotation = 0f;
    private float _yRotation = 0f;
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
            
            // Initialize yRotation to match player rotation
            _yRotation = playerModel.eulerAngles.y;
        }
        
        // If no player model found but CameraRoot has a parent
        if (_playerRigidbody == null && CameraRoot != null && CameraRoot.parent != null)
        {
            _playerRigidbody = CameraRoot.parent.GetComponent<Rigidbody>();
            if (_playerRigidbody != null)
            {
                _yRotation = _playerRigidbody.rotation.eulerAngles.y;
            }
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
        _yRotation += mx * MouseSensitivity * Time.smoothDeltaTime;
        
        // Apply rotation to the camera (both horizontal and vertical)
        if (Camera != null)
        {
            Camera.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }
        
        // Also rotate the player model if needed
        if (RotatePlayerWithCamera && _playerRigidbody != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, _yRotation, 0);
            _playerRigidbody.MoveRotation(Quaternion.Slerp(_playerRigidbody.rotation, targetRotation, 
                                         Time.deltaTime * RotationDamping));
        }
    }
    
    private void CamMovementsWithInputManager()
    {
        if (_inputManager == null) return;
        
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
        _yRotation += mx * MouseSensitivity * Time.smoothDeltaTime;
        
        // Apply rotation to the camera (both horizontal and vertical)
        if (Camera != null)
        {
            Camera.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        }
        
        // Also rotate the player model if needed
        if (RotatePlayerWithCamera && _playerRigidbody != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, _yRotation, 0);
            _playerRigidbody.MoveRotation(Quaternion.Slerp(_playerRigidbody.rotation, targetRotation, 
                                         Time.deltaTime * RotationDamping));
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
        
        if (_playerRigidbody != null)
        {
            _yRotation = _playerRigidbody.rotation.eulerAngles.y;
        }
        else if (playerModel != null)
        {
            _yRotation = playerModel.eulerAngles.y;
        }
        
        if (Camera != null)
        {
            Camera.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
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