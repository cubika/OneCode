'********************************** 模块头 ******************************'
' 模块名:  OpenImageHandler.vb
' 项目名:  VBCustomIEContextMenu
' 版权 (c) Microsoft Corporation.
' 
' 类 OpenImageHandler 实现了接口 IDocHostUIHandler 的 ShowContextMenu 方法.
' 对于接口 IDocHostUIHandler 的其他方法,只返回 1,这意味着将使用默认处理程序.
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

Imports VBCustomIEContextMenu.NativeMethods
Imports mshtml
Imports SHDocVw
Imports System.Runtime.InteropServices

Friend Class OpenImageHandler
    Implements NativeMethods.IDocHostUIHandler, IDisposable

    Private disposed As Boolean = False

    ' 承载此 WebBrowser 控件的 IE 实例.
    Public host As InternetExplorer

    ' 自定义上下文菜单. 
    Private contextMenu As ContextMenuStrip

    Private menuItem As ToolStripMenuItem


    ''' <summary>
    ''' 初始化处理程序.
    ''' </summary>
    Public Sub New(ByVal host As InternetExplorer)
        Me.host = host

        contextMenu = New ContextMenuStrip()
        menuItem = New ToolStripMenuItem()
        menuItem.Size = New Size(180, 100)
        menuItem.Text = "在新选项卡中打开"
        ''menuItem.Text = "Open in new Tab"
        AddHandler menuItem.Click, AddressOf menuItem_Click
        contextMenu.Items.Add(menuItem)
    End Sub

    Private Sub menuItem_Click(ByVal sender As Object, ByVal e As EventArgs)

        Try
            TryCast(host.Document, HTMLDocument).parentWindow.open(
                TryCast(contextMenu.Tag, String))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


#Region "IDocHostUIHandler"

    ''' <summary>
    ''' 显示用于图像的自定义上下文菜单.
    ''' </summary>
    ''' <param name="dwID">
    ''' Dword 值，它指定要显示的快捷菜单中的标识符.
    ''' 请参阅 NativeMethods.CONTEXT_MENU_CONST.
    ''' </param>
    ''' <param name="pt">
    ''' 菜单的屏幕坐标.
    ''' </param>
    ''' <param name="pcmdtReserved"></param>
    ''' <param name="pdispReserved">
    ''' 该对象在 pt 中指定的屏幕坐标处. 
    '''这使得宿主可以传递特定的对象,如锚标签和图像，以提供更多特定指定的上下文.
    ''' </param>
    ''' <returns>
    ''' 返回 0 则意味着宿主显示其用户界面,MSHTML 不会尝试显示其用户界面.
    ''' </returns>
    Public Function ShowContextMenu(ByVal dwID As Integer,
                                    ByVal pt As POINT,
                                    ByVal pcmdtReserved As Object,
                                    ByVal pdispReserved As Object) As Integer _
                                Implements IDocHostUIHandler.ShowContextMenu

        If dwID = NativeMethods.CONTEXT_MENU_CONST.CONTEXT_MENU_IMAGE Then
            Dim img = TryCast(pdispReserved, IHTMLImgElement)
            If Not img Is Nothing Then
                contextMenu.Tag = img.src
                contextMenu.Show(pt.x, pt.y)
                Return 0
            End If
        End If
        Return 1
    End Function

    Public Function GetHostInfo(ByVal info As DOCHOSTUIINFO) As Integer _
        Implements IDocHostUIHandler.GetHostInfo

        Return 1
    End Function

    Public Function ShowUI(ByVal dwID As Integer,
                           ByVal activeObject As IOleInPlaceActiveObject,
                           ByVal commandTarget As IOleCommandTarget,
                           ByVal frame As IOleInPlaceFrame,
                           ByVal doc As IOleInPlaceUIWindow) As Integer _
                       Implements IDocHostUIHandler.ShowUI

        Return 1
    End Function

    Public Function HideUI() As Integer Implements IDocHostUIHandler.HideUI

        Return 1
    End Function

    Public Function UpdateUI() As Integer Implements IDocHostUIHandler.UpdateUI

        Return 1
    End Function

    Public Function EnableModeless(ByVal fEnable As Boolean) As Integer _
        Implements IDocHostUIHandler.EnableModeless

        Return 1
    End Function

    Public Function OnDocWindowActivate(ByVal fActivate As Boolean) As Integer _
        Implements IDocHostUIHandler.OnDocWindowActivate

        Return 1
    End Function

    Public Function OnFrameWindowActivate(ByVal fActivate As Boolean) As Integer _
        Implements IDocHostUIHandler.OnFrameWindowActivate

        Return 1
    End Function

    Public Function ResizeBorder(ByVal rect As COMRECT,
                                 ByVal doc As IOleInPlaceUIWindow,
                                 ByVal fFrameWindow As Boolean) As Integer _
                             Implements IDocHostUIHandler.ResizeBorder

        Return 1
    End Function

    Public Function TranslateAccelerator(ByRef msg_Renamed As MSG,
                                         ByRef group As Guid,
                                         ByVal nCmdID As Integer) As Integer _
                                     Implements IDocHostUIHandler.TranslateAccelerator

        Return 1
    End Function

    Public Function GetOptionKeyPath(ByVal pbstrKey() As String, ByVal dw As Integer) As Integer _
        Implements IDocHostUIHandler.GetOptionKeyPath

        Return 1
    End Function

    Public Function GetDropTarget(ByVal pDropTarget As IOleDropTarget,
                                  <Out()> ByRef ppDropTarget As IOleDropTarget) _
                              As Integer Implements IDocHostUIHandler.GetDropTarget

        ppDropTarget = Nothing
        Return 1
    End Function

    Public Function GetExternal(<System.Runtime.InteropServices.Out()> ByRef ppDispatch As Object) As Integer _
        Implements IDocHostUIHandler.GetExternal

        ppDispatch = Nothing
        Return 1
    End Function

    Public Function TranslateUrl(ByVal dwTranslate As Integer,
                                 ByVal strURLIn As String,
                                 <Out()> ByRef pstrURLOut As String) As Integer _
                             Implements IDocHostUIHandler.TranslateUrl

        pstrURLOut = String.Empty
        Return 1
    End Function

    Public Function FilterDataObject(ByVal pDO As System.Runtime.InteropServices.ComTypes.IDataObject,
                                     <Out()> ByRef ppDORet As System.Runtime.InteropServices.ComTypes.IDataObject) As Integer _
                                  Implements IDocHostUIHandler.FilterDataObject

        ppDORet = Nothing
        Return 1
    End Function


#End Region

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
            If contextMenu IsNot Nothing Then
                contextMenu.Dispose()
            End If

            If menuItem IsNot Nothing Then
                menuItem.Dispose()
            End If
        End If
        disposed = True
    End Sub



End Class
