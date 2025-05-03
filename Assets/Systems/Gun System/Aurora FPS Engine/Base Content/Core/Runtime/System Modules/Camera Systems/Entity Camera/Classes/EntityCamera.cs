/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.CameraSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class EntityCamera : MonoBehaviour, IRestorable
    {
        [SerializeField]
        [NotNull(Format = "Set camera as child of empty game object, which will be used as Hinge and attach it here.", Size = MessageBoxSize.Big)]
        [Order(-998)]
        private Transform hinge;

        [SerializeField]
        [NotNull(Format = "Attach an object relative to which the camera will rotate.")]
        [Order(-997)]
        private Transform target;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            Debug.Assert(hinge != null, $"<b><color=#FF0000>Attach reference of hinge transform to the {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Hinge<i>(field)</i>.</color></b>");
            Debug.Assert(target != null, $"<b><color=#FF0000>Attach reference of target transform to the {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Target<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        protected virtual void LateUpdate()
        {
            ApplyCameraRotation(hinge);
            ApplyTargetRotation(target);
        }

        /// <summary>
        /// Apply rotation to hinge transform.
        /// </summary>
        /// <param name="hinge">Reference of hinge transform.</param>
        protected abstract void ApplyCameraRotation(Transform hinge);

        /// <summary>
        /// Apply rotation to target transform.
        /// </summary>
        /// <param name="target">Reference of target transform.</param>
        protected abstract void ApplyTargetRotation(Transform target);

        /// <summary>
        /// Restore camera to default.
        /// </summary>
        public virtual void Restore()
        {
            hinge.localRotation = Quaternion.identity;
        }

        #region [Getter / Setter]
        public Transform GetTarget()
        {
            return target;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public Transform GetHinge()
        {
            return hinge;
        }

        public void SetHinge(Transform hinge)
        {
            this.hinge = hinge;
        }
        #endregion
    }
}