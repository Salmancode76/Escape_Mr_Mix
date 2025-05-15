using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager;
public class RehanPlayerController : MonoBehaviour
{

        public bool InputEnabled { get; set; } = true;

        [Header("Movement Settings")]
        [SerializeField] public float walkSpeed = 2f;
        [SerializeField] public float runSpeed = 6f;
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private float BrakeForce = 35f;
        [SerializeField] private float Coeff = 0.15f;
        [SerializeField] private float MaxSpeed = 1f;
        [SerializeField] private float minimumAxisMovement = 0.2f;
        [SerializeField] private float speedMultiplier = 3f;
        [SerializeField] private float mobileSpeedRotation = 1f;
        [SerializeField] private float LeftStickSensibility = 5f;

        [Header("Camera Settings")]
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField] private float sensibilityJoystick = 2f;
        [SerializeField] private AnimationCurve animationCurveJoystick;

        [Header("Jump Settings")]
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;
        [SerializeField] private float jumpForce = 3f;
        [SerializeField] private float jumpSpeed = 10f;
        [SerializeField] private float fallCurve;
        [SerializeField] private AnimationCurve animFallCurve;
        [SerializeField] private float GravityFallSpeed = 30f;
        [SerializeField] private float heightRoof = 0.45f;
        [SerializeField] private float minimumJump = 0.6f;

        [Header("Crouch Settings")]
        [SerializeField] private float targetScaleCrouch = 0.5f;
        [SerializeField] private float crouchSpeed = 3f;
        [SerializeField] private float heightCheck = 2.05f;
        [SerializeField] private LayerMask layerCheckCrouch;

        [Header("Collision Settings")]
        [SerializeField] private LayerMask myLayerMask;
        [SerializeField] private LayerMask myLayer;
        [SerializeField] private LayerMask myLayer02;
        [SerializeField] private float overlapSize = 0.5f;
        [SerializeField] private float overlapPos = 0.334f;
        [SerializeField] private float hitDistance = 0.35f;
        [SerializeField] private float hitDistanceMin = 0.45f;
        [SerializeField] private float hitDistanceMax = 0.75f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip[] walkClipsArray;
        [SerializeField] private float footstepInterval = 0.4f;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landingClip;

        // References
        private Rigidbody _playerRigidbody;
        public InputManager _inputManager;
        private Animator _animator;
        private CapsuleCollider _collider;
        private scPreventClimbing preventClimbing;

        // Movement state
        public bool _grounded = false;
        public bool isRunning = false;
        public bool isCrouching = false;
        public bool isSliding = false;
        public bool isJumping = false;
        public bool allowCrouch = true;
        public bool allowJump = true;
        public bool allowRun = true;
        public bool b_TouchLayer12_17 = false;

        // Animation hashes
        private int _xVelHash;
        private int _yVelHash;
        private int _zVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;
        private int _crouchHash;
        private int _slideHash;

        // Internal variables
        private bool _hasAnimator;
        private float _xRotation;
        private Vector2 _currentVelocity;
        private float defaultColliderHeight;
        private Vector3 defaultColliderCenter;
        private float slideTimer = 0f;
        private float slideDuration = 1.0f;
        private bool hasJumped = false;
        private bool hasPlayedLanding = false;
        private AudioClip[] footstepClips;
        private int currentStepIndex = 0;
        private float nextFootstepTime = 0f;
        private bool useOrderedSequence = true;
        private List<int> randomSequence;
        private float currentAngle = 0f;
        private Vector3 circlePos = Vector3.zero;
        private Vector3 rayPosition = Vector3.zero;
        private bool b_Overlap = false;
        private float refScaleCrouch = 1f;
        private float currentSpeedMultiplier = 1f;

        // Public properties for legacy compatibility
        public Transform objCamera => Camera;
        public Rigidbody rbBodyCharacter => _playerRigidbody;
        public float characterSpeed => walkSpeed;
        public bool b_IsJumping => isJumping;
        public bool b_Crouch => isCrouching;
        public bool b_AllowJump => allowJump;
        public bool b_AllowRun => allowRun;

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();
            _collider = GetComponent<CapsuleCollider>();
            preventClimbing = GetComponent<scPreventClimbing>();

            if (_collider != null)
            {
                defaultColliderHeight = _collider.height;
                defaultColliderCenter = _collider.center;
            }

            InitializeAnimationHashes();
            footstepClips = walkClipsArray;
            refScaleCrouch = transform.localScale.y;
        }

        private void InitializeAnimationHashes()
        {
            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");
            _slideHash = Animator.StringToHash("Slid");
        }

        private void FixedUpdate()
        {
            if (!InputEnabled) return;

            AP_OverlapSphere();
            Ap_isOnFloor();
            AP_ApplyGravity();

            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
            HandleSlide();
        }

        private void LateUpdate()
        {
            if (!InputEnabled) return;
            CamMovements();
        }
        private void CamMovements()
        {
            if (!_hasAnimator || !InputEnabled) return;
            float mx = _inputManager.Look.x, my = _inputManager.Look.y;
            Camera.position = CameraRoot.position;
            _xRotation = Mathf.Clamp(_xRotation - my * MouseSensitivity * Time.smoothDeltaTime, UpperLimit, BottomLimit);
            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, mx * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void ResetCollider()
        {
            _collider.height = defaultColliderHeight;
            _collider.center = defaultColliderCenter;
        }
        private void SetColliderHeight(float height)
        {
            _collider.height = height;
            _collider.center = new Vector3(0, height / 2f, 0);
        }

        private void HandleSlide()
        {
            if (!InputEnabled) return;
            if (isSliding)
            {
                slideTimer -= Time.fixedDeltaTime;
                if (slideTimer <= 0f) { isSliding = false; ResetCollider(); }
                return;
            }
            if (_inputManager.Slide && _grounded)
            {
                if (_inputManager.Move.y <= 0.1f) { _inputManager.Slide = false; return; }
                isSliding = true; slideTimer = slideDuration;
                _animator.SetBool(_slideHash, true);
                SetColliderHeight(defaultColliderHeight * 0.25f);
                _inputManager.Slide = false;
            }
            else _animator.SetBool(_slideHash, false);
        }
        public void HandleCrouch()
        {
            if (!InputEnabled) return;
            if (_inputManager.Crouch && !isSliding)
            {
                _animator.SetBool(_crouchHash, true);
                SetColliderHeight(defaultColliderHeight * 0.5f);
            }
            else if (!_inputManager.Crouch && !isSliding)
            {
                _animator.SetBool(_crouchHash, false);
                ResetCollider();
            }
        }


        private void HandleJump()
        {
            if (!_hasAnimator || !_inputManager.Jump || !_grounded || !InputEnabled || hasJumped) return;
            hasJumped = true; hasPlayedLanding = false;
            _animator.SetTrigger(_jumpHash);
            SoundFXManager.instance.playSoundFXClip(jumpClip, transform, 1f);
            nextFootstepTime = Time.time + footstepInterval;
            Debug.Log($"Jump triggered at {Time.time}");
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_groundHash, true);
            if (_inputManager.Crouch)
            {
                _animator.SetBool(_crouchHash, true);
                SetColliderHeight(defaultColliderHeight * 0.5f);
            }
        }

        private void SampleGround()
        {
            if (!_hasAnimator) return;
            bool wasGrounded = _grounded;
            RaycastHit hit;
            _grounded = Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hit, Dis2Ground + 0.1f, GroundCheck);
            if (_grounded)
            {
                if (!wasGrounded)
                {
                    Debug.Log($"Landed at {Time.time}");
                    SoundFXManager.instance.playSoundFXClip(landingClip, transform, 1f);
                    hasPlayedLanding = true; hasJumped = false;
                }
                SetAnimationGrounding();
            }
            else
            {
                if (wasGrounded)
                    Debug.Log($"Left ground at {Time.time}");
                _animator.SetBool(_groundHash, false);
            }
        }

        private List<int> GenerateRandomSequence()
        {
            var seq = new List<int>();
            for (int i = 0; i < footstepClips.Length; i++) seq.Add(i);
            for (int i = 0; i < seq.Count; i++)
            {
                int r = UnityEngine.Random.Range(i, seq.Count);
                int tmp = seq[r]; seq[r] = seq[i]; seq[i] = tmp;
            }
            return seq;
        }

        private void PlayNextFootstep()
        {
            if (footstepClips.Length == 0) return;
            AudioClip clipToPlay = useOrderedSequence
                ? footstepClips[currentStepIndex++]  
                : footstepClips[randomSequence[currentStepIndex++]];

            if (currentStepIndex >= footstepClips.Length)
            {
                useOrderedSequence = !useOrderedSequence;
                currentStepIndex = 0;
                randomSequence = GenerateRandomSequence();
            }

            float volume = _inputManager.Crouch ? 0.68f : 1f;
            SoundFXManager.instance.playSoundFXClip(clipToPlay, transform, volume);
        }

        private void HandleFootsteps(bool isMoving)
        {
            // block footsteps if not ground, sliding, jumping, or no movement
            if (!isMoving || isSliding || !_grounded || hasJumped)
                return;

            float effectiveInterval = _inputManager.Crouch
                ? footstepInterval
                : (_inputManager.Run ? footstepInterval / 1.5f : footstepInterval);

            if (Time.time >= nextFootstepTime)
            {
                nextFootstepTime = Time.time + effectiveInterval;
                Debug.Log($"Footstep at {Time.time}, grounded={_grounded}, isMoving={isMoving}, hasJumped={hasJumped}");
                PlayNextFootstep();
            }
        }

        // public void Move()
        // {
        //     if (!_hasAnimator || !InputEnabled) return;

        //     float targetSpeed = _inputManager.Run ? runSpeed : walkSpeed;
        //     if (_inputManager.Crouch) targetSpeed = 1.5f;
        //     if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

        //     bool isMoving = _inputManager.Move != Vector2.zero;
        //     Vector2 moveInput = _inputManager.Move;
        //     if (moveInput.magnitude > 1f) moveInput.Normalize();

        //     if (_grounded)
        //     {
        //         _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, moveInput.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
        //         _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, moveInput.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

        //         float xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
        //         float zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

        //         _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
        //         HandleFootsteps(isMoving);
        //     }
        //     else
        //     {
        //         _playerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance, 0, _currentVelocity.y * AirResistance), ForceMode.VelocityChange);
        //         nextFootstepTime = Time.time + footstepInterval;
        //     }

        //     _animator.SetFloat(_xVelHash, _currentVelocity.x);
        //     _animator.SetFloat(_yVelHash, _currentVelocity.y);
        // }

        // ... [Keep all other existing methods like HandleFootsteps, PlayNextFootstep, etc.]

        // Add legacy compatibility methods
        public void charaGeneralMovementController() => Move();
        public void charaStopMoving() => _playerRigidbody.velocity = Vector3.zero;

        // In PlayerController.cs - Modified movement control methods
        private Vector2 mobileMoveInput = Vector2.zero;

        public void MoveForward() 
        {
            if (_inputManager.Move != Vector2.zero)
            {
                // If using input manager's move, we can't modify it directly
                mobileMoveInput = new Vector2(0, 1);
            }
            else
            {
                mobileMoveInput = new Vector2(0, 1);
            }
        }

        public void MoveBackward() 
        {
            if (_inputManager.Move != Vector2.zero)
            {
                mobileMoveInput = new Vector2(0, -1);
            }
            else
            {
                mobileMoveInput = new Vector2(0, -1);
            }
        }

        public void MoveLeft() 
        {
            if (_inputManager.Move != Vector2.zero)
            {
                mobileMoveInput = new Vector2(-1, 0);
            }
            else
            {
                mobileMoveInput = new Vector2(-1, 0);
            }
        }

        public void MoveRight() 
        {
            if (_inputManager.Move != Vector2.zero)
            {
                mobileMoveInput = new Vector2(1, 0);
            }
            else
            {
                mobileMoveInput = new Vector2(1, 0);
            }
        }

        public void StopMoving() 
        {
            mobileMoveInput = Vector2.zero;
        }

        // Modify the Move() method to use both input sources:
        public void Move()
        {
            if (!_hasAnimator || !InputEnabled) return;

            // Combine both input sources
            Vector2 combinedMove = _inputManager.Move + mobileMoveInput;
            if (combinedMove.magnitude > 1f) combinedMove.Normalize();

            float targetSpeed = _inputManager.Run ? runSpeed : walkSpeed;
            if (_inputManager.Crouch) targetSpeed = 1.5f;
            if (combinedMove == Vector2.zero) targetSpeed = 0;

            bool isMoving = combinedMove != Vector2.zero;

            if (_grounded)
            {
                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, combinedMove.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, combinedMove.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

                float xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
                float zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
                HandleFootsteps(isMoving);
            }
            else
            {
                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance, 0, _currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
                nextFootstepTime = Time.time + footstepInterval;
            }

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }
        public void AP_Crouch() => isCrouching = !isCrouching;

        private void AP_OverlapSphere()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * overlapPos, overlapSize, myLayer02);
            b_Overlap = hits.Length > 0;
            if (!b_Overlap) _grounded = false;
        }

        private void Ap_isOnFloor()
        {
            float offset = 0.6f * (180 - currentAngle) / 80;
            float checkDistance = _grounded ? hitDistanceMax + offset : hitDistanceMin + offset;

            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, -Vector3.up, checkDistance, myLayer))
            {
                _grounded = b_Overlap;
                rayPosition = transform.position + Vector3.up * 0.1f;
            }
            else
            {
                _grounded = false;
                rayPosition = transform.position;
            }
        }

        public void AP_ApplyGravity()
        {
            if (!_grounded)
            {
                fallCurve = Mathf.MoveTowards(fallCurve, 1, Time.deltaTime);
                float gravityScale = Mathf.MoveTowards(0, 30, animFallCurve.Evaluate(fallCurve) * GravityFallSpeed * Time.deltaTime);
                _playerRigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
                b_TouchLayer12_17 = true;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
                b_TouchLayer12_17 = true;
            else if (collision.gameObject.layer == 18)
                _grounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
                b_TouchLayer12_17 = false;
        }
    
}