/****************************** Module Header ******************************\
 * Module Name:  KeyBoardForm.cs
 * Project:      CSSoftKeyboard
 * Copyright (c) Microsoft Corporation.
 * 
 * 这是展示软键盘的主窗体。当这个窗体被加载的时候，它将会加载 KeysMapping.xml
 * 初始化键盘按钮。
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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using CSSoftKeyboard.Controls;

namespace CSSoftKeyboard
{
    public partial class KeyBoardForm : CSSoftKeyboard.NoActivate.NoActivateWindow
    {

        IEnumerable<KeyButton> keyButtonList = null;
        IEnumerable<KeyButton> KeyButtonList
        {
            get
            {
                if (keyButtonList == null)
                {
                    keyButtonList = this.Controls.OfType<KeyButton>();
                }
                return keyButtonList;
            }
        }


        List<int> pressedModifierKeyCodes = null;

        /// <summary>
        /// 被按下的修饰键。
        /// </summary>
        List<int> PressedModifierKeyCodes
        {
            get
            {
                if (pressedModifierKeyCodes == null)
                {
                    pressedModifierKeyCodes = new List<int>();
                }
                return pressedModifierKeyCodes;
            }
        }


        public KeyBoardForm()
        {
            InitializeComponent();
        }

        #region 处理键盘事件

        private void KeyBoardForm_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeKeyButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 注册键钮的click事件。
            foreach (KeyButton btn in this.KeyButtonList)
            {
                btn.Click += new EventHandler(KeyButton_Click);
            }

            this.Activate();
        }

        /// <summary>
        /// 处理键钮的click事件。
        /// </summary>
        void KeyButton_Click(object sender, EventArgs e)
        {
            KeyButton btn = sender as KeyButton;
            if (btn == null)
            {
                return;
            }

            // 同步按键对，例如 LShiftKey（左Shift键） 和 RShiftKey（右Shift键）.
            SyncKeyPairs(btn);

            // 处理特殊的按键，例如 “AppsKey”。
            if (ProcessSpecialKey(btn))
            {
                return;
            }

            // 如果按下数码锁定键、Shift 键和大小写锁定键 , 更新键钮的文本。
            if (btn.Key == Keys.NumLock || btn.Key == Keys.ShiftKey || btn.Key == Keys.CapsLock)
            {
                UpdateKeyButtonsText(keyButtonLShift.IsPressed, keyButtonNumLock.IsPressed, keyButtonCapsLock.IsPressed);
            }

            // 大小写锁定键、数码锁定键或者滚动锁定键 键被按下。
            if (btn.IsLockKey)
            {
                UserInteraction.KeyboardInput.SendToggledKey(btn.KeyCode);
            }

            // 一个修饰键被按下。
            else if (btn.IsModifierKey)
            {
                // 修饰键被按了两次。
                if (PressedModifierKeyCodes.Contains(btn.KeyCode))
                {
                    UserInteraction.KeyboardInput.SendToggledKey(btn.KeyCode);

                    // 清除所有修饰键的pressed状态。
                    ResetModifierKeyButtons();
                }
                else
                {
                    PressedModifierKeyCodes.Add(btn.KeyCode);
                }
            }

            // 一个正常的按键被按下。
            else
            {
                int btnKeyCode = btn.KeyCode;

                // 如果这个键是数字键盘上的键，并且数码锁定键未被按下，那么使用 UnNumLockKeyCode.
                if (btn.IsNumberPadKey && !keyButtonNumLock.IsPressed && btn.UnNumLockKeyCode > 0)
                {
                    btnKeyCode = btn.UnNumLockKeyCode;
                }

                UserInteraction.KeyboardInput.SendKey(PressedModifierKeyCodes, btnKeyCode);

                // 清除所有修饰键的 pressed 状态。
                ResetModifierKeyButtons();
            }
        }

        /// <summary>
        /// 同步按键对，像 LShiftKey(左shift键) 和 RShiftKey（右shift键）。
        /// </summary>
        void SyncKeyPairs(KeyButton btn)
        {
            if (btn == keyButtonLShift)
            {
                keyButtonRShift.IsPressed = keyButtonLShift.IsPressed;
            }
            if (btn == keyButtonRShift)
            {
                keyButtonLShift.IsPressed = keyButtonRShift.IsPressed;
            }

            if (btn == keyButtonLAlt)
            {
                keyButtonRAlt.IsPressed = keyButtonLAlt.IsPressed;
            }
            if (btn == keyButtonRAlt)
            {
                keyButtonLAlt.IsPressed = keyButtonRAlt.IsPressed;
            }

            if (btn == keyButtonLControl)
            {
                keyButtonRControl.IsPressed = keyButtonLControl.IsPressed;
            }
            if (btn == keyButtonRControl)
            {
                keyButtonLControl.IsPressed = keyButtonRControl.IsPressed;
            }
        }

        /// <summary>
        /// 处理特殊按键，例如 “AppsKey”。
        /// </summary>
        bool ProcessSpecialKey(KeyButton btn)
        {
            bool handled = true;
            switch (btn.Key)
            {

                // 使用 “Shift+F10” 模拟 Apps 键。
                case Keys.Apps:
                    UserInteraction.KeyboardInput.SendKey(
                                        new int[] { (int)Keys.ShiftKey },
                                        (int)Keys.F10);
                    break;
                default:
                    handled = false;
                    break;
            }
            return handled;
        }

        /// <summary>
        /// 初始化键钮。
        /// </summary>
        void InitializeKeyButtons()
        {
            short capsLockState = UserInteraction.UnsafeNativeMethods.GetKeyState((int)Keys.CapsLock);
            keyButtonCapsLock.IsPressed = (capsLockState & 0x0001) != 0;

            short numLockState = UserInteraction.UnsafeNativeMethods.GetKeyState((int)Keys.NumLock);
            keyButtonNumLock.IsPressed = (numLockState & 0x0001) != 0;


            short scrLockState = UserInteraction.UnsafeNativeMethods.GetKeyState((int)Keys.Scroll);
            keyButtonScrollLock.IsPressed = (scrLockState & 0x0001) != 0;

            var keysMappingDoc = XDocument.Load("Resources\\KeysMapping.xml");
            foreach (var key in keysMappingDoc.Root.Elements())
            {
                int keyCode = int.Parse(key.Element("KeyCode").Value);

                IEnumerable<KeyButton> btns = KeyButtonList.Where(btn => btn.KeyCode == keyCode);

                foreach (KeyButton btn in btns)
                {
                    btn.NormalText = key.Element("NormalText").Value;

                    if (key.Elements("ShiftText").Count() > 0)
                    {
                        btn.ShiftText = key.Element("ShiftText").Value;
                    }

                    if (key.Elements("UnNumLockText").Count() > 0)
                    {
                        btn.UnNumLockText = key.Element("UnNumLockText").Value;
                    }

                    if (key.Elements("UnNumLockKeyCode").Count() > 0)
                    {
                        btn.UnNumLockKeyCode =
                            int.Parse(key.Element("UnNumLockKeyCode").Value);
                    }

                    btn.UpdateDisplayText(false, keyButtonNumLock.IsPressed, keyButtonCapsLock.IsPressed);
                }
            }
        }

        /// <summary>
        /// 更新按键的文本。
        /// </summary>
        void UpdateKeyButtonsText(bool isShiftKeyPressed, bool isNumLockPressed, bool isCapsLockPressed)
        {
            foreach (var btn in this.KeyButtonList)
            {
                btn.UpdateDisplayText(isShiftKeyPressed, isNumLockPressed, isCapsLockPressed);
            }
        }

        /// <summary>
        /// 清除所有修饰键的 pressed 状态。
        /// </summary>
        void ResetModifierKeyButtons()
        {
            PressedModifierKeyCodes.Clear();

            keyButtonLShift.IsPressed = false;
            keyButtonRShift.IsPressed = false;
            keyButtonRControl.IsPressed = false;
            keyButtonLControl.IsPressed = false;
            keyButtonRAlt.IsPressed = false;
            keyButtonLAlt.IsPressed = false;
            keyButtonWin.IsPressed = false;

            foreach (var keybtn in KeyButtonList)
            {
                keybtn.UpdateDisplayText(false, keyButtonNumLock.IsPressed, keyButtonCapsLock.IsPressed);
            }
        }

        #endregion

    }
}
