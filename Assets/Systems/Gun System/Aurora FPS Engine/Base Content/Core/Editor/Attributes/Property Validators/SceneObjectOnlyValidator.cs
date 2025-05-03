/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraFPSEditor.Attributes
{
    [ValidatorTarget(typeof(SceneObjectOnlyAttribute))]
    public sealed class SceneObjectOnlyValidator : PropertyValidator
    {
        private bool isActive;

        public override void Validate(SerializedProperty property)
        {
            isActive = true;

            Object[] objects = DragAndDrop.objectReferences;
            if (objects != null && objects.Length > 0)
            {
                GameObject go = objects[0] as GameObject;
                if (go != null)
                {
                    isActive = go.scene != null && go.scene == EditorSceneManager.GetActiveScene();
                }
            }

            if (Event.current.type == EventType.DragExited)
            {
                isActive = true;
            }
        }

        public override void BeforePropertyGUI()
        {
            EditorGUI.BeginDisabledGroup(!isActive);
        }

        public override void AfterPropertyGUI()
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}