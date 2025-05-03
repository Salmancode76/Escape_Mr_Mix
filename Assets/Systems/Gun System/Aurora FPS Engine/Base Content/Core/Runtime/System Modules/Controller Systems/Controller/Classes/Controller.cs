/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class Controller : MonoBehaviour, IControllerVelocity, IControllerGrounded
    {
        [SerializeField]
        [Label("Radius")]
        [Foldout("Grounded Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(991)]
        private float groundCheckRadius = 0.3f;

        [SerializeField]
        [Label("Range")]
        [Foldout("Grounded Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(992)]
        private float groundCheckRange = 0.85f;

        [SerializeField]
        [Label("Culling Layer")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(993)]
        private LayerMask groundCullingLayer = 1 << 0;

        // Stored required properties.
        private bool isGrounded;
        private bool previouslyGrounded = true;
        private RaycastHit groundHitInfo;

        /// <summary>
        /// Called every fixed frame-rate frame.
        /// <br>0.02 seconds (50 calls per second) is the default time between calls.</br>
        /// </summary>
        protected virtual void FixedUpdate()
        {
            isGrounded = CalculateGrounded(transform.position, out groundHitInfo);
            GroundedCallbackHandler();
        }

        /// <summary>
        /// Whether the controller is currently on the ground.
        /// </summary>
        /// <param name="origin">Transform position.</param>
        /// <param name="hitInfo">Ground hit info.</param>
        protected virtual bool CalculateGrounded(Vector3 origin, out RaycastHit hitInfo)
        {
            return Physics.SphereCast(origin, groundCheckRadius, -transform.up, out hitInfo, groundCheckRange, groundCullingLayer);
        }

        /// <summary>
        /// Projects a vector onto a ground defined by a hit normal orthogonal to the ground.
        /// </summary>
        /// <param name="vector">Desired direction vector.</param>
        /// <returns>The location of the vector on the plane.</returns>
        public Vector3 FlattenSlopes(Vector3 vector)
        {
            return Vector3.ProjectOnPlane(vector, groundHitInfo.normal);
        }

        /// <summary>
        /// Whether the controller is currently on the slope.
        /// </summary>
        /// <returns>Slope state.</returns>
        public bool OnSlope()
        {
            return groundHitInfo.normal != transform.up;
        }

        /// <summary>
        /// OnGroundedCallback handler.
        /// </summary>
        protected void GroundedCallbackHandler()
        {
            if (!previouslyGrounded && isGrounded)
            {
                OnGroundedCallback?.Invoke();
            }
            else if (previouslyGrounded && !isGrounded)
            {
                OnBecomeAirCallback?.Invoke();
            }
            previouslyGrounded = isGrounded;
        }

        #region [IControllerVelocity Implementation]
        /// <summary>
        /// The velocity vector of the controller. 
        /// <br>It represents the rate of change of controller position.</br>
        /// </summary>
        public abstract Vector3 GetVelocity();
        #endregion

        #region [IControllerGrounded Implementation]
        /// <summary>
        /// Controller is grounded at the moment.
        /// </summary>
        /// <returns>Grounded state.</returns>
        public bool IsGrounded()
        {
            return isGrounded;
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when the controller lifts off the ground.
        /// </summary>
        public event Action OnBecomeAirCallback;

        /// <summary>
        /// Called when the controller lands on the ground.
        /// </summary>
        public event Action OnGroundedCallback;

        #endregion

        #region [Getter / Setter]
        public float GetGroundCheckRadius()
        {
            return groundCheckRadius;
        }

        public void SetGroundCheckRadius(float value)
        {
            groundCheckRadius = value;
        }

        public float GetGroundCheckRange()
        {
            return groundCheckRange;
        }

        public void SetGroundCheckRange(float value)
        {
            groundCheckRange = value;
        }

        public LayerMask GetGroundCullingLayer()
        {
            return groundCullingLayer;
        }

        public void SetGroundCullingLayer(LayerMask value)
        {
            groundCullingLayer = value;
        }

        public RaycastHit GetGroundHitInfo()
        {
            return groundHitInfo;
        }
        #endregion
    }
}

