using System.Collections;
using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour
{
    public characterMovement characterMovementScript;
    public Transform target;
    public Transform playerModel;

    public float positionDamping = 15f;
    public float playerTurnSpeed = 8f;

    private Rigidbody playerRigidbody;

    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float BottomLimit = 70f;

    private float _xRotation;

    void Start()
    {
        if (playerModel != null)
        {
            playerRigidbody = playerModel.GetComponent<Rigidbody>();
        }

        if (characterMovementScript != null && ingameGlobalManager.instance.b_DesktopInputs)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        FollowTarget();
        UpdatePlayerFacing();
        SyncCameraRotation();
        UpdatePlayerYRotationToCamera();
    }

    void FollowTarget()
    {
        if (target == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            Time.deltaTime * positionDamping);
    }

    void UpdatePlayerFacing()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null || playerModel == null)
        {
            Debug.LogWarning("AP_Cam_Follow: Skipping UpdatePlayerFacing due to null references");
            return;
        }

        // Skip if playerModel is the same as rbBodyCharacter to avoid overriding characterMovement's rotation
        if (playerRigidbody != null && playerRigidbody == characterMovementScript.rbBodyCharacter)
        {
            return;
        }

        // Update camera's X rotation (vertical tilt)
        _xRotation = Mathf.Clamp(
            _xRotation - characterMovementScript.GetMouseYInput() * playerTurnSpeed * Time.smoothDeltaTime,
            UpperLimit,
            BottomLimit);
        target.localRotation = Quaternion.Euler(_xRotation, 0, 0);

        if (playerRigidbody != null)
        {
            playerRigidbody.MoveRotation(Quaternion.Euler(0f, characterMovementScript.objCamera.eulerAngles.y, 0f));
        }
        else
        {
            Vector3 currentEuler = playerModel.eulerAngles;
            currentEuler.y = characterMovementScript.objCamera.eulerAngles.y;
            playerModel.eulerAngles = currentEuler;
        }
    }

    void SyncCameraRotation()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null) return;

        transform.rotation = characterMovementScript.objCamera.rotation;
    }

    void UpdatePlayerYRotationToCamera()
    {
        if (playerModel == null || characterMovementScript == null || characterMovementScript.objCamera == null)
        {
            Debug.LogWarning("AP_Cam_Follow: Skipping UpdatePlayerYRotationToCamera due to null references");
            return;
        }

        if (playerRigidbody != null)
        {
            // Get the camera's Y rotation
            float cameraY = characterMovementScript.objCamera.eulerAngles.y;
            // Create target rotation with only Y component
            Quaternion targetRotation = Quaternion.Euler(0f, cameraY, 0f);
            // Use MoveRotation to ensure physics-based rotation
            playerRigidbody.MoveRotation(Quaternion.Lerp(
                playerRigidbody.rotation,
                targetRotation,
                Time.deltaTime * playerTurnSpeed));
        }
    }

    public void ResetCamera()
    {
        if (characterMovementScript == null || characterMovementScript.objCamera == null) return;

        characterMovementScript.mouseY = 0f;

        if (playerModel != null)
        {
            characterMovementScript.objCamera.localEulerAngles = new Vector3(
                characterMovementScript.mouseY,
                playerModel.eulerAngles.y,
                0f);
        }
    }
}