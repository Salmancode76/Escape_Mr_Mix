/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================== */

using AuroraFPSEditor.Attributes;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AuroraFPSEditor
{
    internal static class ShowcaseLoaderMenu
    {
        private const string ShowcaseLocation = "Base Content/Scenes/Menu Demo/Menu Sample.unity";
        private const string PlayerShowcaseLocation = "Base Content/Scenes/Player Demo/Player Showcase.unity";
        private const string AIShowcaseLocation = "Base Content/Scenes/AI Demo/AI Showcase.unity";

        [MenuItem("Aurora FPS Engine/Showcase", false, 301)]
        private static void Showcase()
        {
            string path = Path.Combine(ApexSettings.RootPath, ShowcaseLocation);
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }
        }

        //[MenuItem("Aurora FPS Engine/Showcase/Player Showcase", false, 301)]
        //private static void PlayerShowcase()
        //{
        //    string path = Path.Combine(ApexSettings.RootPath, PlayerShowcaseLocation);
        //    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        //    {
        //        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        //    }
        //}

        //[MenuItem("Aurora FPS Engine/Showcase/AI Showcase", false, 302)]
        //private static void AIShowcase()
        //{
        //    string path = Path.Combine(ApexSettings.RootPath, AIShowcaseLocation);
        //    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        //    {
        //        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        //    }
        //}
    }
}