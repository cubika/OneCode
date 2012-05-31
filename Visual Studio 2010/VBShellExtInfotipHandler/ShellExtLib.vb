'****************************** 模块头 ******************************'
' 模块名称:  ShellExtLib.vb
' 项目名称:      VBShellExtInfotipHandler
' 版权(c) Microsoft Corporation.
' 
'本模块声明了 Shell interfaces 接口： IQueryInfo 和 
'执行注册和取消注册一个shell infotip事件处理函数

' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports Microsoft.Win32
Imports System.Runtime.InteropServices


#Region "Shell Interfaces"

<ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), _
Guid("00021500-0000-0000-c000-000000000046")> _
Friend Interface IQueryInfo
    ' GetInfoTip原始的签名是 
    ' HRESULT GetInfoTip(DWORD dwFlags, [out] PWSTR *ppwszTip);
    ' 根据需求,执行这个方法的应用程序必须为ppwszTip（CoTaskMemAlloc的一个对象）分配内存空间
    ' 当不在使用本应用程序的时候，要用CoTaskMemFree方法释放内存空间 
    ' 在这个例子中，首先设置PreserveSig的值为false（在com组件中的默认值）作为输出参数 ppwszTip的返回值
    '  同时指定一个字符串最为LPWStr的返回值。
    ' 在CLR的interop层，CoTaskMemAlloc 方法会分配内存空间 并且为.net 字符串指定内存。
    
    Function GetInfoTip(ByVal dwFlags As UInt32) _
        As <MarshalAs(UnmanagedType.LPWStr)> String

    Function GetInfoFlags() As Integer
End Interface

#End Region


#Region "Shell Registration"

Friend Class ShellExtReg

    ''' <summary>
    ''' 注册shell infotip 处理方法
    ''' </summary>
    ''' <param name="t">COM class</param>
    ''' <param name="fileType">
    ''' 文件类型如：“*” 代表所有类型，".txt" 代表所有.txt 文件
    ''' 参数不能为null 或者 空字符串 
    ''' </param>
    ''' <remarks>
    ''' 在执行本方法是会产生以下注册码：
    '''
    '''   HKCR
    '''   {
    '''      NoRemove &lt;File Type&gt;
    '''      {
    '''          NoRemove shellex
    '''          {
    '''              {00021500-0000-0000-C000-000000000046} = s '{&lt;CLSID&gt;}'
    '''          }
    '''      }
    '''   }
    ''' </remarks>
    Public Shared Sub RegisterShellExtInfotipHandler( _
        ByVal t As Type, _
        ByVal fileType As String)

        If String.IsNullOrEmpty(fileType) Then
            Throw New ArgumentException("文件类型不能为空")
        End If

        
        ' 如果文件类型以“.”开始，就试着去找包含ProgID的HKCR\<FIle Type>的默认值。
        If (fileType.StartsWith(".")) Then
            Using key As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileType)
                If (key IsNot Nothing) Then

                    ' 如果key存在并且默认值不空，就用ProgID作为文件的类型
                    Dim defaultVal As String = key.GetValue(Nothing)
                    If (Not String.IsNullOrEmpty(defaultVal)) Then
                        fileType = defaultVal
                    End If
                End If
            End Using
        End If

        ' 产生注册码，并设置它的默认值作为处理事件的CLSID
        ' HKCR\<File Type>\shellex\{00021500-0000-0000-C000-000000000046}, 

        Dim keyName As String = String.Format( _
            "{0}\shellex\{{00021500-0000-0000-C000-000000000046}}", fileType)
        Using key As RegistryKey = Registry.ClassesRoot.CreateSubKey(keyName)
            If (Not key Is Nothing) Then
                key.SetValue(Nothing, t.GUID.ToString("B"))
            End If
        End Using
    End Sub


    ''' <summary>
    ''' 取消注册shell infotip 处理方法
    ''' </summary>
    ''' <param name="t">COM class</param>
    ''' <param name="fileType">
    ''' 文件类型如：“*” 代表所有类型，".txt" 代表所有.txt 文件
    ''' 参数不能为null 或者 空字符串 
    ''' </param>
    ''' <remarks>
    ''' 执行本方法会删除以下注册码
    ''' HKCR\&lt;File Type&gt;\shellex\{00021500-0000-0000-C000-000000000046}.
    ''' </remarks>
    Public Shared Sub UnregisterShellExtInfotipHandler( _
        ByVal t As Type, _
        ByVal fileType As String)

        If String.IsNullOrEmpty(fileType) Then
            Throw New ArgumentException("文件类型不能为空")
        End If

       
        ' 如果文件类型以“.”开始，就试着去找包含ProgID的HKCR\<FIle Type>的默认值。
        If (fileType.StartsWith(".")) Then
            Using key As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileType)
                If (key IsNot Nothing) Then
                    ' 如果key存在并且默认值不空，就用ProgID作为文件的类型
                    Dim defaultVal As String = key.GetValue(Nothing)
                    If (Not String.IsNullOrEmpty(defaultVal)) Then
                        fileType = defaultVal
                    End If
                End If
            End Using
        End If

        ' 删除以下注册码
        ' HKCR\<File Type>\shellex\00021500-0000-0000-C000-000000000046.
        Dim keyName As String = String.Format( _
            "{0}\shellex\{{00021500-0000-0000-C000-000000000046}}", fileType)
        Registry.ClassesRoot.DeleteSubKeyTree(keyName, False)
    End Sub

End Class

#End Region

