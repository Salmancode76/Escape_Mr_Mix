using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour {

    public Transform target;
    public float rotationDamping = 15;

    [Header("Player Facing Sync")]
    public Transform playerModel;

    private float lastMouseX;

    void LateUpdate()
    {
        if (target == null) return;

        // ✅ DO NOT TOUCH THIS
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * rotationDamping);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * rotationDamping);

        // ✅ Only rotate player model when mouse is moving horizontally
        float mouseX = Input.GetAxis("Mouse X");
        if (playerModel != null && Mathf.Abs(mouseX) > 0.01f)
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude > 0.01f)
                playerModel.rotation = Quaternion.LookRotation(forward);
        }
    }
}
