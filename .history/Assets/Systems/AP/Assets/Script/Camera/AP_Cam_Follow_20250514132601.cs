using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour {

    public Transform target;
    public float rotationDamping = 15;

    [Header("Player Facing Sync")]
    public Transform playerModel;

    private float lastMouseX = 0f;

    void LateUpdate()
    {
        if (target != null)
        {
            // ✅ Do NOT change these lines (original logic)
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping);

            // ✅ Only sync player facing when mouse is moving
            float mouseX = Input.GetAxis("Mouse X");

            if (playerModel != null && Mathf.Abs(mouseX) > 0.01f)
            {
                Vector3 camForward = transform.forward;
                camForward.y = 0f;

                if (camForward.sqrMagnitude > 0.01f)
                    playerModel.rotation = Quaternion.LookRotation(camForward);
            }
        }
    }
}
