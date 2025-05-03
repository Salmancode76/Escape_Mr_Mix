using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        float currentFov;
        float lastFireTime;
        float fireInterval
        {
            get
            {
                return 1f / AutomaticFireRate;
            }
        }

        public float CurrentFov
        {
            get
            {
                return currentFov;
            }
        }

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
                // Check which weapon is active
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
                                Shot();
                                break;
                            case WeaponType.UMP5:
                            case WeaponType.M4A1:
                                Shot();
                                break;
                            case WeaponType.SPAS12:
                                ShotgunShot();
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
            // No special end logic
        }

        protected virtual void Shot()
        {
            Ray r = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(r, out hitInfo, 1000, ShotMask, QueryTriggerInteraction.Ignore))
            {
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

        protected virtual void ShotgunShot()
        {
            int pellets = 8; // Number of shotgun pellets
            float spreadAngle = 5f; // Spread angle

            for (int i = 0; i < pellets; i++)
            {
                Vector3 spread = playerCamera.transform.forward;
                spread += playerCamera.transform.right * UnityEngine.Random.Range(-spreadAngle, spreadAngle) * 0.01f;
                spread += playerCamera.transform.up * UnityEngine.Random.Range(-spreadAngle, spreadAngle) * 0.01f;
                spread.Normalize();

                Ray r = new Ray(playerCamera.transform.position, spread);
                RaycastHit hitInfo;

                if (Physics.Raycast(r, out hitInfo, 1000, ShotMask, QueryTriggerInteraction.Ignore))
                {
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
            if (FindChildByName(objectToSearch.transform, "Glock 17 [FP Weapon](Clone)") != null)
                return WeaponType.Glock17;

            if (FindChildByName(objectToSearch.transform, "UMP 5 [FP Weapon](Clone)") != null)
                return WeaponType.UMP5;

            if (FindChildByName(objectToSearch.transform, "M4A1 [FP Weapon](Clone)") != null)
                return WeaponType.M4A1;

            if (FindChildByName(objectToSearch.transform, "SPAS 12 [FP Weapon](Clone)") != null){
                Debug.Log("shutgun");
                return WeaponType.SPAS12;
            }

            return WeaponType.None;
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
