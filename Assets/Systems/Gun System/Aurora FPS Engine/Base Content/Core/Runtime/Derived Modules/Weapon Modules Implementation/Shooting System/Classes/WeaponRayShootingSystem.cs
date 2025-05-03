/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Shooting/Raycast Shooting System")]
    [DisallowMultipleComponent]
    public class WeaponRayShootingSystem : WeaponShootingSystem
    {
        private readonly Vector3 ViewportCenter = new Vector3(0.5f, 0.5f, 0.0f);

        [SerializeField]
        [NotNull]
        private BulletItem bulletItem;

        [SerializeField] 
        [MinValue(0.0f)]
        private float fireRange = 500.0f;

        [SerializeField] 
        private LayerMask cullingLayer = Physics.AllLayers;

        // Stored required components.
        private new Camera camera;

        // Stored required properties.
        private HashSet<int> killedInstanceIDs;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            camera = Camera.main;
            killedInstanceIDs = new HashSet<int>();
        }

        protected override void MakeShoot()
        {
            Ray ray = CalculateShootRay();
            ray.direction.Normalize();
            if (Physics.Raycast(ray, out RaycastHit hitInfo, fireRange, cullingLayer, QueryTriggerInteraction.Ignore))
            {
                Transform hitTransform = hitInfo.transform;
                Decal.Spawn(bulletItem.GetDecalMapping(), hitInfo);
                SendDamage(hitTransform);
                AddImpulseForce(hitTransform, ray.direction);
                OnFireHitCallback?.Invoke(hitInfo);
            }
        }

        protected virtual Ray CalculateShootRay()
        {
            Ray ray = camera.ViewportPointToRay(ViewportCenter);
            ray.origin = GetFirePoint().position;
            return ray;
        }

        /// <summary>
        /// Send damage to transform containing IHealth component.
        /// </summary>
        private void SendDamage(Transform other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(bulletItem.GetDamage());

                IHealth health = GetComponent<IHealth>();
                if (health.IsAlive())
                {
                    killedInstanceIDs.Remove(other.root.GetInstanceID());
                    OnHealthHitCallback?.Invoke(other);
                }
                else if (killedInstanceIDs.Add(other.root.GetInstanceID()))
                {
                    OnHealthKillCallback?.Invoke(other);
                }
            }
        }

        /// <summary>
        /// Add impulse force to rigidbody transform.
        /// </summary>
        private void AddImpulseForce(Transform other, Vector3 direction)
        {
            Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.AddForce(direction * bulletItem.GetImpactImpulse(), ForceMode.Impulse);
            }
        }

        #region [Event Callback Functions]
        /// <summary>
        /// On fire hit callback function.
        /// OnFireHitCallback called when ray fire hitted on any collider.
        /// </summary>
        /// <param name="RaycastHit">Fire raycast hit info.</param>
        public event Action<RaycastHit> OnFireHitCallback;

        /// <summary>
        /// On health hit callback function.
        /// OnHealthHitCallback called when raycast hitted on any alive object with component implemented from HealthComponent abstract class.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnHealthHitCallback;

        /// <summary>
        /// On health kill callback function.
        /// OnHealthKillCallback called every time when raycast hitted on any object with component implemented from HealthComponent abstract class and kills it.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnHealthKillCallback;
        #endregion

        #region [Getter / Setter]
        public BulletItem GetBulletItem()
        {
            return bulletItem;
        }

        public void SetBulletItem(BulletItem value)
        {
            bulletItem = value;
        }

        public float GetFireRange()
        {
            return fireRange;
        }

        public void SetFireRange(float value)
        {
            fireRange = value;
        }

        public LayerMask GetCullingLayer()
        {
            return cullingLayer;
        }

        public void SetCullingLayer(LayerMask value)
        {
            cullingLayer = value;
        }
        #endregion
    }
}