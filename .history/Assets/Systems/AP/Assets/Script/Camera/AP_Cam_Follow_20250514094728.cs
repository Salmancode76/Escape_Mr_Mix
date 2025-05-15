using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour {

    public Transform target; // Usually the player GameObject
    public float rotationDamping = 15f;

    [Header("Player Model")]
    public Transform playerModel; // The model with the Animator

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow position
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);

        // Smooth follow rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping);

        // Make player model face the camera's horizontal direction
        if (playerModel != null)
        {
            Vector3 lookDirection = transform.forward;
            lookDirection.y = 0; // Ignore up/down
            lookDirection.Normalize();

            playerModel.forward = lookDirection;
        }
    }
}
