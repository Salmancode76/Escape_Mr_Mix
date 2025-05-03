/* ================================================================
  ----------------------------------------------------------------
  Project   :   Aurora FPS Engine
  Publisher :   Infinite Dawn
  Developer :   Tamerlan Shakirov
  ----------------------------------------------------------------
  Copyright © 2017 Tamerlan Shakirov All rights reserved.
  ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraFPSRuntime.CoreModules.SceneManagement
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Core Modules/Scene Management/Scene Loader")]
    [DisallowMultipleComponent]
    public sealed class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        [Order(501)]
        private Transition transition = null;

        [SerializeField]
        [VisualClamp(1.0f, 0.001f)]
        [Order(601)]
        private float timeMultiplier = 0.5f;

        // Stored required properties.
        private float loadingProgress;
        private CoroutineObject coroutineObject;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject(this);
            coroutineObject.Start(LoadProcessing);
        }

        /// <summary>
        /// Loading processing with transition.
        /// </summary>
        private IEnumerator LoadProcessing()
        {
            int index = SceneManager.GetTargetScene();
            if(index < 0)
            {
#if UNITY_EDITOR
                Debug.Log("No scene is selected to load or attempt to upload a scene bypassing the scene manager!");
#endif
                index = 0;
            }

            SceneManager.DeleteSceneGUID();

            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = false;

            float targetValue = 0.0f;
            while (loadingProgress < 1.0f)
            {
                targetValue = asyncOperation.progress / 0.9f;
                loadingProgress = Mathf.MoveTowards(loadingProgress, targetValue, timeMultiplier * Time.deltaTime);
                yield return null;
            }

            if (transition)
            {
                yield return transition.WaitForFadeIn();
            }

            asyncOperation.allowSceneActivation = true;
        }

        /// <summary>
        /// Loading progress in [0...1] representation.
        /// </summary>
        public float GetLoadingProgress()
        {
            return loadingProgress;
        }

        #region [Getter / Setter]
        public Transition GetTransition()
        {
            return transition;
        }

        public void SetTransition(Transition value)
        {
            transition = value;
        }

        public float GetTimeMultiplier()
        {
            return timeMultiplier;
        }

        public void SetTimeMultiplier(float value)
        {
            timeMultiplier = value;
        }
        #endregion
    }
}
