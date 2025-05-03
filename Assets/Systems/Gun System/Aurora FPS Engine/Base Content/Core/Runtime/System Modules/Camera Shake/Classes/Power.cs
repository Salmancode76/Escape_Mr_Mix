/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules
{
    public enum Degree
    {
        Linear,
        Quadratic,
        Cubic,
        Quadric
    }

    public static class Power
    {
        public static float Evaluate(float value, Degree degree)
        {
            switch (degree)
            {
                case Degree.Linear:
                    return value;
                case Degree.Quadratic:
                    return value * value;
                case Degree.Cubic:
                    return value * value * value;
                case Degree.Quadric:
                    return value * value * value * value;
                default:
                    return value;
            }
        }
    }
}
