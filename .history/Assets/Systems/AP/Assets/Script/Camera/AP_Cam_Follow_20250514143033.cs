using System;
using System.Collections;
using System.Collections.Generic;
using UnityTutorial.Manager;

using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour 
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.6f, 0); // Default offset for camera position
    
    [Header("Camera Settings")]
    public float mouseSensitivity = 21.9f;
    public float upperLookLimit = -40f;
    public float bottomLookLimit = 70f;
    
    [Header("Smoothing Settings")]
    public float positionSmoothTime = 0.1f;
    public bool smoothRotation = true;
    public float rotationSmoothTime = 0.1f;
    
    // Private variables
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector3 currentVelocity;
    private Vector3 targetPosition;
    
    private void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Initialize rotation
        if(target != null)
        {
            yRotation = target.eulerAngles.y;
        }
    }
    
    private void LateUpdate()
    {
        if(target == null) return;
        
        // Handle mouse input for rotation
        HandleMouseLook();
        
        // Update camera position
        UpdateCameraPosition();
    }
    
    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Apply mouse input to rotation values
        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        yRotation += mouseX * mouseSensitivity * Time.deltaTime;
        
        // Clamp vertical rotation
        xRotation = Mathf.Clamp(xRotation, upperLookLimit, bottomLookLimit);
        
        // Apply rotation to camera
        if(smoothRotation)
        {
            // Smooth rotation
            Quaternion targetRotation = Quaternion.Euler(xRotation, yRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
        }
        else
        {
            // Immediate rotation
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
    }
    
    private void UpdateCameraPosition()
    {
        // Calculate target position with offset
        targetPosition = target.position + offset;
        
        // Smoothly move camera to target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, positionSmoothTime);
    }
    
    // Public method to get the forward direction for movement (without vertical component)
    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
    
    // Public method to get the right direction for movement (without vertical component)
    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }
    
    // Public method to reset camera orientation
    public void ResetCamera()
    {
        xRotation = 0f;
        
        if (target != null)
        {
            yRotation = target.eulerAngles.y;
        }
        
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}