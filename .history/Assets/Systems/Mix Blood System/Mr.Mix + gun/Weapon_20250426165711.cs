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

        public GameObject objectToSearch; // <- New object reference

        protected Animator handsAnimator;

        bool isAiming = false;
        bool alreadyFound = false; // <- To print only once

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
            if (objectToSearch != null && !alreadyFound)
            {
                Transform child = FindChildByName(objectToSearch.transform, "mp5");
                if (child != null)
                {
                    Debug.Log("Found");
                    alreadyFound = true;
                }
            }

            if (AutomaticFire)
            {
                if (Input.GetMouseButton(0) && Time.time > lastFireTime + fireInterval)
                {
                    Shot();
                    lastFireTime = Time.time;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    EndFire();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && Time.time > lastFireTime + fireInterval)
                {
                    Shot();
                    lastFireTime = Time.time;
                }
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

        // New helper function to search recursively
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
