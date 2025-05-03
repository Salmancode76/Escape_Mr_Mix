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
    [DisallowMultipleComponent]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Controller/Character Controller/First Person Controller")]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterController : PlayerController
    {
        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(1999)]
        private bool autoSetupCenter = true;

        // Stored required components.
        private CharacterController characterController;

        // Stored required properties.
        private CollisionFlags collisionFlags = CollisionFlags.None;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            characterController = GetComponent<CharacterController>();
            if (autoSetupCenter)
            {
                characterController.center = new Vector3(0.0f, characterController.height / 2f + characterController.skinWidth, 0.0f);
            }
        }

        /// <summary>
        /// OnControllerColliderHit is called when the controller hits a collider while performing a Move.
        /// </summary>
        /// <param name="hit">Detailed information about the collision and how to deal with it.</param>
        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (collisionFlags == CollisionFlags.Below || body == null || body.isKinematic)
                return;

            body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        /// <summary>
        /// Applying movement vector to controller.
        /// </summary>
        /// <param name="velocity"></param>
        protected override void Move(Vector3 velocity)
        {
            collisionFlags = characterController.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// The velocity vector of the controller. 
        /// <br>It represents the rate of change of controller position.</br>
        /// </summary>
        public override Vector3 GetVelocity()
        {
            return characterController.velocity;
        }

        /// <summary>
        /// Returns the motion update mode.
        /// </summary>
        protected sealed override UpdateMode GetUpdateMode()
        {
            return UpdateMode.Update;
        }

        /// <summary>
        /// Copy controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public sealed override void CopyBounds(out Vector3 center, out float height)
        {
            center = characterController.center;
            height = characterController.height;
        }

        /// <summary>
        /// Edit current controller collider bounds.
        /// </summary>
        /// <param name="center">Controller collider center.</param>
        /// <param name="height">Controller collider height.</param>
        public sealed override void EditBounds(Vector3 center, float height)
        {
            characterController.center = center;
            characterController.height = height;
        }

        #region [Getter / Setter]
        public CharacterController GetCharacterController()
        {
            return characterController;
        }
        #endregion
    }
}