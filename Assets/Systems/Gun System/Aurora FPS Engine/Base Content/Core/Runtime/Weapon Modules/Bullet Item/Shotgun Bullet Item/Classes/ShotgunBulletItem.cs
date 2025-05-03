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
    [CreateAssetMenu(fileName = "New Shotgun Bullet Item", menuName = "Aurora FPS Engine/Weapon/Bullet/Shotgun Bullet", order = 101)]
    public class ShotgunBulletItem : BulletItem
    {
        [SerializeField]
        [Foldout("Bullet Settings", Style = "Header")]
        [MinValue(0)]
        [Order(54)]
        private int ballNumber;

        [SerializeField]
        [MinMaxSlider(-1f, 1f)]
        [Foldout("Bullet Settings", Style = "Header")]
        [Order(55)]
        private Vector2 ballVariance;

        private GaussianDistribution gaussianDistribution = new GaussianDistribution();

        /// <summary>
        /// Generate variance for fire point.
        /// </summary>
        /// <param name="target">Target transform</param>
        public Vector3 GenerateVariance(Vector3 target)
        {
            target.x += gaussianDistribution.Next(0, 1, ballVariance.x, ballVariance.y);
            target.y += gaussianDistribution.Next(0, 1, ballVariance.x, ballVariance.y);
            target.z += gaussianDistribution.Next(0, 1, ballVariance.x, ballVariance.y);
            return target;
        }

        #region [Getter / Setter]
        public int GetBallNumber()
        {
            return ballNumber;
        }

        public void SetBallNumber(int value)
        {
            ballNumber = value;
        }

        public Vector2 GetBallVariance()
        {
            return ballVariance;
        }

        public void SetBallVariance(Vector2 value)
        {
            ballVariance = value;
        }
        #endregion
    }
}