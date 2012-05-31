'***************************** 模块头 *************************************\
' 模块名:  ReadOnlyIStreamWrapper.vb
' 项目名:  VBOneNoteRibbonAddIn
' 版权 (c) Microsoft Corporation.
'
' IStream 包装类
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


#Region "引用的命名空间"

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Runtime.InteropServices.ComTypes
Imports System.IO
#End Region


''' <summary>
''' IStream 包装类
''' </summary>
Friend Class ReadOnlyIStreamWrapper
    Implements IStream
    ' _stream 字段
    Private _stream As Stream

    ''' <summary>
    ''' CCOMStreamWrapper 构造器
    ''' </summary>
    ''' <param name="streamWrap">stream</param>
    Public Sub New(ByVal streamWrap As Stream)
        Me._stream = streamWrap
    End Sub

    ' 总结:
    '     创建一个新的流对象,该流对象有其自己的查找指针,该指针引用了相同的字节作为
    '     原始流.
    '
    ' 参数:
    '   ppstm:
    '     当该方法发返回时,装有新的流对象.该参数被传递时没有初始化.
    Public Sub Clone(ByRef ppstm As IStream) Implements IStream.Clone
        ppstm = New ReadOnlyIStreamWrapper(Me._stream)
    End Sub

    '
    ' 总结:
    '     确保在事务处理模式下,打开的流对象所做的任何更改都能反映在父级存储.
    '
    ' 参数:
    '   grfCommitFlags:
    '     一个值，用于控制流对象的更改是如何达到的.
    Public Sub Commit(ByVal grfCommitFlags As Integer) Implements IStream.Commit
        Me._stream.Flush()
    End Sub

    '
    ' 总结:
    '      从当前查找指针的流中,复制指定的数目的字节,到当前查找指针的另一个流中.
    '
    ' 参数:
    '   pstm:
    '     指向目的流的引用.
    '
    '   cb:
    '     要复制的源流中的字节数.
    '
    '   pcbRead:
    '     成功返回时，包含实际从源读取的字节数.
    '
    '   pcbWritten:
    '     成功返回时,包含实际写到目的流的字节数.
    Public Sub CopyTo(ByVal pstm As IStream, ByVal cb As Long, ByVal pcbRead As IntPtr, _
                      ByVal pcbWritten As IntPtr) Implements IStream.CopyTo
        Throw New NotSupportedException("ReadOnlyIStreamWrapper 不支持 CopyTo")
    End Sub

    '
    ' 总结:
    '    限制对流中指定字节范围的访问.
    '
    ' 参数:
    '   libOffset:
    '     范围的开始的的字节偏移量.
    '
    '   cb:
    '     以字节为单位，限制的范围的长度.
    '
    '   dwLockType:
    '     访问该范围的请求限制.
    Public Sub LockRegion(ByVal libOffset As Long, ByVal cb As Long, ByVal dwLockType As Integer) _
        Implements IStream.LockRegion
        Throw New NotSupportedException("ReadOnlyIStreamWrapper 不支持 LockRegion")
    End Sub

    '
    ' 总结:
    '     从当前查找指针处的开始的内存中读取指定数目的字节.
    '
    ' 参数:
    '   pv:
    '     W此方法返回时，包含从流中读取的数据.此参数被传递时未初始化.
    '
    '   cb:
    '     要从流对象中读取的字节数.
    '
    '   pcbRead:
    '     指向 ULONG 变量的指针, 该变量接收从流对象中读取的实际字节数.
    Public Sub Read(ByVal pv As Byte(), ByVal cb As Integer, ByVal pcbRead As IntPtr) Implements IStream.Read
        System.Runtime.InteropServices.Marshal.WriteInt64(pcbRead, CLng(Me._stream.Read(pv, 0, cb)))
    End Sub

    '
    ' 总结:
    '     自最后一次调用
    '     System.Runtime.InteropServices.ComTypes.IStream.Commit(System.Int32) 
    '     起放弃所有更改的事务处理流 
    Public Sub Revert() Implements IStream.Revert
        Throw New NotSupportedException("ReadOnlyIStreamWrapper 不支持 Revert")
    End Sub

    '
    ' 总结:
    '     更改查找指针到一个新位置,与流的始、末或者当前的查找指针相关.
    '
    ' 参数:
    '   dlibMove:
    '     将添加到 dwOrigin 的位移.
    '
    '   dwOrigin:
    '     查找的初始位置.该起始可以是文件的开端、当前查找指针或者是文件的末端.
    '
    '   plibNewPosition:
    '     成功返回时，包含查找指针从流的开头的偏移量.
    Public Sub Seek(ByVal dlibMove As Long, ByVal dwOrigin As Integer, ByVal plibNewPosition As IntPtr) _
        Implements IStream.Seek

        Dim num As Long = 0L
        System.Runtime.InteropServices.Marshal.WriteInt64(plibNewPosition, Me._stream.Position)
        Select Case dwOrigin
            Case 0
                num = dlibMove
                Exit Select

            Case 1
                num = Me._stream.Position + dlibMove
                Exit Select

            Case 2
                num = Me._stream.Length + dlibMove
                Exit Select
            Case Else

                Return
        End Select
        If (num >= 0L) AndAlso (num < Me._stream.Length) Then
            Me._stream.Position = num
            System.Runtime.InteropServices.Marshal.WriteInt64(plibNewPosition, Me._stream.Position)
        End If
    End Sub

    '
    ' 总结:
    '     更改流对象的大小.
    '
    ' 参数:
    '   libNewSize:
    '     新的流的字节数的大小.
    Public Sub SetSize(ByVal libNewSize As Long) Implements IStream.SetSize
        Me._stream.SetLength(libNewSize)
    End Sub

    '
    ' 总结:
    '    检索此流的 System.Runtime.InteropServices.STATSTG 结构.
    '
    ' 参数:
    '   pstatstg:
    '     此方法返回时，包含 STATSTG 结构，用于描述此流对象.此参数被传递时未初始化.
    '
    '   grfStatFlag:
    '      此方法不返回的 STATSTG 结构的成员,从而节省一些内存分配操作.
    Public Sub Stat(ByRef pstatstg As STATSTG, ByVal grfStatFlag As Integer) Implements IStream.Stat
        pstatstg = New STATSTG()
        pstatstg.cbSize = Me._stream.Length
        If (grfStatFlag And 1) = 0 Then
            pstatstg.pwcsName = Me._stream.ToString()
        End If
    End Sub

    '
    ' 总结:
    '      删除先前用
    '      System.Runtime.InteropServices.ComTypes.IStream.LockRegion(System.Int64,System.Int64,System.Int32) 
    '      方法限制的字节范围的访问限制.      
    '      
    ' 参数:
    '      libOffset:
    '         字节范围开始的偏移量.
    '
    '      cb:
    '         以字节为单位，限制的范围的长度.
    '
    '      dwLockType:
    '         先前置于该范围的访问限制.
    Public Sub UnlockRegion(ByVal libOffset As Long, ByVal cb As Long, ByVal dwLockType As Integer) _
        Implements IStream.UnlockRegion

        Throw New NotSupportedException("ReadOnlyIStreamWrapper 不支持 UnlockRegion")
    End Sub

    '
    ' 总结:
    '     写入指定数量的字节到从当前查找指针开始的流对象.
    '
    ' 参数:
    '   pv:
    '     要写入此流的缓冲区.
    '
    '   cb:
    '     要写入流的字节数.
    '
    '   pcbWritten:
    '     成功返回时，包含实际写入此流对象的字节数. 如果调用方将该指针设置为 
    '     System.IntPtr.Zero，此方法不提供实际写入的字节数.
    Public Sub Write(ByVal pv As Byte(), ByVal cb As Integer, ByVal pcbWritten As IntPtr) _
        Implements IStream.Write

        System.Runtime.InteropServices.Marshal.WriteInt64(pcbWritten, 0L)
        Me._stream.Write(pv, 0, cb)
        System.Runtime.InteropServices.Marshal.WriteInt64(pcbWritten, CLng(cb))
    End Sub
End Class
