using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager;
using UnityStandardAssets.Utility; // Added for FOVKick and Bobbing features

namespace UnityTutorial.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        public bool InputEnabled { get; set; } = true;

        [SerializeField] private AudioClip[] walkClipsArray;
        [SerializeField] private float footstepInterval = 0.4f;
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private Camera Camera1;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landingClip;

        // Added from FirstPersonController (FOV and Head Bobbing)
        [SerializeField] private bool m_UseFovKick = false;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob = false;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.7f;
        [SerializeField] private float m_StickToGroundForce = 5;
        [SerializeField] private float m_GravityMultiplier = 2;

        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        private CapsuleCollider _collider;
        private bool _grounded = false;
        private bool _hasAnimator;

        private int _xVelHash;
        private int _yVelHash;
        private int _zVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;
        private int _crouchHash;
        private int _slideHash;

        private float _xRotation;
        private const float _walkSpeed = 2f;
        private const float _runSpeed = 6f;
        private Vector2 _currentVelocity;

        private AudioClip[] footstepClips;
        private int currentStepIndex = 0;
        private float nextFootstepTime = 0f;
        private bool useOrderedSequence = true;
        private List<int> randomSequence;

        private float defaultColliderHeight;
        private Vector3 defaultColliderCenter;

        private bool isSliding = false;
        private float slideTimer = 0f;
        private float slideDuration = 1.0f;

        private bool hasJumped = false;
        private bool hasPlayedLanding = false;

        // Added from FirstPersonController
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private AudioSource m_AudioSource;

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();
            _collider = GetComponent<CapsuleCollider>();

            if (_collider != null)
            {
                defaultColliderHeight = _collider.height;
                defaultColliderCenter = _collider.center;
            }

            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
            _zVelHash = Animator.StringToHash("Z_Velocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");
            _slideHash = Animator.StringToHash("Slid");

            footstepClips = walkClipsArray;

            // Initialize head bob and FOV kick components
            if (Camera != null)
            {
                m_OriginalCameraPosition = Camera.localPosition;
                if (m_UseFovKick)
                    m_FovKick.Setup(Camera.GetComponent<Camera>());
                if (m_UseHeadBob)
                    m_HeadBob.Setup(Camera1, footstepInterval);
            }

            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_AudioSource = GetComponent<AudioSource>();
            if (m_AudioSource == null)
            {
                m_AudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void FixedUpdate()
        {
            if (!InputEnabled) return;

            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
            HandleSlide();
            
            // Update camera position with head bob
            UpdateCameraPosition();
        }

        private void LateUpdate()
        {
            if (!InputEnabled) return;
            CamMovements();
        }

        private void Move()
        {
            if (!_hasAnimator || !InputEnabled) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            if (_inputManager.Crouch) targetSpeed = 1.5f;
            if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

            bool isMoving = _inputManager.Move != Vector2.zero;
            Vector2 moveInput = _inputManager.Move;
            if (moveInput.magnitude > 1f) moveInput.Normalize();

            if (_grounded)
            {
                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, moveInput.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, moveInput.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

                float xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
                float zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
                
                // Update step cycle for footsteps and head bobbing
                ProgressStepCycle(targetSpeed, isMoving);
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

        private void ProgressStepCycle(float speed, bool isMoving)
        {
            if (!_grounded || !isMoving) return;

            // Calculate step cycle progress based on velocity and running state
            float speedFactor = _inputManager.Run ? (speed * m_RunstepLenghten) : speed;
            m_StepCycle += (_playerRigidbody.velocity.magnitude + speedFactor) * Time.fixedDeltaTime;

            if (m_StepCycle > m_NextStep)
            {
                m_NextStep = m_StepCycle + footstepInterval;
                // Footstep handling is done in HandleFootsteps
            }
        }

        private void HandleFootsteps(bool isMoving)
        {
            // Block footsteps if not ground, sliding, jumping, or no movement
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

        private void CamMovements()
        {
            if (!_hasAnimator || !InputEnabled) return;
            float mx = _inputManager.Look.x, my = _inputManager.Look.y;
            Camera.position = CameraRoot.position;
            _xRotation = Mathf.Clamp(_xRotation - my * MouseSensitivity * Time.smoothDeltaTime, UpperLimit, BottomLimit);
            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, mx * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void HandleCrouch()
        {
            if (!InputEnabled) return;
            if (_inputManager.Crouch && !isSliding)
            {
                _animator.SetBool(_crouchHash, true);
                SetColliderHeight(defaultColliderHeight * 0.5f);

                // Handle FOV transition if using FOV kick
                if (m_UseFovKick)
                {
                    StopAllCoroutines();
                    StartCoroutine(m_FovKick.FOVKickDown());
                }
            }
            else if (!_inputManager.Crouch && !isSliding)
            {
                _animator.SetBool(_crouchHash, false);
                ResetCollider();

                // Handle FOV transition back if using FOV kick and we're moving
                if (m_UseFovKick && _playerRigidbody.velocity.sqrMagnitude > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(m_FovKick.FOVKickUp());
                }
            }
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

        private void HandleJump()
        {
            if (!_hasAnimator || !_inputManager.Jump || !_grounded || !InputEnabled || hasJumped) return;
            hasJumped = true; hasPlayedLanding = false;
            _animator.SetTrigger(_jumpHash);
            SoundFXManager.instance.playSoundFXClip(jumpClip, transform, 1f);
            nextFootstepTime = Time.time + footstepInterval;
            Debug.Log($"Jump triggered at {Time.time}");

            // Start jump bob effect
            if (m_UseHeadBob && Camera != null)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
            }
        }

        public void JumpAddForce()
        {
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
        }

        private void SampleGround()
        {
            if (!_hasAnimator) return;
            bool wasGrounded = _grounded;
            m_PreviouslyGrounded = _grounded; // Keep track for landing logic

            RaycastHit hit;
            _grounded = Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hit, Dis2Ground + 0.1f, GroundCheck);
            
            if (_grounded)
            {
                if (!wasGrounded)
                {
                    Debug.Log($"Landed at {Time.time}");
                    SoundFXManager.instance.playSoundFXClip(landingClip, transform, 1f);
                    hasPlayedLanding = true; hasJumped = false;

                    // Trigger jump bob cycle if we've landed from a fall
                    if (m_UseHeadBob && Camera != null && !m_PreviouslyGrounded)
                    {
                        StartCoroutine(m_JumpBob.DoBobCycle());
                    }
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

        private void UpdateCameraPosition()
        {
            if (Camera == null || !m_UseHeadBob) return;

            Vector3 newCameraPosition;
            
            // Apply head bob if moving and grounded
            if (_playerRigidbody.velocity.magnitude > 0 && _grounded)
            {
                float speed = _inputManager.Run ? _runSpeed : _walkSpeed;
                if (_inputManager.Crouch) speed = 1.5f;
                
                // Calculate bob based on velocity and speed
                Camera.localPosition = m_HeadBob.DoHeadBob(_playerRigidbody.velocity.magnitude + 
                                     (speed * (_inputManager.Run ? m_RunstepLenghten : 1f)));
                                     
                newCameraPosition = Camera.localPosition;
                // Apply jump bob offset
                newCameraPosition.y = Camera.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                // Just apply jump bob if not moving or in air
                newCameraPosition = m_OriginalCameraPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            
            // Apply the final position
            Camera.localPosition = newCameraPosition;
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

        private void SetColliderHeight(float height)
        {
            _collider.height = height;
            _collider.center = new Vector3(0, height / 2f, 0);
        }

        private void ResetCollider()
        {
            _collider.height = defaultColliderHeight;
            _collider.center = defaultColliderCenter;
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Added from FirstPersonController
            // This handles pushing physics objects when colliding with them
            Rigidbody body = hit.collider.attachedRigidbody;
            
            // Don't move the rigidbody if the character is on top of it
            if (_collider.bounds.min.y > hit.transform.position.y)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            
            // Add force to the object we hit
            body.AddForceAtPosition(_playerRigidbody.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}