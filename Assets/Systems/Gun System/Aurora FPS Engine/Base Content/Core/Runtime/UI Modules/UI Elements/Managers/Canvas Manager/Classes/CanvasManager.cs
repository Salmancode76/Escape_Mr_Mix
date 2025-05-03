/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.UIModules.UIElements.Animation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Managers/Canvas Manager")]
    public sealed class CanvasManager : MonoBehaviour
    {
        [System.Serializable]
        public class CanvasProperty
        {
            [SerializeField]
            [NotNull]
            private Canvas canvas;

            [SerializeField]
            [Suffix("Optional", true, ItalicText = true)]
            private string inputPath = "<map>/<action>";

            [SerializeField]
            private bool crossfade = false;

            [SerializeField]
            [VisibleIf("crossfade")]
            [Indent(1)]
            private bool disable = true;

            [SerializeField]
            private bool holdInteraction = false;

            [SerializeField]
            private bool hardwareCursor = true;

            [SerializeField]
            private bool freezeTime = true;

            [SerializeField]
            [Label("Scale")]
            [Slider(0.0f, 0.99f)]
            [VisibleIf("freezeTime")]
            [Indent(1)]
            private float freezeScale = 0.0f;

            [SerializeField]
            [ReorderableList(ElementLabel = null)]
            private string[] lockMapNames = new string[1] { "Player" };

            // Stored required properties.
            private bool isActive;
            private float previousTimeScale;
            private InputAction inputAction;

            // Stored required components.
            private GroupTransition groupTransition;

            public CanvasProperty(Canvas canvas)
            {
                this.canvas = canvas;
            }

            public CanvasProperty(Canvas canvas, string inputPath) : this(canvas)
            {
                this.inputPath = inputPath;
            }

            public CanvasProperty(Canvas canvas, string inputPath, bool crossfade, bool disable) : this(canvas, inputPath)
            {
                this.crossfade = crossfade;
                this.disable = disable;
            }

            public CanvasProperty(Canvas canvas, string inputPath, bool crossfade, bool disable, bool holdInteraction) : this(canvas, inputPath, crossfade, disable)
            {
                this.holdInteraction = holdInteraction;
            }

            public CanvasProperty(Canvas canvas, string inputPath, bool crossfade, bool disable, bool holdInteraction, bool hardwareCursor) : this(canvas, inputPath, crossfade, disable, holdInteraction)
            {
                this.hardwareCursor = hardwareCursor;
            }

            public CanvasProperty(Canvas canvas, string inputPath, bool crossfade, bool disable, bool holdInteraction, bool hardwareCursor, bool freezeTime, float freezeScale) : this(canvas, inputPath, crossfade, disable, holdInteraction, hardwareCursor)
            {
                this.freezeTime = freezeTime;
                this.freezeScale = freezeScale;
            }

            public CanvasProperty(Canvas canvas, string inputPath, bool crossfade, bool disable, bool holdInteraction, bool hardwareCursor, bool freezeTime, float freezeScale, params string[] lockMapNames) : this(canvas, inputPath, crossfade, disable, holdInteraction, hardwareCursor, freezeTime, freezeScale)
            {
                this.lockMapNames = lockMapNames;
            }


            /// <summary>
            /// Initialize canvas property.
            /// </summary>
            public void Inititalize()
            {
                if (crossfade)
                {
                    groupTransition = canvas.GetComponent<GroupTransition>();
                    if (groupTransition == null)
                    {
                        CanvasGroup canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
                        canvasGroup.alpha = 0;

                        groupTransition = canvas.gameObject.AddComponent<GroupTransition>();
                        groupTransition.SetGroup(canvasGroup);

                        if (disable)
                        {
                            canvas.gameObject.SetActive(false);
                        }
                    }

                    if (disable)
                    {
                        groupTransition.OnFadeOutCompleteCallback += () =>
                        {
                            canvas.gameObject.SetActive(false);
                        };
                    }
                }
                

                if (!string.IsNullOrEmpty(inputPath))
                {
                    inputAction = InputReceiver.Asset.FindAction(inputPath);
                }

                isActive = crossfade ? groupTransition.GetGroup().alpha > 0 : canvas.gameObject.activeSelf;
            }

            /// <summary>
            /// Enable input action.
            /// </summary>
            public void EnableInputAction()
            {
                inputAction.Enable();
            }

            /// <summary>
            /// Disable input action.
            /// </summary>
            public void DisableInputAction()
            {
                inputAction.Disable();
            }

            /// <summary>
            /// Toggle group.
            /// </summary>
            public void Toggle()
            {
                Toggle(!isActive);
            }

            /// <summary>
            /// Toggle group.
            /// </summary>
            /// <param name="value">Specified toggle state.</param>
            public void Toggle(bool value)
            {
                if (isActive != value)
                {
                    if (value)
                    {
                        // Enabling canvas.
                        if (crossfade)
                        {
                            if (disable)
                            {
                                canvas.gameObject.SetActive(true);
                            }

                            groupTransition.FadeIn();
                        }
                        else
                        {
                            canvas.gameObject.SetActive(true);
                        }

                        // Apply hold interaction.
                        if (holdInteraction && inputAction != null)
                        {
                            inputAction.canceled += BreckActiveSelf;
                        }

                        // Locking specified input maps
                        InputReceiver.EnableMapPredicate += InputPredicate;
                        for (int i = 0; i < lockMapNames.Length; i++)
                        {
                            InputReceiver.DisableMap(lockMapNames[i]);
                        }

                        // Freezing time.
                        if (freezeTime)
                        {
                            previousTimeScale = Time.timeScale;
                            Time.timeScale = freezeScale;
                        }
                    }
                    else
                    {
                        // Disabling canvas.
                        if (crossfade)
                        {
                            groupTransition.FadeOut();
                        }
                        else
                        {
                            canvas.gameObject.SetActive(false);
                        }

                        // Apply hold interaction.
                        if (holdInteraction && inputAction != null)
                        {
                            inputAction.canceled -= BreckActiveSelf;
                        }

                        // Unlocking specified input maps
                        InputReceiver.EnableMapPredicate -= InputPredicate;
                        for (int i = 0; i < lockMapNames.Length; i++)
                        {
                            InputReceiver.EnableMap(lockMapNames[i]);
                        }

                        // Unfreezing time.
                        if (freezeTime)
                        {
                            Time.timeScale = previousTimeScale;
                        }
                    }

                    // Processing hardware cursor.
                    if (hardwareCursor)
                    {
                        InputReceiver.HardwareCursor(value);
                    }

                    // Update current state.
                    isActive = value;
                }
            }

            public void UnlockInputMaps()
            {
                InputReceiver.EnableMapPredicate -= InputPredicate;
                for (int i = 0; i < lockMapNames.Length; i++)
                {
                    InputReceiver.EnableMap(lockMapNames[i]);
                }
            }

            /// <summary>
            /// Group is active.
            /// </summary>
            public bool IsActive()
            {
                return isActive;
            }

            /// <summary>
            /// The render order in which the canvas is being emitted to the Scene. (Read Only)
            /// <br><i>Switching a canvas with a lower order will be ignored while the canvas with a higher order is open.</i></br>
            /// </summary>
            public int GetOrder()
            {
                return canvas.sortingOrder;
            }

            #region [Event Predicate Callback]
            private bool InputPredicate(string name)
            {
                if (lockMapNames != null)
                {
                    for (int i = 0; i < lockMapNames.Length; i++)
                    {
                        if (name == lockMapNames[i])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            #endregion

            #region [Input Action Wrapper]
            private void OnDisableAction()
            {
                canvas.gameObject.SetActive(false);
            }

            private void BreckActiveSelf(InputAction.CallbackContext context)
            {
                if (isActive && context.canceled)
                {
                    Toggle(false);
                }
            }
            #endregion

            #region [Getter / Setter]
            public Canvas GetCanvas()
            {
                return canvas;
            }

            public void SetCanvas(Canvas value)
            {
                canvas = value;
            }

            public bool Crossfade()
            {
                return crossfade;
            }

            public void Crossfade(bool value)
            {
                crossfade = value;
            }

            public bool HoldInteraction()
            {
                return holdInteraction;
            }

            public void HoldInteraction(bool value)
            {
                holdInteraction = value;
            }

            public string GetInputPath()
            {
                return inputPath;
            }

            public void SetInputPath(string value)
            {
                inputPath = value;
            }

            public InputAction GetInputAction()
            {
                return inputAction;
            }

            public void SetInputAction(InputAction value)
            {
                inputAction = value;
            }

            public bool HardwareCursor()
            {
                return hardwareCursor;
            }

            public void HardwareCursor(bool value)
            {
                hardwareCursor = value;
            }

            public bool FreezeTime()
            {
                return freezeTime;
            }

            public void FreezeTime(bool value)
            {
                freezeTime = value;
            }

            public float GetFreezeScale()
            {
                return freezeScale;
            }

            public void SetFreezeScale(float value)
            {
                freezeScale = value;
            }
            #endregion
        }

        [SerializeField]
        [Array(GetElementLabelCallback = "GetCanvasLabel")]
        private CanvasProperty[] canvasProperties;

        // Stored required properties.
        private CanvasProperty activeCanvasProperty;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            for (int i = 0; i < canvasProperties.Length; i++)
            {
                CanvasProperty canvasProperty = canvasProperties[i];
                canvasProperty.Inititalize();
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            for (int i = 0; i < canvasProperties.Length; i++)
            {
                CanvasProperty canvasProperty = canvasProperties[i];
                if(canvasProperty.GetInputAction() != null)
                {
                    canvasProperty.GetInputAction().performed += OnPerformedAction;
                    canvasProperty.EnableInputAction();
                }
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            for (int i = 0; i < canvasProperties.Length; i++)
            {
                canvasProperties[i].UnlockInputMaps();
            }
        }

        /// <summary>
        /// Called once before the behaviour will destroyed.
        /// </summary>
        private void OnDestroy()
        {
            for (int i = 0; i < canvasProperties.Length; i++)
            {
                CanvasProperty canvasProperty = canvasProperties[i];
                if (canvasProperty.GetInputAction() != null)
                {
                    canvasProperty.GetInputAction().performed -= OnPerformedAction;
                    canvasProperty.DisableInputAction();
                }
            }
        }

        /// <summary>
        /// Search canvas switcher property by specified canvas object.
        /// </summary>
        /// <param name="canvas">Canvas object key.</param>
        /// <param name="canvasProperty">Reference of canvas property.</param>
        public bool TryGetCanvasProperty(Canvas canvas, out CanvasProperty canvasProperty)
        {
            for (int i = 0; i < canvasProperties.Length; i++)
            {
                canvasProperty = canvasProperties[i];
                if(canvas == canvasProperty.GetCanvas())
                {
                    return true;
                }
            }
            canvasProperty = null;
            return false;
        }

        #region [Input Action Wrapper]
        private void OnPerformedAction(InputAction.CallbackContext context)
        {
            if (!activeCanvasProperty?.IsActive() ?? false)
            {
                activeCanvasProperty = null;
            }

            for (int i = 0; i < canvasProperties.Length; i++)
            {
                CanvasProperty property = canvasProperties[i];
                if (property != null && property.GetInputAction().id == context.action.id)
                {
                    if (activeCanvasProperty != null)
                    {
                        if (activeCanvasProperty == property)
                        {
                            property.Toggle();
                        }
                        else if (property.GetOrder() > activeCanvasProperty.GetOrder() || property.GetOrder() == activeCanvasProperty.GetOrder())
                        {
                            activeCanvasProperty.Toggle(false);
                            property.Toggle(true);
                            activeCanvasProperty = property;
                        }
                    }
                    else
                    {
                        property.Toggle(true);
                        activeCanvasProperty = property;
                    }
                }
            }
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetCanvasLabel(UnityEditor.SerializedProperty property, int index)
        {
            return property.GetArrayElementAtIndex(index).FindPropertyRelative("canvas").objectReferenceValue?.name ?? string.Format("Canvas {0}", index + 1);
        }
#endif
        #endregion

        #region [Getter / Setter]
        public CanvasProperty[] GetProperties()
        {
            return canvasProperties;
        }

        public CanvasProperty GetProperty(int index)
        {
            return canvasProperties[index];
        }

        public void SetProperties(CanvasProperty[] value)
        {
            canvasProperties = value;
        }

        public void SetProperty(int index, CanvasProperty value)
        {
            canvasProperties[index] = value;
        }

        public int GetPropertyCount()
        {
            return canvasProperties.Length;
        }

        public CanvasProperty GetActiveProperty()
        {
            return activeCanvasProperty;
        }
        #endregion
    }
}
