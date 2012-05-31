/****************************** Module Header ******************************\
 * Module Name:  KeyButton.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * 这个控件代表一个键盘按钮。
 * 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace CSSoftKeyboard.Controls
{
    public partial class KeyButton : Button
    {

        // 默认的风格。
        static Color normalBackColor = Color.Black;
        static Color mouseOverBackColor = Color.DimGray;
        static Color pressedBackColor = Color.White;

        static Color normalLabelForeColor = Color.White;
        static Color pressedlLabelForeColor = Color.Black;
     
        /// <summary>
        /// 按键的键值。
        /// </summary>
        public int KeyCode { get; set; }

        Keys key;
        public Keys Key
        {
            get
            {
                if (key == Keys.None)
                {
                    key = (Keys)KeyCode;
                }

                return key;
            }
        }

        /// <summary>
        /// 如果NumLock键被按下了，数字键盘上按键的键值。
        /// </summary>
        public int UnNumLockKeyCode { get; set; }

        /// <summary>
        /// 如果NumLock键没有按下，数字键盘上按键的文本。
        /// </summary>
        public string UnNumLockText { get; set; }

        /// <summary>
        /// 按键的正常文本。
        /// </summary>
        public string NormalText { get; set; }

        /// <summary>
        /// Shift 键被按下后按键的文本。
        /// </summary>
        public string ShiftText { get; set; }

        /// <summary>
        /// 指定是不是修饰键。
        /// </summary>
        public bool IsModifierKey
        {
            get
            {
                return Key == Keys.ControlKey
                     || Key == Keys.ShiftKey
                     || Key == Keys.Menu
                     || Key == Keys.LWin
                     || Key == Keys.RWin;
            }
        }

       
        /// <summary>
        /// 指定是不是锁定键。
        /// </summary>
        public bool IsLockKey
        {
            get
            {
                return Key == Keys.Capital
                    || Key == Keys.Scroll
                    || Key == Keys.NumLock;
            }
        }

        /// <summary>
        /// 指定是不是一个字母键。
        /// </summary>
        public bool IsLetterKey
        {
            get
            {
                return Key >= Keys.A && Key <= Keys.Z;
            }
        }

        /// <summary>
        /// 指定是不是数字键盘上的键。
        /// </summary>
        public bool IsNumberPadKey
        {
            get
            {
                return Key >= Keys.NumPad0 && Key <= Keys.NumPad9;
            }
        }

        bool isPressed;

        /// <summary>
        /// 指定按键是否被按下。
        /// </summary>
        public bool IsPressed
        {
            get
            {
                return isPressed;
            }
            set
            {
                if (isPressed != value)
                {
                    isPressed = value;

                    this.OnIsPressedChange(EventArgs.Empty);
                }
            }

        }

        bool isMouseOver;

        /// <summary>
        /// 指定鼠标是否在这个按键上。
        /// </summary>
        bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }
            set
            {
                if (isMouseOver != value)
                {
                    isMouseOver = value;

                    this.OnIsMouseOverChange(EventArgs.Empty);
                }
            }
        }

        public KeyButton()
        {
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackColor = System.Drawing.Color.Black;
            this.ForeColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// 更新按键的文本。
        /// </summary>
        public void UpdateDisplayText(bool isShiftKeyPressed, bool isNumLockPressed, bool isCapsLockPressed)
        {
            if (this.IsLetterKey)
            {
                this.Text = (isShiftKeyPressed ^ isCapsLockPressed) ? ShiftText : NormalText;
            }
            else if (!string.IsNullOrEmpty(this.ShiftText))
            {
                this.Text = isShiftKeyPressed ? ShiftText : NormalText;
            }
            else if (this.IsNumberPadKey)
            {
                this.Text = isNumLockPressed ? NormalText : UnNumLockText;
            }

        }

        #region Update the style of the key board button.

        /// <summary>
        /// MouseDown 事件的句柄。
        /// 改变 IsPressed 属性的值，将会引起按钮的更新。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            IsPressed = !IsPressed;
        }

        /// <summary>
        /// MouseUp 事件的句柄。
        /// 如果按键不是修饰键也不是锁定键，把它的IsPressed属性值设为false，这会使得按钮更新。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!IsModifierKey && !IsLockKey)
            {
                IsPressed = false;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseOver = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOver = false;
        }


        protected virtual void OnIsMouseOverChange(EventArgs e)
        {
            ReDrawKeyButton();
        }

        protected virtual void OnIsPressedChange(EventArgs e)
        {
            ReDrawKeyButton();
        }

        protected virtual void ReDrawKeyButton()
        {
            if (IsPressed)
            {
                this.BackColor = KeyButton.pressedBackColor;
                this.ForeColor = KeyButton.pressedlLabelForeColor;
            }
            else if (IsMouseOver)
            {
                this.BackColor = KeyButton.mouseOverBackColor;
                this.ForeColor = KeyButton.normalLabelForeColor;
            }
            else
            {
                this.BackColor = KeyButton.normalBackColor;
                this.ForeColor = KeyButton.normalLabelForeColor;
            }
        }

        #endregion
    }
}
