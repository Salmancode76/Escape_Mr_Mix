using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager;

namespace UnityTutorial.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        // Sound FX
        [SerializeField] private AudioClip[] walkClipsArray;
        [SerializeField] private float footstepInterval = 0.4f;
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;

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

        // Footstep Audio
        private AudioClip[] footstepClips;
        private int currentStepIndex = 0;
        private float footstepTimer = 0f;
        private bool useOrderedSequence = true;
        private List<int> randomSequence;

        // Collider defaults
        private float defaultColliderHeight;
        private Vector3 defaultColliderCenter;

        // Slide variables
        private bool isSliding = false;
        private float slideTimer = 0f;
        private float slideDuration = 1.0f; // Slide duration in seconds

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
            _slideHash = Animator.StringToHash("Slide");

            footstepClips = walkClipsArray;
        }

        private void FixedUpdate()
        {
            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
            HandleSlide();
        }

        private void LateUpdate()
        {
            CamMovements();
        }

        private void Move()
        {
            if (!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            if (_inputManager.Crouch) targetSpeed = 1.5f;
            if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

            bool isMoving = _inputManager.Move != Vector2.zero;

            // Use a temporary vector for move input.
            Vector2 moveInput = _inputManager.Move;
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            if (_grounded)
            {
                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, moveInput.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, moveInput.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

                float xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
                float zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);

                HandleFootsteps(isMoving);
            }
            else
            {
                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance, 0, _currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
                footstepTimer = 0f;
            }

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
            Debug.Log(_animator.GetFloat(_xVelHash));
            Debug.Log(_animator.GetFloat(_yVelHash));
        }

        private void HandleFootsteps(bool isMoving)
        {
            if (!isMoving)
            {
                footstepTimer = 0f;
                useOrderedSequence = true;
                currentStepIndex = 0;
                return;
            }

            // Adjust interval if running.
            float effectiveInterval = _inputManager.Run ? footstepInterval / 1.5f : footstepInterval;
            footstepTimer += Time.fixedDeltaTime;

            if (footstepTimer >= effectiveInterval)
            {
                footstepTimer = 0f;
                PlayNextFootstep();
            }
        }

        private void PlayNextFootstep()
        {
            if (footstepClips.Length == 0)
                return;

            AudioClip clipToPlay = null;

            if (useOrderedSequence)
            {
                clipToPlay = footstepClips[currentStepIndex];
                currentStepIndex++;

                if (currentStepIndex >= footstepClips.Length)
                {
                    useOrderedSequence = false;
                    currentStepIndex = 0;
                    randomSequence = GenerateRandomSequence();
                }
            }
            else
            {
                int randomIndex = randomSequence[currentStepIndex];
                clipToPlay = footstepClips[randomIndex];
                currentStepIndex++;

                if (currentStepIndex >= footstepClips.Length)
                {
                    currentStepIndex = 0;
                    randomSequence = GenerateRandomSequence();
                }
            }

            SoundFXManager.instance.playSoundFXClip(clipToPlay, transform, 1f);
        }

        private List<int> GenerateRandomSequence()
        {
            List<int> sequence = new List<int>();
            for (int i = 0; i < footstepClips.Length; i++)
                sequence.Add(i);

            for (int i = 0; i < sequence.Count; i++)
            {
                int rnd = UnityEngine.Random.Range(i, sequence.Count);
                int temp = sequence[rnd];
                sequence[rnd] = sequence[i];
                sequence[i] = temp;
            }
            return sequence;
        }

        private void CamMovements()
        {
            if (!_hasAnimator) return;

            float Mouse_X = _inputManager.Look.x;
            float Mouse_Y = _inputManager.Look.y;
            Camera.position = CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void HandleCrouch()
        {
            // If not sliding, adjust collider based on crouch toggle.
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

        private void HandleSlide()
        {
            if (isSliding)
            {
                slideTimer -= Time.fixedDeltaTime;
                if (slideTimer <= 0f)
                {
                    isSliding = false;
                    ResetCollider();
                }
                return;
            }

            if (_inputManager.Slide && _grounded)
            {
                // Do not slide if moving backwards.
                if (_inputManager.Move.y < 0)
                {
                    _inputManager.Slide = false;
                    return;
                }

                // Begin slide.
                isSliding = true;
                slideTimer = slideDuration;
                _animator.SetBool(_slideHash, true);

                // Set collider to quarter height.
                SetColliderHeight(defaultColliderHeight * 0.25f);

                // If standing still, move forward 15 studs.
                if (_inputManager.Move == Vector2.zero)
                    _playerRigidbody.MovePosition(transform.position + transform.forward * 15f);

                // Reset the slide trigger.
                _inputManager.Slide = false;
            }
            else
            {
                _animator.SetBool(_slideHash, false);
            }
        }

        private void HandleJump()
        {
            if (!_hasAnimator) return;
            if (!_inputManager.Jump) return;
            if (!_grounded) return;

            _animator.SetTrigger(_jumpHash);
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
            RaycastHit hitInfo;
            if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f, GroundCheck))
            {
                _grounded = true;
                SetAnimationGrounding();
                return;
            }
            _grounded = false;
            _animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
            SetAnimationGrounding();
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_fallingHash, !_grounded);
            _animator.SetBool(_groundHash, _grounded);
        }

        private void SetColliderHeight(float newHeight)
        {
            if (_collider != null)
            {
                _collider.height = newHeight;
                // Adjust the center based on the new height.
                float heightDiff = defaultColliderHeight - newHeight;
                _collider.center = defaultColliderCenter - new Vector3(0, heightDiff / 2f, 0);
            }
        }

        private void ResetCollider()
        {
            if (_collider != null)
            {
                _collider.height = defaultColliderHeight;
                _collider.center = defaultColliderCenter;
            }
        }
    }
}
