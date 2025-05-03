/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.CoreModules.PhysicsEngine;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Shooting/Physics Shooting System")]
    [DisallowMultipleComponent]
    public class WeaponPhysicsShootingSystem : WeaponShootingSystem
    {
        protected readonly Vector3 ViewportCenter = new Vector3(0.5f, 0.5f, 0.0f);

        [SerializeField]
        [NotNull]
        private PhysicsBullet bullet;

        [SerializeField]
        [MinValue(1.0f)]
        private float bulletSpeedMultiplier = 1.15f;

        // Stored required components.
        private new Camera camera;
        private PoolManager poolManager;

        // Stored required properties.
        private LayerMask cullingLayer;
        private HashSet<int> killedInstanceIDs;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            camera = Camera.main;
            poolManager = PoolManager.GetRuntimeInstance();
            LayerMask physicsShellLayer = LayerMask.NameToLayer("Physics Shell");
            cullingLayer = PhysicsCollisionMatrix.LoadMaskForLayer(physicsShellLayer.value);
            killedInstanceIDs = new HashSet<int>();
            RegisterBulletCallbacks();
        }

        protected override void MakeShoot()
        {
            Vector3 direction = CalculateShootDirection().normalized;
            PhysicsBullet physicsBullet = poolManager.CreateOrPop<PhysicsBullet>(bullet, GetFirePoint().position, Quaternion.LookRotation(direction));
            physicsBullet.ApplySpeed(direction, bulletSpeedMultiplier);
            OnFireBulletCallback?.Invoke(physicsBullet);
        }

        protected void RegisterBulletCallbacks()
        {
            OnFireBulletCallback += (bullet) =>
            {
                bullet.OnCollisionCallback += OnBulletCollisionCallbackWrapper;
                bullet.OnHealthCollisionCallback += OnBulletHealthCollisionCallbackWrapper;
                bullet.OnHealthCollisionKillCallback += OnBulletHealthCollisionKIllCallbackWrapper;

                bullet.OnBeforePushCallback += () =>
                {
                    bullet.OnCollisionCallback -= OnBulletCollisionCallbackWrapper;
                    bullet.OnHealthCollisionCallback -= OnBulletHealthCollisionCallbackWrapper;
                    bullet.OnHealthCollisionKillCallback -= OnBulletHealthCollisionKIllCallbackWrapper;
                };
            };
        }

        private void OnBulletCollisionCallbackWrapper(Transform other)
        {
            OnBulletCollisionCallback?.Invoke(other);
        }

        private void OnBulletHealthCollisionCallbackWrapper(Transform other)
        {
            killedInstanceIDs.Remove(other.root.GetInstanceID());
            OnBulletHealthCollisionCallback?.Invoke(other);
        }

        private void OnBulletHealthCollisionKIllCallbackWrapper(Transform other)
        {
            if (killedInstanceIDs.Add(other.root.GetInstanceID()))
            {
                OnBulletHealthCollisionKillCallback?.Invoke(other);
            }
        }

        protected virtual Vector3 CalculateShootDirection()
        {
            Vector3 direction = camera.transform.forward;
            Ray ray = camera.ViewportPointToRay(ViewportCenter);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, cullingLayer))
            {
                direction = hitInfo.point - GetFirePoint().position;
            }
            return direction;
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when weapon fired and instantiating new physics bullet.
        /// </summary>
        /// <param name="PhysicsBullet">Fired bullet instance.</param>
        public event Action<PhysicsBullet> OnFireBulletCallback;

        /// <summary>
        /// Called when bullet become collide any of other collider.
        /// </summary>
        /// <param name="Transform">The Transform data associated with this collision.</param>
        public event Action<Transform> OnBulletCollisionCallback;

        /// <summary>
        /// Called when bullet has become collide any of component which implemented of IDamageable interface.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnBulletHealthCollisionCallback;

        /// <summary>
        /// Called when bull has become kill any of component which implemented of IHealth interface.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnBulletHealthCollisionKillCallback;
        #endregion

        #region [Getter / Setter]
        public PhysicsBullet GetBullet()
        {
            return bullet;
        }

        public void SetBullet(PhysicsBullet value)
        {
            bullet = value;
        }

        public float GetImpulseAmplifier()
        {
            return bulletSpeedMultiplier;
        }

        public void SetImpulseAmplifier(float value)
        {
            bulletSpeedMultiplier = value;
        }

        public Camera GetMainCamera()
        {
            return camera;
        }

        public LayerMask GetBulletCollisionMask()
        {
            return cullingLayer;
        }

        public PoolManager GetPoolManager()
        {
            return poolManager;
        }

        protected void SetPoolManager(PoolManager value)
        {
            poolManager = value;
        }

        protected Camera GetCamera()
        {
            return camera;
        }
        #endregion
    }
}