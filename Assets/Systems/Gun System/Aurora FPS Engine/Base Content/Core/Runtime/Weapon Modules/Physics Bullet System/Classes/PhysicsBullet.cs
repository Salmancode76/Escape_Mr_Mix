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
using AuroraFPSRuntime.SystemModules;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Physics Shell/Physics Bullet")]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class PhysicsBullet : PoolObject
    {
        // Base physics shell properties.
        [SerializeField] 
        [NotNull]
        private BulletItem bulletItem;

        [SerializeField]
        private float bulletSpeed = 50;

        // Stored required components.
        private new Rigidbody rigidbody;
        private new Collider collider;

        // Stored required properties.
        private HashSet<int> killedInstanceIDs;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            killedInstanceIDs = new HashSet<int>();
        }

        public virtual void ApplySpeed(Vector3 direction)
        {
            rigidbody.AddForce(direction * bulletSpeed, ForceMode.Impulse);
        }

        public virtual void ApplySpeed(Vector3 direction, float speedMultiplier)
        {
            rigidbody.AddForce(direction * (bulletSpeed + speedMultiplier), ForceMode.Impulse);
        }

        /// <summary>
        /// Called before pushing object to pool.
        /// </summary>
        protected override void OnBeforePush()
        {
            base.OnBeforePush();
            rigidbody.velocity = Vector3.zero;
        }

        /// <summary>
        /// OnCollisionEnter is called when this collider/rigidbody has begun
        /// touching another rigidbody/collider.
        /// </summary>
        /// <param name="other">The Collision data associated with this collision.</param>
        protected virtual void OnCollisionEnter(Collision other)
        {
            Transform otherTransform = other.transform;
            SendDamage(otherTransform, bulletItem.GetDamage());
            SendImpulse(otherTransform, bulletItem.GetImpactImpulse());
            Decal.Spawn(bulletItem.GetDecalMapping(), other.contacts[0]);
            Push();
            OnCollisionCallback?.Invoke(otherTransform);
        }

        /// <summary>
        /// Trying to send damage to transform.
        /// </summary>
        protected virtual void SendDamage(Transform other, float damage)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);

                IHealth health = other.GetComponent<IHealth>();
                if(health != null)
                {
                    if (health.IsAlive())
                    {
                        killedInstanceIDs.Remove(other.root.GetInstanceID());
                        OnHealthCollisionCallback?.Invoke(other);
                    }
                    else if (killedInstanceIDs.Add(other.root.GetInstanceID()))
                    {
                        OnHealthCollisionKillCallback?.Invoke(other);
                    }
                }
            }
        }

        /// <summary>
        /// Trying to send physics impulse force to transform.
        /// </summary>
        protected virtual void SendImpulse(Transform other, float impulse)
        {
            Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.AddForce(transform.forward * impulse, ForceMode.Impulse);
            }
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called when shell become collide any of other collider.
        /// </summary>
        /// <param name="Transform">The Transform data associated with this collision.</param>
        public event Action<Transform> OnCollisionCallback;

        /// <summary>
        /// Called when shell has become collide any of component which implemented of IDamageable interface.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnHealthCollisionCallback;

        /// <summary>
        /// Called when shell has become kill any of component which implemented of IHealth interface.
        /// </summary>
        /// <param name="Transform">The Transform data associated with health.</param>
        public event Action<Transform> OnHealthCollisionKillCallback;
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

        public Rigidbody GetRigidbody()
        {
            return rigidbody;
        }

        public Collider GetCollider()
        {
            return collider;
        }
        #endregion
    }
}