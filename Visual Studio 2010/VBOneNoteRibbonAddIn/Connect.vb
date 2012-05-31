'***************************** 模块头 *************************************\
' 模块名:  Connect.vb
' 项目名:  VBOneNoteRibbonAddIn
' 版权 (c) Microsoft Corporation.
'
' 寄宿发生于外接程序上的事件通知，例如,当它们被加载、卸载、更新等等.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports Extensibility
Imports System.Runtime.InteropServices
Imports Microsoft.Office.Core
Imports System.IO
Imports VBOneNoteRibbonAddIn.My
Imports Microsoft.Office.Interop.OneNote
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices.ComTypes
Imports VBOneNoteRibbonAddIn
Imports OneNote = Microsoft.Office.Interop.OneNote


#Region " 插件安装和信息设置提示."
' 在运行时，外接程序向导准备注册表加载.
' 在以后的时间，如果外接程序由于某些原因,变为不可用,例如:
'   1) 您移动此项目到一台计算机上,该计算机并不是最初创建该项目的机器.
'   2) 当显示一条消息询问您是否要删除该外接程序,您选择了"是".
'   3) 注册表损坏.
' 您将需要通过生成 VBOneNoteRibbonAddInSetup 项目，重新注册,在“解决方案资源管理器”
' 中右击该项目,选择安装.
#End Region

<GuidAttribute("89BE33A8-624A-47AE-B7BA-E8B9117A5BC4"),
 ProgIdAttribute("VBOneNoteRibbonAddIn.Connect")> _
Public Class Connect

    Implements Extensibility.IDTExtensibility2
    Implements IRibbonExtensibility

    Private applicationObject As Object
    Private addInInstance As Object

    Public Sub OnBeginShutdown(ByRef custom As System.Array) _
        Implements Extensibility.IDTExtensibility2.OnBeginShutdown

        MessageBox.Show("VBOneNoteRibbonAddIn OnBeginShutdown")
        If Me.applicationObject IsNot Nothing Then
            Me.applicationObject = Nothing
        End If
    End Sub

    Public Sub OnAddInsUpdate(ByRef custom As System.Array) _
        Implements Extensibility.IDTExtensibility2.OnAddInsUpdate

        MessageBox.Show("VBOneNoteRibbonAddIn OnAddInsUpdate")
    End Sub

    Public Sub OnStartupComplete(ByRef custom As System.Array) _
        Implements Extensibility.IDTExtensibility2.OnStartupComplete

        MessageBox.Show("VBOneNoteRibbonAddIn OnStartupComplete")
    End Sub

    Public Sub OnDisconnection(ByVal RemoveMode As Extensibility.ext_DisconnectMode, _
                               ByRef custom As System.Array) _
                           Implements Extensibility.IDTExtensibility2.OnDisconnection

        MessageBox.Show("VBOneNoteRibbonAddIn OnDisconnection")
        Me.applicationObject = Nothing
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub

    Public Sub OnConnection(ByVal application As Object, _
                            ByVal connectMode As Extensibility.ext_ConnectMode, _
                            ByVal addInInst As Object, _
                            ByRef custom As System.Array) _
                        Implements Extensibility.IDTExtensibility2.OnConnection
        MessageBox.Show("VBOneNoteRibbonAddIn OnConnection")
        applicationObject = application
        addInInstance = addInInst
    End Sub

    ''' <summary>
    '''     从自定义功能区用户界面的 XML 自定义文件中,加载的 XML 标记 
    ''' </summary>
    ''' <param name="RibbonID">RibbonX 用户界面的 ID </param>
    ''' <returns>string</returns>
    Public Function GetCustomUI(ByVal RibbonID As String) As String _
        Implements IRibbonExtensibility.GetCustomUI

        Return Resources.customUI
    End Function

    ''' <summary>
    '''    实现方法 customUI.xml 中的 OnGetImage
    ''' </summary>
    ''' <param name="imageName">customUI.xml 中的图像名</param>
    ''' <returns>装有图像数据的内存流</returns>
    Public Function OnGetImage(ByVal imageName As String) As IStream
        Dim stream As New MemoryStream()

        If imageName = "showform.png" Then
            Resources.showform.Save(stream, ImageFormat.Png)
        End If

        Return New ReadOnlyIStreamWrapper(stream)
    End Function

    ''' <summary>
    '''     用于显示窗体的方法
    ''' </summary>
    ''' <param name="control">表示传递给每个功能区用户界面 (UI) 控件的	
    ''' 回调过程的对象.</param>
    Public Sub ShowForm(ByVal control As IRibbonControl)
        Dim context As OneNote.Window = TryCast(control.Context, OneNote.Window)
        Dim owner As New CWin32WindowWrapper(CType(context.WindowHandle, IntPtr))
        Dim form As New TestForm(TryCast(applicationObject, OneNote.Application))
        form.ShowDialog(owner)

        form.Dispose()
        form = Nothing
        context = Nothing
        owner = Nothing
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()
    End Sub

End Class
