'********************************* 模块头 *********************************\
' 模块名称:        ProjectFileItem.vb
' 项目 :           VBVSXSaveProject
' 版权所有（c）微软公司
'
' 这个包主要介绍了如何将菜单增加到集成开发环境中区，它提供以下功能
' 1 将整个项目复制到一个新的位置。
' 2 在当前的集成开发环境中打开新的项目。
' 
'
' The source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***********************************************************************************/

Imports System.ComponentModel.Design
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports EnvDTE
Imports Microsoft.VisualStudio.Shell

''' <summary>
''' 继承于这个包的类被程序集所暴漏
'''
''' 一个类被认为是Visual Studio的有效的包的最低要求是实施了IVsPackage接口和在shell中注册。
''' 本软件包使用内部的管理软件包框架（MPF）定义做助手类：它来源于从IVsPackage接口所提供的包类。
''' 使用在框架中定义的注册属性来定义其本身和她的shell控件。

''' </summary>
' 这个属性告诉PkgDef创建工具（CreatePkgDef.exe），这个类是一个包。
' 这个属性是用来显示注册所需的信息，这个显示在Visual Studio帮助/关于对话框包中。
' 此属性是需要让shell知道这个包提供了哪一些菜单。
<PackageRegistration(UseManagedResourcesOnly:=True),
 InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400),
 ProvideMenuResource("Menus.ctmenu", 1),
 Guid(GuidList.guidVBVSXSaveProjectPkgString)>
Public NotInheritable Class CSVSXSaveProjectPackage
    Inherits Package

    ''' <summary>
    ''' 指定在此应用中的DTE对象。
    ''' </summary>
    Private dte As DTE
    Friend ReadOnly Property DTEObject() As DTE
        Get
            If dte Is Nothing Then
                dte = TryCast(Me.GetService(GetType(DTE)), DTE)
            End If
            Return dte
        End Get
    End Property

    ''' <summary>
    '''' 为包提供默认的构造器。
    '''' 在此方法中，你可以在不需要任何Visual Studio服务的情况下，放置任何的初始化代码。
    '''  因为在时，包的对象被创建，但还没有在Visual Studio环境中获得地址。所有初始化的地方位于Initialize方法。
    '''' </summary>
    Public Sub New()
        Trace.WriteLine(String.Format(
                        CultureInfo.CurrentCulture,
                        "Entering constructor for: {0}",
                        Me.ToString()))
    End Sub

    '///////////////////////////////////////////////////////////////////////////////
    ' 重写包的实现
#Region "包成员"

    ''' <summary>
    ''' 包的初始化，这个方法在包被设置好后调用, 所以这就是一个你可以放置所有的依赖、VisualStudio提供的服务的初始化代码的地方。
    ''' </summary>
    Protected Overrides Sub Initialize()
        Trace.WriteLine(String.Format(CultureInfo.CurrentCulture,
                                      "Entering Initialize() of: {0}",
                                      Me.ToString()))
        MyBase.Initialize()

        ' 为我们的菜单添加命令处理程序（命令中必须存在的.vsct文件）。
        Dim mcs As OleMenuCommandService =
            TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
        If Nothing IsNot mcs Then
            ' 创建菜单项命令。
            Dim menuCommandID As New CommandID(GuidList.guidVBVSXSaveProjectCmdSet,
                                               CInt(Fix(PkgCmdIDList.cmdidVBVSXSaveProjectCommandID)))
            Dim menuItem As New MenuCommand(AddressOf MenuItemCallback, menuCommandID)
            mcs.AddCommand(menuItem)

            ' 创建VSXSaveProjectCmdSet的菜单项命令。
            Dim vbVSXSaveProjectContextCommandID As New CommandID(GuidList.guidVBVSXSaveProjectContextCmdSet,
                                                                  CInt(Fix(PkgCmdIDList.cmdidVBVSXSaveProjectContextCommandID)))
            Dim vbVSXSaveProjectMenuContextCommand As New OleMenuCommand(AddressOf MenuItemCallback, vbVSXSaveProjectContextCommandID)
            mcs.AddCommand(vbVSXSaveProjectMenuContextCommand)
        End If
    End Sub
#End Region

    '''' <summary>
    '''' 此功能是用于执行命令菜单项被点击时的回调。请参阅Initialize方法，
    '''' 看到菜单项是如何关联到OleMenuCommandService服务和MenuCommand类使用该功能。
    '''' </summary>

    Private Sub MenuItemCallback(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' 获取当前活动项目的对象。
            Dim proj = Me.GetActiveProject()

            If proj IsNot Nothing Then
                ' 获取项目的信息
                Dim vsProj = New Files.VSProject(proj)

                ' 获取在项目中包含的文件
                Dim includedFiles = vsProj.GetIncludedFiles()

                ' 获取在项目文件夹下面的文件
                Dim projfolderFiles =
                    Files.ProjectFolder.GetFilesInProjectFolder(proj.FullName)

                ' 新增的其他文件，如项目文件夹下的文件，使用户可以选择他们。
                Dim totalItems = New List(Of Files.ProjectFileItem)(includedFiles)
                For Each fileItem As Files.ProjectFileItem In projfolderFiles
                    If includedFiles.Where(Function(f) f.FullName.Equals(fileItem.FullName,
                                               StringComparison.OrdinalIgnoreCase)).Count() = 0 Then
                        totalItems.Add(fileItem)
                    End If
                Next fileItem

                ' 显示用户界面。
                Using dialog As New SaveProjectDialog()
                    ' 显示所有文件
                    dialog.FilesItems = totalItems
                    dialog.OriginalFolderPath = vsProj.ProjectFolder.FullName

                    Dim result = dialog.ShowDialog()

                    ' 新建新的项目
                    If result = DialogResult.OK AndAlso dialog.OpenNewProject Then
                        Dim newProjectPath As String = String.Format(
                            "{0}\{1}",
                            dialog.NewFolderPath,
                            proj.FullName.Substring(vsProj.ProjectFolder.FullName.Length))

                        Dim cmd As String = String.Format(
                            "File.OpenProject ""{0}""", newProjectPath)

                        Me.DTEObject.ExecuteCommand(cmd)
                    End If
                End Using
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 获取活动项目的对象。
    ''' </summary>
    Friend Function GetActiveProject() As Project
        Dim activeProject As Project = Nothing

        ' 在解决方案资源管理器获取所有项目。
        Dim activeSolutionProjects As Array =
            TryCast(Me.DTEObject.ActiveSolutionProjects, Array)
        If activeSolutionProjects IsNot Nothing _
            AndAlso activeSolutionProjects.Length > 0 Then
            ' 获取活动项目。
            activeProject = TryCast(activeSolutionProjects.GetValue(0), Project)
        End If
        Return activeProject
    End Function
End Class