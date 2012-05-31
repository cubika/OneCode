'*************************** Module Header ******************************'
' 模块名称:  ImageListExplorerBar.vb
' 项目:	    VBIEExplorerBar
' Copyright (c) Microsoft Corporation.
' 
' ImageListExplorerBar类继承了类System.Windows.Forms.UserControl，并实现了
' IObjectWithSite、IDeskBand、IDockingWindow、IOleWindow和IInputObject接口。
' 
' 想要在IE中添加一个浏览器栏目，你需要做以下几步：
' 
' 1. 为ComVisible 类创建一个有效的GUID。 
' 
' 2. 实现IObjectWithSite、IDeskBand、IDockingWindow、IOleWindow和IInputObject接口。
'    
' 3. 将这个程序集注册到COM中。
' 
' 4.  创建一个新的密钥，并用你创建的Explorer Bar的类型的类别标识符 (CATID)作为这个密钥的名称
'    可能会是以下值中的一个： 
'    {00021494-0000-0000-C000-000000000046} 定义一个横向的浏览器栏。
'    {00021493-0000-0000-C000-000000000046} 定义一个垂直的浏览器栏。
'    
'    结果类似于：
'
'    HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021494-0000-0000-C000-000000000046}
'    或  
'    HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021493-0000-0000-C000-000000000046}
'    
' 5. 删除以下的注册表项，因为这些注册表项用于缓存 ExplorerBar 的枚举。
' 
'    HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
'    Component Categories\{00021493-0000-0000-C000-000000000046}\Enum
'    或
'    HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
'    Component Categories\{00021494-0000-0000-C000-000000000046}\Enum
'
'
' 6. 在注册表中设置浏览器栏的大小。
' 
'    HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer\Explorer Bars\<Your GUID>\BarSize
'
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Imports SHDocVw

<ComVisible(True), Guid("5802D092-1784-4908-8CDB-99B6842D353F")>
Partial Public Class ImageListExplorerBar
    Inherits UserControl
    Implements NativeMethods.IObjectWithSite, NativeMethods.IDeskBand, NativeMethods.IDockingWindow, NativeMethods.IOleWindow, NativeMethods.IInputObject

    ' 浏览器栏的标题。
    Private Const imageListExplorerBarTitle As String = "Image List Explorer Bar"

    ' 定义一个垂直的浏览器栏。 
    Private Const verticalExplorerBarCATID As String = "{00021493-0000-0000-C000-000000000046}"

    ' IInputObjectSite对象。
    ' VB注意：变量site重新命名，由于Visual Basic不允许类的成员具有相同的名称：
    Private site_Renamed As NativeMethods.IInputObjectSite

    ' 浏览器栏实例
    Private explorer As InternetExplorer

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "NativeMethods.IObjectWithSite"

    Public Sub SetSite(ByVal pUnkSite As Object) Implements NativeMethods.IObjectWithSite.SetSite

        ' 释放之前的COM对象。
        If Me.site_Renamed IsNot Nothing Then
            Marshal.ReleaseComObject(Me.site_Renamed)
        End If
        If Me.explorer IsNot Nothing Then
            Marshal.ReleaseComObject(Me.explorer)
            Me.explorer = Nothing
        End If

        ' pUnkSite是一个实现IOleWindowSite对象的指针。
        Me.site_Renamed = TryCast(pUnkSite, NativeMethods.IInputObjectSite)

        If Me.site_Renamed IsNot Nothing Then

            ' site实现了IServiceProvider接口并可以被用来获取InternetExplorer实例。
            Dim provider = TryCast(Me.site_Renamed, NativeMethods._IServiceProvider)
            Dim guid_Renamed As New Guid("{0002DF05-0000-0000-C000-000000000046}")
            Dim riid As New Guid("{00000000-0000-0000-C000-000000000046}")
            Try
                Dim webBrowser As Object
                provider.QueryService(guid_Renamed, riid, webBrowser)
                Me.explorer = TryCast(webBrowser, InternetExplorer)
            Catch e1 As COMException
            End Try
        End If

    End Sub


    Public Sub GetSite(ByRef riid As Guid, <Out()> ByRef ppvSite As Object) Implements NativeMethods.IObjectWithSite.GetSite
        ppvSite = Me.site_Renamed
    End Sub

#End Region

#Region "NativeMethods.IDeskBand"

    ''' <summary>
    ''' 获取一个band对象的信息。
    ''' </summary>
    Public Sub GetBandInfo(ByVal dwBandID As UInteger,
                           ByVal dwViewMode As UInteger,
                           ByRef pdbi As NativeMethods.DESKBANDINFO) _
                       Implements NativeMethods.IDeskBand.GetBandInfo
        pdbi.wszTitle = ImageListExplorerBar.imageListExplorerBarTitle
        pdbi.ptActual.X = MyBase.Size.Width
        pdbi.ptActual.Y = MyBase.Size.Height
        pdbi.ptMaxSize.X = -1
        pdbi.ptMaxSize.Y = -1
        pdbi.ptMinSize.X = -1
        pdbi.ptMinSize.Y = -1
        pdbi.ptIntegral.X = -1
        pdbi.ptIntegral.Y = -1
        pdbi.dwModeFlags = NativeMethods.DBIM.NORMAL Or NativeMethods.DBIM.VARIABLEHEIGHT
    End Sub

    Public Sub ShowDW(ByVal fShow As Boolean) _
        Implements NativeMethods.IDeskBand.ShowDW, NativeMethods.IDockingWindow.ShowDW
        If fShow Then
            Me.Show()
        Else
            Me.Hide()
        End If
    End Sub

    Public Sub CloseDW(ByVal dwReserved As UInteger) _
        Implements NativeMethods.IDeskBand.CloseDW, NativeMethods.IDockingWindow.CloseDW
        Me.Dispose(True)
    End Sub

    Public Sub ResizeBorderDW(ByVal prcBorder As IntPtr,
                              ByVal punkToolbarSite As Object,
                              ByVal fReserved As Boolean) _
                          Implements NativeMethods.IDeskBand.ResizeBorderDW, NativeMethods.IDockingWindow.ResizeBorderDW
    End Sub

    Public Sub GetWindow(<Out()> ByRef hwnd As IntPtr) _
        Implements NativeMethods.IDeskBand.GetWindow, NativeMethods.IDockingWindow.GetWindow, NativeMethods.IOleWindow.GetWindow
        hwnd = Me.Handle
    End Sub

    Public Sub ContextSensitiveHelp(ByVal fEnterMode As Boolean) _
        Implements NativeMethods.IDeskBand.ContextSensitiveHelp, NativeMethods.IDockingWindow.ContextSensitiveHelp, NativeMethods.IOleWindow.ContextSensitiveHelp
    End Sub

#End Region


#Region "NativeMethods.IInputObject"

    ''' <summary>
    ''' 用户界面激活或停用对象。
    ''' </summary>
    ''' <param name="fActivate">
    ''' 表示对象是否是激活或停用。如果值为非零，则对象是激活的，如果值为零，则对象是停用的。
    ''' </param>
    Public Sub UIActivateIO(ByVal fActivate As Integer,
                            ByRef msg As NativeMethods.MSG) _
                        Implements NativeMethods.IInputObject.UIActivateIO
        If fActivate <> 0 Then
            Dim nextControl As Control = MyBase.GetNextControl(Me, True)
            If Control.ModifierKeys = Keys.Shift Then
                nextControl = MyBase.GetNextControl(nextControl, False)
            End If
            If nextControl IsNot Nothing Then
                nextControl.Select()
            End If
            MyBase.Focus()
        End If

    End Sub

    Public Function HasFocusIO() As Integer Implements NativeMethods.IInputObject.HasFocusIO
        If Not MyBase.ContainsFocus Then
            Return 1
        End If
        Return 0
    End Function

    ''' <summary>
    ''' 启用对象来处理键盘快捷方式。
    ''' </summary>
    Public Function TranslateAcceleratorIO(ByRef msg As NativeMethods.MSG) As Integer _
        Implements NativeMethods.IInputObject.TranslateAcceleratorIO
        If (msg.message = 256) AndAlso ((msg.wParam = 9) OrElse (msg.wParam = 117)) Then
            If MyBase.SelectNextControl(MyBase.ActiveControl, Control.ModifierKeys <> Keys.Shift, True, True, False) Then
                Return 0
            End If
        End If
        Return 1

    End Function
#End Region

#Region "ComRegister Functions"

    ''' <summary>
    ''' 当派生类被注册为一个COM服务器时被调用。
    ''' </summary>
    <ComRegisterFunctionAttribute()>
    Public Shared Sub Register(ByVal t As Type)

        ' 为浏览器栏添加类别标示符并设置其他一些值。
        Dim clsidkey As RegistryKey = Registry.ClassesRoot.CreateSubKey("CLSID\" & t.GUID.ToString("B"))
        clsidkey.SetValue(Nothing, ImageListExplorerBar.imageListExplorerBarTitle)
        clsidkey.SetValue("MenuText", ImageListExplorerBar.imageListExplorerBarTitle)
        clsidkey.SetValue("HelpText", "See Readme.txt for detailed help!")
        clsidkey.CreateSubKey("Implemented Categories").CreateSubKey(ImageListExplorerBar.verticalExplorerBarCATID)
        clsidkey.Close()

        ' 设置栏的大小。
        Dim explorerBarKeyPath As String = "SOFTWARE\Microsoft\Internet Explorer\Explorer Bars\" & t.GUID.ToString("B")
        Dim explorerBarKey As RegistryKey = Registry.LocalMachine.CreateSubKey(explorerBarKeyPath)
        explorerBarKey.SetValue("BarSize", New Byte() {6, 1, 0, 0, 0, 0, 0, 0}, RegistryValueKind.Binary)
        explorerBarKey.Close()


        Registry.CurrentUser.CreateSubKey(explorerBarKeyPath).SetValue(
            "BarSize", New Byte() {6, 1, 0, 0, 0, 0, 0, 0}, RegistryValueKind.Binary)

        ' 删除缓存。
        Try
            Dim catPath As String =
                "Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\Component Categories\" _
                & ImageListExplorerBar.verticalExplorerBarCATID

            Registry.CurrentUser.OpenSubKey(catPath, True).DeleteSubKey("Enum")
        Catch
        End Try

        Try
            Dim catPath As String =
                "Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\Component Categories64\" _
                & ImageListExplorerBar.verticalExplorerBarCATID

            Registry.CurrentUser.OpenSubKey(catPath, True).DeleteSubKey("Enum")
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' 当派生类以一个COM服务器注销时被调用。
    ''' </summary>
    <ComUnregisterFunctionAttribute()>
    Public Shared Sub Unregister(ByVal t As Type)
        Dim clsidkeypath As String = t.GUID.ToString("B")
        Registry.ClassesRoot.OpenSubKey("CLSID", True).DeleteSubKeyTree(clsidkeypath)

        Dim explorerBarsKeyPath As String = "SOFTWARE\Microsoft\Internet Explorer\Explorer Bars"

        Registry.LocalMachine.OpenSubKey(explorerBarsKeyPath, True).DeleteSubKey(t.GUID.ToString("B"))
        Registry.CurrentUser.OpenSubKey(explorerBarsKeyPath, True).DeleteSubKey(t.GUID.ToString("B"))


    End Sub


#End Region

    Private Sub btnGetImg_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGetImg.Click
        lstImg.Items.Clear()

        Dim doc As mshtml.HTMLDocument = TryCast(explorer.Document, mshtml.HTMLDocument)

        Dim imgs = doc.getElementsByTagName("img")

        For Each img In imgs
            Dim imgElement As mshtml.IHTMLImgElement = TryCast(img, mshtml.IHTMLImgElement)
            If imgElement IsNot Nothing AndAlso (Not lstImg.Items.Contains(imgElement.src)) Then
                lstImg.Items.Add(imgElement.src)
            End If
        Next img
    End Sub

    Private Sub lstImg_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles lstImg.DoubleClick
        If lstImg.SelectedItem IsNot Nothing Then
            Dim url As String = TryCast(lstImg.SelectedItem, String)
            Dim doc As mshtml.HTMLDocument = TryCast(explorer.Document, mshtml.HTMLDocument)
            doc.parentWindow.open(url)
        End If
    End Sub
End Class
