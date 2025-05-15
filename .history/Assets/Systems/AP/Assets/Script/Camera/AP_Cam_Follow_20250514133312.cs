using UnityEngine;

public class AP_Cam_Follow : MonoBehaviour 
{
    [Header("Target Settings")]
    public Transform target;
    public float positionSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;
    
    [Header("Player Facing Settings")]
    public Transform playerModel;
    public float rotationSensitivity = 2.0f;
    public float minRotationThreshold = 0.01f;
    public bool smoothPlayerRotation = true;
    public float playerRotationSmoothTime = 0.1f;
    
    // Private variables for smooth movement
    private Vector3 positionVelocity = Vector3.zero;
    private float rotationVelocity = 0f;
    private Quaternion targetPlayerRotation;
    private Vector3 playerRotationVelocity = Vector3.zero;
    
    // Input handling
    private float mouseX;
    private bool isMouseMoving;
    
    private void Awake()
    {
        // Initialize target rotation if player model exists
        if (playerModel != null)
        {
            targetPlayerRotation = playerModel.rotation;
        }
    }
    
    private void Update()
    {
        // Process input
        mouseX = Input.GetAxis("Mouse X");
        isMouseMoving = Mathf.Abs(mouseX) > minRotationThreshold;
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Smooth camera position follow with different smoothing algorithm
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            target.position, 
            ref positionVelocity, 
            positionSmoothTime
        );
        
        // Smooth camera rotation follow
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target.rotation,
            1 - Mathf.Exp(-rotationSmoothTime * Time.deltaTime * 10)
        );
        
        // Handle player model rotation when mouse is moving
        UpdatePlayerRotation();
    }
    
    private void UpdatePlayerRotation()
    {
        if (playerModel == null) return;
        
        if (isMouseMoving)
        {
            // Get camera forward direction and normalize it on the horizontal plane
            Vector3 forward = transform.forward;
            forward.y = 0f;
            
            // Only rotate if we have a valid direction
            if (forward.sqrMagnitude > 0.01f)
            {
                // Apply rotation with sensitivity
                Vector3 rotationDirection = forward * (mouseX > 0 ? 1 : -1);
                targetPlayerRotation = Quaternion.LookRotation(forward.normalized);
                
                // Apply the rotation immediately or smoothly
                if (smoothPlayerRotation)
                {
                    playerModel.rotation = Quaternion.Slerp(
                        playerModel.rotation,
                        targetPlayerRotation,
                        1 - Mathf.Exp(-playerRotationSmoothTime * Time.deltaTime * 10)
                    );
                }
                else
                {
                    playerModel.rotation = targetPlayerRotation;
                }
            }
        }
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            
            if (playerModel != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(playerModel.position, playerModel.forward);
            }
        }
    }
}
