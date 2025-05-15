using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public Transform target; // Player's Transform
    public float rotationDamping = 15f;

    [Header("Player Model")]
    public Transform playerModel;

    public float horizontalSensitivity = 5f;
    public float verticalSensitivity = 2f;

    private float cameraRotationX = 0f;
    private float currentYRotation = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Directly follow the target to avoid camera shake
        transform.position = target.position;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity;

        currentYRotation += mouseX;
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -80f, 80f);

        // Apply camera rotation
        Quaternion targetRotation = Quaternion.Euler(cameraRotationX, currentYRotation, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);

        // Rotate player model to face forward
        if (playerModel != null)
        {
            Vector3 cameraForward = transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            if (cameraForward.magnitude > 0.1f)
            {
                Quaternion targetModelRotation = Quaternion.LookRotation(cameraForward);
                playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetModelRotation, Time.deltaTime * rotationDamping);
            }
        }
    }
}
