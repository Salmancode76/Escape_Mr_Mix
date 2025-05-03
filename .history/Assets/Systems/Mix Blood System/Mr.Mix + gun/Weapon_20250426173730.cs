using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AuroraFPSRuntime.WeaponModules;

namespace Knife.RealBlood.SimpleController
{
    public class Weapon : MonoBehaviour
    {
        public Camera playerCamera;
        public LayerMask ShotMask;
        public float Damage = 10f;
        public float DefaultFov = 60f;
        public float AimFov = 35f;
        public bool AutomaticFire;
        public float AutomaticFireRate = 10;
        public GameObject objectToSearch;

        protected Animator handsAnimator;

        bool isAiming = false;

        enum WeaponType { None, Glock17, UMP5, M4A1, SPAS12 }
        WeaponType currentWeapon = WeaponType.None;

        WeaponStandardReloadSystem weaponReloadSystem;

        float currentFov;
        float lastFireTime;
        float fireInterval => 1f / AutomaticFireRate;

        public float CurrentFov => currentFov;

        void Start()
        {
            handsAnimator = GetComponent<Animator>();
            lastFireTime = -fireInterval;
        }

        private void OnDisable()
        {
            currentFov = DefaultFov;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (objectToSearch != null)
                {
                    currentWeapon = DetectWeapon();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (Time.time > lastFireTime + fireInterval)
                {
                    if (currentWeapon != WeaponType.None)
                    {
                        switch (currentWeapon)
                        {
                            case WeaponType.Glock17:
                            case WeaponType.UMP5:
                            case WeaponType.M4A1:
                            case WeaponType.SPAS12:
                                Shot();
                                break;
                        }
                    }
                    lastFireTime = Time.time;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndFire();
            }

            if (Input.GetMouseButtonDown(1))
            {
                isAiming = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                isAiming = false;
            }

            currentFov = Mathf.Lerp(currentFov, isAiming ? AimFov : DefaultFov, Time.deltaTime * 12f);
        }

        protected virtual void EndFire()
        {
        }

        protected virtual void Shot()
        {
            Ray r = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(r, out hitInfo, 1000, ShotMask, QueryTriggerInteraction.Ignore))
            {
                if (weaponReloadSystem != null && weaponReloadSystem.GetAmmoCount() <= 0)
                {
                    return;
                }

                var hittable = hitInfo.collider.GetComponent<IHittable>();
                if (hittable != null)
                {
                    DamageData[] damage = new DamageData[1]
                    {
                        new DamageData()
                        {
                            amount = Damage,
                            direction = r.direction,
                            normal = hitInfo.normal,
                            point = hitInfo.point
                        }
                    };

                    hittable.TakeDamage(damage);
                }
            }

            DebugShot(r, hitInfo);
        }

        protected void DebugShot(Ray r, RaycastHit hitInfo)
        {
            if (hitInfo.collider != null)
            {
                Debug.DrawLine(r.origin, hitInfo.point, Color.green, 3f);
            }
            else
            {
                Debug.DrawLine(r.origin, r.GetPoint(1000), Color.red, 3f);
            }
        }

        public Vector3 GetLookDirection()
        {
            return playerCamera.transform.forward;
        }

        private WeaponType DetectWeapon()
        {
            Transform weaponTransform = null;

            if ((weaponTransform = FindChildByName(objectToSearch.transform, "Glock 17 [FP Weapon](Clone)")) != null)
                currentWeapon = WeaponType.Glock17;
            else if ((weaponTransform = FindChildByName(objectToSearch.transform, "UMP 5 [FP Weapon](Clone)")) != null)
                currentWeapon = WeaponType.UMP5;
            else if ((weaponTransform = FindChildByName(objectToSearch.transform, "M4A1 [FP Weapon](Clone)")) != null)
                currentWeapon = WeaponType.M4A1;
            else if ((weaponTransform = FindChildByName(objectToSearch.transform, "SPAS 12 [FP Weapon](Clone)")) != null)
                currentWeapon = WeaponType.SPAS12;
            else
                currentWeapon = WeaponType.None;

            if (weaponTransform != null)
            {
                weaponReloadSystem = weaponTransform.GetComponent<WeaponStandardReloadSystem>();
            }
            else
            {
                weaponReloadSystem = null;
            }

            return currentWeapon;
        }

        private Transform FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform found = FindChildByName(child, name);
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
