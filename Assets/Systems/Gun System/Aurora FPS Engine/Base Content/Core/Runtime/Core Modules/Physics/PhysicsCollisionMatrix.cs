/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.PhysicsEngine
{
    public static class PhysicsCollisionMatrix
    {
        private static Dictionary<int, int> masksByLayer;

        static PhysicsCollisionMatrix()
        {
            masksByLayer = new Dictionary<int, int>();
            for (int i = 0; i < 32; i++)
            {
                int mask = 0;
                for (int j = 0; j < 32; j++)
                {
                    if (!Physics.GetIgnoreLayerCollision(i, j))
                    {
                        mask |= 1 << j;
                    }
                }
                masksByLayer.Add(i, mask);
            }
        }

        /// <summary>
        /// Load culling mask for target layer index.
        /// </summary>
        /// <param name="layer">Index of layer.</param>
        public static int LoadMaskForLayer(int layer)
        {
            return masksByLayer[layer];
        }
    }
}