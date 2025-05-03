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
    [AddComponentMenu("Aurora FPS Engine/Core Modules/Scene Management/Scene Manager")]
    [DisallowMultipleComponent]
    public sealed class SceneManager : MonoBehaviour, ISceneManager
    {
        private const string TARGET_SCENE_GUID = "Target Scene ID";

        [SerializeField]
        [SceneSelecter]
        private int loadingScene;

        [SerializeField]
        [Foldout("Transition Settings", Style = "Header")]
        [Order(501)]
        private Transition transition = null;

        // Stored required properties.
        private CoroutineObject<int> coroutineObject;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            coroutineObject = new CoroutineObject<int>(this);
        }

        /// <summary>
        /// Load target scene.
        /// </summary>
        public void LoadScene()
        {
            coroutineObject.Start(LoadProcessing, loadingScene);
        }

        /// <summary>
        /// Loading processing with transition.
        /// </summary>
        private IEnumerator LoadProcessing(int index)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = false;

            while (asyncOperation.progress < 0.9f)
            {
                yield return null;
            }

            if (transition)
            {
                yield return transition.WaitForFadeIn();
            }

            asyncOperation.allowSceneActivation = true;
        }

        #region [UI Callback Wrapper]
        public void SetTargetSceneUICallback(int index)
        {
            SetTargetScene(index);
        }
        #endregion

        #region [Static Methods]
        public static int GetTargetScene()
        {
            if (!PlayerPrefs.HasKey(TARGET_SCENE_GUID))
            {
                return -1;
            }
            return PlayerPrefs.GetInt(TARGET_SCENE_GUID);
        }

        public static void SetTargetScene(int value)
        {
            PlayerPrefs.SetInt(TARGET_SCENE_GUID, value);
        }

        internal static void DeleteSceneGUID()
        {
            PlayerPrefs.DeleteKey(TARGET_SCENE_GUID);
        }
        #endregion

        #region [Getter / Setter]
        public Transition GetTransition()
        {
            return transition;
        }

        public void SetTransition(Transition value)
        {
            transition = value;
        }
        #endregion
    }
}
