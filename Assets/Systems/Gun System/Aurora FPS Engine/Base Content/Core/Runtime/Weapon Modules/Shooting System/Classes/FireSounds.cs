/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [System.Serializable]
    public sealed class FireSounds
    {
        public enum FetchType
        {
            Sequential,
            Random
        }

        [SerializeField]
        private FetchType fetchType = FetchType.Sequential;

        [SerializeField]
        [ReorderableList(DisplayHeader = false, ElementLabel = null)]
        private AudioClip[] clips;

        // Stored required properties.
        private int clipIndex = -1;

        public AudioClip FetchClip()
        {
            switch (fetchType)
            {
                case FetchType.Sequential:
                    return GetNextClip();
                case FetchType.Random:
                    return GetRandomClip();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get next clip from step clips array.
        /// </summary>
        public AudioClip GetNextClip()
        {
            if (clips != null && clips.Length > 0)
            {
                int nextIndex = ++clipIndex;
                clipIndex = nextIndex >= clips.Length ? 0 : nextIndex;
                return clips[clipIndex];
            }
            return null;
        }

        /// <summary>
        /// Get new random clip form steps clips array.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (clips != null && clips.Length > 0)
            {
                if (clips.Length > 1)
                {
                    int randomIndex = clipIndex;
                    while (randomIndex == clipIndex)
                    {
                        randomIndex = Random.Range(0, clips.Length);
                    }

                    clipIndex = randomIndex;
                }
                return clips[clipIndex];
            }
            return null;
        }

        #region [Getter / Setter]
        public FetchType GetFetchType()
        {
            return fetchType;
        }

        public void SetFetchType(FetchType value)
        {
            fetchType = value;
        }

        public AudioClip[] GetClips()
        {
            return clips;
        }

        public void SetClips(AudioClip[] value)
        {
            clips = value;
        }
        #endregion
    }
}