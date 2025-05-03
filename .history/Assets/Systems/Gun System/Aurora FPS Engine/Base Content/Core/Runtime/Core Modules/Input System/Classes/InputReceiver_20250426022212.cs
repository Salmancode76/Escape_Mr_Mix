/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.CoreModules.InputSystem
{
    public static class InputReceiver
    {
        /// <summary>
        /// Current input action map asset.
        /// </summary>
		public static InputActionAsset Asset { get; private set; }

        /// <summary>
        /// Current input config.
        /// </summary>
        public static InputConfig Config { get; private set; }

        /// <summary>
        /// Called once before splash screen.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Config = Resources.FindObjectsOfTypeAll<InputConfig>().FirstOrDefault() ?? ScriptableObject.CreateInstance<InputConfig>();
            Asset = Resources.Load<InputActionAsset>("Input/InputMap");
            Debug.Assert(Asset != null, "Input Map Asset not found!\nCreate at least one Input Action Asset in the project.");
            Asset.Enable();
            MovementVerticalAction = Asset.FindAction(Config.GetMovementVerticalPath(), true);
            MovementHorizontalAction = Asset.FindAction(Config.GetMovementHorizontalPath(), true);
            CameraVerticalAction = Asset.FindAction(Config.GetCameraVerticalPath(), true);
            CameraHorizontalAction = Asset.FindAction(Config.GetCameraHorizontalPath(), true);
            JumpAction = Asset.FindAction(Config.GetJumpPath(), true);
            CrouchAction = Asset.FindAction(Config.GetCrouchPath(), true);
            SprintAction = Asset.FindAction(Config.GetSprintPath(), true);
            LightWalkAction = Asset.FindAction(Config.GetLightWalkPath(), true);
            InteractAction = Asset.FindAction(Config.GetInteractPath(), true);
            ZoomAction = Asset.FindAction(Config.GetZoomPath(), true);
            AttackAction = Asset.FindAction(Config.GetAttackPath(), true);
            ReloadAction = Asset.FindAction(Config.GetReloadPath(), true);
            SwitchFireModeAction = Asset.FindAction(Config.GetSwitchFireModePath(), true);
            ScrollItemsAction = Asset.FindAction(Config.GetScrollItemsPath(), true);
            HideItemAction = Asset.FindAction(Config.GetHideItemPath(), true);
            TossItemAction = Asset.FindAction(Config.GetTossItemPath(), true);
            GrabObjectAction = Asset.FindAction(Config.GetGrabObjectPath(), true);
            ThrowObjectAction = Asset.FindAction(Config.GetThrowObjectPath(), true);
        }

        public static void EnableMap(string name)
        {
            InputActionMap actionMap = Asset.FindActionMap(name, false);
            if (actionMap != null && (EnableMapPredicate?.Invoke(name) ?? true))
            {
                actionMap.Enable();
            }
        }

        public static void DisableMap(string name)
        {
            InputActionMap actionMap = Asset.FindActionMap(name, false);
            actionMap?.Disable();
        }

        public static void EnableAction(string path)
        {
            InputAction action = Asset.FindAction(path, false);
            action?.Enable();
        }

        public static void DisableAction(string path)
        {
            InputAction action = Asset.FindAction(path, false);
            action?.Disable();
        }

        public static void HardwareCursor(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = value;
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Additional condition for enabling specified input map.
        /// <br><i><b>Parameter type of (string)</b>: Specified map name of input map asset.</i></br>
        /// </summary>
        public static event System.Predicate<string> EnableMapPredicate;
        #endregion

        #region [Getter]
        public static InputAction MovementVerticalAction { get; private set; }
        public static InputAction MovementHorizontalAction { get; private set; }
        public static InputAction CameraVerticalAction { get; private set; }
        public static InputAction CameraHorizontalAction { get; private set; }
        public static InputAction JumpAction { get; private set; }
        public static InputAction CrouchAction { get; private set; }
        public static InputAction SprintAction { get; private set; }
        public static InputAction LightWalkAction { get; private set; }
        public static InputAction InteractAction { get; private set; }
        public static InputAction ZoomAction { get; private set; }
        public static InputAction AttackAction { get; private set; }
        public static InputAction ReloadAction { get; private set; }
        public static InputAction SwitchFireModeAction { get; private set; }
        public static InputAction ScrollItemsAction { get; private set; }
        public static InputAction HideItemAction { get; private set; }
        public static InputAction TossItemAction { get; private set; }
        public static InputAction GrabObjectAction { get; private set; }
        public static InputAction ThrowObjectAction { get; private set; }
        #endregion
    }
}