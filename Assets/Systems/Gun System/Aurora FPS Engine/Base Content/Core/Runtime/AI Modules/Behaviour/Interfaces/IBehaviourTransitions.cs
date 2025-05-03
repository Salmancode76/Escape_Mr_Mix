/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.Transitions;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    public interface IBehaviourTransitions
    {
        void AddTransition(Transition transition);

        bool RemoveTransition(Transition transition);
    }
}