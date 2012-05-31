'*************************** 模块头 ******************************'
' 模块名:  FTPFileSystem.vb
' 项目:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
'  FTPFileSystem 类代表一个远程FTP服务器文件，当运行FTP列表协议方法得到一个详细的文件列表在一个FTP服务器上，
'这个服务器将相应许多信息记录，每个记录代表一个文件，依靠服务器的FTP目录列表格式，记录如下：
' 1. MSDOS
'    1.1. Directory
'         12-13-10  12:41PM  <DIR>  Folder A
'    1.2. File
'         12-13-10  12:41PM  [Size] File B  
'         
'   NOTE: The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit
'         years is not checked in IIS.
'        
' 2. UNIX
'    2.1. Directory
'         drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
'    2.2. File
'         -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
' 
'    NOTE: The date segment does not contains year.
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

Imports System.Text.RegularExpressions
Imports System.Text

Public Class FTPFileSystem
    ''' <summary>
    ''' 原始字符串记录
    ''' </summary>
    Public Property OriginalRecordString() As String

    ''' <summary>
    ''' MSDOS or UNIX.
    ''' </summary>
    Public Property DirectoryListingStyle() As FTPDirectoryListingStyle

    ''' <summary>
    ''' 服务器地址
    ''' </summary>
    Public Property Url() As Uri

    ''' <summary>
    ''' FTPFileSystem 实例名字
    ''' </summary>
    Public Property Name() As String

    ''' <summary>
    ''' 指定是否FTPFileSystem 实例是一个目录
    ''' </summary>
    Public Property IsDirectory() As Boolean

    ''' <summary>
    ''' FTPFileSystem 实例最近修改的时间
    ''' </summary>
    Public Property ModifiedTime() As Date

    ''' <summary>
    ''' 假如不是一个目录FTPFileSystem 实例的大小
    ''' </summary>
    Public Property Size() As Integer

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 重写ToString（）方法展示更多友好信息.
    ''' </summary>
    Public Overrides Function ToString() As String
        Return String.Format("{0}" & vbTab & "{1}" & vbTab & vbTab & "{2}",
                             Me.ModifiedTime.ToString("yyyy-MM-dd HH:mm"),
                             If(Me.IsDirectory, "<DIR>", Me.Size.ToString()), Me.Name)
    End Function

    ''' <summary>
    ''' 从记录字符串中指出FTP Directory Listing Style
    ''' </summary>
    Public Shared Function GetDirectoryListingStyle(ByVal recordString As String) _
        As FTPDirectoryListingStyle
        Dim regex_Renamed As Regex =
            New System.Text.RegularExpressions.Regex("^[d-]([r-][w-][x-]){3}$")

        Dim header As String = recordString.Substring(0, 10)

        ' 如果类型是UNIX，开头如"drwxrwxrwx"
        If regex_Renamed.IsMatch(header) Then
            Return FTPDirectoryListingStyle.UNIX
        Else
            Return FTPDirectoryListingStyle.MSDOS
        End If
    End Function

    ''' <summary>
    ''' 从记录字符串得到一个FTPFileSystem
    ''' </summary>
    Public Shared Function ParseRecordString(ByVal baseUrl As Uri,
                                             ByVal recordString As String,
                                             ByVal type As FTPDirectoryListingStyle) _
                                         As FTPFileSystem
        Dim fileSystem As FTPFileSystem = Nothing

        If type = FTPDirectoryListingStyle.UNIX Then
            fileSystem = ParseUNIXRecordString(recordString)
        Else
            fileSystem = ParseMSDOSRecordString(recordString)
        End If

        ' 如果是目录加“/”到路径
        fileSystem.Url = New Uri(baseUrl, fileSystem.Name & (If(fileSystem.IsDirectory,
                                                                "/", String.Empty)))

        Return fileSystem
    End Function

    ''' <summary>
    ''' 记录字符串如下：
    ''' Directory: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
    ''' File:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
    ''' NOTE: The date segment does not contains year.
    ''' </summary>
    Shared Function ParseUNIXRecordString(ByVal recordString As String) As FTPFileSystem
        Dim fileSystem As New FTPFileSystem()

        fileSystem.OriginalRecordString = recordString.Trim()
        fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX

        ' The segments is like "drwxrwxrwx", "",  "", "1", "owner", "", "", "", 
        ' "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
        ' "0", "Dec", "13", "11:25", "Folder", "A".
        Dim segments() As String = fileSystem.OriginalRecordString.Split(" "c)

        Dim index As Integer = 0

        ' 许可部分如"drwxrwxrwx
        Dim permissionsegment As String = segments(index)

        ' 如果属性以“d”开始，意味是个目录
        fileSystem.IsDirectory = permissionsegment.Chars(0) = "d"c

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 跳过目录部分

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 跳过所有者部分

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 跳过组部分

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 如果文件流是个文件，大小大于0 
        fileSystem.Size = Integer.Parse(segments(index))

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 月份部分
        Dim monthsegment As String = segments(index)

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        '  天部分
        Dim daysegment As String = segments(index)

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 时间部分.
        Dim timesegment As String = segments(index)

        fileSystem.ModifiedTime =
            Date.Parse(String.Format("{0} {1} {2} ", timesegment, monthsegment, daysegment))

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 在原始字符串中计算文件名索引
        Dim filenameIndex As Integer = 0

        For i As Integer = 0 To index - 1
            '在初始字符中' represents ' 
            If segments(i) = String.Empty Then
                filenameIndex += 1
            Else
                filenameIndex += segments(i).Length + 1
            End If
        Next i
        ' 文件名可能包括许多部分因为名字包括''
        fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()

        Return fileSystem
    End Function

    ''' <summary>
    ''' 12-13-10  12:41PM       <DIR>          Folder A
    ''' </summary>
    ''' <param name="recordString"></param>
    ''' <returns></returns>
    Shared Function ParseMSDOSRecordString(ByVal recordString As String) As FTPFileSystem
        Dim fileSystem As New FTPFileSystem()

        fileSystem.OriginalRecordString = recordString.Trim()
        fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.MSDOS

        ' The segments is like "12-13-10",  "", "12:41PM", "", "","", "",
        ' "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
        Dim segments() As String = fileSystem.OriginalRecordString.Split(" "c)

        Dim index As Integer = 0

        ' 如果四位年在IIS中没被选中，日期部分像"12-13-10"代替"12-13-2010"
        Dim dateSegment As String = segments(index)
        Dim dateSegments() As String =
            dateSegment.Split(New Char() {"-"c}, StringSplitOptions.RemoveEmptyEntries)

        Dim month As Integer = Integer.Parse(dateSegments(0))
        Dim day As Integer = Integer.Parse(dateSegments(1))
        Dim year As Integer = Integer.Parse(dateSegments(2))

        ' 如果年大于50小于100是19**
        If year >= 50 AndAlso year < 100 Then
            year += 1900

            ' 如果年小于50是20**
        ElseIf year < 50 Then
            year += 2000
        End If

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        '  时间部分.
        Dim timesegment As String = segments(index)

        fileSystem.ModifiedTime =
            Date.Parse(String.Format("{0}-{1}-{2} {3}", year, month, day, timesegment))

        '跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 大小或目录部分，如果是“<DIR>”它意味一个目录，否则是文件大小。
        Dim sizeOrDirSegment As String = segments(index)

        fileSystem.IsDirectory =
            sizeOrDirSegment.Equals("<DIR>", StringComparison.OrdinalIgnoreCase)

        '如果fileSystem 是一个文件，大小大于0 
        If Not fileSystem.IsDirectory Then
            fileSystem.Size = Integer.Parse(sizeOrDirSegment)
        End If

        ' 跳过空部分
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 计算在原始字符串中文件名部分的索引
        Dim filenameIndex As Integer = 0

        For i As Integer = 0 To index - 1
            '在原始字符串中"" represents ' 
            If segments(i) = String.Empty Then
                filenameIndex += 1
            Else
                filenameIndex += segments(i).Length + 1
            End If
        Next i
        '文件名包括许多部分因为名字能包含' '  
        fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()

        Return fileSystem
    End Function
End Class
