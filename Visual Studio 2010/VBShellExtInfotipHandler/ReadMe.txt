=============================================================================
         类库 : VBShellExtInfotipHandler 项目描述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

         这个vb.net 例子演示如何在.net Framework 4中创建 一个shell infotip 处理程序

        信息提示是 shell 扩展处理程序，也就是 当用户的鼠标放在项目上弹出来的文字内容。
    它是用户能灵活的使用信息提示的一种方法。除了它，还有其他方式，一种是指定一个
    固定的字符串或者特定的文件属性列表显示（更多关于用户提示信息，请点击以下链接
    http://msdn.microsoft.com/en-us/library/cc144067.aspx)

	    在.net Framework 4之前，shell 扩展开发程序使用托管代码是没有正式支持的，
    因为CLR限制了一个进程只能有一个.net 运行时。CLR的一个项目经理 Jesse Kaplan
	在下面的链接中详细的解释了这点：
	http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/1428326d-7950-42b4-ad94-8e962124043e.

	    在.net 4中，已经实现了一个程序可以有多个运行时，微软可以编写托管代码，
	来支持shell 扩展程序，甚至任意程序都可以使用托管代码。VBShellExtInfotipHandler
	就是这样的一个托管shell 扩展程序示例。但是，请注意，您仍然不能在.NET Framework 4
	之前的版本中使用shell 扩展程序，因为那些版本的运行时不会加载另一个进程，可能会在很
	多情况下导致失败

	    例子中的 信息提示处理程序的 CLSID是： 
    {44BDEF95-C00F-493E-A55B-17937DB1F04E}
	    
		例子是用vb做了一个信息提示项目，在windows explorer中当您的鼠标指针悬浮在a.vb文件上的时候，你会看到
	以下的文件信息

    File: <File path, e.g. D:\Test.vb>
    Lines: <Line number, e.g. 123 or N/A>
    - Infotip displayed by VBShellExtInfotipHandler


/////////////////////////////////////////////////////////////////////////////
创建和移除：

A. 创建
   
  以管理员身份登陆系统，打开开始菜单选中Visual Studio 2010，点击Visual Studio Tools 
  运行'Visual Studio Command Prompt (2010)' (or 'Visual Studio x64 Win64 
Command Prompt (2010)'如果你是64位操作系统），在命令输入窗口，输入以下命令：
 

    Regasm.exe VBShellExtInfotipHandler.dll /codebase

	如果你看到下面这句话就说明信息提示处理程序创建成功：

    "Types registered successfully"

B.移除

      以管理员身份登陆系统，打开开始菜单选中Visual Studio 2010，点击Visual Studio Tools 
  运行'Visual Studio Command Prompt (2010)' (or 'Visual Studio x64 Win64 
  Command Prompt (2010)'如果你是64位操作系统），在命令输入窗口，输入以下命令：
 
    Regasm.exe VBShellExtInfotipHandler.dll /unregister

     如果你看到下面这句话就说明信息提示处理程序移除成功：

    "Types un-registered successfully"


/////////////////////////////////////////////////////////////////////////////
演示:

第一步. 先管理员身份登陆系统，创建一个以VBShellExtInfotipHandler命名的项目，然后
     打开开始菜单选中Visual Studio 2010，点击Visual Studio Tools 
     运行'Visual Studio Command Prompt (2010)' (or 'Visual Studio x64 Win64 
     Command Prompt (2010)'如果你是64位操作系统），在命令输入窗口，输入以下命令（不要关闭）：

    Regasm.exe VBShellExtInfotipHandler.dll /codebase

  如果说你看到下面这句话说明程序创建成功

    "Types registered successfully"
第二步. 在 Windows Explorer 找到一个文件，如FileInfotipExt.vb，然后把鼠标停在文件名上面，
       你就会看到以下的提示信息

    File: <File path, e.g. D:\VBShellExtInfotipHandler\FileInfotipExt.vb>
    Lines: <Line number, e.g. 123 or N/A>
    - Infotip displayed by VBShellExtInfotipHandler

第三步. 在同一个 命令窗口，输入以下命令，取消信息提示

    Regasm.exe VBShellExtInfotipHandler.dll /unregister



/////////////////////////////////////////////////////////////////////////////
代码逻辑:

A. 创建并配置项目
 打开Visual Studio 2010，创建一个名字为 VBShellExtInfotipHandler 的 
 Visual Basic / Windows / Class Library 项目，右键项目名称，选择属性，在
Signing 选项卡上，在“sign the assembly”打勾，并命名 

-----------------------------------------------------------------------------

B. 创建一个com 组件

shell 扩展处理程序在com 中都是 dll， 所以 创建一个com 组件是非常合适的。
你要定义一个有 ComVisible（true）public 的类，根据向导指定它的 CLSID，并指定
继承哪个com接口，例如：
  <ClassInterface(ClassInterfaceType.None), _
    Guid("44BDEF95-C00F-493E-A55B-17937DB1F04E"), ComVisible(True)> _
    Public Class SimpleObject
        Implements ISimpleObject
        ... ' Implements the interface
    End Class
	
-----------------------------------------------------------------------------

C.添加一个类，创建执行信息提示处理事件和注册事件 


-----------

创建执行信息处理事件：

FileInfotipExt.vb文件定义了一个信息提示处理程序。它必须继承IQueryInfo 接口，才可以为
tooltip创建文本，还必须继承PersistFile 接口去初始化程序


    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), _
    Guid("00021500-0000-0000-c000-000000000046")> _
    Friend Interface IQueryInfo
        
        Function GetInfoTip(ByVal dwFlags As UInt32) _
            As <MarshalAs(UnmanagedType.LPWStr)> String

        Function GetInfoFlags() As Integer
    End Interface

 IPersistFile 接口 是可以用的，它在 
System.Runtime.InteropServices.ComTypes 命名空间中. 详细请点击下面链接
http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.comtypes.ipersistfile.aspx
	
    <ClassInterface(ClassInterfaceType.None), _
    Guid("44BDEF95-C00F-493E-A55B-17937DB1F04E"), ComVisible(True)> _
    Public Class FileInfotipExt
        Implements IPersistFile, IQueryInfo

        #Region "IPersistFile Members"

        Public Sub GetClassID(<Out()> ByRef pClassID As Guid) _
            Implements IPersistFile.GetClassID
            ...
        End Sub

        Public Sub GetCurFile(<Out()> ByRef ppszFileName As String) _
            Implements IPersistFile.GetCurFile
            ...
        End Sub

        Public Function IsDirty() As Integer _
            Implements IPersistFile.IsDirty
            ...
        End Function

        Public Sub Load(ByVal pszFileName As String, ByVal dwMode As Integer) _
            Implements IPersistFile.Load
            ...
        End Sub

        Public Sub Save(ByVal pszFileName As String, ByVal fRemember As Boolean) _
            Implements IPersistFile.Save
            ...
        End Sub

        Public Sub SaveCompleted(ByVal pszFileName As String) _
            Implements IPersistFile.SaveCompleted
            ...
        End Sub

    #End Region

    #Region "IQueryInfo Members"

        Public Function GetInfoTip(ByVal dwFlags As UInt32) As String _
            Implements IQueryInfo.GetInfoTip
            ...
        End Function

        Public Function GetInfoFlags() As Integer _
            Implements IQueryInfo.GetInfoFlags
            ...
        End Function

    #End Region

    End Class

	当你创建了你的处理程序，使用"Create GUID"工具创建shell 扩展程序，
	你就必须为它创建一个新的CLSID，并且指定guid属性值


    ...
    Guid("44BDEF95-C00F-493E-A55B-17937DB1F04E"), ComVisible(True)> _
    Public Class FileInfotipExt


  1. 执行 IPersistFile
   当执行IPersistFile时，shell查询它的扩展方法，并传递被鼠标指定的文件名字给它的Load方法
  
  IPersistFile.Load 打开指定的文件，并初始化需要的数据。
  在本代码中，我们保存文件的绝对路径 
  

    Public Sub Load(ByVal pszFileName As String, ByVal dwMode As Integer) _
        Implements IPersistFile.Load
        
        Me.selectedFile = pszFileName
    End Sub

	IPersistFile 方法我们没用到，所以我可以不写代码或者作为NotImplementedException扔出去
  2. 执行 IQueryInfo

  在IPersistFile 执行之后，shell 查询就执行IQueryInfo 接口和 GetInfoTip 方法
  GetInfoTip 返回一个包含提示信息的字符串
  
  在这个代码中，提示信息 包含 文件的路径和 文件代码行数


    
    Dim lineNum As Integer = 0
    Using reader As StreamReader = File.OpenText(Me.selectedFile)
        Do While (Not reader.ReadLine Is Nothing)
            lineNum += 1
        Loop
    End Using

    Return "File: " & Me.selectedFile & Environment.NewLine & _
        "Lines: " & lineNum.ToString & Environment.NewLine & _
        "- Infotip displayed by VBShellExtInfotipHandler"

		IQueryInfo.GetInfoFlags 我们没用到，作为NotImplementedException扔出去 

-----------

在一个类文件中存放注册处理信息：

信息处理程序可以存储在一个类文件里。本信息处理程序集是用下面的默认的注册码作为ＣＬＳＩＤ
注册的。

    HKEY_CLASSES_ROOT\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}

注册信息提示处理程序是通过FileInfotipExt方法执行的。该方法添加了一个ComRegisterFunction
属性，允许用户扩展方法，而不是基于ｃｏｍ　组件基类。
在注册时通过调用　ShellExtLib.vb　中的ShellExtReg.RegisterShellExtInfotipHandler　方法
去处理某一个文件。如果文件是以“．”开头的，那么该方法就试着读取它的包含ＩＤ（文件类型相关）
HKCR\<File Type>的默认值。如果默认值不为空，就用ＩＤ作为文件类型来处理。

　例如：　这段代码是处理　“．ｃｓ”开始的文件，HKCR\.cs的默认值是'VisualStudio.cs.10.0'
　所以我们就用HKCR\VisualStudio.cs.10.0　代替HKCR\.cs。
　以下的注册码和默认值就被添加到同一个事件处理程序中。
    HKCR
    {
        NoRemove .cs = s 'VisualStudio.cs.10.0'
        NoRemove VisualStudio.cs.10.0
            NoRemove shellex
            {
        {
                {00021500-0000-0000-C000-000000000046} = 
                    s '{44BDEF95-C00F-493E-A55B-17937DB1F04E}'
            }
        }
    }
	删除注册时执行　FileInfotipExt　的Unregister方法，和注册的方法类似，
	同样是添加了一个属性ComUnregisterFunction，允许用户在删除注册的时候扩展代码
	本程序删除了一下注册吗：
 　
HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}.


/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/cc144105.aspx
http://msdn.microsoft.com/en-us/library/bb761359.aspx
http://www.codeproject.com/KB/shell/ShellExtGuide3.aspx


/////////////////////////////////////////////////////////////////////////////