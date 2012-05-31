/****************************** 模块头 ******************************\
* 模块名:  IMAGE_OPTIONAL_HEADER32.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 表示32位应用程序的可选头部格式.
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace CSCheckEXEType.IMAGE
{

    public struct IMAGE_OPTIONAL_HEADER32
    {
        /// <summary>
        /// 这是一个签名字, 表示这是哪种类型的头部. 
        /// 其最常用的两个值是 IMAGE_NT_OPTIONAL_HDR32_MAGIC 0x10b 和 
        /// IMAGE_NT_OPTIONAL_HDR64_MAGIC 0x20b.
        /// </summary>
        public UInt16 Magic;

        /// <summary>
        /// 这是曾生成这个可执行文件的链接器的主要版本. 对于来自于微软链接器的PE文件，
        /// 这个版本号对应着Visual Studio 的版本号
        /// (例如, 版本 6 表示 Visual Studio 6.0).
        /// </summary>
        public Byte MajorLinkerVersion;

        /// <summary>
        /// 这是曾生成这个可执行文件的链接器的主要版本.
        /// </summary>
        public Byte MinorLinkerVersion;

        /// <summary>
        /// 这是将所有节和IMAGE_SCN_CNT_CODE属性合并后的总大小.
        /// </summary>
        public UInt32 SizeOfCode;

        /// <summary>
        /// 这是所有初始化数据节的总大小.
        /// </summary>
        public UInt32 SizeOfInitializedData;

        /// <summary>
        /// 这是所有数据属性未初始化的节的大小. 这个字段 
        /// 常常是 0, 因为链接器能将未初始化的数据添加到
        /// 常规数据节的后面.
        /// </summary>
        public UInt32 SizeOfUninitializedData;

        /// <summary>
        /// 这是文件中第一个将被执行的代码字节的相对虚拟地址. 对于DLL, 
        /// 这个入口点将在进程初始化和关闭以及线程的创建和销毁的时候被调用.  
        /// 在绝大多数可执行文件里, 这个地址没有
        /// 直接指向 main, WinMain, 或者 DllMain函数. 不过, 它指向 
        /// 那些调用前面提到的函数的运行时库代码. 这个字段 
        /// 在DLL中可以被设成0, 并且以前的通知都不会被接受.
        /// 链接器 /NOENTRY 开关设置此字段位0.
        /// </summary>
        public UInt32 AddressOfEntryPoint;

        /// <summary>
        /// 这个指加载到内存中的第一个代码相对虚拟地址.
        /// </summary>
        public UInt32 BaseOfCode ;


        /// <summary>
        /// 理论上, 这是当数据加载到内存后的数据的第一个字节的相对偏移地址. 
        /// 但是, 这个值与不同版本的微软连接器不相关. 
        /// 此字段是不是64位可执行本.
        /// </summary>
        public UInt32 BaseOfData;

        /// <summary>
        /// 文件在内存中的首选加载地址. 如果可以，加载器将尝试加载这个地址的PE文件. 
        /// (也就是说, 如果没有其他什么占用这个块内存， 
        /// 它将正确对其在一个合法地址, 等等).
        /// 如果可执行文件是在这个地址加载的, 那么加载器可以跳过
        /// 基本重定向这一步(在本文第2部分有叙述). 
        /// 对EXE文件, 默认映像地址是0x400000. 对于DLL, 它是 0x10000000. 
        /// 这个映像地址可以在链接时用 /BASE 开关设置, 或者稍后用 
        /// REBASE 工具.
        /// </summary>
        public UInt32 ImageBase;

        /// <summary>
        /// 加载到内存中的节对齐. 该值必须大于等于文件中的对齐属性 
        /// (接下来会提到). 默认的对齐是 
        /// 目标CPU的页大小. 对于用户模式的执行本， 
        /// 为了在 Windows 9x 或者 Windows Me上运行, 最小的对齐大小是一页 (4KB).
        /// 该字段可以由链接器的 /ALIGN 开关设置.
        /// </summary>
        public UInt32 SectionAlignment;

        /// <summary>
        /// PE文件中的节的对齐. 对于 x86 执行本, 该值 
        /// 通常要么是 0x200 或者 0x1000. 默认值是随着微软链接器的版本改变的. 
        /// 这个值必须是2的幂, 并且如果节对齐值比CPU的页大小还小，
        /// 那么这个值必须和节对齐值一样.
        /// 这个链接器开关 /OPT:WIN98 会将x86的执行体的对齐 
        /// 设为 0x1000, 而 /OPT:NOWIN98 则设置成 0x200.
        /// </summary>
        public UInt32 FileAlignment;

        /// <summary>
        /// 这是所需系统的主要版本. 随着 
        /// 这么多WINDOWS版本的问世, 这个字段实际上已经成了无关紧要的了.
        /// </summary>
        public UInt16 MajorOperatingSystemVersion;

        /// <summary>
        /// 所需的OS的次要版本号.
        /// </summary>
        public UInt16 MinorOperatingSystemVersion;

        /// <summary>
        /// 文件主要版本号. 不会被系统用到并可以为0. 
        /// 它可由链接器开关 /VERSION 设置.
        /// </summary>
        public UInt16 MajorImageVersion;

        /// <summary>
        /// 文件次要版本号.
        /// </summary>
        public UInt16 MinorImageVersion;

        /// <summary>
        /// 运行这个可执行文件的子操作系统所需要的主要版本号. 
        /// 曾一度它被用来表明所需的用户接口是更新的WINDOWS95还是Windows NT4.0，
        /// 用以区别老版本Windows NT 接口. 
        /// 现在, 由于各种Windows版本的扩散， 
        /// 这个字段虽然有效但系统不再使用并且它通常设置成4. 
        /// 链接器用这个 /SUBSYSTEM 开关来设置其值.
        /// </summary>
        public UInt16 MajorSubsystemVersion;

        /// <summary>
        /// 运行这个可执行文件的子操作系统所需要的次要版本号.
        /// </summary>
        public UInt16 MinorSubsystemVersion;

        /// <summary>
        /// 另一个从来没启用的字段. 通常是0.
        /// </summary>
        public UInt32 Win32VersionValue;

        /// <summary>
        /// SizeOfImage 包含了相对虚拟地址，这个地址被赋给了最后一节后面的那一节 
        /// (如果存在的话).
        /// 这是当加载文件到内存里，系统需要保留的有效内存量. 
        /// 这个字段必须是节对齐的倍数.
        /// </summary>
        public UInt32 SizeOfImage;

        /// <summary>
        /// 这是MS-DOS头部，PE头部和节表的总共大小. 
        /// 所有的这些项都会在PE文件里的任何代码或数据之前出现.  
        /// 此字段的值被舍入到一个文件的多个对齐. 
        /// </summary>
        public UInt32 SizeOfHeaders;

        /// <summary>
        /// 映像的校验. IMAGEHLP.DLL中的CheckSumMappedFile API 可以 
        /// 计算出这个值. kernel-mode 驱动和一些系统DLL需要该校验. 
        /// 否则, 该字段可以为0. 
        /// 使用 /RELEASE 链接器开关可以将校验放在文件里.
        /// </summary>
        public UInt32 CheckSum;


        /// <summary>
        /// 这是一个枚举值，它表是该可执行文件需要什么子系统（用户界面类型） . 
        /// 这个字段只对EXE重要. 包括的值有:
        /// IMAGE_SUBSYSTEM_NATIVE       // 映像不需要子系统
        /// IMAGE_SUBSYSTEM_WINDOWS_GUI  // 使用Windows图形用户界面
        /// IMAGE_SUBSYSTEM_WINDOWS_CUI  // 以控制台应用程序模式运行.
		///                              // 当运行时, 操作系统为它创建一个控制台窗口，
		///                              // 并且提供 stdin,
		///                              // stdout, 和 stderr 文件句柄
        ///
        /// </summary>
        public UInt16 Subsystem;

        /// <summary>
        /// 表示这个DLL特点的标志. 其对应的值形如 
        /// IMAGE_DLLCHARACTERISTICS_xxx 字段. 可用的值有:
        /// IMAGE_DLLCHARACTERISTICS_NO_BIND		// 不生成这个映像
        /// IMAGE_DLLCHARACTERISTICS_WDM_DRIVER		// 以WDM 模式驱动
        /// IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE // 当终端服务器加载一个不是
		///                                                // Terminal- Services-aware
		///                                                //  的应用程序的时候, it
		///                                                // 它也加载一个包含兼容代码的
		///                                                // 动态链接库.
        /// </summary>
        public UInt16 DllCharacteristics;

        /// <summary>
        /// 这是在EXE文件中, 进程中的最初的线程可以增长的最大大小. 
        /// 默认是1MB. 并非所有的内存最初都会被提交.
        /// </summary>
        public UInt32 SizeOfStackReserve;

        /// <summary>
        /// 这是在EXE文件中, 最初用于栈的内存总量. 
        /// 这个值默认是 4KB.
        /// </summary>
        public UInt32 SizeOfStackCommit;

        /// <summary>
        /// 这是在EXE文件中, 默认进程最初的保留大小. 
        /// 改值默认 1MB . 但是当前版本的Windows中, 
        /// 在没有用户干预的情况下，堆可以增长到超出这个大小.
        /// </summary>
        public UInt32 SizeOfHeapReserve;

        /// <summary>
        /// 这是在EXE文件中, 堆提交的内存大小. 
        /// 默认情况下, 该值为 4KB.
        /// </summary>
        public UInt32 SizeOfHeapCommit;

        /// <summary>
        /// 该值过时了.
        /// </summary>
        public UInt32 LoaderFlags;

        /// <summary>
        ///  该值是在 IMAGE_NT_HEADERS 结构体中的 
        ///  一个 IMAGE_DATA_DIRECTORY 结构体类型的数组. 该字段 还包含了 这个数组中实体的数量.
        ///  从最早的Windows NT版本以来，这个字段的值就是16
        /// </summary>
        public UInt32 NumberOfRvaAndSizes;

    }
}
