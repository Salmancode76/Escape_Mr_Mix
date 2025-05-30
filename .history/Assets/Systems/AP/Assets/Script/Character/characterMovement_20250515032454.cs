﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class characterMovement : MonoBehaviour
{
    // Animation variables
    private Animator _animator;
    private Vector2 _currentVelocity;
    [SerializeField] private float AnimBlendSpeed = 8.9f;
    private bool _hasAnimator;
    private int _xVelHash;
    private int _yVelHash;
    private int _jumpHash;
    private int _groundHash;
    private int _crouchHash;

    public bool SeeInspector = true;

    public Rigidbody rbBodyCharacter; // Reference to the character body
    public Transform tangentStartPosition;
    public Transform objCamera; // Reference to the camera
    public GameObject addForceObj; // Position where forces is add to the character
    public Transform refHead; // use for focus camera in inGameGloabalManager in the Hierarchy

    private string s_mouseAxisX = "Mouse X"; // Default Mouse Inputs
    private string s_mouseAxisY = "Mouse Y";
    public int forwardKeyboard = 0;
    public int backwardKeyboard = 1;
    public int leftKeyboard = 2;
    public int rightKeyboard = 3;

    public int VerticalAxisBody = 0;
    public int HorizontalAxisBody = 1;

    public int JoystickVerticalAxisCam = 2;
    public int JoystickHorizontalAxisCam = 3;

    public int mouseInvertYAxisCam = 0;
    public int joystickInvertYAxisCam = 0;

    public int mouseInGameSensibilty = 0; // Sensibility chosen by the player in Game
    public int gamepadLookInGameSensibilty = 0; // Sensibility chosen by the player in Game

    public float currentDesktop_X_Axis = 0;
    public float currentDesktop_Y_Axis = 0;
    public float speedKeybordMovement = 2;

    //Crouch
    public int JoystickCrouch = 2;
    public int KeyboardCrouch = 2;

    public float minimum = -60f; // Limit camera Y movement
    public float maximum = 60f;

    public float characterSpeed = 2; // Character speed when moving left right or forward backward

    public float sensibilityMouse = 2; // Mouse sensibility
    public AnimationCurve animationCurveMouse;
    public float sensibilityJoystick = 2; // Joystick sensibility
    public AnimationCurve animationCurveJoystick;

    private float minimumAxisMovement = .2f;

    public float mouseY = 0; // current X camera Rotation

    private float tmpXAxis = 0; // temporary values
    private float tmpYAxis = 0;

    public VirtualController mobileToystickController;
    public float sensibilityMobile = 2;
    public AnimationCurve animationCurveMobile;

    private bool b_MoveForward = false; // Use on Mobile
    private bool b_MoveBackward = false;
    private bool b_MoveLeft = false; // Use on Mobile
    private bool b_MoveRight = false;

    // Mobile Move Camera Second system
    public bool b_MobileCamRotation_Stick = true;
    public float mobileSpeedRotation = 1f;
    private Vector2[] arrStartPos = new Vector2[10];
    // Mobile Move Player Second system
    public bool b_MobileMovement_Stick = true;
    public VirtualController mobileLeftJoystickToMove;
    public float LeftStickSensibility = 5;

    private float smoothStart = 0;
    public AnimationCurve animationCurveMobileSmoothMove;

    private float mouseYLastValue = 0; // Use to prevent bug on mac with the cursor lock state
    private bool b_Once = true;

    public LayerMask myLayerMask;

    private float joyHorizontal = 0;
    float joyVertical = 0;

    private float axisHorizontal = 0;
    private float AxisVertical = 0;

    private float XAxis = 0;
    private float YAxis = 0;

    private float mouseVertical = 0;

    float mouseInputX = 0;

    float yRot;
    float mouseHorizontal = 0;

    public float BrakeForce = 35f;
    public float Coeff = .15f;
    public float MaxSpeed = 1f;

    public scPreventClimbing preventClimbing;

    // Crouch
    public bool allowCrouch = false;
    public bool b_Crouch = false;
    public float targetScaleCrouch = .5f;
    private float refScaleCrouch = 1f;
    public float crouchSpeed = 3f;
    public float heightCheck = 2.05f;
    public LayerMask layerCheckCrouch;

    // Run
    public int JoystickRun = 7;
    public int KeyboardRun = 10;
    public bool bMobileRun = false;
    public float speedMultiplier = 3;
    private float currentSpeedMultiplier = 1;
    public bool b_AllowRun = false;
    public bool isRunning = false;

    // Jump
    public bool b_AllowJump = true;
    public int JoystickJump = 8;
    public int KeyboardJump = 11;
    public float jumpForce = 3;
    public float jumpSpeed = 10;
    public bool b_IsJumping = false;
    public float fallCurve;
    public AnimationCurve animFallCurve;
    public float GravityFallSpeed = 30;
    public float heightRoof = .45f;
    public float minimumJump = .6f;

    public float gravityScale = 1.0f;
    private static float globalGravity = -9.81f;
    public float MaxAngle = 70;
    private float currentAngle = 0;
    private Vector3 circlePos = Vector3.zero;
    public bool moreInfoMaxAngle = true;

    //-> Variables use to check if the character is touching the floor
    public bool isOnFloor = true;
    private float hitDistance = .35f;
    public float hitDistanceMin = .45f;
    public float hitDistanceMax = .75f;
    public LayerMask myLayer;
    public Vector3 rayPosition = Vector3.zero;

    public PhysicMaterial pMove;
    public PhysicMaterial pStop;
    public PhysicMaterial pIce;
    private CapsuleCollider charCol;

    // Use to know if the player touching something. Use to if the character is grounded
    public LayerMask myLayer02;
    public float overlapSize = .5f;
    public float overlapPos = .334f;
    public bool b_Overlap = false;

    //Layers 12 and 17. Use to know if the character is touching a door or a drawer
    public bool b_TouchLayer12_17 = false;

public float GetMouseXInput()
{
    return Input.GetAxisRaw("Mouse X") * 10f;
}

public float GetMouseYInput()
{
    return Input.GetAxisRaw("Mouse Y") * 10f;
}


    private void Start()
    {
        refScaleCrouch = gameObject.transform.localScale.y; // Save the character standing size
        charCol = GetComponent<CapsuleCollider>();

        // Animation initialization
        _hasAnimator = TryGetComponent(out _animator);
        _xVelHash = Animator.StringToHash("X_Velocity");
        _yVelHash = Animator.StringToHash("Y_Velocity");
        _jumpHash = Animator.StringToHash("Jump");
        _groundHash = Animator.StringToHash("Grounded");
        _crouchHash = Animator.StringToHash("Crouch");
    }

    public void charaGeneralMovementController()
    {
        // Left right forward backward
        if (ingameGlobalManager.instance.b_bodyMovement)
        {
            if (!b_MobileMovement_Stick || ingameGlobalManager.instance.b_DesktopInputs)
                bodyMovement();
            else
                AP_Mobile_bodyMovement_LeftStick();
        }
    }

    Vector3 joyInput = Vector3.zero;

    void Update()
    {
        joyVertical = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[JoystickVerticalAxisCam]);
        joyHorizontal = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[JoystickHorizontalAxisCam]);

        joyInput = new Vector3(joyVertical * returnInvertJoystickAxis(), joyHorizontal, 0);

        if (joyInput.sqrMagnitude > 1.0f)
            joyInput = joyInput.normalized;

        mouseHorizontal = Input.GetAxis(s_mouseAxisX);
        mouseVertical = Input.GetAxis(s_mouseAxisY);

        axisHorizontal = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[HorizontalAxisBody]);
        AxisVertical = Input.GetAxis(ingameGlobalManager.instance.inputListOfStringGamepadAxis[VerticalAxisBody]);

        XAxis = returnDesktopXAxis();
        YAxis = returnDesktopYAxis();

        mouseInputX = Input.GetAxis("Mouse X");

        if (b_MoveRight || b_MoveLeft || b_MoveBackward || b_MoveForward)
        {
            smoothStart = Mathf.MoveTowards(smoothStart, 1, Time.deltaTime * 2);
        }

        if (ingameGlobalManager.instance.b_InputIsActivated)
        {
            if (ingameGlobalManager.instance.saveAndLoadManager.b_IngameDataHasBeenLoaded
                && ingameGlobalManager.instance.b_AllowCharacterMovment
                && !ingameGlobalManager.instance.b_Ingame_Pause)
            {
                //-> Desktop Case
                if (ingameGlobalManager.instance.b_DesktopInputs)
                {
                    bodyRotation();
                    cameraRotation();
                }
                //-> Mobile case
                else
                {
                    bodyRotationMobile();
                    CamRotationMobile();
                }

                //-> Jump
                if (b_AllowJump)
                {
                    Debug.DrawRay(refHead.transform.position + Vector3.up * .1f, Vector3.up * heightRoof, Color.blue);
                    if ((Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickJump]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        ingameGlobalManager.instance.b_Joystick

                    ||

                        Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardJump]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        !ingameGlobalManager.instance.b_Joystick)

                        && !b_IsJumping && isOnFloor)
                    {
                        StartCoroutine(Jump());
                    }
                }

                //-> Crouch
                if (allowCrouch)
                {
                    if (Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickCrouch]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        ingameGlobalManager.instance.b_Joystick

                    ||

                        Input.GetKeyDown(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardCrouch]) &&
                        ingameGlobalManager.instance.b_DesktopInputs &&
                        !ingameGlobalManager.instance.b_Joystick)
                    {
                        if (b_Crouch && AP_CheckIfPlayerCanStopCrouching() || !b_Crouch)
                        {
                            b_Crouch = !b_Crouch;
                            if (_hasAnimator)
                            {
                                _animator.SetBool(_crouchHash, b_Crouch);
                            }
                        }
                    }
                }

                //-> Run
                if (b_AllowRun)
                {
                    if ((Input.GetKey(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickRun]) &&
                       ingameGlobalManager.instance.b_DesktopInputs &&
                       ingameGlobalManager.instance.b_Joystick

                   ||

                       Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardRun]) &&
                       ingameGlobalManager.instance.b_DesktopInputs &&
                       !ingameGlobalManager.instance.b_Joystick

                       ||

                       bMobileRun &&
                       !ingameGlobalManager.instance.b_DesktopInputs)

                        && !b_Crouch)
                    {
                        isRunning = true;
                        currentSpeedMultiplier = speedMultiplier;
                    }
                    else
                    {
                        isRunning = false;
                        currentSpeedMultiplier = 1;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        AP_OverlapSphere();
        Ap_isOnFloor();
        AP_ApplyGravity();

        // Crouch: Check if the character scale need to be updated
        // if (allowCrouch)
        // {
        //     if (b_Crouch && gameObject.transform.localScale.y != targetScaleCrouch)
        //     {
        //         gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale,
        //                                                               new Vector3(gameObject.transform.localScale.x, targetScaleCrouch, gameObject.transform.localScale.z),
        //                                                               Time.deltaTime * crouchSpeed);
        //     }
        //     else if (!b_Crouch && gameObject.transform.localScale.y != refScaleCrouch)
        //     {
        //         gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale,
        //                                                               new Vector3(gameObject.transform.localScale.x, refScaleCrouch, gameObject.transform.localScale.z),
        //                                                               Time.deltaTime * crouchSpeed);
        //     }
        // }

        // Update grounded state for animations
        if (_hasAnimator)
        {
            _animator.SetBool(_groundHash, isOnFloor);
        }
    }

    //--> Desktop Case : Body rotation
    private void bodyRotation()
    {
        if (ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove)
        {
            if (!ingameGlobalManager.instance.b_Joystick && mouseHorizontal != 0)
            {
                tmpXAxis = mouseInputX * 1.1f;
                tmpXAxis *= sensibilityMouse * ingameGlobalManager.instance.inputListOfFloatKeyboardButton[mouseInGameSensibilty];
            }
            else
            {
                tmpXAxis = 0;
            }

            objCamera.transform.Rotate(0, tmpXAxis, 0);
        }
        else if (ingameGlobalManager.instance.b_Joystick)
        {
            objCamera.localEulerAngles += joyInput * animationCurveJoystick.Evaluate(joyInput.magnitude) * ingameGlobalManager.instance.inputListOfFloatGamepadButton[gamepadLookInGameSensibilty] * 100f * Time.deltaTime;
            objCamera.localEulerAngles = new Vector3(ClampAngle(objCamera.localEulerAngles.x, minimum, maximum),
                                                     objCamera.localEulerAngles.y,
                                                     0);
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    //--> Desktop case : camera rotation X Axis
    private void cameraRotation()
    {
        if (ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove)
        {
            // Mouse Case
            if (!ingameGlobalManager.instance.b_Joystick && mouseVertical != 0)
            {
                tmpYAxis = mouseVertical;
                tmpYAxis = Mathf.Clamp(tmpYAxis, -3f, 3f);
                tmpYAxis *= 1.5f;
                mouseY -= tmpYAxis * sensibilityMouse * returnInvertMouseAxis() * ingameGlobalManager.instance.inputListOfFloatKeyboardButton[mouseInGameSensibilty];
            }

            mouseY = Mathf.Clamp(mouseY, minimum, maximum);

            objCamera.localEulerAngles = new Vector3(
                mouseY,
                objCamera.localEulerAngles.y,
                0);
        }
        // Joystick Case
        else if (ingameGlobalManager.instance.b_Joystick)
        {
            // camera X rotation is calculated in section bodyRotation() for gamepad inputs
        }
        //-> Prevent Mac bug with the cursor lockstate
        else if (!ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove && mouseYLastValue != Input.GetAxis(s_mouseAxisY) && b_Once)
        {
            StartCoroutine(waitToInitMouseMovement());
        }

        mouseYLastValue = Input.GetAxis(s_mouseAxisY);
    }

    private IEnumerator waitToInitMouseMovement()
    {
        b_Once = false;
        yield return new WaitForEndOfFrame();
        ingameGlobalManager.instance.mouseWaitUnitlFirstMouseMove = true;
        b_Once = true;
    }

    public void JumpAddForce()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    void bodyMovement()
    {
        addForceObj.transform.localEulerAngles = new Vector3(
            addForceObj.transform.localEulerAngles.x,
            objCamera.transform.localEulerAngles.y,
            addForceObj.transform.localEulerAngles.z);

        Vector3 Direction = new Vector3(0, 0, 0);
        Vector2 moveInput = Vector2.zero;

        if (ingameGlobalManager.instance.b_Joystick)
        {
            // --> Left and Right Movement Joystick
            if (axisHorizontal > minimumAxisMovement)
            {
                Direction += FindTangentX() * axisHorizontal;
                moveInput.x = axisHorizontal;
            }
            else if (axisHorizontal < -minimumAxisMovement)
            {
                Direction -= FindTangentX() * -axisHorizontal;
                moveInput.x = axisHorizontal;
            }
        }
        else if (ingameGlobalManager.instance.b_DesktopInputs)
        {
            Direction += FindTangentX() * XAxis;
            moveInput.x = XAxis;
        }

        if (ingameGlobalManager.instance.b_Joystick)
        {
            // --> Forward backward movement Joystick
            if (b_TouchLayer12_17 && !isOnFloor) { }
            else
            {
                if (AxisVertical > minimumAxisMovement)
                {
                    Direction += FindTangentZ() * -AxisVertical;
                    moveInput.y = -AxisVertical;
                }
                else if (AxisVertical < -minimumAxisMovement)
                {
                    Direction -= FindTangentZ() * AxisVertical;
                    moveInput.y = AxisVertical;
                }
            }
        }
        else if (ingameGlobalManager.instance.b_DesktopInputs)
        {
            if (b_TouchLayer12_17 && !isOnFloor) { }
            else
            {
                Direction += FindTangentZ() * YAxis;
                moveInput.y = YAxis;
            }
        }

        if (b_MoveRight)
        {
            Direction += FindTangentX() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
            moveInput.x = animationCurveMobileSmoothMove.Evaluate(smoothStart);
        }
        else if (b_MoveLeft)
        {
            Direction -= FindTangentX() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
            moveInput.x = -animationCurveMobileSmoothMove.Evaluate(smoothStart);
        }

        if (b_MoveForward)
        {
            Direction += FindTangentZ() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
            moveInput.y = animationCurveMobileSmoothMove.Evaluate(smoothStart);
        }
        else if (b_MoveBackward)
        {
            Direction -= FindTangentZ() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
            moveInput.y = -animationCurveMobileSmoothMove.Evaluate(smoothStart);
        }

        if (preventClimbing.b_preventClimbing)
        {
            Direction.y = 0;
        }

        if (isOnFloor)
        {
            if (currentAngle >= 180 - MaxAngle)
                rbBodyCharacter.AddForceAtPosition(Direction * characterSpeed * currentSpeedMultiplier, addForceObj.transform.position, ForceMode.Force);

            Vector3 opposite = rbBodyCharacter.transform.InverseTransformDirection(-rbBodyCharacter.velocity);
            rbBodyCharacter.AddRelativeForce(opposite * BrakeForce * Coeff, ForceMode.Force);
        }
        else
        {
            rbBodyCharacter.AddForceAtPosition(Direction * characterSpeed * currentSpeedMultiplier, addForceObj.transform.position, ForceMode.Force);
        }

        // Update animation parameters
        if (_hasAnimator)
        {
            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, moveInput.x * characterSpeed * currentSpeedMultiplier, AnimBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, moveInput.y * characterSpeed * currentSpeedMultiplier, AnimBlendSpeed * Time.fixedDeltaTime);

            // Set velocity cap based on running state
            float velocityCap = isRunning ? 6f : 3f;

            // Cap the velocity components
            _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -velocityCap, velocityCap);
            _currentVelocity.y = Mathf.Clamp(_currentVelocity.y, -velocityCap, velocityCap);

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        if (rbBodyCharacter.velocity.magnitude > MaxSpeed)
            rbBodyCharacter.velocity = rbBodyCharacter.velocity.normalized * MaxSpeed;
    }

    public IEnumerator Jump()
    {
        fallCurve = 0;
        float t = 0;
        b_IsJumping = true;

        // Trigger jump animation
        if (_hasAnimator)
        {
            _animator.SetTrigger(_jumpHash);
        }

        while (t != jumpForce)
        {
            if (!Input.GetKey(ingameGlobalManager.instance.inputListOfStringGamepadButton[JoystickJump]) &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                ingameGlobalManager.instance.b_Joystick && t > jumpForce * minimumJump

                ||

                !Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[KeyboardJump]) &&
                ingameGlobalManager.instance.b_DesktopInputs &&
                !ingameGlobalManager.instance.b_Joystick && t > jumpForce * minimumJump

                || AP_CheckIfPlayerIsTouchingRoof())
            {
                t = jumpForce;
            }
            else
            {
                rbBodyCharacter.AddForceAtPosition(Vector3.up * t * Time.deltaTime * 50, addForceObj.transform.position, ForceMode.Impulse);
                t = Mathf.MoveTowards(t, jumpForce, Time.deltaTime * jumpSpeed);
            }

            yield return null;
        }

        b_IsJumping = false;
        fallCurve = 0;

        // Reset jump trigger
        if (_hasAnimator)
        {
            _animator.ResetTrigger(_jumpHash);
        }
    }

    private float returnDesktopXAxis()
    {
        float result = currentDesktop_X_Axis;
        bool b_PressKey = false;
        if (Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[leftKeyboard]))
        {
            if (result > 0)
                result = 0;
            result = Mathf.MoveTowards(result, -1, Time.deltaTime * speedKeybordMovement);
            b_PressKey = true;
        }
        if (Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[rightKeyboard]))
        {
            if (result < 0)
                result = 0;
            result = Mathf.MoveTowards(result, 1, Time.deltaTime * speedKeybordMovement);
            b_PressKey = true;
        }

        if (!b_PressKey)
        {
            result = Mathf.MoveTowards(result, 0, Time.deltaTime * speedKeybordMovement * 2);
        }

        currentDesktop_X_Axis = result;
        return result;
    }

    private float returnDesktopYAxis()
    {
        float result = currentDesktop_Y_Axis;
        bool b_PressKey = false;
        if (Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[backwardKeyboard]))
        {
            if (result > 0)
                result = 0;
            result = Mathf.MoveTowards(result, -1, Time.deltaTime * speedKeybordMovement);
            b_PressKey = true;
        }
        if (Input.GetKey(ingameGlobalManager.instance.inputListOfStringKeyboardButton[forwardKeyboard]))
        {
            if (result < 0)
                result = 0;
            result = Mathf.MoveTowards(result, 1, Time.deltaTime * speedKeybordMovement);
            b_PressKey = true;
        }

        if (!b_PressKey)
        {
            result = Mathf.MoveTowards(result, 0, Time.deltaTime * speedKeybordMovement * 2);
        }

        currentDesktop_Y_Axis = result;
        return result;
    }

    public void CamRotationMobile()
    {
        if (b_MobileCamRotation_Stick)
        {
            float virtualJoyVertical = mobileToystickController.inputVector.z;

            mouseY -= animationCurveMobile.Evaluate(Mathf.Abs(virtualJoyVertical)) * virtualJoyVertical * sensibilityMobile;
            mouseY = Mathf.Clamp(mouseY, minimum, maximum);

            objCamera.localEulerAngles = new Vector3(
                mouseY,
                objCamera.localEulerAngles.y,
                objCamera.localEulerAngles.z);
        }
        else
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(i).deltaPosition;

                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    arrStartPos[i] = Input.GetTouch(i).position;
                }

                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    float swipe = (new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, 0) - new Vector3(arrStartPos[i].x, arrStartPos[i].y, 0)).magnitude;

                    Vector2 newPos = Input.GetTouch(i).position;

                    if (swipe > 50 && !ingameGlobalManager.instance.mobileInputFinger.checkIfFingerTouchAUIButton(newPos))
                    {
                        objCamera.transform.Rotate(Vector3.up * touchDeltaPosition.x * mobileSpeedRotation * Time.deltaTime, Space.World);

                        mouseY -= touchDeltaPosition.y * mobileSpeedRotation * Time.deltaTime;
                        mouseY = Mathf.Clamp(mouseY, minimum, maximum);

                        objCamera.localEulerAngles = new Vector3(
                            mouseY,
                            objCamera.localEulerAngles.y,
                            objCamera.localEulerAngles.z);
                    }
                }
            }
        }
    }

    private void bodyRotationMobile()
    {
        float virtualJoyVertical = mobileToystickController.inputVector.x;

        tmpXAxis = animationCurveMobile.Evaluate(Mathf.Abs(virtualJoyVertical)) * virtualJoyVertical;
        tmpXAxis *= sensibilityMobile;

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, tmpXAxis, 0));
        rbBodyCharacter.MoveRotation(rbBodyCharacter.rotation * deltaRotation);
    }

    public void pointerDrag()
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mobileToystickController.backgroundImage.rectTransform,
            mobileToystickController.eventData.position,
            mobileToystickController.eventData.pressEventCamera,
            out pos))
        {
            pos.x = pos.x / mobileToystickController.backgroundImage.rectTransform.sizeDelta.x;
            pos.y = pos.y / mobileToystickController.backgroundImage.rectTransform.sizeDelta.y;

            mobileToystickController.inputVector = new Vector3(
                pos.x * 2,
                0,
                pos.y * 2);

            if (mobileToystickController.inputVector.magnitude > 1)
            {
                mobileToystickController.inputVector = mobileToystickController.inputVector.normalized;
            }

            mobileToystickController.virtualCenter.rectTransform.anchoredPosition =
                new Vector2(mobileToystickController.inputVector.x * (mobileToystickController.backgroundImage.rectTransform.sizeDelta.x / 3),
                    mobileToystickController.inputVector.z * (mobileToystickController.backgroundImage.rectTransform.sizeDelta.y / 3));
        }
    }

    public void pointerUp()
    {
        if (mobileToystickController.inputVector != Vector3.zero)
        {
            mobileToystickController.virtualCenter.rectTransform.anchoredPosition = Vector2.zero;
        }
        mobileToystickController.inputVector = Vector3.zero;
    }

    public void pointerDrag_MoveWithLeftStick()
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mobileLeftJoystickToMove.backgroundImage.rectTransform,
            mobileLeftJoystickToMove.eventData.position,
            mobileLeftJoystickToMove.eventData.pressEventCamera,
            out pos))
        {
            pos.x = pos.x / mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.x;
            pos.y = pos.y / mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.y;

            mobileLeftJoystickToMove.inputVector = new Vector3(
                pos.x * 2,
                0,
                pos.y * 2);

            if (mobileLeftJoystickToMove.inputVector.magnitude > 1)
            {
                mobileLeftJoystickToMove.inputVector = mobileLeftJoystickToMove.inputVector.normalized;
            }

            mobileLeftJoystickToMove.virtualCenter.rectTransform.anchoredPosition =
                new Vector2(mobileLeftJoystickToMove.inputVector.x * (mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.x / 3),
                    mobileLeftJoystickToMove.inputVector.z * (mobileLeftJoystickToMove.backgroundImage.rectTransform.sizeDelta.y / 3));
        }
    }

    public void pointerUp_MoveWithLeftStick()
    {
        if (mobileLeftJoystickToMove.inputVector != Vector3.zero)
        {
            mobileLeftJoystickToMove.virtualCenter.rectTransform.anchoredPosition = Vector2.zero;
        }
        mobileLeftJoystickToMove.inputVector = Vector3.zero;
    }

    void AP_Mobile_bodyMovement_LeftStick()
    {
        if (mobileLeftJoystickToMove)
        {
            if (mobileLeftJoystickToMove.inputVector.magnitude > 1)
            {
                mobileLeftJoystickToMove.inputVector = mobileLeftJoystickToMove.inputVector.normalized;
            }

            addForceObj.transform.localEulerAngles = new Vector3(
                addForceObj.transform.localEulerAngles.x,
                objCamera.transform.localEulerAngles.y,
                addForceObj.transform.localEulerAngles.z);

            Vector3 Direction = new Vector3(0, 0, 0);
            Vector2 moveInput = Vector2.zero;

            if (mobileLeftJoystickToMove.inputVector.x > minimumAxisMovement)
            {
                Direction += FindTangentX() * mobileLeftJoystickToMove.inputVector.x;
                moveInput.x = mobileLeftJoystickToMove.inputVector.x;
            }
            else if (mobileLeftJoystickToMove.inputVector.x < -minimumAxisMovement)
            {
                Direction -= FindTangentX() * -mobileLeftJoystickToMove.inputVector.x;
                moveInput.x = mobileLeftJoystickToMove.inputVector.x;
            }

            if (mobileLeftJoystickToMove.inputVector.z > minimumAxisMovement)
            {
                Direction += FindTangentZ() * mobileLeftJoystickToMove.inputVector.z;
                moveInput.y = mobileLeftJoystickToMove.inputVector.z;
            }
            else if (mobileLeftJoystickToMove.inputVector.z < -minimumAxisMovement)
            {
                Direction -= FindTangentZ() * -mobileLeftJoystickToMove.inputVector.z;
                moveInput.y = mobileLeftJoystickToMove.inputVector.z;
            }

            if (b_MoveForward)
            {
                Direction += FindTangentZ() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
                moveInput.y = animationCurveMobileSmoothMove.Evaluate(smoothStart);
            }
            else if (b_MoveBackward)
            {
                Direction -= FindTangentZ() * animationCurveMobileSmoothMove.Evaluate(smoothStart);
                moveInput.y = -animationCurveMobileSmoothMove.Evaluate(smoothStart);
            }

            if (preventClimbing.b_preventClimbing)
            {
                Direction.y = 0;
            }

            rbBodyCharacter.AddForceAtPosition(Direction * characterSpeed * currentSpeedMultiplier, addForceObj.transform.position, ForceMode.Force);

            Vector3 opposite = rbBodyCharacter.transform.InverseTransformDirection(-rbBodyCharacter.velocity);
            rbBodyCharacter.AddRelativeForce(opposite * BrakeForce * Coeff, ForceMode.Force);

            if (_hasAnimator)
            {
                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, moveInput.x * characterSpeed * currentSpeedMultiplier, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, moveInput.y * characterSpeed * currentSpeedMultiplier, AnimBlendSpeed * Time.fixedDeltaTime);

                // Set velocity cap based on running state
                float velocityCap = isRunning ? 6f : 3f;

                // Cap the velocity components
                _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -velocityCap, velocityCap);
                _currentVelocity.y = Mathf.Clamp(_currentVelocity.y, -velocityCap, velocityCap);

                _animator.SetFloat(_xVelHash, _currentVelocity.x);
                _animator.SetFloat(_yVelHash, _currentVelocity.y);
            }

            if (rbBodyCharacter.velocity.magnitude > MaxSpeed)
                rbBodyCharacter.velocity = rbBodyCharacter.velocity.normalized * MaxSpeed;
        }
    }

    public void MoveForward()
{
    b_MoveForward = true;
    b_MoveBackward = false;
    b_MoveLeft = false;
    b_MoveRight = false;
}

public void MoveBackward()
{
    b_MoveBackward = true;
    b_MoveForward = false;
    b_MoveLeft = false;
    b_MoveRight = false;
}

public void StopMoving()
{
    b_MoveBackward = false;
    b_MoveForward = false;
    b_MoveLeft = false;
    b_MoveRight = false;
    smoothStart = 0;
}

public void MoveLeft()
{
    b_MoveBackward = false;
    b_MoveForward = false;
    b_MoveLeft = true;
    b_MoveRight = false;
}

public void MoveRight()
{
    b_MoveBackward = false;
    b_MoveForward = false;
    b_MoveLeft = false;
    b_MoveRight = true;
}

public void AP_Crouch()
{
    if (allowCrouch)
        b_Crouch = !b_Crouch;
}

Vector3 FindTangentZ()
{
    Vector3 tangente = Vector3.zero;
    RaycastHit hit2;
    if (Physics.Raycast(tangentStartPosition.position, -Vector3.up, out hit2, 10, myLayerMask))
    {
        hit2.normal.Normalize();
        tangente = Vector3.Cross(hit2.normal, -addForceObj.transform.right);
        if (tangente.magnitude == 0)
        {
            tangente = Vector3.Cross(hit2.normal, Vector3.up);
        }
        Debug.DrawRay(hit2.point, tangente, Color.yellow);
    }
    return tangente;
}

Vector3 FindTangentX()
{
    Vector3 tangente = Vector3.zero;
    RaycastHit hit2;
    if (Physics.Raycast(tangentStartPosition.position, -Vector3.up, out hit2, 10, myLayerMask))
    {
        hit2.normal.Normalize();
        Vector3 myDirection = Vector3.Cross(addForceObj.transform.right, hit2.normal);
        tangente = Vector3.Cross(hit2.normal, myDirection);
        if (tangente.magnitude == 0)
            tangente = Vector3.Cross(hit2.normal, Vector3.up);
        Debug.DrawRay(hit2.point, tangente, Color.red);
    }
    return tangente;
}

int returnInvertJoystickAxis()
{
    return ingameGlobalManager.instance.inputListOfBoolGamepadButton[joystickInvertYAxisCam] ? -1 : 1;
}

int returnInvertMouseAxis()
{
    return ingameGlobalManager.instance.inputListOfBoolKeyboardButton[mouseInvertYAxisCam] ? -1 : 1;
}

public void charaStopMoving()
{
    if (!ingameGlobalManager.instance.b_AllowCharacterMovment && rbBodyCharacter.velocity != Vector3.zero)
    {
        rbBodyCharacter.velocity = Vector3.zero;
    }
}

    private void AP_ApplyGravity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * .1f, -Vector3.up, out hit, 100.0f))
        {
            if (isOnFloor)
            {
                currentAngle = Vector3.SignedAngle(hit.normal, -Vector3.up, Vector3.up);
                gravityScale = 1 - (180 - currentAngle) / 80;
                circlePos = hit.point;
            }
        }

        if (b_TouchLayer12_17 && isOnFloor)
        {
            charCol.material = pIce;
            gravityScale = 0;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (currentAngle < 180 - MaxAngle || !isOnFloor)
        {
            charCol.material = pIce;
            fallCurve = Mathf.MoveTowards(fallCurve, 1, Time.deltaTime);
            gravityScale = Mathf.MoveTowards(gravityScale, 30, animFallCurve.Evaluate(fallCurve) * GravityFallSpeed * Time.deltaTime);
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (YAxis == 0 && XAxis == 0 && ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick ||
                 AxisVertical == 0 && axisHorizontal == 0 && ingameGlobalManager.instance.b_Joystick ||
                 mobileLeftJoystickToMove && mobileLeftJoystickToMove.inputVector.z == 0 && mobileLeftJoystickToMove.inputVector.x == 0 && !ingameGlobalManager.instance.b_DesktopInputs && b_MobileMovement_Stick ||
                 !b_MoveForward && !b_MoveBackward && !b_MoveLeft && !b_MoveRight && !ingameGlobalManager.instance.b_DesktopInputs && !b_MobileMovement_Stick)
        {
            charCol.material = pStop;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }
        else if (YAxis == 0 && XAxis != 0 && ingameGlobalManager.instance.b_DesktopInputs && !ingameGlobalManager.instance.b_Joystick ||
                 AxisVertical == 0 && axisHorizontal != 0 && ingameGlobalManager.instance.b_Joystick ||
                 mobileLeftJoystickToMove && mobileLeftJoystickToMove.inputVector.z == 0 && mobileLeftJoystickToMove.inputVector.x != 0 && !ingameGlobalManager.instance.b_DesktopInputs && b_MobileMovement_Stick ||
                 !b_MoveForward && !b_MoveBackward && (b_MoveLeft || b_MoveRight) && !ingameGlobalManager.instance.b_DesktopInputs && !b_MobileMovement_Stick)
        {
            charCol.material = pMove;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }
        else
        {
            charCol.material = pMove;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (rbBodyCharacter.velocity.sqrMagnitude * 10000 < 2 && YAxis == 0 && XAxis == 0 && ingameGlobalManager.instance.b_DesktopInputs && isOnFloor && !b_TouchLayer12_17 && !ingameGlobalManager.instance.b_Joystick ||
            rbBodyCharacter.velocity.sqrMagnitude * 10000 < 2 && AxisVertical == 0 && axisHorizontal == 0 && ingameGlobalManager.instance.b_Joystick && isOnFloor && !b_TouchLayer12_17 ||
            mobileLeftJoystickToMove && rbBodyCharacter.velocity.sqrMagnitude * 10000 < 2 && mobileLeftJoystickToMove.inputVector.z == 0 && mobileLeftJoystickToMove.inputVector.x == 0 && !ingameGlobalManager.instance.b_DesktopInputs && isOnFloor && !b_TouchLayer12_17 && b_MobileMovement_Stick ||
            rbBodyCharacter.velocity.sqrMagnitude * 10000 < 2 && !b_MoveForward && !b_MoveBackward && !b_MoveLeft && !b_MoveRight && !ingameGlobalManager.instance.b_DesktopInputs && isOnFloor && !b_TouchLayer12_17 && !b_MobileMovement_Stick)
        {
            rbBodyCharacter.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }

        if (b_IsJumping)
        {
            charCol.material = pIce;
            rbBodyCharacter.constraints = RigidbodyConstraints.FreezeRotation;
            gravityScale = 0;
        }

        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rbBodyCharacter.AddForce(gravity, ForceMode.Acceleration);
    }

    public void Ap_isOnFloor()
    {
        float offset = .6f * (180 - currentAngle) / 80;

        if (isOnFloor)
            hitDistance = hitDistanceMax + offset;
        else
            hitDistance = hitDistanceMin + offset;

        if (Physics.Raycast(transform.position + Vector3.up * .1f, -Vector3.up, hitDistance, myLayer))
        {
            if (b_Overlap)
                isOnFloor = true;
            rayPosition = transform.position + Vector3.up * .1f;
        }
        else
        {
            if (b_Overlap)
                isOnFloor = false;
            rayPosition = transform.position;
        }
    }

    private void AP_OverlapSphere()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.up * overlapPos, overlapSize, myLayer02);

        if (hits.Length > 0)
        {
            b_Overlap = true;
        }
        else
        {
            b_Overlap = false;
            isOnFloor = false;
        }
    }

    private bool AP_CheckIfPlayerCanStopCrouching()
    {
        Debug.DrawRay(transform.position + Vector3.up * .1f, Vector3.up * heightCheck, Color.yellow);
        if (Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.up, heightCheck, layerCheckCrouch))
            return false;
        else
            return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {
            b_TouchLayer12_17 = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {
            b_TouchLayer12_17 = true;
        }
        if (collision.gameObject.layer == 18)
        {
            isOnFloor = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 12 || collision.gameObject.layer == 17)
        {
            b_TouchLayer12_17 = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(circlePos, .1f);

        if (rayPosition == transform.position)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.blue;

        Gizmos.DrawSphere(rayPosition, .1f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + Vector3.up * overlapPos, overlapSize);
    }

    private bool AP_CheckIfPlayerIsTouchingRoof()
    {
        if (Physics.Raycast(refHead.transform.position + Vector3.up * .1f, Vector3.up, heightRoof, layerCheckCrouch))
            return true;
        else
            return false;
    }
}