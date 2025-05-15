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

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse Input for Camera and Player Rotation
        float mx = Input.GetAxis("Mouse X") * horizontalSensitivity; // Horizontal mouse movement (left/right)
        float my = Input.GetAxis("Mouse Y") * verticalSensitivity; // Vertical mouse movement (up/down)

        // Rotate the player model (horizontal movement)
        if (playerModel != null)
        {
            playerModel.Rotate(Vector3.up * mx);
        }

        // Rotate the camera (vertical movement)
        _xRotation -= my;
        _xRotation = Mathf.Clamp(_xRotation, upperLimit, bottomLimit); // Clamping to prevent excessive up/down rotation
        transform.localRotation = Quaternion.Euler(_xRotation, transform.localRotation.eulerAngles.y, 0f); // Apply vertical rotation

        // Smooth follow position for camera
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);

        // Smooth follow rotation for camera (horizontal rotation)
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping);
    }
}
