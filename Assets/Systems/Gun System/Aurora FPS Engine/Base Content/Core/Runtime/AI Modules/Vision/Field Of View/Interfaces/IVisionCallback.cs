/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine;
using System;

namespace AuroraFPSRuntime.AIModules.Vision
{
    public interface IVisionCallback
    {
        event Action<Transform> OnTargetBecomeVisible;

        event Action OnTargetsBecomeInvisible;
    }
}