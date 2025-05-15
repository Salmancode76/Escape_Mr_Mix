using UnityEngine;

[RequireComponent(typeof(RehanPlayerController))]
public class LegacyCharacterMovement : MonoBehaviour
{
    [Header("Dependencies")]
    public RehanPlayerController playerController;
    public Rigidbody rbBodyCharacter;
    public Transform tangentStartPosition;
    public GameObject addForceObj;
    public Transform objCamera;
    public scPreventClimbing preventClimbing;

    [Header("Movement Settings")]
    public float characterSpeed = 2f;
    public float BrakeForce = 35f;
    public float Coeff = 0.15f;
    public float MaxSpeed = 1f;
    public float minimumAxisMovement = 0.2f;

    [Header("Jump Settings")]
    public bool b_AllowJump = true;
    public float jumpForce = 3f;
    public float jumpSpeed = 10f;
    public float heightRoof = 0.45f;
    public float minimumJump = 0.6f;

    [Header("Camera Settings")]
    public float minimum = -60f;
    public float maximum = 60f;
    public float sensibilityMouse = 2f;
    public float sensibilityJoystick = 2f;
    public AnimationCurve animationCurveMouse;
    public AnimationCurve animationCurveJoystick;

    [Header("Mobile Settings")]
    public float mobileSpeedRotation = 1f;
    public float LeftStickSensibility = 5f;
    public AnimationCurve animationCurveMobile;
    public AnimationCurve animationCurveMobileSmoothMove;

    [Header("Crouch Settings")]
    public bool allowCrouch = false;
    public float targetScaleCrouch = 0.5f;
    public float crouchSpeed = 3f;
    public float heightCheck = 2.05f;
    public LayerMask layerCheckCrouch;
    public bool isCrouching = false;

    [Header("Gravity Settings")]
    public float gravityScale = 1.0f;
    private static float globalGravity = -9.81f;
    public float fallCurve;
    public AnimationCurve animFallCurve;
    public float GravityFallSpeed = 30f;

    
    [Header("Collision Settings")]
    public LayerMask myLayerMask;
    public LayerMask myLayer;
    public LayerMask myLayer02;
    public float overlapSize = 0.5f;
    public float overlapPos = 0.334f;
    public float hitDistance = 0.35f;
    public float hitDistanceMin = 0.45f;
    public float hitDistanceMax = 0.75f;
    public Vector3 rayPosition = Vector3.zero;

    // Private variables
    private float mouseY = 0f;
    private float currentSpeedMultiplier = 1f;
    private float refScaleCrouch = 1f;
    private bool b_Overlap = false;
    private bool b_TouchLayer12_17 = false;
    private CapsuleCollider charCol;
    private float currentAngle = 0f;
    private Vector3 circlePos = Vector3.zero;



    

    private void Awake()
    {
        if (!playerController) playerController = GetComponent<RehanPlayerController>();
        if (!rbBodyCharacter) rbBodyCharacter = GetComponent<Rigidbody>();
        charCol = GetComponent<CapsuleCollider>();
        
        // Initialize values from PlayerController
        if (playerController)
        {
            characterSpeed = playerController.walkSpeed;
            MaxSpeed = playerController.runSpeed;
            allowCrouch = playerController.allowCrouch;
            b_AllowJump = playerController.allowJump;
            refScaleCrouch = transform.localScale.y;
        }
    }

    public void charaGeneralMovementController()
    {
        // Movement is now handled by PlayerController
        playerController.Move();
    }

    public void charaStopMoving()
    {
        playerController.StopMoving();
    }

    // Other legacy methods that might be called by external systems
    public void MoveForward() => playerController.MoveForward();
    public void MoveBackward() => playerController.MoveBackward();
    public void StopMoving() => playerController.StopMoving();
    public void MoveLeft() => playerController.MoveLeft();
    public void MoveRight() => playerController.MoveRight();
    public void AP_Crouch() => playerController.HandleCrouch();

    // Physics update to maintain legacy behavior
    private void FixedUpdate()
    {
        AP_OverlapSphere();
        Ap_isOnFloor();
        AP_ApplyGravity();
        UpdateCrouch();
    }

    private void UpdateCrouch()
    {
        if (!allowCrouch) return;

        float targetHeight = playerController.isCrouching ? targetScaleCrouch : refScaleCrouch;
        if (Mathf.Abs(transform.localScale.y - targetHeight) > 0.01f)
        {
            transform.localScale = Vector3.MoveTowards(
                transform.localScale,
                new Vector3(transform.localScale.x, targetHeight, transform.localScale.z),
                Time.deltaTime * crouchSpeed
            );
        }
    }

    private void AP_OverlapSphere()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * overlapPos, overlapSize, myLayer02);
        b_Overlap = hits.Length > 0;
        if (!b_Overlap) playerController._grounded = false;
    }

    private void Ap_isOnFloor()
    {
        float offset = 0.6f * (180 - currentAngle) / 80;
        float checkDistance = playerController._grounded ? hitDistanceMax + offset : hitDistanceMin + offset;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, -Vector3.up, out RaycastHit hit, checkDistance, myLayer))
        {
            playerController._grounded = b_Overlap;
            rayPosition = transform.position + Vector3.up * 0.1f;
        }
        else
        {
            playerController._grounded = false;
            rayPosition = transform.position;
        }
    }

private void AP_ApplyGravity()
    {
        if (!playerController._grounded)
        {
            fallCurve = Mathf.MoveTowards(fallCurve, 1, Time.deltaTime);
            float currentGravityScale = Mathf.MoveTowards(gravityScale, 30, 
                animFallCurve.Evaluate(fallCurve) * GravityFallSpeed * Time.deltaTime);
            
            Vector3 gravity = globalGravity * currentGravityScale * Vector3.up;
            rbBodyCharacter.AddForce(gravity, ForceMode.Acceleration);
        }
        else
        {
            fallCurve = 0;
        }
    }

    // Add any other legacy methods that might be needed by external systems
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {
            b_TouchLayer12_17 = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {
            b_TouchLayer12_17 = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {
            b_TouchLayer12_17 = false;
        }
    }
}