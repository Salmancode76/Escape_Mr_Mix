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
using UnityEngine;

#region [Unity Editor Section]
#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
#endif
#endregion

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class CharacterHealth : ObjectHealth
    {
        public enum DeathAnimation
        {
            AnimationOnly,
            RagdollOnly,
            Blend
        }

        [SerializeReference]
        [Foldout("Custom Effects", Style = "Header")]
        [ReorderableList(DisplayHeader = false, 
            GetElementLabelCallback = "GetHealthEffectsLabelCallback", 
            OnDropdownButtonCallback = "OnAddHealthEffectsCallback",
            NoneElementLabel = "Add new effects...")]
        [Order(100)]
        private HealthEffect[] healthEffect;

        [SerializeReference]
        [Label("Type")]
        [DropdownReference(FoldoutToggle = false)]
        [Foldout("Respawn Settings", Style = "Header")]
        [Order(200)]
        private CharacterRespawnEvent characterRespawnEvent;

        [SerializeReference]
        [Label("Type")]
        [DropdownReference(FoldoutToggle = false)]
        [Foldout("Death Settings", Style = "Header")]
        [Order(310)]
        private CharacterDeathEvent characterDeathEvent;

        // Stored required properties.
        private CoroutineObject respawnDelayCoroutine;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            respawnDelayCoroutine = new CoroutineObject(this);
            characterRespawnEvent?.Initialize(this);
            characterDeathEvent?.Initialize(this);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            InitializeHealthEffects();
            InitializeHealthHitAreas();
        }

        protected virtual void InitializeHealthEffects()
        {
            for (int i = 0; i < healthEffect.Length; i++)
            {
                healthEffect[i].Initialization(this);
            }
        }

        /// <summary>
        /// Initialize all health hit areas, which found in child objects.
        /// </summary>
        protected virtual void InitializeHealthHitAreas()
        {
            Hitbox[] healthHitAreas = GetComponentsInChildren<Hitbox>();
            for (int i = 0; i < healthHitAreas.Length; i++)
            {
                healthHitAreas[i].Initialize(this);
            }
        }

        /// <summary>
        /// Called once when object health become zero.
        /// Implement this method to make custom death logic.
        /// </summary>
        protected override void OnDead()
        {
            characterDeathEvent?.OnDead();
            if(characterRespawnEvent != null)
            {
                respawnDelayCoroutine.Start(characterRespawnEvent.DelayCoroutine, true);
            }
        }

        protected override void OnRevive()
        {
            if(characterRespawnEvent != null)
            {
                respawnDelayCoroutine.Stop();
                characterRespawnEvent.OnRevive();
            }
        }

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private string GetHealthEffectsLabelCallback(SerializedProperty property, int index)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFullTypename.Split(' ');
            Type type = Type.GetType(baseTypeAndAssemblyName[1]);
            HealthFunctionMenu menuAttribute = type.GetCustomAttribute<HealthFunctionMenu>();
            if (menuAttribute != null)
            {
                return menuAttribute.name;
            }
            else
            {
                return "Function " + index;
            }
        }

        private void OnAddHealthEffectsCallback(Rect position, SerializedProperty property)
        {
            Assembly assembly = typeof(HealthEffect).Assembly;
            Type[] assemblyTypes = assembly.GetTypes();
            Type[] functionTypes = assemblyTypes.Where(t => t.IsSubclassOf(typeof(HealthEffect))).ToArray();

            GenericMenu genericMenu = new GenericMenu();
            for (int i = 0; i < functionTypes.Length; i++)
            {
                Type functionType = functionTypes[i];
                HealthFunctionMenu menuAttribute = functionType.GetCustomAttribute<HealthFunctionMenu>();
                if (menuAttribute != null)
                {
                    genericMenu.AddItem(new GUIContent(menuAttribute.path), false, () =>
                    {
                        int index = property.arraySize;
                        property.arraySize++;
                        property.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(functionType);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            genericMenu.DropDown(position);
        }

#endif
        #endregion

        #region [Getter / Setter]
        public HealthEffect[] GetHealthEffect()
        {
            return healthEffect;
        }

        public void SetHealthEffect(HealthEffect[] value)
        {
            healthEffect = value;
        }
        #endregion
    }
}