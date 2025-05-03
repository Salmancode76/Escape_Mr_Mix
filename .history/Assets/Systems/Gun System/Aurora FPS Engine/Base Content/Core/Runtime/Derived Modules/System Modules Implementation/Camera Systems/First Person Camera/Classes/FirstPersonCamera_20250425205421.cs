/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Mathematics;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.CameraSystems
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Camera Systems/First Person Camera")]
    [DisallowMultipleComponent]
    public class FirstPersonCamera : PlayerCamera
    {
        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        [Order(-899)]
        private Vector2 rotationSmooth = new Vector2(20, 20);

        [SerializeField]
        [Foldout("Control Settings", Style = "Header")]
        [Order(-898)]
        private bool clampVerticalRotation = true;

        [SerializeField]
        [MinMaxSlider(-180, 180)]
        [Foldout("Control Settings", Style = "Header")]
        [VisibleIf("clampVerticalRotation")]
        [Indent(1)]
        [Order(-897)]
        private Vector2 verticalRotationLimits = new Vector2(-90, 90);
      
        [SerializeField]
        [Slider(0, 1)]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(-596)]
        private float crouchHeightPercent = 0.6f;

        // Stored required properties.
        private float xSmoothAngle;
        private float defaultCameraHeight;
        private float crouchCameraHeight;
        private float crouchStandHeightDifference;
        private float defaultControllerHeight;
        private float crouchControllerHeight;
        private Quaternion yDesiredRotation;
        private Quaternion ySmoothRotation;
        private Vector2 desiredVector;

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            PlayerController playerController = GetPlayerController();
            Debug.Assert(playerController != null, $"<b><color=#FF0000>Attach reference of the player controller to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Controller<i>(field)</i>.</color></b>");

            playerController.OnCrouchingCallback += CameraCrouchProcessing;

            playerController.CopyBounds(out Vector3 center, out defaultControllerHeight);
            crouchControllerHeight = defaultControllerHeight * crouchHeightPercent;
            crouchStandHeightDifference = defaultControllerHeight - crouchControllerHeight;

            defaultCameraHeight = GetHinge().localPosition.y;
            crouchCameraHeight = defaultCameraHeight - crouchStandHeightDifference;
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        protected override void ApplyCameraRotation(Transform camera)
        {
            Vector2 customRotation = GetCustomRotation();
            desiredVector.y += GetControlInput().y * Time.deltaTime;
            desiredVector.y += customRotation.y;
            customRotation.y = 0;
            SetCustomRotation(customRotation);

            if (clampVerticalRotation)
                desiredVector.y = Math.Clamp(desiredVector.y, verticalRotationLimits);

            yDesiredRotation = Quaternion.AngleAxis(desiredVector.y, -Vector3.right);
            ySmoothRotation = Quaternion.Slerp(ySmoothRotation, yDesiredRotation, rotationSmooth.y * Time.deltaTime);
            GetHinge().localRotation = ySmoothRotation;
        }

        /// <summary>
        /// The camera target is rotated.
        /// </summary>
        protected override void ApplyTargetRotation(Transform target)
        {
            Vector2 customRotation = GetCustomRotation();
            desiredVector.x = GetControlInput().x * Time.deltaTime;
            desiredVector.x += customRotation.x;
            customRotation.x = 0;
            SetCustomRotation(customRotation);

            xSmoothAngle = Mathf.Lerp(xSmoothAngle, desiredVector.x, rotationSmooth.x * Time.deltaTime);
            target.Rotate(Vector3.up, xSmoothAngle, Space.Self);
            GetHinge().localRotation = ySmoothRotation;
        }

        /// <summary>
        /// Restore camera to default.
        /// </summary>
        public override void Restore()
        {
            base.Restore();
            Vector3 localPosition = GetHinge().localPosition;
            localPosition.y = defaultCameraHeight;
            GetHinge().localPosition = localPosition;
        }

        private void CameraCrouchProcessing(bool crouch, float time)
        {
            Vector3 cameraPosition = GetHinge().localPosition;
            float desiredCameraHeight = crouch ? crouchCameraHeight : defaultCameraHeight;

            cameraPosition.y = Mathf.Lerp(cameraPosition.y, desiredCameraHeight, time);
            GetHinge().localPosition = cameraPosition;
        }

        #region [Getter / Setter]
        public float GetVerticalRotationMin()
        {
            return verticalRotationLimits.x;
        }

        public float GetVerticalRotationMax()
        {
            return verticalRotationLimits.y;
        }

        public void SetVerticalRotationLimits(float min, float max)
        {
            verticalRotationLimits.x = min;
            verticalRotationLimits.y = max;
        }

        public bool ClampVerticalRotation()
        {
            return clampVerticalRotation;
        }

        public void ClampVerticalRotation(bool value)
        {
            clampVerticalRotation = value;
        }

        public Vector2 GetRotationSmooth()
        {
            return rotationSmooth;
        }

        public void SetRotationSmooth(Vector2 value)
        {
            rotationSmooth = value;
        }

        public float GetCrouchHeightPercent()
        {
            return crouchHeightPercent;
        }

        public void SetCrouchHeightPercent(float value)
        {
            crouchHeightPercent = value;
        }
        #endregion
    }
}
