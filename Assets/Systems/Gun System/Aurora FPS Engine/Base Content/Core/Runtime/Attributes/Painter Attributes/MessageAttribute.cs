/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.Attributes
{
    public enum MessageStyle
    {
        Info,
        Warning,
        Error
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MessageAttribute : PainterAttribute
    {
        public readonly string text;
        public readonly MessageStyle messageStyle;

        public MessageAttribute(string text)
        {
            this.text = text;
            this.messageStyle = MessageStyle.Info;
        }

        public MessageAttribute(string text, MessageStyle messageType) : this(text)
        {
            this.messageStyle = messageType;
        }
    }
}