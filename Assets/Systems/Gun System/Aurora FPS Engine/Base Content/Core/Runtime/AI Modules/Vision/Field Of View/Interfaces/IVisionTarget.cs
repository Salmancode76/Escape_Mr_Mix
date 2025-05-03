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

namespace AuroraFPSRuntime.AIModules.Vision
{
    public interface IVisionTarget
    {
        IReadOnlyList<Transform> GetVisibleTargets();

        Transform GetFirstTarget();

        Transform GetNearestTarget();

        Transform GetDistantTarget();
    }
}