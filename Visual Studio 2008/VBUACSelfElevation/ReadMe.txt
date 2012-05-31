=============================================================================
          应用程序：VBUACSelfElevation 项目概述
=============================================================================

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//

摘要：

用户账户控制 （UAC）是Windows Vista及后续操作系统中的一个新安全组件。当UAC被
完全开启时，管理员的交互操作通常运行在普通用户权限下。这个示例演示了如何去检
测当前进程的权限等级，和如何通过许可验证对话框来确认并自我提升此线程的权限等
级。
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//

先决条件：

你必须在Windows Vista及后续操作系统中运行此示例程序。
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//

演示：

以下步骤演示了此UAC示例程序。

步骤1、在使用Visual Studio2008成功生成示例项目后， 你将得到一个应用程序：
CSUACSelfElevation.exe


步骤2、在一个UAC完全开启的Windows Vista或Windows 7系统中，使用受保护的管理员账
户执行此程序。此程序会显示一个对话框包含以下内容：

  用户是否是系统管理员：  是
  是否以管理员身份运行：  否
  进程权限是否被提升：    否
  完整性级别：            中
  
在“自我提升权限”按钮上有一个UAC盾状图标。

步骤3、点击“自我提升权限”按钮， 你将会看到一个许可验证对话框

  
  用户账户控制
  ---------------------------------- 
  您想允许来自未知发布者的以下程序对此计算机进行更改吗？
  

步骤4、点击“是”允许提升权限。之前的程序会被启动并显示以下内容

  用户是否是系统管理员：  是
  是否以管理员身份运行：  是
  进程权限是否被提升：    是
  完整性级别：            高

此时对话框中的“自我提升权限”按钮没有了之前的UAC盾状图标。这是由于此应用程序
以一个已提升的管理员运行。成功进行权限提升。如果你再次点击“自我提升权限”按钮，
此程序会告知你它已经作为管理员身份运行。

步骤5、 点击[X]关闭此应用程序。

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//
创建过程：

步骤1、创建一个新的Visual Basic Windows Forms项目，并命名为CSUACSelfElevation.

步骤2、在主窗体上添加以下控件

  类型: Button
  ID: btnElevate
  Caption: "自我提升权限"
  
  类型: Label
  ID: lbInAdminGroup
  用法：即使在还没有为当前用户提升权限的前提下，此控件显示拥有此进程的主访问令
  牌的用户是否是本地管理员组的成员。
  
  类型: Label
  ID: lbIsRunAsAdmin
  用法：此控件显示此程序以管理员身份运行。
  
  类型: Label
  ID: lbIsElevated
  用法：此控件显示当前进程的权限是否已经被提升。令牌提升只有在Windows Vista
  及后续版本的Windows中才被支持。此控件在Windows Vista之前版的Windows中显示
  显示N/A。
  
  类型: Label
  ID: lbIntegrityLevel
  用法：此控件显示当前进程的完整性级别。完整性级别只有在Windows Vista及后续版
  本的Windows中才被支持。此控件在Windows Vista之前版的Windows中显示显示N/A。
  

步骤3、在程序初始化主对话框时检查并显示当前进程的“以管理员身份运行”的状态，
权限提升信息及完整性级别。

创建以下四个辅助函数：

    ''' <summary>
    ''' 即使在还没有为当前用户提升权限的前提下，此函数检测拥有此进程的主访
    ''' 问令牌的用户是否是本地管理员组的成员。
    ''' </summary>
    ''' <returns>
    ''' 如果拥有主访问令牌的用户是管理员组成员则返回TRUE，反之则返回FALSE。
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' 如果任何原生的Windows API函数出错，此函数会抛出一个包含最后错误代码的Win32Exception。
    ''' </exception>
    Friend Function IsUserInAdminGroup() As Boolean

    ''' <summary>
    ''' 此函数检测当前进程是否以管理员身份运行。换而言之，此进程要求用户是
    ''' 拥有主访问令牌的用户是管理员组成员并且已经执行了权限提升。
    ''' </summary>
    ''' <returns>
    ''' 如果拥有主访问令牌的用户是管理员组成员且已经执行了权限提升则返回
    ''' TRUE，反之则返回FALSE。
    ''' </returns>
    Friend Function IsRunAsAdmin() As Boolean

    ''' <summary>
    ''' 此函数获取当前进程的权限提升信息。它由此进程是否进行了权限提升所
	''' 决定。令牌权限提升只有在Windows Vista及后续版本的Windows中有效。所以在
	''' Windows Vista之前的版本中执行IsProcessElevated， 它会抛出一个C++异常。
	''' 此函数并不适用于检测是否此进程以管理员身份运行。
    ''' </summary>
    ''' <returns>
    ''' 如果此进程的权限已被提升，返回TRUE，反之则返回FALSE。
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' 如果任何原生的Windows API函数出错，此函数会抛出一个包含最后错误代码的Win32Exception。
    ''' </exception>
    ''' <remarks>
    ''' TOKEN_INFORMATION_CLASS提供了TokenElevationType以便对当前进程的提升
    ''' 类型（TokenElevationTypeDefault / TokenElevationTypeLimited /
    ''' TokenElevationTypeFull）进行检测。 它和TokenElevation的不同之处在于：当UAC
    ''' 关闭时，即使当前进程已经被提升(完整性级别 == 高)，权限提升类型总是返回
    ''' TokenElevationTypeDefault。换而言之，以此来确认当前线程的提升类型是不安全的。
    ''' 相对的，我们应该使用TokenElevation。
    ''' </remarks>
    Friend Function IsProcessElevated() As Boolean

    ''' <summary>
    ''' 此函数获取当前线程的完整性级别。完整性级别只有在Windows Vista及后
    ''' 续版本的Windows中有效。所以在Windows Vista之前的版本中执行GetProcessIntegrityLevel， 
    ''' 它会抛出一个C++异常。
    ''' </summary>
    ''' <returns>
    ''' 返回当前进程的完整性级别。它可能是以下某一个值。
	'''
	'''     SECURITY_MANDATORY_UNTRUSTED_RID (SID: S-1-16-0x0)
	'''     表示不被信任的级别。它被用于一个匿名组成员起动的进程。这时大多数
	'''     写入操作被禁止。
	'''
	'''     SECURITY_MANDATORY_LOW_RID (SID: S-1-16-0x1000)
	'''     表示低完整性级别。它被用于保护模式下的IE浏览器。这时大多数系统中对
	'''     象（包括文件及注册表键值）的写入操作被禁止。
	'''
	'''     SECURITY_MANDATORY_MEDIUM_RID (SID: S-1-16-0x2000)
	'''     表示中完整性级别。它被用于在UAC开启时启动普通应用程序。
	'''
	'''
	'''     SECURITY_MANDATORY_HIGH_RID (SID: S-1-16-0x3000)
	'''     表示高完整性级别。它被用于用户通过UAC提升权限启动一个管理员应用程序。
	'''     或则当UAC关闭时，管理员用户启动一个普通程序。
	'''
	'''
	'''     SECURITY_MANDATORY_SYSTEM_RID (SID: S-1-16-0x4000)
	'''     表示系统完整性级别。它被用于服务或则其他系统级别的应用程序（比如
	'''     Wininit, Winlogon, Smss，等等）
    ''' 
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' 如果任何原生的Windows API函数出错，此函数会抛出一个包含最后错误代码的Win32Exception。
    ''' </exception>
    Friend Function GetProcessIntegrityLevel() As Integer

一些方法需要P/Invoke原生的Windows API。 NativeMethod.vb定义了这些P/Invoke注释

在主窗体的构造函数中，检查并显示“以管理员身份运行”的状态，权限提升
的信息，以及当前进程的完整性级别。

    '' 获取并显示即使在还没有为当前用户提升权限的前提下，拥有此进程的主访
    '' 问令牌的用户是否是本地管理员组的成员。(IsUserInAdminGroup)。
    Try
        Dim fInAdminGroup As Boolean = Me.IsUserInAdminGroup
        Me.lbInAdminGroup.Text = IIf(fInAdminGroup, "是", "否")
    Catch ex As Exception
        Me.lbInAdminGroup.Text = "N/A"
        MessageBox.Show(ex.Message, "在IsUserInAdminGroup中发生一个错误", _
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Try

    ' 获取并显示是否此进程以管理员身份运行。（IsRunAsAdmin）。

    Try
        Dim fIsRunAsAdmin As Boolean = Me.IsRunAsAdmin
        Me.lbIsRunAsAdmin.Text = IIf(fIsRunAsAdmin, "是", "否")
    Catch ex As Exception
        Me.lbIsRunAsAdmin.Text = "N/A"
        MessageBox.Show(ex.Message, "在IsRunAsAdmin中发生一个错误", _
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Try
    
    '' 获取并显示进程权限提升信息(IsProcessElevated)和完整性级别（GetProcessIntegrityLevel）
    '' 注意：这些信息在Windows Vista之前的Windows中不存在。    
    If (Environment.OSVersion.Version.Major >= 6) Then
        ' Running Windows Vista or later (major version >= 6). 
        ' 运行于Windows Vista或后续版本（主版本号 >= 6）。


        Try
            ' 获取并显示进程权限提升信息
            Dim fIsElevated As Boolean = Me.IsProcessElevated
            Me.lbIsElevated.Text = IIf(fIsElevated, "是", "否")

            ' 如果进程尚未被提升，更新“自我提升权限”按钮以在UI中显示UAC盾形
            ' 图标。
            Me.btnElevate.FlatStyle = FlatStyle.System
            NativeMethod.SendMessage(Me.btnElevate.Handle, NativeMethod.BCM_SETSHIELD, _
                                     0, IIf(fIsElevated, IntPtr.Zero, New IntPtr(1)))
        Catch ex As Exception
            Me.lbIsElevated.Text = "N/A"
            MessageBox.Show(ex.Message, "在IsProcessElevated发生一个错误", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Try
            ' 获取并显示进程的完整性级别
            Dim IL As Integer = Me.GetProcessIntegrityLevel
            Select Case IL
                Case NativeMethod.SECURITY_MANDATORY_UNTRUSTED_RID
                        Me.lbIntegrityLevel.Text = "不信任"
                    Case NativeMethod.SECURITY_MANDATORY_LOW_RID
                        Me.lbIntegrityLevel.Text = "低"
                    Case NativeMethod.SECURITY_MANDATORY_MEDIUM_RID
                        Me.lbIntegrityLevel.Text = "中"
                    Case NativeMethod.SECURITY_MANDATORY_HIGH_RID
                        Me.lbIntegrityLevel.Text = "高"
                    Case NativeMethod.SECURITY_MANDATORY_SYSTEM_RID
                        Me.lbIntegrityLevel.Text = "系统"
                    Case Else
                        Me.lbIntegrityLevel.Text = "未知"
            End Select
        Catch ex As Exception
            Me.lbIntegrityLevel.Text = "N/A"
            MessageBox.Show(ex.Message, "在GetProcessIntegrityLevel中发生一个错误!", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand)
        End Try

    Else
        Me.lbIsElevated.Text = "N/A"
        Me.lbIntegrityLevel.Text = "N/A"
    End If


步骤4、响应“自我提升权限”按钮的单击事件。当用户单击按钮，如果
此进程不是以管理员身份运行，我们调用ProcessStartInfo.UseShellExecute = true和
ProcessStartInfo.Verb = "runas" 来重启本应用程序并提升权限。

    Private Sub btnElevate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnElevate.Click
        ' Elevate the process if it is not run as administrator.
        ' 检测当前进程的“以管理员身份运行”的状态
        If (Not Me.IsRunAsAdmin) Then
            ' Launch itself as administrator
            ' 以管理员身份重启本程序
            Dim proc As New ProcessStartInfo
            proc.UseShellExecute = True
            proc.WorkingDirectory = Environment.CurrentDirectory
            proc.FileName = Application.ExecutablePath
            proc.Verb = "runas"

            Try
                Process.Start(proc)
            Catch
                ' 用户拒绝提升
                ' 什么都不做并直接返回......                
                Return
            End Try

            Application.Exit()  ' 退出
        Else
            MessageBox.Show("此进程已以管理员身份运行", "UAC")
        End If
    End Sub


步骤5、当进程启动时自动提升其权限。

如果你的应用程序总是要求管理员权限，比如在安装中，每当你的程序要求时，操作系统
会自动提示用户要求权限提升。

如果一个特殊的资源（RT_MANIFEST）被嵌入在可执行程序中，系统会查询并处理
<trustInfo>中的内容。这个示例演示了在清单文件（manifest）中的相关部分。
    <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
       <security>
          <requestedPrivileges>
             <requestedExecutionLevel
                level="requireAdministrator"
             />
          </requestedPrivileges>
       </security>
    </trustInfo>

对于等级属性来说，我们有三个可选项。

  a) requireAdministrator 
  此应用程序必须以管理员权限启动，否则它将不能执行。

  b) highestAvailable 
  此应用程序以目前最高权限运行。如果用户登陆为一个管理员账户，这将会出现
  权限提升对话框。如果用户是一个普通用户，此程序以普通用户权限运行（没有
  权限提升对话框）

  c) asInvoker 
  使用调用此应用程序的进程相同的权限来启动这个程序。


如果需要在Visual Basic Windows Forms项目中配置提升等级， 我们可以打开项目属性，转
到应用程序页面，选择“查看UAC设置”。此操作创建了一个app.manifest文件并且设置该项
目的嵌入式清单。你可以从解决方案管理器中的属性文件夹中打开此“app.manifest”文件。
默认情况下此文件包含以下内容。

<?xml version="1.0" encoding="utf-8"?>
<asmv1:assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1" 
xmlns:asmv1="urn:schemas-microsoft-com:asm.v1" xmlns:asmv2="urn:schemas-microsoft-com:
asm.v2" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <assemblyIdentity version="1.0.0.0" name="MyApplication.app"/>
  <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
    <security>
      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
        <!-- UAC 清单选项
            如果希望更改 Windows 用户帐户控制级别，请用以下节点之一替换 
            requestedExecutionLevel 节点。

        <requestedExecutionLevel  level="asInvoker" uiAccess="false" />
        <requestedExecutionLevel  level="requireAdministrator" uiAccess="false" />
        <requestedExecutionLevel  level="highestAvailable" uiAccess="false" />

            如果您希望利用文件和注册表虚拟化提供
            向后兼容性，请删除 requestedExecutionLevel 节点。
        -->
        <requestedExecutionLevel level="asInvoker" uiAccess="false" />
      </requestedPrivileges>
    </security>
  </trustInfo>
</asmv1:assembly>

这里我们关注行：
    <requestedExecutionLevel level="asInvoker" uiAccess="false" />

你可以把它修改成
    <requestedExecutionLevel level="requireAdministrator" uiAccess="false" />

以要求应用程序总是以管理员权限启动。

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//
参考资料：

MSDN: User Account Control
http://msdn.microsoft.com/en-us/library/aa511445.aspx

MSDN: Windows Vista Application Development Requirements for User Account 
Control Compatibility
http://msdn.microsoft.com/en-us/library/bb530410.aspx


'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''//