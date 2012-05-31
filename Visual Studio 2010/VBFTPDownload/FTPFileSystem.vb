'*************************** Module Header ******************************'
' 模块名:  FTPFileSystem.vb
' 项目名:	    VBFTPDownload
' 版权(c)  Microsoft Corporation.
' 
'这个类表示远程FTP服务上的一个文件。当运行FTP LIST 协议方法来获得一个文件的详细列
'表时，这个服务将响应一些信息的记录。每一个记录代表一个文件，依赖于服务上的FTP目录列
'表的类型 

' 如该记录
' 1. MSDOS
'    1.1. Directory
'         12-13-10  12:41PM  <DIR>  Folder A
'    1.2. File
'         12-13-10  12:41PM  [Size] File B  
'         
'   注意: 日期段，如“12-13-10“而不是”12-13-2010“，如果年是四位数在IIS或者FTP服务中
'        是不被检查的。.
'        
' 2. UNIX
'    2.1. Directory
'         drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
'    2.2. File
'         -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
' 
'    注意: 日期段不包含年.
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
    ''' 起初记录的字符串.
    ''' </summary>
    Public Property OriginalRecordString() As String

    ''' <summary>
    ''' MSDOS or UNIX.
    ''' </summary>
    Public Property DirectoryListingStyle() As FTPDirectoryListingStyle

    ''' <summary>
    ''' 这个服务的路径.
    ''' </summary>
    Public Property Url() As Uri

    ''' <summary>
    '''  这个FTPFileSystem实例的名字.
    ''' </summary>
    Public Property Name() As String

    ''' <summary>
    ''' 指定这个FTPFileSystem实例是一个目录.
    ''' </summary>
    Public Property IsDirectory() As Boolean

    ''' <summary>
    '''  这个FTPFileSystem实例最后修改的时间.
    ''' </summary>
    Public Property ModifiedTime() As Date

    ''' <summary>
    ''' 如果它不是一个目录，这个 FTPFileSystem 实例的大小.
    ''' </summary>
    Public Property Size() As Integer

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 重写ToString方法来显示一个更友好的信息.
    ''' </summary>
    Public Overrides Function ToString() As String
        Return String.Format("{0}" & vbTab & "{1}" & vbTab & vbTab & "{2}",
                             Me.ModifiedTime.ToString("yyyy-MM-dd HH:mm"), 
                             If(Me.IsDirectory, "<DIR>", Me.Size.ToString()), Me.Name)
    End Function

    ''' <summary>
    ''' 从recordString中找到FTP目录列表的样式.
    ''' </summary>
    Public Shared Function GetDirectoryListingStyle(ByVal recordString As String) _
        As FTPDirectoryListingStyle
        Dim regex_Renamed As Regex =
            New System.Text.RegularExpressions.Regex("^[d-]([r-][w-][x-]){3}$")

        Dim header As String = recordString.Substring(0, 10)

        ' 如果这个样式是UNIX, 这个头部分如 "drwxrwxrwx".
        If regex_Renamed.IsMatch(header) Then
            Return FTPDirectoryListingStyle.UNIX
        Else
            Return FTPDirectoryListingStyle.MSDOS
        End If
    End Function

    ''' <summary>
    '''从recordString中获得一个FTPFileSystem. 
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

        '如果它是一个目录我们将 "/"添加到Url中
        fileSystem.Url = New Uri(baseUrl, fileSystem.Name _
                                 & (If(fileSystem.IsDirectory, "/", String.Empty)))

        Return fileSystem
    End Function

    ''' <summary>
    ''' 这个recordString是像：
    ''' 目录: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
    ''' 文件:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
    ''' 注意: 这个日期设置不能包括年
    ''' </summary>
    Shared Function ParseUNIXRecordString(ByVal recordString As String) As FTPFileSystem
        Dim fileSystem As New FTPFileSystem()

        fileSystem.OriginalRecordString = recordString.Trim()
        fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX

        ' 这个设置如 "drwxrwxrwx", "",  "", "1", "owner", "", "", "", 
        ' "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
        ' "0", "Dec", "13", "11:25", "Folder", "A".
        Dim segments() As String = fileSystem.OriginalRecordString.Split(" "c)

        Dim index As Integer = 0

        ' 这个权限设置如： "drwxrwxrwx".
        Dim permissionsegment As String = segments(index)

        '  如果这个属性第一个字母是 'd', 它意味着是一个目录.
        fileSystem.IsDirectory = permissionsegment.Chars(0) = "d"c

        ' 跳过空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        '  跳过这个部分的目录.

        ' 跳过这个空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 跳过这个部分的目录.

        '  跳过这个空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 跳过这组的部分.

        ' 跳过这个空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 如果这个文件流是一个文件, 它的大小大于0. 
        fileSystem.Size = Integer.Parse(segments(index))

        '  跳过这个空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        '  这个月的部分.
        Dim monthsegment As String = segments(index)

        ' 跳过这个空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 日期部分.
        Dim daysegment As String = segments(index)

        ' 跳过空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        '时间部分.
        Dim timesegment As String = segments(index)

        fileSystem.ModifiedTime =
            Date.Parse(String.Format("{0} {1} {2} ",
                                     timesegment, monthsegment, daysegment))

        ' 跳过空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 在原始字符串中计算文件名索引的部分.
        Dim filenameIndex As Integer = 0

        For i As Integer = 0 To index - 1
            '  在原始字符串中的"" 表示' '.
            If segments(i) = String.Empty Then
                filenameIndex += 1
            Else
                filenameIndex += segments(i).Length + 1
            End If
        Next i
        ' 这个名可能包含一些空字符.          
        fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()

        Return fileSystem
    End Function

    ''' <summary>
    ''' 12-13-10  12:41PM       <DIR>          Folder A
    ''' </summary>
    ''' <param name="recordString"></param>
    ''' <returns></returns>
    Shared Function ParseMSDOSRecordString(ByVal recordString As String) _
        As FTPFileSystem
        Dim fileSystem As New FTPFileSystem()

        fileSystem.OriginalRecordString = recordString.Trim()
        fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.MSDOS

        ' 这个部分如 "12-13-10",  "", "12:41PM", "", "","", "",
        ' "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
        Dim segments() As String = fileSystem.OriginalRecordString.Split(" "c)

        Dim index As Integer = 0

        ' 这个数据部分如 "12-13-10" instead of "12-13-2010" 如果年是
        ' 四位数就不在ISS中查找.
        Dim dateSegment As String = segments(index)
        Dim dateSegments() As String =
            dateSegment.Split(New Char() {"-"c}, StringSplitOptions.RemoveEmptyEntries)

        Dim month As Integer = Integer.Parse(dateSegments(0))
        Dim day As Integer = Integer.Parse(dateSegments(1))
        Dim year As Integer = Integer.Parse(dateSegments(2))

        ' 如果year大于等于50并且小雨100，将意味着这个年时19**
        If year >= 50 AndAlso year < 100 Then
            year += 1900

            ' 如果year小于50，它意味着年时20** 
        ElseIf year < 50 Then
            year += 2000
        End If

        ' 跳过空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        '时间部分.
        Dim timesegment As String = segments(index)

        fileSystem.ModifiedTime =
            Date.Parse(String.Format("{0}-{1}-{2} {3}", year, month, day, timesegment))

        '  跳过空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 这个部分的大小和目录.
        ' 如果这部分是 "<DIR>", 它意味着是一个目录,否则它是一个文件的大小

        Dim sizeOrDirSegment As String = segments(index)

        fileSystem.IsDirectory =
            sizeOrDirSegment.Equals("<DIR>", StringComparison.OrdinalIgnoreCase)

        ' 如果这个文件流是一个文件,这个大小是大于0的. 
        If Not fileSystem.IsDirectory Then
            fileSystem.Size = Integer.Parse(sizeOrDirSegment)
        End If

        '  掉过空的部分.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' 在原始字符串中计算文件名索引的部分.
        Dim filenameIndex As Integer = 0

        For i As Integer = 0 To index - 1
            ' 在原始字符串中计算文件名索引的部分.
            If segments(i) = String.Empty Then
                filenameIndex += 1
            Else
                filenameIndex += segments(i).Length + 1
            End If
        Next i
        ' 这个名可能包含一些空字符.          
        fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()

        Return fileSystem
    End Function
End Class