/****************************** 模块头 ************************************\
模块名:  ReadOnlyIStreamWrapper.cs
项目名:  CSOneNoteRibbonAddIn
版权 (c) Microsoft Corporation.

IStream 包装类

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region 引用的命名空间

using System;
using System.Runtime.InteropServices.ComTypes;
using System.IO; 
#endregion

namespace CSOneNoteRibbonAddIn
{
    /// <summary>
    /// IStream 包装类
    /// </summary>
    internal class ReadOnlyIStreamWrapper : IStream
    {
        // _stream 字段
        private Stream _stream;

        /// <summary>
        /// CCOMStreamWrapper 构造器
        /// </summary>
        /// <param name="streamWrap">stream</param>
        public ReadOnlyIStreamWrapper(Stream streamWrap)
        {
            this._stream = streamWrap;
        }

        // 总结:
        //     创建一个新的流对象,该流对象有其自己的查找指针,该指针引用了相同的字节作为
        //     原始流.
        //
        // 参数:
        //   ppstm:
        //     当该方法发返回时,装有新的流对象.该参数被传递时没有初始化.
        public void Clone(out IStream ppstm)
        {
            ppstm = new ReadOnlyIStreamWrapper(this._stream);
        }

        //
        // 总结:
        //     确保在事务处理模式下,打开的流对象所做的任何更改都能反映在父级存储.
        //
        // 参数:
        //   grfCommitFlags:
        //     一个值，用于控制流对象的更改是如何达到的.
        public void Commit(int grfCommitFlags)
        {
            this._stream.Flush();
        }

        //
        // 总结:
        //     从当前查找指针的流中,复制指定的数目的字节,到当前查找指针的另一个流中.
        //
        // 参数:
        //   pstm:
        //     指向目的流的引用.
        //
        //   cb:
        //     	要复制的源流中的字节数.
        //
        //   pcbRead:
        //     	成功返回时，包含实际从源读取的字节数.
        //
        //   pcbWritten:
        //     成功返回时,包含实际写到目的流的字节数.
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotSupportedException("ReadOnlyIStreamWrapper 不支持 CopyTo");
        }

        //
        // 总结:
        //    限制对流中指定字节范围的访问.
        //
        // 参数:
        //   libOffset:
        //     范围的开始的的字节偏移量.
        //
        //   cb:
        //     以字节为单位，限制的范围的长度.
        //
        //   dwLockType:
        //     访问该范围的请求限制.
        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException("ReadOnlyIStreamWrapper 不支持 CopyTo");
        }

        //
        // 总结:
        //     从当前查找指针处的开始的内存中读取指定数目的字节.
        //
        // 参数:
        //   pv:
        //     此方法返回时，包含从流中读取的数据.此参数被传递时未初始化.
        //
        //   cb:
        //     	要从流对象中读取的字节数.
        //
        //   pcbRead:
        //     指向 ULONG 变量的指针, 该变量接收从流对象中读取的实际字节数.
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            System.Runtime.InteropServices.Marshal.WriteInt64(pcbRead, (long)this._stream.Read(pv, 0, cb));
        }

        //
        // 总结:
        //     自最后一次调用
        //     System.Runtime.InteropServices.ComTypes.IStream.Commit(System.Int32) 
        //     起放弃所有更改的事务处理流 
        public void Revert()
        {
            throw new NotSupportedException("Stream 不支持 CopyTo");
        }

        //
        // 总结:
        //     更改查找指针到一个新位置,与流的始、末或者当前的查找指针相关.
        //
        // 参数:
        //   dlibMove:
        //     将添加到 dwOrigin 的位移.
        //
        //   dwOrigin:
        //     查找的初始位置.该起始可以是文件的开端、当前查找指针或者是文件的末端.
        //
        //   plibNewPosition:
        //    成功返回时，包含查找指针从流的开头的偏移量.
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            long num = 0L;
            System.Runtime.InteropServices.Marshal.WriteInt64(plibNewPosition, this._stream.Position);
            switch (dwOrigin)
            {
                case 0:
                    num = dlibMove;
                    break;

                case 1:
                    num = this._stream.Position + dlibMove;
                    break;

                case 2:
                    num = this._stream.Length + dlibMove;
                    break;

                default:
                    return;
            }
            if ((num >= 0L) && (num < this._stream.Length))
            {
                this._stream.Position = num;
                System.Runtime.InteropServices.Marshal.WriteInt64(plibNewPosition, this._stream.Position);
            }

        }

        //
        // 总结:
        //     更改流对象的大小.
        //
        // 参数:
        //   libNewSize:
        //     新的流的字节数的大小.
        public void SetSize(long libNewSize)
        {
           this._stream.SetLength(libNewSize);
        }

        //
        // 总结:
        //     检索此流的 System.Runtime.InteropServices.STATSTG 结构.
        //
        // 参数:
        //   pstatstg:
        //     此方法返回时，包含 STATSTG 结构，用于描述此流对象.此参数被传递时未初始化.
        //
        //   grfStatFlag:
        //     此方法不返回的 STATSTG 结构的成员,从而节省一些内存分配操作.
        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new STATSTG();
            pstatstg.cbSize = this._stream.Length;
            if ((grfStatFlag & 1) == 0)
            {
                pstatstg.pwcsName = this._stream.ToString();
            }
        }

        //
        // 总结:
        //     删除先前用
        //     System.Runtime.InteropServices.ComTypes.IStream.LockRegion(System.Int64,System.Int64,System.Int32) 
        //     方法限制的字节范围的访问限制.      
        //
        // 参数:
        //   libOffset:
        //     字节范围开始的偏移量.
        //
        //   cb:
        //     以字节为单位，限制的范围的长度.
        //
        //   dwLockType:
        //      先前置于该范围的访问限制.
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException("ReadOnlyIStreamWrapper 不支持 UnlockRegion");
        }

        //
        // 总结:
        //     写入指定数量的字节到从当前查找指针开始的流对象.
        //
        // 参数:
        //   pv:
        //     	要写入此流的缓冲区.
        //
        //   cb:
        //     要写入流的字节数.
        //
        //   pcbWritten:
        //     成功返回时，包含实际写入此流对象的字节数. 如果调用方将该指针设置为 
        //     System.IntPtr.Zero，此方法不提供实际写入的字节数.
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            System.Runtime.InteropServices.Marshal.WriteInt64(pcbWritten, 0L);
            this._stream.Write(pv, 0, cb);
            System.Runtime.InteropServices.Marshal.WriteInt64(pcbWritten, (long)cb);
        }
    }
}
