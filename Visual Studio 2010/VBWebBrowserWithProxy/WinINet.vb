'*************************** Module Header ******************************'
' 模块名称:  WinINet.vb
' 项目名称:	    VBWebBrowserWithProxy
' Copyright (c) Microsoft Corporation.
' 
' 这个类是一个简单的.NET对wininet.dll的封装。它包含了2个对wininet.dll的扩展方法
' （InternetSetOption和InternetQueryOption）。这个类可以设置、激活、备份、保存对 
' internet的设置。
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

Public NotInheritable Class WinINet
    ''' <summary>
    ''' 设置Internet选项
    ''' </summary>
    <DllImport("wininet.dll", CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function InternetSetOption(ByVal hInternet As IntPtr,
                                                  ByVal dwOption As INTERNET_OPTION,
                                                  ByVal lpBuffer As IntPtr,
                                                  ByVal lpdwBufferLength As Integer) As Boolean
    End Function

    ''' <summary>
    ''' 在一个指定的句柄上查询Internet选项值。这个名柄通常为0。
    ''' </summary>
    <DllImport("wininet.dll", EntryPoint:="InternetQueryOption",
        CharSet:=CharSet.Ansi, SetLastError:=True)>
    Private Shared Function InternetQueryOptionList(ByVal Handle As IntPtr,
                                                        ByVal OptionFlag As INTERNET_OPTION,
                                                        ByRef OptionList As INTERNET_PER_CONN_OPTION_LIST,
                                                        ByRef size As Integer) As Boolean
    End Function

    ''' <summary>
    ''' 为局域网设定代理
    ''' </summary>
    Private Sub New()
    End Sub
    Public Shared Function SetConnectionProxy(ByVal proxyServer As String) As Boolean
        ' 创建3个选项
        Dim Options(2) As INTERNET_PER_CONN_OPTION

        ' 设置代理标记
        Options(0) = New INTERNET_PER_CONN_OPTION()
        Options(0).dwOption = CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS)
        Options(0).Value.dwValue = CInt(INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_PROXY)

        ' 设置代理名称
        Options(1) = New INTERNET_PER_CONN_OPTION()
        Options(1).dwOption =
            CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER)
        Options(1).Value.pszValue = Marshal.StringToHGlobalAnsi(proxyServer)

        ' 设置代理口令。
        Options(2) = New INTERNET_PER_CONN_OPTION()
        Options(2).dwOption =
            CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS)
        Options(2).Value.pszValue = Marshal.StringToHGlobalAnsi("local")

        ' 申请内存储存设置
        Dim buffer As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options(0)) _
                                                      + Marshal.SizeOf(Options(1)) _
                                                      + Marshal.SizeOf(Options(2)))

        Dim current As IntPtr = buffer

        '将数据从类的实体中转存到内存中。
        For i As Integer = 0 To Options.Length - 1
            Marshal.StructureToPtr(Options(i), current, False)
            current = CType(CInt(current) + Marshal.SizeOf(Options(i)), IntPtr)
        Next i

        ' 初始化INTERNET_PER_CONN_OPTION_LIST数据结构的实体。
        Dim option_list As New INTERNET_PER_CONN_OPTION_LIST()

        ' 定义指向被申请的内存的指针。
        option_list.pOptions = buffer

        ' 返回不受管理的实体大小。
        option_list.Size = Marshal.SizeOf(option_list)

        ' IntPtr为0表示局域网连接。
        option_list.Connection = IntPtr.Zero

        option_list.OptionCount = Options.Length
        option_list.OptionError = 0
        Dim size As Integer = Marshal.SizeOf(option_list)

        ' 为INTERNET_PER_CONN_OPTION_LIST实体申请内存。
        Dim intptrStruct As IntPtr = Marshal.AllocCoTaskMem(size)

        ' 将数据从一个受管理的类中移到不受管理的内存中去。
        Marshal.StructureToPtr(option_list, intptrStruct, True)

        ' 设定internet选项。
        Dim bReturn As Boolean =
            InternetSetOption(IntPtr.Zero,
                              INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION,
                              intptrStruct,
                              size)

        ' 释放内存
        Marshal.FreeCoTaskMem(buffer)
        Marshal.FreeCoTaskMem(intptrStruct)

        ' 当这个操作失败时抛出异常。
        If Not bReturn Then
            Throw New ApplicationException(" Set Internet Option Failed!")
        End If

        ' 通知系统注册值的改变，以便注册中的句柄能使用这些设定重读代理数据。
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                          IntPtr.Zero, 0)
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                          IntPtr.Zero, 0)

        Return bReturn
    End Function

    ''' <summary>
    ''' 使局域网的代理连接失效。.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DisableConnectionProxy() As Boolean
        ' 通过直接连接internet来建立连接。
        Dim Options(0) As INTERNET_PER_CONN_OPTION
        Options(0).dwOption = CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS)
        Options(0).Value.dwValue = CInt(INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_DIRECT)

        ' 将数据从一个受管理的类移到不受管理的内存中。
        Dim buffer As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options(0)))
        Marshal.StructureToPtr(Options(0), buffer, False)

        ' 初始化INTERNET_PER_CONN_OPTION_LIST数据结构的实体。
        Dim request As New INTERNET_PER_CONN_OPTION_LIST()

        ' 定义指向被申请的内存的指针。
        request.pOptions = buffer

        request.Size = Marshal.SizeOf(request)

        ' IntPtr为0表示局域网连接。
        request.Connection = IntPtr.Zero
        request.OptionCount = Options.Length
        request.OptionError = 0
        Dim size As Integer = Marshal.SizeOf(request)

        ' 为INTERNET_PER_CONN_OPTION_LIST实体申请内存。
        Dim intptrStruct As IntPtr = Marshal.AllocCoTaskMem(size)

        ' 将数据从一个受管理的类移到不受管理的内存中。
        Marshal.StructureToPtr(request, intptrStruct, True)

        ' 设定internet选项。
        Dim bReturn As Boolean =
            InternetSetOption(IntPtr.Zero,
                              INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION,
                              intptrStruct,
                              size)

        ' 释放内存
        Marshal.FreeCoTaskMem(buffer)
        Marshal.FreeCoTaskMem(intptrStruct)

        ' 当这个操作失败时抛出异常。
        If Not bReturn Then
            Throw New ApplicationException(" Set Internet Option Failed! ")
        End If

        ' 通知系统注册值的改变，以便注册中的句柄能使用这些设定重读代理数据。
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                          IntPtr.Zero, 0)
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                          IntPtr.Zero, 0)

        Return bReturn
    End Function

    ''' <summary>
    ''' 备份当前局域网连接的设置，确认恢复后内存被释放。
    ''' </summary>
    Public Shared Function BackupConnectionProxy() As INTERNET_PER_CONN_OPTION_LIST

        ' 查询以下选项
        Dim Options(2) As INTERNET_PER_CONN_OPTION

        Options(0) = New INTERNET_PER_CONN_OPTION()
        Options(0).dwOption =
            CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS)

        Options(1) = New INTERNET_PER_CONN_OPTION()
        Options(1).dwOption =
            CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER)

        Options(2) = New INTERNET_PER_CONN_OPTION()
        Options(2).dwOption =
            CInt(INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS)

        ' 为选项申请内存。
        Dim buffer As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options(0)) _
                                                      + Marshal.SizeOf(Options(1)) _
                                                      + Marshal.SizeOf(Options(2)))

        Dim current As IntPtr = CType(buffer, IntPtr)

        ' 将数据从一个受管理的类移到不受管理的内存中。
        For i As Integer = 0 To Options.Length - 1
            Marshal.StructureToPtr(Options(i), current, False)
            current = CType(CInt(current) + Marshal.SizeOf(Options(i)), IntPtr)
        Next i

        ' 初始化INTERNET_PER_CONN_OPTION_LIST数据结构的实体。
        Dim Request As New INTERNET_PER_CONN_OPTION_LIST()

        ' 定义指向被申请的内存的指针。
        Request.pOptions = buffer

        Request.Size = Marshal.SizeOf(Request)

        ' IntPtr为0表示局域网连接。
        Request.Connection = IntPtr.Zero

        Request.OptionCount = Options.Length
        Request.OptionError = 0
        Dim size As Integer = Marshal.SizeOf(Request)

        ' 查询internet设置。 
        Dim result As Boolean =
            InternetQueryOptionList(IntPtr.Zero,
                                    INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION,
                                    Request,
                                    size)
        If Not result Then
            Throw New ApplicationException(" Set Internet Option Failed! ")
        End If

        Return Request
    End Function

    ''' <summary>
    ''' 恢复局域网的设置
    ''' </summary>
    ''' <param name="request"></param>
    ''' <returns></returns>
    Public Shared Function RestoreConnectionProxy(ByVal request As INTERNET_PER_CONN_OPTION_LIST) As Boolean
        Dim size As Integer = Marshal.SizeOf(request)

        ' 申请内存 . 
        Dim intptrStruct As IntPtr = Marshal.AllocCoTaskMem(size)

        ' 转换数据结构到IntPtr 
        Marshal.StructureToPtr(request, intptrStruct, True)

        '设置internet选项。
        Dim bReturn As Boolean =
            InternetSetOption(IntPtr.Zero,
                              INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION,
                              intptrStruct,
                              size)

        ' 释放内存
        Marshal.FreeCoTaskMem(request.pOptions)
        Marshal.FreeCoTaskMem(intptrStruct)

        If Not bReturn Then
            Throw New ApplicationException(" Set Internet Option Failed! ")
        End If

        ' 通知系统注册值的改变，以便注册中的句柄能使用这些设定重读代理数据。
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                          IntPtr.Zero, 0)
        InternetSetOption(IntPtr.Zero, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                          IntPtr.Zero, 0)

        Return bReturn
    End Function
End Class
