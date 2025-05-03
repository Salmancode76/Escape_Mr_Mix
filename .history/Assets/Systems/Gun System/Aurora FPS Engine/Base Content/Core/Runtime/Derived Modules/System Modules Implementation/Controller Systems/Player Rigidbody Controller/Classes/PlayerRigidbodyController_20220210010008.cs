/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Controller/Rigidbody Controller/First Person Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerRigidbodyController : PlayerController
    {
        // Stored required components.
        private new Rigidbody rigidbody;
        private CapsuleCollider capsuleCollider;

        // Stored physics materials.
        private PhysicMaterial idlePhysicMaterial;
        private PhysicMaterial movePhysicMaterial;
        private PhysicMaterial airPhysicMaterial;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            CopyIdlePhysicMaterial(out idlePhysicMaterial);
            CopyMovePhysicMaterial(out movePhysicMaterial);
            CopyAirPhysicMaterial(out airPhysicMaterial);
        }

        /// <summary>
        /// Called every fixed frame-rate frame.
        /// <br>0.02 seconds (50 calls per second) is the default time between calls.</br>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdatePhysicMaterial();
        }

        /// <summary>
        /// Applying movement vector to controller.
        /// </summary>
        /// <param name="velocity">Movement velocity.</param>
        protected override void Move(Vector3 velocity)
        {
            rigidbody.velocity = velocity;
        }

        /// <summary>
        /// The velocity vector of the controller. 
        /// <br>It represents the rate of change of controller position.</br>
        /// </summary>
        public override Vector3 GetVelocity()
        {
            return rigidbody.velocity;
        }

        /// <summary>
        /// Returns the motion update mode.
        /// </summary>
        protected sealed override UpdateMode GetUpdateMode()
        {
            return UpdateMode.FixedUpdate;
        }

        /// <summary>
        /// Copy controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public sealed override void CopyBounds(out Vector3 center, out float height)
        {
            center = capsuleCollider.center;
            height = capsuleCollider.height;
        }

        /// <summary>
        /// Edit current controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public sealed override void EditBounds(Vector3 center, float height)
        {
            capsuleCollider.center = center;
            capsuleCollider.height = height;
        }

        /// <summary>
        /// Called every fixed frame-rate frame, 
        /// for switching capsule collider physic material relative controller state.
        /// </summary>
        protected virtual void UpdatePhysicMaterial()
        {
            if (IsGrounded() && GetControlInput() == Vector2.zero)
                capsuleCollider.material = idlePhysicMaterial;
            else if (IsGrounded() && GetControlInput() != Vector2.zero)
                capsuleCollider.material = movePhysicMaterial;
            else
                capsuleCollider.material = airPhysicMaterial;
        }

        /// <summary>
        /// Used while controller is idle.
        /// </summary>
        protected virtual void CopyMovePhysicMaterial(out PhysicMaterial physicMaterial)
        {
            physicMaterial = new PhysicMaterial();
            physicMaterial.name = "Move Physic Material";
            physicMaterial.staticFriction = .25f;
            physicMaterial.dynamicFriction = .25f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }

        /// <summary>
        /// Used while controller is moving.
        /// </summary>
        protected virtual void CopyIdlePhysicMaterial(out PhysicMaterial physicMaterial)
        {
            physicMaterial = new PhysicMaterial();
            physicMaterial.name = "Idle Physic Material";
            physicMaterial.staticFriction = 1f;
            physicMaterial.dynamicFriction = 1f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }

        /// <summary>
        /// Used while controller is in air.
        /// </summary>
        protected virtual void CopyAirPhysicMaterial(out PhysicMaterial physicMaterial)
        {
            physicMaterial = new PhysicMaterial();
            physicMaterial.name = "Air Physic Material";
            physicMaterial.staticFriction = 0f;
            physicMaterial.dynamicFriction = 0f;
            physicMaterial.bounciness = 0.0f;
            physicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            physicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }

        #region [Getter / Setter]
        public Rigidbody GetRigidbody()
        {
            return rigidbody;
        }

        public CapsuleCollider GetCapsuleCollider()
        {
            return capsuleCollider;
        }
        #endregion
    }
}
