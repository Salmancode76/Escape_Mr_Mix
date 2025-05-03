/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace AuroraFPSRuntime.UIModules.UIElements
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Scene Loading/Loading Tips")]
    [DisallowMultipleComponent]
    public sealed class LoadingTips : MonoBehaviour
    {
        [System.Serializable]
        private struct Tip
        {
            [SerializeField]
            private string text;

            [SerializeField]
            private float time;

            public bool Equals(Tip tip)
            {
                return text == tip.GetText();
            }

            #region [Getter / Setter]
            public string GetText()
            {
                return text;
            }

            public void SetText(string value)
            {
                text = value;
            }

            public float GetTime()
            {
                return time;
            }

            public void SetTime(float value)
            {
                time = value;
            }
            #endregion
        }

        public enum FetchType
        {
            Sequental,
            Random
        }

        public enum ShowMethod
        {
            OnEnable,
            Manually
        }

        [SerializeField]
        private ShowMethod showMethod = ShowMethod.OnEnable;

        [SerializeField]
        private FetchType fetchType = FetchType.Random;

        [SerializeField]
        [NotNull]
        private Text text;

        [SerializeField]
        private Transition transition;

        [SerializeField]
        [VisibleIf("fetchType", "Random")]
        [MinValue(0)]
        private int bufferSize = 0;

        [SerializeField]
        [ReorderableList(OnElementGUICallback = "OnTipGUI")]
        private Tip[] tips;

        // Stored required properties.
        private Queue<string> buffer;
        private HashSet<string> bufferHash;
        private CoroutineObject coroutineObject;

        
        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            buffer = new Queue<string>(bufferSize);
            bufferHash = new HashSet<string>();
            coroutineObject = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            if (showMethod == ShowMethod.OnEnable)
            {
                ShowTips();
            }
        }

        /// <summary>
        /// Start showing tips.
        /// </summary>
        public void ShowTips()
        {
            coroutineObject.Start(ShowingTipsProcessing, true);
        }

        /// <summary>
        /// Pause showing tips.
        /// </summary>
        public void Pause()
        {
            coroutineObject.Stop();
        }

        /// <summary>
        /// Showing tips processing coroutine.
        /// </summary>
        private IEnumerator ShowingTipsProcessing()
        {
            int lastIndex = 0;
            while (true)
            {
                Tip tip = default;
                switch (fetchType)
                {
                    case FetchType.Sequental:
                        tip = tips[lastIndex++];
                        if(lastIndex >= tips.Length)
                        {
                            lastIndex = 0;
                        }
                        break;

                    case FetchType.Random:
                        if(bufferSize > 0)
                        {
                            do
                            {
                                tip = tips[Random.Range(0, tips.Length)];
                                yield return null;
                            }
                            while (!bufferHash.Add(tip.GetText()));

                            if (buffer.Count >= bufferSize)
                            {
                                bufferHash.Remove(buffer.Dequeue());
                            }
                            buffer.Enqueue(tip.GetText());
                        }
                        else
                        {
                            tip = tips[Random.Range(0, tips.Length)];
                        }
                        break;
                }
                text.text = tip.GetText();
                yield return transition.WaitForFadeIn();
                yield return new WaitForSeconds(tip.GetTime());
                yield return transition.WaitForFadeOut();
            }
        }

        #region [Editor Section]
#if UNITY_EDITOR
        private void OnTipGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.SerializedProperty textProperty = property.FindPropertyRelative("text");
            Rect textPosition = new Rect(position.x, position.y, position.width - 33, UnityEditor.EditorGUIUtility.singleLineHeight);
            textProperty.stringValue = UnityEditor.EditorGUI.TextField(textPosition, textProperty.stringValue);

            UnityEditor.SerializedProperty timeProperty = property.FindPropertyRelative("time");
            Rect timePosition = new Rect(textPosition.xMax + 2, position.y, position.width - textPosition.width, UnityEditor.EditorGUIUtility.singleLineHeight);
            timeProperty.floatValue = UnityEditor.EditorGUI.FloatField(timePosition, timeProperty.floatValue);
        }

#endif
        #endregion

        #region [Getter / Setter]
        public ShowMethod GetShowMethod()
        {
            return showMethod;
        }

        public void SetShowMethod(ShowMethod value)
        {
            showMethod = value;
        }

        public FetchType GetFetchType()
        {
            return fetchType;
        }

        public void SetFetchType(FetchType value)
        {
            fetchType = value;
        }

        public Text GetTextComponent()
        {
            return text;
        }

        public void SetTextComponent(Text value)
        {
            text = value;
        }

        public int GetBufferSize()
        {
            return bufferSize;
        }

        public void SetBufferSize(int value)
        {
            bufferSize = value;
        }

        private Tip[] GetTips()
        {
            return tips;
        }

        private void SetTips(Tip[] value)
        {
            tips = value;
        }
        #endregion
    }
}
