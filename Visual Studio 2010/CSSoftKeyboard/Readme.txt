================================================================================
	   Windows 应用程序：CSSoftkeyboard 概述                   
===============================================================================

/////////////////////////////////////////////////////////////////////////////
简介：

这个例子演示了如何创建一个软键盘。它有以下特点：

1.当单击一个键钮时，它将得不到焦点。

2.如果用户在非用户区（例如标题栏）按下鼠标左键的话，它将会被激活。当鼠标左键被释放的
  时候，它将会激活先前的前台窗口。

3.当用户点击其上的一个字符的时候，像 A 或者 1 ，它将会把这个键值发送给现行应用程序。

4.它支持特殊的按键，比如 WinKey 和 Delete。

5.它还支持组合键，例如 Ctrl+C 。

注意：Ctrl+Alt+Del 是不支持的，因为它将会引起安全问题。

////////////////////////////////////////////////////////////////////////////////
演示：

步骤一、在 VS2010 中生成项目。

步骤二、打开 NotePad.exe，然后运行 CSSoftKeyboard.exe。确定 NotePad.exe 是现行应用程序。


演示一个正常的按键。

步骤三、点击键盘上的 “a” 键，你将会看到 NotePad.exe 中出现一个 “a”。


演示一个锁键按钮。

步骤四、点击 “Caps” 键，你将会看到它的背景颜色变为白色，并且所有的字母键都变成了大写。

步骤五、点击键盘上的 “a” 键，你将会看到 NotePad.exe 中出现一个字母 “A”。

步骤六、再次点击 “Caps” 键，你将会看到它的背景颜色又变回黑色，并且所有的字母键又都变回小写。


演示 Win 键。

步骤七、点击 “Win” 键，你将会看到它的背景颜色变为白色。

步骤八、再次点击 “win” 键，你将会看到它的背景颜色又变回黑色。并且 “开始” 菜单被打开了。


演示 shift 键。

步骤九、点击左侧的 “shift” 键，你将会看到左侧和右侧的 “shift” 键的背景颜色都变为白色。
	   并且能和 “shift” 组合的键将都显示成组合使用时的内容。例如，“=” 键就显示成 “+” 。

步骤十、点击键盘上的 “+” 键，你将会看到 NotePad.exe 出现一个 “+” 。
	   左右两侧的 “shift” 键都将会变回黑色，并且那些能和 “shift” 组合的键都显现正常内容。


演示组合键。

步骤十一、点击左侧的 “Ctrl” 键，你将会看到它和右侧的 “Ctrl”键的背景颜色都变为白色。

步骤十二、点击 “s” 键，你将会看到 NotePad.exe 弹出一个保存文件对话框。

/////////////////////////////////////////////////////////////////////////////
代码逻辑：

1、设计一个 NoActiveWindow 类，被主窗体 KeyBoardForm 继承。

	这个 NoActiveWindow 类代表一个窗体，这个窗体不会被激活，直到用户在非用户区（比如标题
	栏、菜单栏、或者窗口框架）按下鼠标左键。当鼠标左键被释放的时候，这个窗口将会激活先前
	的前台窗口。

		/// <summary>
		/// 设置窗体风格为 WS_EX_NOACTIVATE，以便它将获得不到焦点。
        /// </summary>
        protected override CreateParams CreateParams
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCLBUTTONDOWN:

					// 获得当前的前台窗口。
                    var foregroundWindow = UnsafeNativeMethods.GetForegroundWindow();

					// 如果这个窗口不是当前前台窗口，那么就激活它。
                    if (foregroundWindow != this.Handle)
                    {
                        UnsafeNativeMethods.SetForegroundWindow(this.Handle);

						// 保存先前前台窗口的句柄。
                        if (foregroundWindow != IntPtr.Zero)
                        {
                            previousForegroundWindow = foregroundWindow;
                        }
                    }

                    break;
                case WM_NCMOUSEMOVE:

					// 判断先前窗口是否存在。如果存在的话，那么就激活它。
					// 注意：有这样的情况，先前窗口关闭了，但是这个相同的句柄赋值给一个新的窗口。
                    if ( UnsafeNativeMethods.IsWindow(previousForegroundWindow))
                    {
                        UnsafeNativeMethods.SetForegroundWindow(previousForegroundWindow);
                    }

                    break;
                default:
                    break;
            }

            base.WndProc(ref m);
        }

2、KeyboardInput 类覆盖了 User32.dll 的 SendInput 方法，并且提供 SendKey 方法模拟正常的按键事件。
   它还支持组合按键，例如 “Ctrl+C” 。

   有三种不同的情形：
   2.1、单个按键被按下，例如 “A”。
   
			var inputs = new NativeMethods.INPUT[1];
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
                UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs[0]));
   
   2.2、带有修饰键的键被按下，例如 “Ctrl+A”。

                // 为了模拟这种情形，输入的信息包括被锁定修饰键事件、按键事件、释放修饰键事件。
                // 例如，为了模拟Ctrl+C，我们必须发送三次输入信息。
                // 1、Ctrl被按下。
                // 2、C被按下。
                // 3、Ctrl被释放。
                var inputs = new NativeMethods.INPUT[modifierKeys.Count()*2 + 1];
                
                int i = 0;

				// 模拟锁定修饰键。
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey;
                    i++;
                }

                // 模拟按键。
                inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                inputs[i].inputUnion.ki.wVk = (short)key;
                i++;

                // 模拟释放修饰键。
                foreach (var modifierKey in modifierKeys)
                {
                    inputs[i].type = NativeMethods.INPUT_KEYBOARD;
                    inputs[i].inputUnion.ki.wVk = (short)modifierKey; 
                    inputs[i].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;
                    i++;
                }
                
                UnsafeNativeMethods.SendInput((uint)inputs.Length, inputs,
                    Marshal.SizeOf(inputs[0]));

   2.3、可锁定键被按下，例如大小写锁定键、数码锁定键和滚动锁定键。

                var inputs = new NativeMethods.INPUT[2];
             
                // 按下按键。
                inputs[0].type = NativeMethods.INPUT_KEYBOARD;
                inputs[0].inputUnion.ki.wVk = (short)key;
             
                // 释放按键。
                inputs[1].type = NativeMethods.INPUT_KEYBOARD;
                inputs[1].inputUnion.ki.wVk = (short)key;
                inputs[1].inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP;
             
                UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs[0]));

       
3、这是展示键盘的主窗体。当这个窗体被加载的时候，它将会加载 KeysMapping.xml 初始化键盘按钮。



/////////////////////////////////////////////////////////////////////////////
参考：

SendInput 函数
http://msdn.microsoft.com/en-us/library/ms646310(VS.85).aspx

GetKeyState 函数
http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx

Control.CreateParams 属性
http://msdn.microsoft.com/en-us/library/system.windows.forms.control.createparams.aspx

GetForegroundWindow 函数
http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx

SetForegroundWindow 函数
http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx
/////////////////////////////////////////////////////////////////////////////



                