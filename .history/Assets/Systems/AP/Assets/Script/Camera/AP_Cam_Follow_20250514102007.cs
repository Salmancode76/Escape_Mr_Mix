using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public Transform target; // Usually the player GameObject
    public float rotationDamping = 15f;

    [Header("Player Model")]
    public Transform playerModel; // The model with the Animator

    // Sensitivity for mouse movement
    public float horizontalSensitivity = 5f;
    public float verticalSensitivity = 2f; // Sensitivity for vertical camera movement

    private float cameraRotationX = 0f;  // Vertical rotation of the camera
    private float currentYRotation = 0f; // Current horizontal rotation

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow position
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);

        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity;

        // Rotate the camera horizontally (left/right)
        currentYRotation += mouseX;
        
        // Rotate the camera vertically (up/down)
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -80f, 80f); // Limit vertical rotation

        // Apply combined rotation to the camera
        Quaternion targetRotation = Quaternion.Euler(cameraRotationX, currentYRotation, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);

        // Make the player model face the camera's forward direction (but stay upright)
        if (playerModel != null)
        {
            // Get camera's forward direction but ignore vertical rotation
            Vector3 cameraForward = transform.forward;
            cameraForward.y = 0; // Keep player model upright
            cameraForward.Normalize();

            // Smoothly rotate player model to face camera forward
            if (cameraForward.magnitude > 0.1f)
            {
                Quaternion targetModelRotation = Quaternion.LookRotation(cameraForward);
                playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetModelRotation, Time.deltaTime * rotationDamping);
            }
        }
    }
}