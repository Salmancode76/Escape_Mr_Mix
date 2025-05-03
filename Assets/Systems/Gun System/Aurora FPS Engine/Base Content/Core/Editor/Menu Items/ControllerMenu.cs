/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================== */

using UnityEditor;
using UnityEngine;
using AuroraFPSEditor.Attributes;

namespace AuroraFPSEditor
{
    internal static class ControllerMenu
    {
        private const string CharacterControllerLocation = "Base Content/Core/Editor/Editor Resources/Templates/Controller/Character Controller.prefab";
        private const string RigidbodyControllerLocation = "Base Content/Core/Editor/Editor Resources/Templates/Controller/Rigidbody Controller.prefab";

        [MenuItem("Aurora FPS Engine/Create/Controller/Based on Character Controller", false, 100)]
        [MenuItem("GameObject/Aurora FPS Engine/Controller/Based on Character Controller", false, 3)]
        private static void CreateBasedOnCharacterController()
        {

            Object controller = AssetDatabase.LoadAssetAtPath<Object>(System.IO.Path.Combine(ApexSettings.RootPath, CharacterControllerLocation));
            if(controller != null)
            {
                Object controllerClone = Object.Instantiate(controller, Vector3.zero, Quaternion.identity);
                controllerClone.name = "New Player Based on Character Controller";
                EditorGUIUtility.PingObject(controllerClone);
            }
            else
            {
                Debug.LogError($"Controller cannot be created, because of location (Path: {CharacterControllerLocation}) with template is empty!");
            }
        }

        [MenuItem("Aurora FPS Engine/Create/Controller/Based on Rigidbody", false, 101)]
        [MenuItem("GameObject/Aurora FPS Engine/Controller/Based on Rigidbody", false, 4)]
        private static void CreateBasedOnRigidbodyController()
        {
            Object controller = AssetDatabase.LoadAssetAtPath<Object>(System.IO.Path.Combine(ApexSettings.RootPath, RigidbodyControllerLocation));
            if (controller != null)
            {
                Object controllerClone = Object.Instantiate(controller, Vector3.zero, Quaternion.identity);
                controllerClone.name = "New Player Based on Rigidbody Controller";
                EditorGUIUtility.PingObject(controllerClone);
            }
            else
            {
                Debug.LogError($"Controller cannot be created, because of location (Path: {RigidbodyControllerLocation}) with template is empty!");
            }
        }
    }
}