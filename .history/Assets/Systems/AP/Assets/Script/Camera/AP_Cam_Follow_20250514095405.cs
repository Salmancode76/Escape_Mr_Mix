using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public Transform target; // Usually the player GameObject
    public float rotationDamping = 15f;

    [Header("Player Model")]
    public Transform playerModel; // The model with the Animator

    // Sensitivity for mouse movement
    public float horizontalSensitivity = 5f;

    private float cameraRotationX = 0f;  // Vertical rotation of the camera

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow position
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);

        // Smooth follow rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping);

        // Horizontal rotation based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
        if (playerModel != null)
        {
            // Rotate the player model
            playerModel.Rotate(Vector3.up * mouseX);
        }

        // Vertical camera rotation
        float mouseY = Input.GetAxis("Mouse Y") * 10f * Time.deltaTime;
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -80f, 80f); // Limit up and down camera rotation
        transform.localRotation = Quaternion.Euler(cameraRotationX, transform.localRotation.eulerAngles.y, 0f);
    }
}
