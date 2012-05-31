'*************************** 模块头 ******************************'
' 模块名称:  BHOIEContextMenu.vb
' 项目名称:	VBBrowserHelperObject
' 版权：Copyright (c) Microsoft Corporation.
' 
' BHOIEContextMenu类是一个运行在IE浏览器并提供额外服务的Browser Helper Object.
' 
' BHO 是一个能够把它自己附加到IE浏览器或资源管理器的任意新实例的动态链接库 (DLL)
' 这样的模块能够通过容器中的地址和浏览器保持联系
' 一般来说，一个地址是放在容器中间的一个中间对象并且都包含对象
' 当这个容器是IE浏览器（或者Windows资源管理器）时，对象
' 需要实现一个简单的轻量级接口IObjectWithSite. 
' 它只提供了 SetSite 和 GetSite两个方法. 
' 
' 这个类用作使IE浏览器的右键菜单失效. 它还能够提供方法把BHO注册到IE浏览器上去
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
Imports mshtml

''' <summary>
''' 设置类的GUID并制定这个类为 ComVisible.
''' BHO 必须实现接口 IObjectWithSite. 
''' </summary>
<ComVisible(True), ClassInterface(ClassInterfaceType.None),
Guid("C42D40F0-BEBF-418D-8EA1-18D99AC2AB17")>
Public Class BHOIEContextMenu
    Implements IObjectWithSite
    ' 当前的IE实例， 支持IE7或之后的版本, 一个IE标签就是一个IE实例
    Private ieInstance As InternetExplorer

    ' 注册一个BHO, 需要在这个键下新建一个键.
    Private Const BHORegistryKey As String =
    "Software\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"



#Region "Com Register/UnRegister Methods"
    ''' <summary>
    ''' 当这个类注册到COM, 给 BHORegistryKey 添加一个新键
    ''' 让IE能够使用BHO.
    ''' 在64位机器上, 如果这个程序集的平台和安装器是x86,
    ''' 32位浏览器可以使用这个BHO.  如果这个程序集的平台和安装器是x64,
    ''' 64位浏览器可以使用这个BHO.
    ''' </summary>
    <ComRegisterFunction()>
    Public Shared Sub RegisterBHO(ByVal t As Type)
        Dim key As RegistryKey = Registry.LocalMachine.OpenSubKey(BHORegistryKey, True)
        If key Is Nothing Then
            key = Registry.LocalMachine.CreateSubKey(BHORegistryKey)
        End If

        ' 32 个数字用连字符隔开, 包含在大括号内: 
        ' {00000000-0000-0000-0000-000000000000}
        Dim bhoKeyStr As String = t.GUID.ToString("B")

        Dim bhoKey As RegistryKey = key.OpenSubKey(bhoKeyStr, True)

        ' 创建一个新键.
        If bhoKey Is Nothing Then
            bhoKey = key.CreateSubKey(bhoKeyStr)
        End If

        ' 没有浏览器:dword = 1 阻止浏览器加载BHO
        Dim name As String = "NoExplorer"
        Dim value As Object = CObj(1)
        bhoKey.SetValue(name, value)
        key.Close()
        bhoKey.Close()
    End Sub

    ''' <summary>
    '''当这个类从COM取消注册，删除这个键
    ''' </summary>
    <ComUnregisterFunction()>
    Public Shared Sub UnregisterBHO(ByVal t As Type)
        Dim key As RegistryKey = Registry.LocalMachine.OpenSubKey(BHORegistryKey, True)
        Dim guidString As String = t.GUID.ToString("B")
        If key IsNot Nothing Then
            key.DeleteSubKey(guidString, False)
        End If
    End Sub
#End Region

#Region "IObjectWithSite Members"
    ''' <summary>
    ''' 当BHO实例化或消亡时调用这个方法. 地址是实现了 InternetExplorer接口的对象
    ''' </summary>
    ''' <param name="site"></param>
    Public Sub SetSite(ByVal site As Object) Implements IObjectWithSite.SetSite
        If site IsNot Nothing Then
            ieInstance = CType(site, InternetExplorer)

            ' 注册DocumentComplete 事件.
            AddHandler ieInstance.DocumentComplete, AddressOf ieInstance_DocumentComplete
        End If
    End Sub

    ''' <summary>
    ''' 通过SetSite()设置从上一个地址获得并返回指定接口
    ''' 典型的实现将会向指定接口查询之前存储的pUnkSite指针
    ''' </summary>
    Public Sub GetSite(ByRef guid_Renamed As Guid,
                       <System.Runtime.InteropServices.Out()> ByRef ppvSite As Object) _
                   Implements IObjectWithSite.GetSite
        Dim punk As IntPtr = Marshal.GetIUnknownForObject(ieInstance)
        ppvSite = New Object()
        Dim ppvSiteIntPtr As IntPtr = Marshal.GetIUnknownForObject(ppvSite)
        Dim hr As Integer = Marshal.QueryInterface(punk, guid_Renamed, ppvSiteIntPtr)
        Marshal.ThrowExceptionForHR(hr)
        Marshal.Release(ppvSiteIntPtr)
        Marshal.Release(punk)
    End Sub
#End Region

#Region "event handler"

    ''' <summary>
    ''' 处理 DocumentComplete 事件.
    ''' </summary>
    ''' <param name="pDisp">
    '''  pDisp是一个实现了 InternetExplorer接口的实例.
    ''' 默认的, 这个实例和 ieInstance相同, 但是当页面
    ''' 包含多个框架时，每一个框架有它们自己的文档
    ''' </param>
    Private Sub ieInstance_DocumentComplete(ByVal pDisp As Object, ByRef URL As Object)
        Dim urlstring As String = TryCast(URL, String)

        If String.IsNullOrEmpty(urlstring) _
            OrElse urlstring.Equals("about:blank", StringComparison.OrdinalIgnoreCase) Then
            Return
        End If

        Dim explorer As InternetExplorer = TryCast(pDisp, InternetExplorer)

        ' 在IE览器中设置文档的事件解决句柄.
        If explorer IsNot Nothing Then
            SetHandler(explorer)
        End If
    End Sub


    Private Sub SetHandler(ByVal explorer As InternetExplorer)
        Try

            ' 在IE浏览器中注册文档的 oncontextmenu 事件
            Dim helper As New HTMLDocumentEventHelper(
                TryCast(explorer.Document, HTMLDocument))
            AddHandler helper.oncontextmenu, AddressOf oncontextmenuHandler
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' 处理 oncontextmenu 事件.
    ''' </summary>
    ''' <param name="e"></param>
    Private Sub oncontextmenuHandler(ByVal e As IHTMLEventObj)

        ' 取消默认行为, 把事件对象的返回值属性设置为false.
        e.returnValue = False

    End Sub

#End Region

End Class
