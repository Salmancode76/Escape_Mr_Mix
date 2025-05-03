/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Infinite Dawn
   Author    :    Alexandra Averyanova
   ---------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    public class InputBindingField
    {
        private InputBinding binding;
        private Text inputField;

        #region [Constructor]
        public InputBindingField(InputBinding binding, Text inputField)
        {
            this.binding = binding;
            this.inputField = inputField;
        }
        #endregion

        #region [Getter / Setter]
        public InputBinding GetBinding()
        {
            return binding;
        }

        public void SetBinding(InputBinding value)
        {
            binding = value;
        }

        public Text GetInputField()
        {
            return inputField;
        }

        public void SetInputField(Text value)
        {
            inputField = value;
        }
        #endregion
    }
}
