/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Shooting/Raycast Shotgun Shooting System")]
    [DisallowMultipleComponent]
    public class WeaponRayShotgunShootingSystem : WeaponRayShootingSystem
    {
        private ShotgunBulletItem shotgunBulletItem;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            shotgunBulletItem = GetBulletItem() as ShotgunBulletItem;
        }

        protected override void MakeShoot()
        {
            if(shotgunBulletItem != null)
            {
                for (int i = 0; i < shotgunBulletItem.GetBallNumber(); i++)
                {
                    base.MakeShoot();
                }
            }
            else
            {
                base.MakeShoot();
            }
        }


        protected override Ray CalculateShootRay()
        {
            Ray ray = base.CalculateShootRay();
            ray.direction = shotgunBulletItem.GenerateVariance(ray.direction);
            return ray;
        }
    }
}