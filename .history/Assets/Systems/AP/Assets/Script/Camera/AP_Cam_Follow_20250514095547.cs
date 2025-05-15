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

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow position
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);

        // Smooth follow rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping);

        // Get horizontal mouse movement (left/right)
        float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity;

        // Rotate the player model horizontally (left/right)
        if (playerModel != null)
        {
            playerModel.Rotate(Vector3.up * mouseX);
        }

        // Get vertical mouse movement (up/down)
        float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity;

        // Rotate the camera vertically (up/down)
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -80f, 80f); // Limit vertical rotation

        // Apply vertical rotation to the camera
        transform.localRotation = Quaternion.Euler(cameraRotationX, transform.localRotation.eulerAngles.y, 0f);
    }
}
