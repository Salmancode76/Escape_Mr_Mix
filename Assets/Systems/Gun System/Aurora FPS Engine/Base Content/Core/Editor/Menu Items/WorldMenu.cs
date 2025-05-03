/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================== */

using UnityEditor;
using AuroraFPSRuntime.SystemModules;

namespace AuroraFPSEditor
{
    internal static class WorldMenu
    {
        [MenuItem("Aurora FPS Engine/Create/World/Pool Manager", false, 102)]
        [MenuItem("GameObject/Aurora FPS Engine/World/Pool Manager", false, 5)]
        private static void CreatePoolManager()
        {
            PoolManager poolManager = PoolManager.GetRuntimeInstance();
            EditorGUIUtility.PingObject(poolManager);
        }

        [MenuItem("Aurora FPS Engine/Create/World/Terrain Manager", false, 103)]
        [MenuItem("GameObject/Aurora FPS Engine/World/Terrain Manager", false, 6)]
        private static void CreateTerrainManager()
        {
            TerrainManager terrainManager = TerrainManager.GetRuntimeInstance();
            EditorGUIUtility.PingObject(terrainManager);
        }
    }
}