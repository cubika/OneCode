'****************************** 模块头 ******************************'
' 模块名:  FileContextMenuExt.vb
' 项目名:      VBShellExtContextMenuHandler
' 版权 (c) Microsoft Corporation.
' 
' FileContextMenuExt.vb定义了一个实现IShellExtInit和IContextMenu接口的上下文菜单应用程序
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes

#End Region


<ClassInterface(ClassInterfaceType.None), _
Guid("1E25BCD5-F299-496A-911D-51FB901F7F40"), ComVisible(True)> _
Public Class FileContextMenuExt
    Implements IShellExtInit, IContextMenu

    ' 被选中的文件名.
    Private selectedFile As String

    Private menuText As String = "&Display File Name (VB)"
    Private verb As String = "vbdisplay"
    Private verbCanonicalName As String = "VBDisplayFileName"
    Private verbHelpText As String = "Display File Name (VB)"
    Private IDM_DISPLAY As UInteger = 0


    Private Sub OnVerbDisplayFileName(ByVal hWnd As IntPtr)
        System.Windows.Forms.MessageBox.Show("被选中的文件是 " & _
            Environment.NewLine & Environment.NewLine & Me.selectedFile, _
            "VBShellExtContextMenuHandler")
    End Sub


#Region "Shell Extension Registration"

    <ComRegisterFunction()> _
    Public Shared Sub Register(ByVal t As Type)
        Try
            ShellExtReg.RegisterShellExtContextMenuHandler(t, ".vb", _
                "VBShellExtContextMenuHandler.FileContextMenuExt")
        Catch ex As Exception
            Console.WriteLine(ex.Message) ' 记录错误日志
            Throw ' 抛出异常
        End Try
    End Sub


    <ComUnregisterFunction()> _
    Public Shared Sub Unregister(ByVal t As Type)
        Try
            ShellExtReg.UnregisterShellExtContextMenuHandler(t, ".vb")
        Catch ex As Exception
            Console.WriteLine(ex.Message) ' 记录错误日志
            Throw ' 再次抛出异常
        End Try
    End Sub

#End Region


#Region "IShellExtInit Members"

    ''' <summary>
    '''  初始化上下文菜单处理程序.
    ''' </summary>
    ''' <param name="pidlFolder">
    ''' 指向标识文件夹的 ITEMIDLIST 结构的指针.
    ''' </param>
    ''' <param name="pDataObj">
    ''' 一个指向 IDataObject 接口对象的指针可以用来检索对象.
    ''' </param>
    ''' <param name="hKeyProgID">
    ''' 对象或文件夹的文件类型的注册表项.
    ''' </param>
    Public Sub Initialize( _
        ByVal pidlFolder As IntPtr, _
        ByVal pDataObj As IntPtr, _
        ByVal hKeyProgID As IntPtr) _
        Implements IShellExtInit.Initialize

        If (pDataObj = IntPtr.Zero) Then
            Throw New ArgumentException
        End If

        Dim fe As New FORMATETC
        With fe
            .cfFormat = CLIPFORMAT.CF_HDROP
            .ptd = IntPtr.Zero
            .dwAspect = DVASPECT.DVASPECT_CONTENT
            .lindex = -1
            .tymed = TYMED.TYMED_HGLOBAL
        End With

        Dim stm As New STGMEDIUM

        ' pDataObj 指针指向对象. 在这示例，我们得到一个HDROP的句柄的枚举在选定的文件和文件夹.

        Dim dataObject As IDataObject = Marshal.GetObjectForIUnknown(pDataObj)
        dataObject.GetData(fe, stm)

        Try
            ' 取到HDROP句柄.
            Dim hDrop As IntPtr = stm.unionmember
            If (hDrop = IntPtr.Zero) Then
                Throw New ArgumentException
            End If

            ' 确定在此操作中涉及多少个文件.
            Dim nFiles As UInteger = NativeMethods.DragQueryFile(hDrop, _
                UInt32.MaxValue, Nothing, 0)

            ' 只有一个文件被选中时，示例代码将显示自定义上下文菜单项中. 
            If (nFiles = 1) Then
                ' 获取文件名.
                Dim fileName As New StringBuilder(260)
                If (0 = NativeMethods.DragQueryFile(hDrop, 0, fileName,
                    fileName.Capacity)) Then
                    Marshal.ThrowExceptionForHR(WinError.E_FAIL)
                End If
                Me.selectedFile = fileName.ToString
            Else
                Marshal.ThrowExceptionForHR(WinError.E_FAIL)
            End If

            ' [-或者-]

            ' 枚举被选中的文件和文件夹.
            'If (nFiles > 0) Then
            '    Dim selectedFiles As New StringCollection()
            '    Dim fileName As New StringBuilder(260)
            '    For i As UInteger = 0 To nFiles - 1
            '        ' Get the next file name.
            '        If (0 <> NativeMethods.DragQueryFile(hDrop, i, fileName, _
            '            fileName.Capacity)) Then
            '            ' Add the file name to the list.
            '            selectedFiles.Add(fileName.ToString())
            '        End If
            '    Next

            '    ' If we did not find any files we can work with, throw exception.
            '    If (selectedFiles.Count = 0) Then
            '        Marshal.ThrowExceptionForHR(WinError.E_FAIL)
            '    End If
            'Else
            '    Marshal.ThrowExceptionForHR(WinError.E_FAIL)
            'End If

        Finally
            NativeMethods.ReleaseStgMedium((stm))
        End Try
    End Sub

#End Region


#Region "IContextMenu Members"

    ''' <summary>
    ''' 将命令添加到快捷菜单.
    ''' </summary>
    ''' <param name="hMenu">快捷菜单的句柄.</param>
    ''' <param name="iMenu">
    ''' 从零位置开始插入第一个新菜单项.
    ''' </param>
    ''' <param name="idCmdFirst">
    ''' 菜单项 ID 指定的最小值.
    ''' </param>
    ''' <param name="idCmdLast">
    ''' 菜单项 ID 指定的最大值.
    ''' </param>
    ''' <param name="uFlags">
    ''' 可选参数，用来标示快捷菜单如何改变.
    ''' </param>
    ''' <returns>
    ''' 如果成功返回一个HRESULT类型的值 
    '''  将代码值设置为最大的命令标识符的偏移量将被分配并且加一 
    ''' </returns>
    Public Function QueryContextMenu( _
        ByVal hMenu As IntPtr, _
        ByVal iMenu As UInt32, _
        ByVal idCmdFirst As UInt32, _
        ByVal idCmdLast As UInt32, _
        ByVal uFlags As UInt32) As Integer _
        Implements IContextMenu.QueryContextMenu

        ' 如果uFlags参数包含CMF_DEFAULTONLY，我们就可以什么都不要做了.
        If ((CMF.CMF_DEFAULTONLY And uFlags) <> 0) Then
            Return WinError.MAKE_HRESULT(WinError.SEVERITY_SUCCESS, 0, 0)
        End If

        ' 使用InsertMenu 或 InsertMenuItem添加菜单项.

        Dim mii As New MENUITEMINFO
        With mii
            .cbSize = Marshal.SizeOf(mii)
            .fMask = MIIM.MIIM_TYPE Or MIIM.MIIM_STATE Or MIIM.MIIM_ID
            .wID = idCmdFirst + Me.IDM_DISPLAY
            .fType = MFT.MFT_STRING
            .dwTypeData = Me.menuText
            .fState = MFS.MFS_ENABLED
        End With
        If Not NativeMethods.InsertMenuItem(hMenu, iMenu, True, mii) Then
            Return Marshal.GetHRForLastWin32Error
        End If

        ' 增加一个分隔符.
        Dim sep As New MENUITEMINFO
        With sep
            .cbSize = Marshal.SizeOf(sep)
            .fMask = MIIM.MIIM_TYPE
            .fType = MFT.MFT_SEPARATOR
        End With
        If Not NativeMethods.InsertMenuItem(hMenu, iMenu + 1, True, sep) Then
            Return Marshal.GetHRForLastWin32Error
        End If

        ' 如果成功返回一个HRESULT类型的值. 
        ' 将代码值设置为最大的命令标识符的偏移量将被分配并且加一 
        Return WinError.MAKE_HRESULT(0, 0, (Me.IDM_DISPLAY + 1))
    End Function


    ''' <summary>
    ''' 当用户点击相关联的菜单项后则调用该方法.
    ''' </summary>
    ''' <param name="pici">
    ''' 参数pici是一个指向包含相关必须信息的CMINVOKECOMMANDINFO或CMINVOKECOMMANDINFOEX指针
    ''' </param>
    Public Sub InvokeCommand(ByVal pici As IntPtr) _
        Implements IContextMenu.InvokeCommand

        Dim isUnicode As Boolean = False

        '检查什么类型的结构体被传递进函数，CMINVOKECOMMANDINFO还是CMINVOKECOMMANDINFOEX取决于
        'lpcmi的成员变量cbSize，虽然lpcmi在 Shlobj.h中指向CMINVOKECOMMANDINFO结构体
        '但通常lpcmi指向的是一个CMINVOKECOMMANDINFOEX结构体，CMINVOKECOMMANDINFOEX是CMINVOKECOMMANDINFO的扩展
        '可以接受Unicode字符串 
        Dim ici As CMINVOKECOMMANDINFO = Marshal.PtrToStructure(pici, GetType(CMINVOKECOMMANDINFO))
        Dim iciex As New CMINVOKECOMMANDINFOEX
        If (ici.cbSize = Marshal.SizeOf(GetType(CMINVOKECOMMANDINFOEX))) Then
            If (ici.fMask And CMIC.CMIC_MASK_UNICODE) <> 0 Then
                isUnicode = True
                iciex = Marshal.PtrToStructure(pici, GetType(CMINVOKECOMMANDINFOEX))
            End If
        End If

        ' 检查命令是动态的还是偏移产生的.
        ' 有两种方法来确定:
        ' 
        '   1)  命令是动态的  
        '   2)  命令是偏移的
        ' 
        ' 如果在ANSI的情况下lpcmi->lpVerb或Unicode情况下的lpcmi->lpVerbW为非0,且保存了一个动态的字符串则可以判断其为动态的，
        ' 反之则可以确定为偏移的


        If (Not isUnicode AndAlso (NativeMethods.HighWord(ici.lpVerb.ToInt32) <> 0)) Then
            ' Is the verb supported by this context menu extension?
            If (Marshal.PtrToStringAnsi(ici.lpVerb) = Me.verb) Then
                OnVerbDisplayFileName(ici.hwnd)
            Else
                ' 上下文菜单是否支持动态的?
                Marshal.ThrowExceptionForHR(WinError.E_FAIL)
            End If

        ElseIf (isUnicode AndAlso (NativeMethods.HighWord(iciex.lpVerbW.ToInt32) <> 0)) Then

            ' 在Unicode情况下, 不为0, 动态命令保存在 lpcmi->lpVerbW. 
            ' 该上下文菜单是否支持动态?
            If (Marshal.PtrToStringUni(iciex.lpVerbW) = Me.verb) Then
                OnVerbDisplayFileName(ici.hwnd)
            Else
                ' 如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上,并抛出异常

                Marshal.ThrowExceptionForHR(WinError.E_FAIL)
            End If

        Else
            ' 如果该命令不能通过动态确定,则可以确定为偏移的.

            ' 上下文菜单是否支持偏移的?
            If (NativeMethods.LowWord(ici.lpVerb.ToInt32) = Me.IDM_DISPLAY) Then
                OnVerbDisplayFileName(ici.hwnd)
            Else
                ' 如果上下文菜单处理程序不能识别该命令, 必须返回E_FAIL,并传递到其他的能识别该命令的上下文菜单项上.
                Marshal.ThrowExceptionForHR(WinError.E_FAIL)
            End If
        End If
    End Sub


    ''' <summary>
    ''' 获取一个快捷菜单命令包括帮助字符串，信息独立，语言规范，命令名称
    ''' </summary>
    ''' <param name="idCmd">菜单命令标识符偏移量.</param>
    ''' <param name="uFlags">
    ''' 指定要返回的信息的标志. 这个参数只能是 GCS_HELPTEXTA, GCS_HELPTEXTW, GCS_VALIDATEA, GCS_VALIDATEW, GCS_VERBA, GCS_VERBW其中之一.
    ''' </param>
    ''' <param name="pReserved">保留. 必须是IntPtr.Zero</param>
    ''' <param name="pszName">
    ''' 以null 结尾的接收字符串缓冲区的地址.
    ''' </param>
    ''' <param name="cchMax">
    ''' 在缓冲区中接收 null 结尾的字符串的字符的大小.
    ''' </param>
    Public Sub GetCommandString( _
        ByVal idCmd As UIntPtr, _
        ByVal uFlags As UInt32, _
        ByVal pReserved As IntPtr, _
        ByVal pszName As StringBuilder, _
        ByVal cchMax As UInt32) _
        Implements IContextMenu.GetCommandString

        If (idCmd.ToUInt32 = Me.IDM_DISPLAY) Then
            Select Case DirectCast(uFlags, GCS)
                Case GCS.GCS_VERBW
                    If (Me.verbCanonicalName.Length > (cchMax - 1)) Then
                        Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER)
                    Else
                        pszName.Clear()
                        pszName.Append(Me.verbCanonicalName)
                    End If
                    Exit Select

                Case GCS.GCS_HELPTEXTW
                    If (Me.verbHelpText.Length > (cchMax - 1)) Then
                        Marshal.ThrowExceptionForHR(WinError.STRSAFE_E_INSUFFICIENT_BUFFER)
                    Else
                        pszName.Clear()
                        pszName.Append(Me.verbHelpText)
                    End If
                    Exit Select
            End Select
        End If
    End Sub

#End Region

End Class