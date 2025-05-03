/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Pattern;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Terrain Manager/Terrain Manager")]
    [DisallowMultipleComponent]
    public class TerrainManager : Singleton<TerrainManager>
    {
        // Base TerrainManager properties.
        [SerializeField] 
        [ReorderableList(ElementLabel = null)]
        private string[] excludingTerrains;

        // Stored required properties.
        private TerrainTextureDetector[] terrainTextureDetectors;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            CacheAllTerrains(excludingTerrains);
        }

        /// <summary>
        /// Find all terrains at the scene and generate TerrainTextureDetector array by them.
        /// </summary>
        public void CacheAllTerrains()
        {
            Terrain[] terrains = Terrain.activeTerrains;

            int length = terrains.Length;
            terrainTextureDetectors = new TerrainTextureDetector[length];
            for (int i = 0; i < length; i++)
            {
                Terrain terrain = terrains[i];
                if (terrain.terrainData != null)
                {
                    terrainTextureDetectors[i] = new TerrainTextureDetector(terrains[i]);
                }
            }
        }

        /// <summary>
        /// Find all terrains at the scene and generate TerrainTextureDetector array by them.
        /// </summary>
        public void CacheAllTerrains(string[] excluding)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            List<Terrain> cacheTerrains = new List<Terrain>();
            for (int i = 0; i < terrains.Length; i++)
            {
                Terrain terrain = terrains[i];
                bool isExcluding = false;
                if(excluding != null)
                {
                    for (int j = 0; j < excludingTerrains.Length; j++)
                    {
                        if (terrain.name == excludingTerrains[i])
                        {
                            isExcluding = true;
                        }
                    }
                }
                if (!isExcluding)
                {
                    cacheTerrains.Add(terrain);
                }
            }

            int length = cacheTerrains.Count;
            terrainTextureDetectors = new TerrainTextureDetector[length];
            for (int i = 0; i < length; i++)
            {
                Terrain terrain = cacheTerrains[i];
                if (terrain.terrainData != null)
                {
                    terrainTextureDetectors[i] = new TerrainTextureDetector(terrain);
                }
            }
        }

        #region [Getter / Setter]
        public TerrainTextureDetector[] GetTerrainTextureDetectors()
        {
            return terrainTextureDetectors;
        }

        public void SetTerrainTextureDetectors(TerrainTextureDetector[] value)
        {
            terrainTextureDetectors = value;
        }

        public TerrainTextureDetector GetTerrainTextureDetector(int index)
        {
            return terrainTextureDetectors[index];
        }

        public void SetTerrainTextureDetector(int index, TerrainTextureDetector value)
        {
            terrainTextureDetectors[index] = value;
        }

        public string[] GetExcludingTerrains()
        {
            return excludingTerrains;
        }

        public void SetExcludingTerrains(string[] value)
        {
            excludingTerrains = value;
        }

        public string GetExcludingTerrain(int index)
        {
            return excludingTerrains[index];
        }

        public void SetExcludingTerrain(int index, string value)
        {
            excludingTerrains[index] = value;
        }

        public int GetTerrainTextureDetectorsLength()
        {
            return terrainTextureDetectors?.Length ?? 0;
        }
        #endregion
    }
}