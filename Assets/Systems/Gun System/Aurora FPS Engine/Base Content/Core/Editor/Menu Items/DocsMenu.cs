/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEditor;

namespace AuroraFPSEditor
{
    static class DocsMenu
    {
        const string ManualURL = "https://renowned-games.gitbook.io/aurora-fps-toolkit-manual/";

        [MenuItem("Aurora FPS Engine/Documentation/Manual", false, 10)]
        static void OpenManual()
        {
            Help.BrowseURL(ManualURL);
        }
    }
}
