'****************************** 模块头 ******************************'
' 模块名:  ShellExtLib.vb
' 项目:      VBShellExtContextMenuHandler
' 版权 (c) Microsoft Corporation.
' 
'该文件声明导入命令行程序接口： IShellExtInit 和 IContextMenu，
'实现用于注册和注销壳上下文的帮助器函数
'处理程序菜单，并定义 Win32 枚举、 结构、 consts 和所使用的功能代码示例
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
Imports Microsoft.Win32

#End Region


#Region "Shell Interfaces"

<ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), _
Guid("000214e8-0000-0000-c000-000000000046")> _
Friend Interface IShellExtInit
    Sub Initialize( _
        ByVal pidlFolder As IntPtr, _
        ByVal pDataObj As IntPtr, _
        ByVal hKeyProgID As IntPtr)
End Interface


<ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), _
Guid("000214e4-0000-0000-c000-000000000046")> _
Friend Interface IContextMenu

    <PreserveSig()> _
    Function QueryContextMenu( _
        ByVal hMenu As IntPtr, _
        ByVal iMenu As UInt32, _
        ByVal idCmdFirst As UInt32, _
        ByVal idCmdLast As UInt32, _
        ByVal uFlags As UInt32) _
    As Integer

    Sub InvokeCommand(ByVal pici As IntPtr)

    Sub GetCommandString( _
        ByVal idCmd As UIntPtr, _
        ByVal uFlags As UInt32, _
        ByVal pReserved As IntPtr, _
        ByVal pszName As StringBuilder, _
        ByVal cchMax As UInt32)

End Interface

#End Region


#Region "Shell Registration"

Friend Class ShellExtReg

    ''' <summary>
    ''' 注册上下文菜单处理程序.
    ''' </summary>
    ''' <param name="t">COM class</param>
    ''' <param name="fileType">
    ''' 注册上下文菜单处理程序关联的文件类型. 比如 '*' 表示所有文件; '.txt' 表示文本文件 .该参数不能是null值和空字符串 
    ''' </param>
    ''' <param name="friendlyName">组件的别名</param>
    Public Shared Sub RegisterShellExtContextMenuHandler( _
        ByVal t As Type, _
        ByVal fileType As String, _
        ByVal friendlyName As String)

        If String.IsNullOrEmpty(fileType) Then
            Throw New ArgumentException("文件类型不能为null或空字符串")
        End If

        '如果文件的类型是以'.'开头的，然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件

        If (fileType.StartsWith(".")) Then
            Using key As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileType)
                If (key IsNot Nothing) Then

                    '如果该键存在且不为空, 使用ProgID 为该文件类型
                    Dim defaultVal As String = key.GetValue(Nothing)
                    If (Not String.IsNullOrEmpty(defaultVal)) Then
                        fileType = defaultVal
                    End If
                End If
            End Using
        End If

        ' 创建键HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}
        Dim keyName As String = String.Format("{0}\shellex\ContextMenuHandlers\{1}", _
            fileType, t.GUID.ToString("B"))
        Using key As RegistryKey = Registry.ClassesRoot.CreateSubKey(keyName)
            ' 为该键设置默认值.
            If ((key IsNot Nothing) AndAlso _
                (Not String.IsNullOrEmpty(friendlyName))) Then
                key.SetValue(Nothing, friendlyName)
            End If
        End Using
    End Sub


    ''' <summary>
    ''' 反注册上下文菜单处理程序.
    ''' </summary>
    ''' <param name="t">COM class</param>
    ''' <param name="fileType">
    '''注册上下文菜单处理程序关联的文件类型. 比如 '*' 表示所有文件; '.txt' 表示文本文件 .该参数不能是null值和空字符串. 
    ''' </param>
    Public Shared Sub UnregisterShellExtContextMenuHandler( _
        ByVal t As Type, ByVal fileType As String)

        If String.IsNullOrEmpty(fileType) Then
            Throw New ArgumentException("fileType must not be null or empty")
        End If

        ' 如果文件的类型是以'.'开头的，然后在注册表的HKCR\<File Type>下获取该文件类型的对应的Program ID来关联该文件
        If (fileType.StartsWith(".")) Then
            Using key As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileType)
                If (key IsNot Nothing) Then
                    ' 如果该键存在且不为空, 使用ProgID 为该文件类型
                    Dim defaultVal As String = key.GetValue(Nothing)
                    If (Not String.IsNullOrEmpty(defaultVal)) Then
                        fileType = defaultVal
                    End If
                End If
            End Using
        End If

        ' 删除HKCR\<File Type>\shellex\ContextMenuHandlers\{<CLSID>}键.
        Dim keyName As String = String.Format("{0}\shellex\ContextMenuHandlers\{1}", _
            fileType, t.GUID.ToString("B"))
        Registry.ClassesRoot.DeleteSubKeyTree(keyName, False)
    End Sub

End Class

#End Region


#Region "Enums & Structs"

Friend Enum GCS As UInt32
    GCS_VERBA = 0
    GCS_HELPTEXTA = 1
    GCS_VALIDATEA = 2
    GCS_HELPTEXTW = 5
    GCS_UNICODE = 4
    GCS_VERBW = 4
    GCS_VALIDATEW = 6
    GCS_VERBICONW = 20
End Enum


<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
Friend Structure CMINVOKECOMMANDINFO
    Public cbSize As UInt32
    Public fMask As CMIC
    Public hwnd As IntPtr
    Public lpVerb As IntPtr
    <MarshalAs(UnmanagedType.LPStr)> _
    Public parameters As String
    <MarshalAs(UnmanagedType.LPStr)> _
    Public directory As String
    Public nShow As Integer
    Public dwHotKey As UInt32
    Public hIcon As IntPtr
End Structure


<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
Friend Structure CMINVOKECOMMANDINFOEX
    Public cbSize As UInt32
    Public fMask As CMIC
    Public hwnd As IntPtr
    <MarshalAs(UnmanagedType.LPStr)> _
    Public verb As String
    <MarshalAs(UnmanagedType.LPStr)> _
    Public parameters As String
    <MarshalAs(UnmanagedType.LPStr)> _
    Public directory As String
    Public nShow As Integer
    Public dwHotKey As UInt32
    Public hIcon As IntPtr
    <MarshalAs(UnmanagedType.LPStr)> _
    Public title As String
    Public lpVerbW As IntPtr
    Public parametersW As String
    Public directoryW As String
    Public titleW As String
    Private ptInvoke As POINT
End Structure


<Flags()> _
Friend Enum CMIC As UInt32
    CMIC_MASK_ICON = &H10
    CMIC_MASK_HOTKEY = &H20
    CMIC_MASK_NOASYNC = &H100
    CMIC_MASK_FLAG_NO_UI = &H400
    CMIC_MASK_UNICODE = &H4000
    CMIC_MASK_NO_CONSOLE = &H8000
    CMIC_MASK_ASYNCOK = &H100000
    CMIC_MASK_NOZONECHECKS = &H800000
    CMIC_MASK_FLAG_LOG_USAGE = &H4000000
    CMIC_MASK_SHIFT_DOWN = &H10000000
    CMIC_MASK_PTINVOKE = &H20000000
    CMIC_MASK_CONTROL_DOWN = &H40000000
End Enum


<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
Public Structure POINT
    Public X As Integer
    Public Y As Integer
End Structure


Friend Enum CLIPFORMAT As UInt32
    CF_TEXT = 1
    CF_BITMAP = 2
    CF_METAFILEPICT = 3
    CF_SYLK = 4
    CF_DIF = 5
    CF_TIFF = 6
    CF_OEMTEXT = 7
    CF_DIB = 8
    CF_PALETTE = 9
    CF_PENDATA = 10
    CF_RIFF = 11
    CF_WAVE = 12
    CF_UNICODETEXT = 13
    CF_ENHMETAFILE = 14
    CF_HDROP = 15
    CF_LOCALE = &H10
    CF_MAX = &H11

    CF_OWNERDISPLAY = &H80
    CF_DSPTEXT = &H81
    CF_DSPBITMAP = &H82
    CF_DSPMETAFILEPICT = &H83
    CF_DSPENHMETAFILE = &H8E

    CF_PRIVATEFIRST = &H200
    CF_PRIVATELAST = &H2FF

    CF_GDIOBJFIRST = &H300
    CF_GDIOBJLAST = &H3FF
End Enum


<Flags()> _
Friend Enum CMF
    CMF_NORMAL = 0
    CMF_DEFAULTONLY = 1
    CMF_VERBSONLY = 2
    CMF_EXPLORE = 4
    CMF_NOVERBS = 8
    CMF_CANRENAME = &H10
    CMF_NODEFAULT = &H20
    CMF_INCLUDESTATIC = &H40
    CMF_ITEMMENU = &H80
    CMF_EXTENDEDVERBS = &H100
    CMF_DISABLEDVERBS = &H200
    CMF_ASYNCVERBSTATE = &H400
    CMF_OPTIMIZEFORINVOKE = &H800
    CMF_SYNCCASCADEMENU = &H1000
    CMF_DONOTPICKDEFAULT = &H2000
    CMF_RESERVED = &HFFFF0000
End Enum


<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
Friend Structure MENUITEMINFO
    Public cbSize As UInt32
    Public fMask As MIIM
    Public fType As MFT
    Public fState As MFS
    Public wID As UInt32
    Public hSubMenu As IntPtr
    Public hbmpChecked As IntPtr
    Public hbmpUnchecked As IntPtr
    Public dwItemData As UIntPtr
    Public dwTypeData As String
    Public cch As UInt32
    Public hbmpItem As IntPtr
End Structure


<Flags()> _
Friend Enum MIIM As UInt32
    MIIM_STATE = 1
    MIIM_ID = 2
    MIIM_SUBMENU = 4
    MIIM_CHECKMARKS = 8
    MIIM_TYPE = &H10
    MIIM_DATA = &H20
    MIIM_STRING = &H40
    MIIM_BITMAP = &H80
    MIIM_FTYPE = &H100
End Enum


Friend Enum MFT As UInt32
    MFT_STRING = 0
    MFT_BITMAP = 4
    MFT_MENUBARBREAK = &H20
    MFT_MENUBREAK = &H40
    MFT_OWNERDRAW = &H100
    MFT_RADIOCHECK = &H200
    MFT_SEPARATOR = &H800
    MFT_RIGHTORDER = &H2000
    MFT_RIGHTJUSTIFY = &H4000
End Enum


Friend Enum MFS As UInt32
    MFS_ENABLED = 0
    MFS_UNCHECKED = 0
    MFS_UNHILITE = 0
    MFS_DISABLED = 3
    MFS_GRAYED = 3
    MFS_CHECKED = 8
    MFS_HILITE = &H80
    MFS_DEFAULT = &H1000
End Enum

#End Region


Friend Class NativeMethods

    ''' <summary>
    ''' 检索成功的拖放操作中的已删除文件的名称.
    ''' </summary>
    ''' <param name="hDrop">
    ''' 结构，其中包含已删除的文件的文件名称的标识符.
    ''' </param>
    ''' <param name="iFile">
    ''' 要查询文件的索引. 如果此参数的值是 0xFFFFFFFF,DragQueryFile 返回删除的文件的计数. 
    ''' </param>
    ''' <param name="pszFile">
    ''' 返回接收的已删除的文件的文件名缓冲区的地址.
    ''' </param>
    ''' <param name="cch">
    ''' pszFile 缓冲区的大小.
    ''' </param>
    ''' <returns>一个非零值指示成功调用.</returns>
    <DllImport("shell32", CharSet:=CharSet.Unicode)> _
    Public Shared Function DragQueryFile( _
        ByVal hDrop As IntPtr, _
        ByVal iFile As UInt32, _
        ByVal pszFile As StringBuilder, _
        ByVal cch As Integer) As UInt32
    End Function


    ''' <summary>
    ''' 释放指定的存储空间.
    ''' </summary>
    ''' <param name="pmedium">
    ''' 对要释放的存储空间的引用.
    ''' </param>
    <DllImport("ole32.dll", CharSet:=CharSet.Unicode)> _
    Public Shared Sub ReleaseStgMedium(ByRef pmedium As STGMEDIUM)
    End Sub


    ''' <summary>
    ''' 在菜单特定位置插入一个新的菜单.
    ''' </summary>
    ''' <param name="hMenu">
    ''' 新插入的菜单项的句柄. 
    ''' </param>
    ''' <param name="uItem">
    ''' 新插入菜单项的前一个菜单项目的地址或位置.
    ''' </param>
    ''' <param name="fByPosition">
    ''' 菜单项的标识符. 
    ''' </param>
    ''' <param name="mii">
    '''  包含新的菜单项的 MENUITEMINFO 结构信息的引用.
    ''' </param>
    ''' <returns>
    ''' 如果执行成功将返回True.
    ''' </returns>
    <DllImport("user32", CharSet:=CharSet.Unicode, SetLastError:=True)> _
    Public Shared Function InsertMenuItem( _
        ByVal hMenu As IntPtr, _
        ByVal uItem As UInt32, _
        <MarshalAs(UnmanagedType.Bool)> ByVal fByPosition As Boolean, _
        ByRef mii As MENUITEMINFO) _
    As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    Public Shared Function HighWord(ByVal number As Integer) As Integer
        Return If(((number And &H80000000) = &H80000000), _
            (number >> &H10), ((number >> &H10) And &HFFFF))
    End Function


    Public Shared Function LowWord(ByVal number As Integer) As Integer
        Return (number And &HFFFF)
    End Function

End Class


Friend Class WinError

    Public Const S_OK As Integer = 0
    Public Const S_FALSE As Integer = 1
    Public Const E_FAIL As Integer = -2147467259
    Public Const E_INVALIDARG As Integer = -2147024809
    Public Const E_OUTOFMEMORY As Integer = -2147024882
    Public Const STRSAFE_E_INSUFFICIENT_BUFFER As Integer = -2147024774

    Public Const SEVERITY_ERROR As UInt32 = 1
    Public Const SEVERITY_SUCCESS As UInt32 = 0

    ''' <summary>
    '''返回HRESULT.
    ''' </summary>
    ''' <param name="sev">严重度</param>
    ''' <param name="fac">The facility to be used</param>
    ''' <param name="code">错误代码</param>
    ''' <returns>通过上面三个值构造一个HRESULT类型返回值</returns>
    Public Shared Function MAKE_HRESULT( _
        ByVal sev As UInt32, _
        ByVal fac As UInt32, _
        ByVal code As UInt32) As Integer
        Return CInt((((sev << &H1F) Or (fac << &H10)) Or code))
    End Function

End Class