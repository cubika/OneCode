=============================================================================
       Windows 应用程序: VBCustomIEContextMenu 概述                        
=============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:

该实例说明如何在使用自定义的 IE 上下文菜单，在一个新选项卡中打开一张图像. 
该实例提供了以下特征：

1. 将项添加到 IE 标准的上下文菜单.
2. 使用浏览器辅助对象（Browser Helper Object）重写了 IE 标准上下文菜单.
3. 部署自定义的 IE 上下文菜单.

注意:
1. 在 IE8 中，可以用名为加速器或者活动的新方法，将项添加到IE上下文菜单.加速器使得
   从一个网页复制信息到另一个网页变得更为容易。同时，"将项添加到IE标准上下文菜单"
   仍然是一个在本地工作的好方法.

2. 如果您在一个 BHO中重写 IE 标准上下文菜单，请确保一次仅有一个加载项能够重写 
   IDocHostUIHandler，多个加载项很容易互相引起冲突. 你也可以创建你自己的 web 浏览
   器来设置这个句柄. 

3. 在 Windows Server 2008 或者 Windows Server 2008 R2中，IE 浏览器增强的安全配置
   （Internet Explorer Enhanced Security Configuration，IE ESC）是默认开启的。这
    意味中该扩展不能被IE加载. 你必须关闭 IE ESC.

////////////////////////////////////////////////////////////////////////////////
安装和卸载:

A. 安装

针对 x 86 或 x64 的操作系统上的 32 位 IE ，安装 VBCustomIEContextMenuSetup(x86).msi.
VBCustomIEContextMenuSetup(x86).msi 是 VBCustomIEContextMenuSetup(x86)安装项目的
输出结果.

针对 x64 的操作系统上的 64 位 IE,安装 VBCustomIEContextMenuSetup(x64).msi.
VBCustomIEContextMenuSetup(x64).msi 是 VBCustomIEContextMenuSetup(x64)安装项目的
输出结果.

B. 卸载

针对 x 86 或 x64 的操作系统上的 32 位 IE ,卸载 VBCustomIEContextMenuSetup(x86).msi.
VBCustomIEContextMenuSetup(x86).msi 是 VBCustomIEContextMenuSetup(x86)安装项目的
输出结果.

针对 x64 的操作系统上的 64 位 IE ,卸载 VBCustomIEContextMenuSetup(x64).msi.
VBCustomIEContextMenuSetup(x64).msi 是 VBCustomIEContextMenuSetup(x64)安装项目的
输出结果.

////////////////////////////////////////////////////////////////////////////////
演示:
 
步骤1. 在Visual Studio 2010 中打开该项目，设置该解决方案的平台为 x86. 确保在“配
       置管理器”中选择了生成 VBCustomIEContextMenu 和 
	   VBCustomIEContextMenuSetup(x86).
       
       注意：如果你想在 64 位IE中运行该实例，将平台设为 x64,同时选择生成 
       VBCustomIEContextMenu 和 VBCustomIEContextMenuSetup(x64). 
        
步骤2. 生成该解决方案.
 
步骤3. 在解决方案资源管理器中,右击 VBCustomIEContextMenuSetup(x86) 项目,选择"安装".
       
       在安装完成后，打开32位IE，单击工具=>管理加载项，在“管理加载项”对话框，你
	   可以发现项目 VBCustomIEContextMenu.BHOIEContextMenu.	  

演示增加项到 IE 标准上下文菜单.

步骤4. 打开32位 IE,单击工具=>管理加载项,禁用"VBCustomIEContextMenu.BHOIEContextMenu".
       您可能需要重启 IE 以使之生效.

步骤5.访问 http://www.microsoft.com/. 右击该网页上的一张图像，在文本菜单上，你将看
      到“在新选项卡中打开图像”项.点击该项，IE 将会打开一个新的选项卡以打开图像.

演示重写IE标准上下文菜单
 
步骤6. 打开32位 IE,单击工具=>管理加载项，启用"VBCustomIEContextMenu.BHOIEContextMenu".
       您可能需要重启 IE 以使之生效. 

步骤7. 访问 http://www.microsoft.com/. 右击该网页上的一张图像，在文本菜单上，你将看
       到“在新选项卡中打开图像”项.点击该项，IE 将会打开一个新的选项卡以打开图像.

////////////////////////////////////////////////////////////////////////////////
实现:

A. 创建项目并添加引用.

在 Visual Studio 2010 中, 创建一个 Visual Basic / Windows / Class Library 项目命名
为"VBCustomIEContextMenu". 

在解决方案资源管理器中右击该项目，选择“添加引用”.在COM选项卡中添加“Microsoft HTML 
 Object Library" 和"Microsoft Internet Controls" .

-----------------------------------------------------------------------------

B. 增加项到 IE 标准上下文菜单.

为了给 IE 标准上下文菜单增加项，您可以在“HKEY_CURRENT_USER\Software\Microsoft\Internet
Explorer\MenuExt\”下增加一个键.键的默认值是将会处理事件的 html 文件路径.
请参照 http://msdn.microsoft.com/en-us/library/aa753589(VS.85).aspx 以获取详细信息.

OpenImageMenuExt 类提供了两个方案以在注册表中增加或者移除项.
html 页 Resource\OpenImage.htm 是用于在新选项卡中打开一张图像的.

-----------------------------------------------------------------------------

C. 使用浏览器辅助对象重写 IE 标准上下文菜单. 

1. OpenImageHandler 类实现了 IDocHostUIHandler 接口，它会重写默认的上下文菜单.

2. OpenImageBHO 类是一个浏览器辅助对象,它将会把OpenImageHandler 设置为 html 文件
   的 UIHandler.有关如何创建 / 部署 BHO，请参照微软 All-In-One Code 框架中的实例
   "VBBrowserHelperObject".

-----------------------------------------------------------------------------

D. 用一个安装程序项目来部署自定义上下文菜单.

  (1) 在 VBCustomIEContextMenu中, 增加一个安装程序类 (在该代码示例中名为 
      CustomIEContextMenuInstaller)，以定义安装过程中的自定义操作.该类继承自 
	  System.Configuration.Install.Installer.我们可以用自定义操作来 添加/移除 
	  注册表中的 IE 文本菜单项， 并在用户安装 / 卸载该组件时，注册 / 注销当前
	  托管程序集中的 COM 可见的类.
 
    [RunInstaller(true), ComVisible(false)]
    public partial class CustomIEContextMenuInstaller : System.Configuration.Install.Installer
    {
        public CustomIEContextMenuInstaller()
        {
            InitializeComponent();
        }
  
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            OpenImageMenuExt.RegisterMenuExt();

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(this.GetType().Assembly,
            AssemblyRegistrationFlags.SetCodeBase))
            {
                throw new InstallException("注册COM失败");
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            OpenImageMenuExt.UnRegisterMenuExt();

            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("注销COM失败");
            }
        }
    }


  (2) 为了增加一个部署项目， 在“文件”菜单上，指向“添加”，然后单击“新建项目”. 
  在"添加新项目"对话框中，展开“其他项目类型”节点，展开“安装和部署项目”，单击
  “Visual Studio 安装程序”，请单击，然后单击安装项目.在“名称”中，键入 
  VBCustomIEContextMenuSetup(x86)。单击“确定”以创建项目.
  	
  在安装项目的“属性”对话框，请确保 TargetPlatform 属性设置为 x 86.此安装项目是
  用于部署 x 86 或 x64 Windows 操作系统中的 32 位 IE 的.

  右击该安装项目，选择“添加->项目输出”...
  
  在 “Add Project Output Group”对话框中,VBCustomIEContextMenu 将会显示在Project
  列表中. 选择“Primary Output”并单击“确定”. Visual Studio 将会检测
  VBCustomIEContextMenu 的依赖关系，包括.NET Framework 4.0 (Client Profile).

  右击该安装项目，选择 “视图 / 自定义操作”.
  在 Custom Actions Editor,右键单击自定义操作的根节点. 在 Action 菜单中,
  单击“Add Custom Action”.在“项目”对话框中的Select项中，双击 Application Folder.
  选择 VBCustomIEContextMenu的 Primary Output。这添加了我们在VBCustomIEContextMenu的
  BHOInstaller 中定义的自定义操作. 
   
  用鼠标右键单击安装项目，并选择查看 / 文件系统.在应用程序文件夹中，添加一个名为"资
  源"文件夹，并将 OpenImage.htm 添加到此文件夹中.

  生成安装项目。如果生成成功，您将得到一个.msi 文件和 Setup.exe 文件.您可以将它们发布
  给您的用户以安装或卸载此 BHO.

  (3) 要部署用于 x64 操作系统上的 64 位 IE BHO，您必须创建一个新的安装项目 （例如，在
  此代码示例中的 VBCustomIEContextMenuSetup(x64)），并将其 TargetPlatform 属性设置为
  x64. 
  
  尽管TargetPlatform 属性设置为 x64,与 .msi 文件打包在一起的本地安装文件,仍然是一个 
  32 位的可执行文件. Visual Studio 将 InstallUtilLib.dll 的 32 位版本,作为 InstallUtil 
  嵌入到二进制表中.所以,这些自定义操作将在 32 位模式中运行,这是该代码示例的意外.要解决此
  问题，并确保这些自定义操作在 64 位模式下运行,您要么需要导入合适的 InstallUtilLib.dll
  到二进制表中，以用于 InstallUtil 记录，或者——如果您已有或将有 32 位的托管自定义操作,
  将其作为新纪录添加到二进制表中,调整 CustomeAction 表,以便为 64 位托管自定义操作使用 
  64 位二进制表的记录.这篇博客介绍了如何用 Orca 手动实现它:
  http://blogs.msdn.com/b/heaths/archive/2006/02/01/64-bit-managed-custom-actions-with-visual-studio.aspx

  在此代码示例中，我们使用了后期生成的javascript:Fix64bitInstallUtilLib.js,来自动化 
  InstallUtil 的修改.在 VBCustomIEContextMenuSetup(x64) 的项目文件夹中,您可以找到该
  脚本文件.要配置该脚本以便在post-build事件中运行, 在解决方案资源管理器中，选择 
  VBCustomIEContextMenuSetup(x64) 项目，并在“属性”窗口中查找 PostBuildEvent 属性.
  指定它的值为  
  
	"$(ProjectDir)Fix64bitInstallUtilLib.js" "$(BuiltOuputPath)" "$(ProjectDir)"

  在重复 (2) 中剩下的步骤以添加项目的输出,设置自定义操作,配置先决条件,并生成该项目.

/////////////////////////////////////////////////////////////////////////////
诊断:

如果要调试 IE 上下文菜单，您可以附加到 iexplorer.exe.


///////////////////////////////////////////////////////////////////////////// 
 
参考:

Adding Entries to the Standard Context Menu
http://msdn.microsoft.com/en-us/library/aa753589(VS.85).aspx

Context Menus and Extensions
http://msdn.microsoft.com/en-us/library/aa770042(VS.85).aspx#wbc_ctxmenus

Browser Helper Objects: The Browser the Way You Want It
http://msdn.microsoft.com/en-us/library/ms976373.aspx

/////////////////////////////////////////////////////////////////////////////

