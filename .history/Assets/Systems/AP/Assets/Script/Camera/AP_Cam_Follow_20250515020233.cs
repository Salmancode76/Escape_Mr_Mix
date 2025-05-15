using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public characterMovement characterMovementScript;
    public Transform target;
    public Transform playerModel;
    public float positionDamping = 15f;
    public float mouseSensitivity = 100f;
    public float upperLimit = -30f;
    public float bottomLimit = 70f;

    private float _xRotation = 0f;
    private Rigidbody _playerRigidbody;

    void Start()
    {
        if (playerModel != null)
            _playerRigidbody = playerModel.GetComponent<Rigidbody>();

        if (characterMovementScript != null && ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (target == null) Debug.LogError("AP_Cam_Follow: Target is not assigned!");
        if (characterMovementScript == null) Debug.LogError("AP_Cam_Follow: characterMovementScript is not assigned!");
    }

    void LateUpdate()
    {
        if (characterMovementScript == null || target == null) return;

        FollowTarget();
        HandleCameraRotation();
        RotatePlayerToCamera();
    }

    void FollowTarget()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            Time.deltaTime * positionDamping);
    }

    void HandleCameraRotation()
    {
        float mouseX = characterMovementScript.GetMouseXInput();
        float mouseY = characterMovementScript.GetMouseYInput();

        _xRotation = Mathf.Clamp(
            _xRotation - mouseY * mouseSensitivity * Time.deltaTime,
            upperLimit,
            bottomLimit);

        transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }

    void RotatePlayerToCamera()
    {
        if (_playerRigidbody == null) return;

        Vector3 cameraForward = transform.forward;
        cameraForward.y = 0f;

        if (cameraForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            _playerRigidbody.MoveRotation(targetRotation);
        }
    }

    public void ResetCamera()
    {
        _xRotation = 0f;
        if (_playerRigidbody != null)
        {
            transform.localEulerAngles = new Vector3(0, _playerRigidbody.rotation.eulerAngles.y, 0);
        }
    }
}
