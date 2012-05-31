=============================================================================
       Windows应用程序: VBRegisterHotkey 项目概述                        
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

VBRegisterHotkeyThe案例阐述了如何给当前应用程序注册和注销一个热键。

User32.dll包含两种外来方法RegisterHotKey和UnregisterHotKey，它们是用来定义或
者取消整个系统的热键。Application.AddMessageFilter方法是用来给系统消息添加
一个消息过滤器的，因为它们对于它们的目标是有选择的。在一个消息分派之前，方
法PreFilterMessage能够处理它。

/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 用VS2010建立VBRegisterHotkey 项目. 

步骤2. 运行VBRegisterHotkey.exe.

步骤3. 点击窗体中的文本框并按Alt+Ctrl+T键。在文本框中你将会看到“Alt,Control+T”，
       并且“注册”按钮是可利用的。

步骤4. 点击“注册”按钮,然后文本框和“注册”按钮将不能利用,“注销”按钮将变得可以
       利用。
       
       如果热键已经被注册了，你将会得到一个提醒“该热键已经被利用”，你
       可以利用其他的热键比如Alt+M。

步骤5. 当应用程序不是活动窗口的时候，按Alt+Ctrl+T键这应用程序将会显示为活动窗空。

/////////////////////////////////////////////////////////////////////////////
代码逻辑：

1. 定义一个类HotKeyRegister覆盖User32.dll的两种外来方法RegisterHotKey和Unregister-
   HotKey。该类同时也支持一个静态方法GetModifiers从KeyEventArgs的KeyData属性来获得
   修饰语和关键词。

   当用参数处理、地址、修改器和关键词的方式处理该类的一个新的实例时，构造函数将声
   明一个RegisterHotKey方法去注册特定的热键。

2. 枚举类型KeyModifiers包含了如CTRL，ALT和SHIFT的修饰语。该WinKey还支持RegisterHot-
   Key方法，但在操作系统中键盘热键包括的Windows键是保留使用的。

3. 设计的用户界面MainFrom其中包含一个文本框，2个按钮及一些标签。

   用户界面MainFrom将处理文本框的KeyDown事件，并检查该按键是否有效，必须按下按键必
   须包括Ctrl，Shift或Alt键，如Ctrl + Alt+T。
   
   用户界面MainFrom也会处理按钮的Click事件去定义或者取消一个系统热键。当窗体是关
   闭的时候，它会处理HotKeyRegister的实例。

/////////////////////////////////////////////////////////////////////////////
参考资料:

MSDN: RegisterHotKey函数
http://msdn.microsoft.com/en-us/library/ms646309.aspx

MSDN: UnregisterHotKey函数
http://msdn.microsoft.com/en-us/library/s646327.aspx

MSDN: Application.AddMessageFilter方法 
http://msdn.microsoft.com/en-us/library/system.windows.forms.application.addmessagefilter.aspx

IMessageFilter接口
http://msdn.microsoft.com/en-us/library/system.windows.forms.imessagefilter.aspx


/////////////////////////////////////////////////////////////////////////////
