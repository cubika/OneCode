'*************************** 模块头 ******************************'
' 模块名:  TabbedWebBrowserContainer.vb
' 项目名:	    VBTabbedWebBrowser
' 版权 (c) Microsoft Corporation.
' 
' 这是一个UserControl并且包含一个 System.Windows.Forms.TabControl。
' 在用户界面，TabControl 不支持创建/关闭一个标签。
' 所以UserControl 提供一个方法去创建/关闭一个标签。 
'  
' 向Tabcontrol中添加一个新的TabPage时，WebBrowserTabPage类型继承System.Windows.Forms.TabPage，
' 并且UerControl会显示出System.Windows.Forms.WebBrowser类中的 向前，后退和刷新方法。
' 在Internet Explorer中NewWindow 事件被触发时，它将会创建一个标签并打开标签。
' 
' 在WebBorwser中“在新建选项卡中打开”默认状态下是被禁用的。
' 你可以向键值 HKCU\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING中，
' 添加一个*.exe=1(*表示该进程的名字)
' 
' 当应用程序重新启动后，这个菜单才会生效。
' 参见： http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************'

Imports Microsoft.Win32
Imports System.Security.Permissions

Partial Public Class TabbedWebBrowserContainer
    Inherits UserControl

    ''' <summary>
    ''' 一个静态属性来获取或设置WebBrowser中的“在新建选项卡中打开”的下拉菜单是否启用。.
    ''' </summary>
    Public Shared Property IsTabEnabled() As Boolean
        <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
        Get
            Using key As RegistryKey =
                Registry.CurrentUser.OpenSubKey(
                    "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING")
                If (key IsNot Nothing) Then
                    Dim processName As String = Process.GetCurrentProcess().ProcessName & ".exe"
                    Dim keyValue As Integer = CInt(Fix(key.GetValue(processName, 0)))
                    Return keyValue = 1
                Else
                    Return False
                End If
            End Using
        End Get
        <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
        Set(ByVal value As Boolean)
            Using key As RegistryKey =
                Registry.CurrentUser.CreateSubKey(
                    "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING")
                Dim processName As String = Process.GetCurrentProcess().ProcessName & ".exe"
                Dim keyValue As Integer = CInt(Fix(key.GetValue(processName, 0)))

                Dim isEnabled As Boolean = keyValue = 1
                If isEnabled <> value Then
                    key.SetValue(processName, If(value, 1, 0))
                End If
            End Using
        End Set
    End Property


    ''' <summary>
    ''' 标签控件的选择标签.
    ''' </summary>
    Public ReadOnly Property ActiveTab() As WebBrowserTabPage
        Get
            If tabControl.SelectedTab IsNot Nothing Then
                Return TryCast(tabControl.SelectedTab, WebBrowserTabPage)
            Else
                Return Nothing
            End If
        End Get
    End Property


    ''' <summary>
    ''' 此控件至少含有一个标签.
    ''' </summary>
    Public ReadOnly Property CanCloseActivePage() As Boolean
        Get
            Return tabControl.TabPages.Count > 1
        End Get
    End Property


    Public Sub New()
        InitializeComponent()
    End Sub


    ''' <summary>
    ''' 创建一个新的标签，当空间载入时，导航栏设置为“about:blank”。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)
        NewTab("about:blank")
    End Sub


    ''' <summary>
    ''' 操作ACTIVETAB下的WEB浏览器控制这个URL.
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub Navigate(ByVal url As String)
        If Me.ActiveTab IsNot Nothing Then
            Me.ActiveTab.WebBrowser.Navigate(url)
        End If
    End Sub


    ''' <summary>
    ''' 创建一个新的 WebBrowserTabPage 实例，把它添加到标签控件中，然后签署它的NewWindow事件。
    ''' </summary>
    ''' <returns></returns>
    Private Function CreateTabPage() As WebBrowserTabPage
        Dim tab As New WebBrowserTabPage()
        AddHandler tab.NewWindow, AddressOf tab_NewWindow
        Me.tabControl.TabPages.Add(tab)
        Me.tabControl.SelectedTab = tab
        Return tab
    End Function


    ''' <summary>
    ''' 创建一个WebBrowserTabPage，然后进入到URL中。l.
    ''' </summary>
    ''' <param name="url"></param>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub NewTab(ByVal url As String)
        CreateTabPage()
        Navigate(url)
    End Sub


    ''' <summary>
    ''' 关闭激活的标签.
    ''' </summary>
    Public Sub CloseActiveTab()
        ' 此控件至少含有一个标签.
        If CanCloseActivePage Then
            Dim tabToClose = Me.ActiveTab
            Me.tabControl.TabPages.Remove(tabToClose)
        End If
    End Sub


    ''' <summary>
    ''' 处理WebBrowserTabPage的NewWindow 事件。
    ''' 当WebBrowser中 NewWindow 事件被触发时，在nternet Explorer中药创建一个标签并且打开主页.  
    ''' </summary>
    Private Sub tab_NewWindow(ByVal sender As Object, ByVal e As WebBrowserNewWindowEventArgs)
        If TabbedWebBrowserContainer.IsTabEnabled Then
            NewTab(e.Url)
            e.Cancel = True
        End If
    End Sub


    ''' <summary>
    ''' 在ActiveTab中显示WebBrowser 控件的后退方法
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub GoBack()
        Me.ActiveTab.WebBrowser.GoBack()
    End Sub


    ''' <summary>
    ''' 在ActiveTab中显示WebBrowser 控件的向前方法。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub GoForward()
        Me.ActiveTab.WebBrowser.GoForward()
    End Sub


    ''' <summary>
    ''' 在ActiveTab中显示WebBrowser 控件的刷新方法。
    ''' </summary>
    <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
    Public Sub RefreshWebBrowser()
        Me.ActiveTab.WebBrowser.Refresh()
    End Sub

    
End Class
