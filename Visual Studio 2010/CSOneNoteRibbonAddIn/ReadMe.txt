=============================================================================
          应用程序 : CSOneNoteRibbonAddIn 项目概述
=============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要: 
该实例演示了一个实现了IDTExtensibility2 的 OneNote 2010 COM 外接程序.

该外接程序还支持通过实现 IRibbonExtensibility 接口，定制功能区.

CSOneNoteRibbonAddIn: 为 CSOneNoteRibbonAddInSetup 项目生成 
CSOneNoteRibbonAddIn.dll 的项目.

CSOneNoteRibbonAddInSetup: 生成用于 OneNote 2010 的 setup.exe 和
CSOneNoteRibbonAddInSetup.msi 的安装项目.

/////////////////////////////////////////////////////////////////////////////
前提条件：

您必须在安装有 Microsoft OneNote 2010 的计算机上运行此代码示例.

/////////////////////////////////////////////////////////////////////////////
演示:

下列步骤将指导通过 CSOneNoteRibbonAddIn 的演示实例.

步骤 1. 以管理员的身份,打开解决方案文件 CSOneNoteRibbonAddIn.sln;

步骤 2. 首先,在 Visual Studio 2010中,生成 CSOneNoteRibbonAddIn，然后生成安装项
目 CSOneNoteRibbonAddInSetup, 那么，你会得到引导程序 setup.exe 和应用程序 
CSOneNoteRibbonAddInSetup.msi;

步骤3. 安装 setup.exe;

步骤4. 打开 OneNote 2010, 您将看到三个消息框:
MessageBox.Show("CSOneNoteRibbonAddIn OnConnection"),
MessageBox.Show("CSOneNoteRibbonAddIn OnAddInsUpdate"),
MessageBox.Show("CSOneNoteRibbonAddIn OnStartupComplete");

步骤5. 单击"审阅"选项卡,您将看到"统计组"中,包含有外接程序加到功能区的按钮 ShowForm.
单击 ShowForm 按钮，窗体窗口弹出，您可以单击该窗体上的按钮获取当前页的标题.

步骤6. 当关闭 OneNote 2010 时, 您将看到三个消息框:
MessageBox.Show("CSOneNoteRibbonAddIn OnBeginShutdown"),
MessageBox.Show("CSOneNoteRibbonAddIn OnDisconnection")

/////////////////////////////////////////////////////////////////////////////
创建:

步骤1. 如下所示,创建一个共享外接程序的扩展性,和共享的外接程序向导:
  
	以管理员的身份,打开 Visual Studio 2010;
	创建一个共享的外接程序 (Other Project Types->Extensibility) 

	使用 Visual C# 创建一个外接程序; 

	选择 Microsoft Access （因为不存在Microsoft OneNote 可供选择,第一，您可以
	选择 Microsoft Access 选项,但记得要修改安装项目的的注册表项HKCU为OneNote);
    
	填写外接程序的名称和说明;
	在选择外接程序选项中选择两个复选框。
	
步骤2. 修改 CSOneNoteRibbonAddInSetup 注册表 
(右击"项目"->视图->注册表) 
[HKEY_CLASSES_ROOT\AppID\{Your GUID}]
"DllSurrogate"=""
[HKEY_CLASSES_ROOT\CLSID\{Your GUID}]
"AppID"="{Your GUID}"

[HKEY_CURRENT_USER\Software\Microsoft\Office\OneNote\AddIns\
CSOneNoteRibbonAddIn.Connect]
"LoadBehavior"=dword:00000003
"FriendlyName"="CSOneNoteRibbionAddIn"
"Description"="OneNote2010 Ribbon AddIn Sample"

[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\AppID\{Your GUID}]
"DllSurrogate"=""
[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{Your GUID}]
"AppID"="{Your GUID}"

步骤3. 添加 customUI.xml 和 showform.png 资源文件到项目 CSOneNoteRibbonAddIn 中.

步骤4. 创建继承自 IRibbonExtensibility 的类 Connect,并实现方法 GetCustomUI.
       
	/// <summary>		     
        /// 从自定义功能区用户界面的 XML 自定义文件中,加载的 XML 标记 
        /// </summary>
        /// <param name="RibbonID"> RibbonX 用户界面的 ID </param>
        /// <returns>string</returns>
        public string GetCustomUI(string RibbonID)
        {
            return Properties.Resources.customUI;
        }

步骤5. 根据 customUI.xml 的内容,实现方法 OnGetImage 和 ShowForm.

        /// <summary>
        ///  实现 customUI.xml 中的 OnGetImage 方法
        /// </summary>
        /// <param name="imageName"> customUI.xml 中的图像名 </param>
        /// <returns> 装有图像数据的内存流 </returns>
        public IStream OnGetImage(string imageName)
        {
            MemoryStream stream = new MemoryStream();
            if (imageName == "showform.png")
            {
                Resources.ShowForm.Save(stream, ImageFormat.Png);
            }

            return new ReadOnlyIStreamWrapper(stream);
        }

        /// <summary>
        ///  用于显示窗体的方法
        /// </summary>
        /// <param name="control"> 表示传递给每个功能区用户界面 (UI) 控件的	
        /// 回调过程的对象. </param>
        public void ShowForm(IRibbonControl control)
        {
            OneNote.Window context = control.Context as OneNote.Window;
            CWin32WindowWrapper owner =
                new CWin32WindowWrapper((IntPtr)context.WindowHandle);
            TestForm form = new TestForm(applicationObject as OneNote.Application);
            form.ShowDialog(owner);

            form.Dispose();
            form = null;
            context = null;
            owner = null;           
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

步骤6. 添加 ReadOnlyIStreamWrapper 类和 CWin32WindowWrapper 类到项目
CSOneNoteRibbonAddIn 中并添加一个用于测试中打开的窗体.

步骤7. 注册输出程序集为 COM 组件.

若要执行此操作，请单击“项目”->“项目属性...”按钮. 在项目属性页，导航
到"生成"选项卡,并选中复选框"为 COM Interop 注册".

步骤8. 生成您的 CSOneNoteRibbonAddIn 项目, 
然后生成 CSOneNoteRibbonAddInSetup 项目以产生 setup.exe 和 
CSOneNoteRibbonAddInSetup.msi.

/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: Creating OneNote 2010 Extensions with the OneNote Object Model
http://msdn.microsoft.com/en-us/magazine/ff796230.aspx

Jeff Cardon's Blog
http://blogs.msdn.com/b/onenotetips/

/////////////////////////////////////////////////////////////////////////////

