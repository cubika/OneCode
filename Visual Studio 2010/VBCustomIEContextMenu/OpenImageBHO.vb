'********************************** 模块头 ******************************'
' 模块名:  OpenImageBHO.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类 OpenImageBHO 是一个运行在IE内，提供额外服务的浏览器辅助对象（BHO）.
' 
' 一个 BHO 是一个动态链接库 (DLL),它能够将自身附加到任何 IE 浏览器或 Windows
' 资源管理器的新实例中.这种模块可以通过容器的站点与浏览器取得联系.一般情况下，
' 一个站点是放在容器和所包含的每个对象中间的一个中间对象.容器是 IE 浏览器 
'（或 Windows 资源管理器） 时，则需要该对象实现一个更简单、 更轻的、名为 
' IObjectWithSite的接口.它仅仅提供了 SetSite 和 GetSite 两个方法.
' 
' 此类用于设置 HtmlDocument 的 IDocHostUIHandler.
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
Imports VBCustomIEContextMenu.NativeMethods
Imports Microsoft.Win32
Imports SHDocVw

''' <summary>
''' 设置该类的 GUID,指定该类为 ComVisible.
'''一个 BHO 必须实现接口 IObjectWithSite.
''' </summary>
<ComVisible(True)>
<ClassInterface(ClassInterfaceType.None)>
<Guid("AA0B1334-E7F5-4F75-A1DE-0993098AAF01")>
Public Class OpenImageBHO
    Implements IObjectWithSite, IDisposable

    Private disposed As Boolean = False

    ' 为了注册一个 BHO，应当在此键下创建一个新键.
    Private Const BHORegistryKey As String =
        "Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"

    ' 当前的 IE 实例. 对于 IE7 或更高版本,一个 IE 选项卡是只是一个
    ' IE 的实例.
    Private ieInstance As InternetExplorer

    Private openImageDocHostUIHandler As OpenImageHandler


#Region "Com 注册/注销 Methods"
    ''' <summary>
    ''' 当这个类被注册到 COM 时，添加一个新键到 BHORegistryKey 以便让 IE 可以使用
    ''' 这个 BHO.
    ''' 在 64 位机器上，如果该程序集的平台和安装程序是 x86,32 位的 IE 可以使用这
    ''' 个 BHO.如果该程序集的平台和安装程序是 x64,64 位的 IE 可以使用这个 BHO.
    ''' </summary>
    <ComRegisterFunction()>
    Public Shared Sub RegisterBHO(ByVal t As Type)

        ' 如果该键存在， CreateSubKey 将会打开它.
        Dim bhosKey As RegistryKey =
            Registry.LocalMachine.CreateSubKey(BHORegistryKey, RegistryKeyPermissionCheck.ReadWriteSubTree)

        ' 括在大括号中,用连字符分隔的 32 位: 
        ' {00000000-0000-0000-0000-000000000000}
        Dim bhoKeyStr As String = t.GUID.ToString("B")

        Dim bhoKey As RegistryKey = bhosKey.OpenSubKey(bhoKeyStr, True)

        ' 创建一个新键.
        If bhoKey Is Nothing Then
            bhoKey = bhosKey.CreateSubKey(bhoKeyStr)
        End If

        ' NoExplorer:dword = 1 阻止浏览器加载这个 BHO.
        bhoKey.SetValue("NoExplorer", 1)
        bhosKey.Close()
        bhoKey.Close()
    End Sub

    ''' <summary>
    ''' 从 COM 注销该类时,删除该键.
    ''' </summary>
    <ComUnregisterFunction()>
    Public Shared Sub UnregisterBHO(ByVal t As Type)
        Dim bhosKey As RegistryKey =
            Registry.LocalMachine.OpenSubKey(BHORegistryKey, True)
        Dim guidString As String = t.GUID.ToString("B")
        If bhosKey IsNot Nothing Then
            bhosKey.DeleteSubKey(guidString, False)
        End If

        bhosKey.Close()
    End Sub


#End Region

#Region "IObjectWithSite 成员"
    ''' <summary>
    ''' 当实例化或者销毁 BHO 时，调用该方法. 这个站点是一个
    ''' 实现了 InternetExplorer 接口的对象.
    ''' </summary>
    ''' <param name="site"></param>
    Public Sub SetSite(ByVal site As Object) Implements IObjectWithSite.SetSite

        If site IsNot Nothing Then
            ieInstance = CType(site, InternetExplorer)

            openImageDocHostUIHandler = New OpenImageHandler(ieInstance)

            ' 登记 DocumentComplete 事件.
            AddHandler ieInstance.DocumentComplete, AddressOf ieInstance_DocumentComplete
        End If
    End Sub

    ''' <summary>
    ''' 通过设置SetSite()从最后一个站点检索并返回指定的接口.典型的实现将
    ''' 会为指定的接口查询以先前存储的 pUnkSite 指针.
    ''' </summary>
    Public Sub GetSite(ByRef guid_Renamed As Guid,
                       <Out()> ByRef ppvSite As Object) Implements IObjectWithSite.GetSite

        Dim punk As IntPtr = Marshal.GetIUnknownForObject(ieInstance)
        ppvSite = New Object()
        Dim ppvSiteIntPtr As IntPtr = Marshal.GetIUnknownForObject(ppvSite)
        Dim hr As Integer = Marshal.QueryInterface(punk, guid_Renamed, ppvSiteIntPtr)
        Marshal.Release(punk)
    End Sub
#End Region

#Region "事件处理"

    ''' <summary>
    ''' 处理 DocumentComplete 事件.
    ''' </summary>
    ''' <param name="pDisp">
    ''' pDisp 是一个实现了 InternetExplorer 接口的对象.
    ''' 默认情况下,该对象与 ieInstance 相同, 但是如果页面包含多个框架,每一个框
    ''' 架会有自己的文件.
    ''' </param>
    Private Sub ieInstance_DocumentComplete(ByVal pDisp As Object, ByRef URL As Object)
        Dim urlstr As String = TryCast(URL, String)

        If String.IsNullOrEmpty(urlstr) _
            OrElse urlstr.Equals("about:blank", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim explorer As InternetExplorer = TryCast(pDisp, InternetExplorer)

        ' 设置 InternetExplorer 内文件的句柄.
        If explorer IsNot Nothing Then
            Dim customDoc As NativeMethods.ICustomDoc = CType(explorer.Document, NativeMethods.ICustomDoc)
            customDoc.SetUIHandler(openImageDocHostUIHandler)
        End If
    End Sub
#End Region

#Region "IDisposable 支持"
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        ' 防止被多次调用.
        If disposed Then
            Return
        End If

        If disposing Then
            ' 清理所有托管资源.
            If openImageDocHostUIHandler IsNot Nothing Then
                openImageDocHostUIHandler.Dispose()
            End If

        End If
        disposed = True
    End Sub
#End Region

    
End Class
