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
    public interface IAmplitudeController
    {
        /// <summary>
        /// Sets value to which amplitude will move over time.
        /// </summary>
        void SetTargetAmplitude(float value);

        /// <summary>
        /// Sets amplitude to zero and finishes the shake when zero is reached.
        /// </summary>
        void Finish();

        /// <summary>
        /// Immediately finishes the shake.
        /// </summary>
        void FinishImmediately();
    }
}
