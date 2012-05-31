/********************************* 模块头 *********************************\
* 模块名:      NativeMethod.cs
* 项目名:      CSCreateLowIntegrityProcess
* 版权 (c) Microsoft Corporation.
* 
* 一些在此项目中使用的关于Native API P/Invoke注释
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
#endregion


namespace CSCreateLowIntegrityProcess
{
    /// <summary>
    /// TOKEN_INFORMATION_CLASS枚举类型定义了获取或设定访问令牌时使用的信息
    /// </summary>
    internal enum TOKEN_INFORMATION_CLASS
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUIAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        MaxTokenInfoClass
    }

    /// <summary>
    /// SECURITY_IMPERSONATION_LEVEL枚举定义了安全身份模拟级别。身份模拟级别
    /// 定义了一个服务器进程可以扮演一个客户端进程的度量机制。
    /// </summary>
    internal enum SECURITY_IMPERSONATION_LEVEL
    {
        SecurityAnonymous, 
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
    }

    /// <summary>
    /// TOKEN_TYPE枚举类型定义了主令牌及模拟令牌
    /// </summary>
    internal enum TOKEN_TYPE
    {
        TokenPrimary = 1,
        TokenImpersonation
    }

    /// <summary>
    /// 这个结构体描述了一个安全标识符（SID）和它的属性。SID被用于描述
    /// 唯一的用户或组
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SID_AND_ATTRIBUTES
    {
        public IntPtr Sid;
        public UInt32 Attributes;
    }

    /// <summary>
    /// 这个结构描述了令牌的完整性等级。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_MANDATORY_LABEL
    {
        public SID_AND_ATTRIBUTES Label;
    }

    /// <summary>
    /// 创建时主窗口时定义的窗体，桌面，标准句柄以及表现形式。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    /// <summary>
    /// 包括新创建的进程和主线程的信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    /// <summary>
    /// 表示一个令牌句柄的wrapper类
    /// </summary>
    internal class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        internal SafeTokenHandle(IntPtr handle)
            : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethod.CloseHandle(base.handle);
        }
    }

    internal class NativeMethod
    {
        // 令牌所指定的访问权限

        public const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const UInt32 STANDARD_RIGHTS_READ = 0x00020000;
        public const UInt32 TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const UInt32 TOKEN_DUPLICATE = 0x0002;
        public const UInt32 TOKEN_IMPERSONATE = 0x0004;
        public const UInt32 TOKEN_QUERY = 0x0008;
        public const UInt32 TOKEN_QUERY_SOURCE = 0x0010;
        public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const UInt32 TOKEN_ADJUST_GROUPS = 0x0040;
        public const UInt32 TOKEN_ADJUST_DEFAULT = 0x0080;
        public const UInt32 TOKEN_ADJUST_SESSIONID = 0x0100;
        public const UInt32 TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        public const UInt32 TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE |
            TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES |
            TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID);


        public const Int32 ERROR_INSUFFICIENT_BUFFER = 122;


        // 完整性级别

        public const Int32 SECURITY_MANDATORY_UNTRUSTED_RID = 0x00000000;
        public const Int32 SECURITY_MANDATORY_LOW_RID = 0x00001000;
        public const Int32 SECURITY_MANDATORY_MEDIUM_RID = 0x00002000;
        public const Int32 SECURITY_MANDATORY_HIGH_RID = 0x00003000;
        public const Int32 SECURITY_MANDATORY_SYSTEM_RID = 0x00004000;

        
        // 组相关的SID信息

        public const UInt32 SE_GROUP_MANDATORY = 0x00000001;
        public const UInt32 SE_GROUP_ENABLED_BY_DEFAULT = 0x00000002;
        public const UInt32 SE_GROUP_ENABLED = 0x00000004;
        public const UInt32 SE_GROUP_OWNER = 0x00000008;
        public const UInt32 SE_GROUP_USE_FOR_DENY_ONLY = 0x00000010;
        public const UInt32 SE_GROUP_INTEGRITY = 0x00000020;
        public const UInt32 SE_GROUP_INTEGRITY_ENABLED = 0x00000040;
        public const UInt32 SE_GROUP_LOGON_ID = 0xC0000000;
        public const UInt32 SE_GROUP_RESOURCE = 0x20000000;
        public const UInt32 SE_GROUP_VALID_ATTRIBUTES = (SE_GROUP_MANDATORY |
            SE_GROUP_ENABLED_BY_DEFAULT | SE_GROUP_ENABLED | SE_GROUP_OWNER |
            SE_GROUP_USE_FOR_DENY_ONLY | SE_GROUP_LOGON_ID | SE_GROUP_RESOURCE |
            SE_GROUP_INTEGRITY | SE_GROUP_INTEGRITY_ENABLED);


        /// <summary>
        /// 这个函数打开一个进程所属的访问令牌。
        /// </summary>
        /// <param name="hProcess">
        /// 已打开的访问令牌的线程的句柄。
        /// </param>
        /// <param name="desiredAccess">
        /// 指定一个访问标记。它用于表示访问令牌的访问请求类型。
        /// </param>
        /// <param name="hToken">
        /// 此函数返回时输出一个新开的访问令牌的句柄。
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            IntPtr hProcess,
            UInt32 desiredAccess, 
            out SafeTokenHandle hToken);


        /// <summary>
        /// DuplicateTokenEx函数以现有令牌创建一个新的访问令牌。这个函数既可以
        /// 创建一个主访问令牌也可以创建一个模拟令牌
        /// </summary>
        /// <param name="hExistingToken">
        /// 以TOKEN_DUPLICATE打开的访问令牌的句柄。
        /// </param>
        /// <param name="desiredAccess">
        /// 指定新令牌的访问权限
        /// </param>
        /// <param name="pTokenAttributes">
        /// 一个指向SECURITY_ATTRIBUTES的指针，其为新令牌指定一个安全标示符并
        /// 指出其子进程时候可以继承令牌。如果lpTokenAttributes为空，此令牌获
        /// 得一个默认的安全标示符并且此句柄不能被继承。
        /// </param>
        /// <param name="ImpersonationLevel">
        /// 指定一个安全身份模拟级别的枚举类型。此类型将应用于新建的令牌。
        /// </param>
        /// <param name="TokenType">
        /// TokenPrimary - 新令牌是一个主令牌。你可以在CreateProcessAsUser函数
        /// 中使用此令牌。
        /// TokenImpersonation - 新令牌是一个模拟令牌。
        /// </param>
        /// <param name="hNewToken">
        /// 获得一个新令牌。
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateTokenEx(
            SafeTokenHandle hExistingToken,
            UInt32 desiredAccess, 
            IntPtr pTokenAttributes,
            SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
            TOKEN_TYPE TokenType, 
            out SafeTokenHandle hNewToken);


        /// <summary>
        /// 这个函数用于获取一个访问令牌的信息。调用此函数的进程必须拥有适当的
        /// 权限才能得到此信息。
        /// </summary>
        /// <param name="hToken">
        /// 获取信息的访问进程的句柄
        /// </param>
        /// <param name="tokenInfoClass">
        /// 指定一个TOKEN_INFORMATION_CLASS枚举中的一个值。它将被用于指定获取
        /// 信息的种类
        /// </param>
        /// <param name="pTokenInfo">
        /// 用于接收信息的缓存的指针
        /// </param>
        /// <param name="tokenInfoLength">
        /// 指定TokenInformation缓存的大小（以字节计算）
        /// </param>
        /// <param name="returnLength">
        /// 用于记录TokenInformation缓存接收到字节数的变量的指针
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            SafeTokenHandle hToken,
            TOKEN_INFORMATION_CLASS tokenInfoClass,
            IntPtr pTokenInfo,
            Int32 tokenInfoLength,
            out Int32 returnLength);


        /// <summary>
        /// 此函数为一个指定的访问令牌设定了不同的信息类型。此信息用于替换
        /// 现有信息。调用进程必须拥有合适的访问权限以便设置此信息
        /// </summary>
        /// <param name="hToken">
        /// 用于设置信息的访问令牌的句柄。
        /// </param>
        /// <param name="tokenInfoClass">
        /// TOKEN_INFORMATION_CLASS枚举中的一个值。它指定了此函数所指定的
        /// 信息类型。
        /// </param>
        /// <param name="pTokenInfo">
        /// 一个指向设置到此访问令牌的信息缓存的指针
        /// </param>
        /// <param name="tokenInfoLength">
        /// 指定TokenInformation缓存的长度（以字节表示）
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetTokenInformation(
            SafeTokenHandle hToken,
            TOKEN_INFORMATION_CLASS tokenInfoClass,
            IntPtr pTokenInfo,
            Int32 tokenInfoLength);


        /// <summary>
        /// 此函数返回一个在安全标识符（SID）中的指定的次级授权的指针。 它的
        /// 值是一个相对标识符（RID）。
        /// </summary>
        /// <param name="pSid">
        /// 一个指向用于返回次级授权的SID结构体的指针。
        /// </param>
        /// <param name="nSubAuthority">
        /// 用于指定一个索引值。该索引值用于指定在次级授权列表中需要返回的元素
        /// </param>
        /// <returns>
        /// 如果函数调用成功，返回值是一个指向次级授权的指针。可以调用GetLastError获取
        /// 额外详细的错误信息。如果此函数调用失败，返回值则没有定义。如果指定的SID结构
        /// 不合法或则次级授权的索引值越界，则此函数调用失败。
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetSidSubAuthority(
            IntPtr pSid, 
            UInt32 nSubAuthority);


        /// <summary>
        /// 函数ConvertStringSidToSid转换一个字符串形式的安全标示符到一个合法
        /// 的安全标示符。你可以使用此函数从ConvertSidToStringSid函数转换的字
        /// 符串形式的标示符中获得一个SID
        /// </summary>
        /// <param name="StringSid">用于转换的字符串形式的安全标示符</param>
        /// <param name="Sid">
        /// 指向一个用于接收成功转换的SID变量的指针。调用LocalFree函数以便释放
        /// 返回的缓存
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertStringSidToSid(
            string StringSid,
            out IntPtr Sid);


        /// <summary>
        /// 此函数返回一个合法的安全标示符的长度（以字节计算）
        /// </summary>
        /// <param name="pSID">
        /// 一个指向返回的SID结构的指针
        /// </param>
        /// <returns>
        /// 如果此SID结构是合法的，返回值则为结构的长度（以字符计算）
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetLengthSid(IntPtr pSID);


        /// <summary>
        /// 创建一个新进程及其主线程。新的进程使用指定的用户安全令牌。
        /// </summary>
        /// <param name="hToken">
        /// 主用户令牌的句柄
        /// </param>
        /// <param name="applicationName">
        /// 需要被执行的模块的名字
        /// </param>
        /// <param name="commandLine">
        /// 需要执行的命令行。此字符串的最大长度为32K个字符
        /// </param>
        /// <param name="pProcessAttributes">
        /// 一个指向SECURITY_ATTRIBUTES结构体的指针。他为新的进程指定了一个
        /// 安全标示符，并且指明是否子进程能够继承此进程的句柄。
        /// </param>
        /// <param name="pThreadAttributes">
        /// 一个指向SECURITY_ATTRIBUTES结构体的指针。他为新的线程指定了一个
        /// 安全标示符，并且指明是否子线程能够继承此进程的句柄。
        /// </param>
        /// <param name="bInheritHandles">
        /// 如果这个变量为ture， 当前调用进程中每一个可继承的句柄都将被新进程
        /// 继承。反之，这些句柄则不被继承。
        /// </param>
        /// <param name="dwCreationFlags">
        /// 此标识控制了创建新进程的优先等级。
        /// </param>
        /// <param name="pEnvironment">
        /// 指向一个新进程的环境设定的指针。
        /// </param>
        /// <param name="currentDirectory">
        /// 进程当前文件夹的完整路径。
        /// </param>
        /// <param name="startupInfo">
        /// 表示STARTUPINFO结构体。
        /// </param>
        /// <param name="processInformation">
        /// 输出一个PROCESS_INFORMATION结构体。 它包含了新进程的具体信息。
        /// </param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcessAsUser(
            SafeTokenHandle hToken,
            string applicationName,
            string commandLine,
            IntPtr pProcessAttributes,
            IntPtr pThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr pEnvironment,
            string currentDirectory,
            ref STARTUPINFO startupInfo,
            out PROCESS_INFORMATION processInformation);


        /// <summary>
        /// 关闭一个已打开的句柄。
        /// </summary>
        /// <param name="handle">已打开的对象的合法句柄</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
    }

    /// <summary>
    /// 已知文件夹路径
    /// </summary>
    internal class KnownFolder
    {
        private static readonly Guid LocalAppDataGuid = new Guid(
            "F1B32785-6FBA-4FCF-9D55-7B8E7F157091");
        public static string LocalAppData
        {
            get { return SHGetKnownFolderPath(LocalAppDataGuid); }
        }

        private static readonly Guid LocalAppDataLowGuid = new Guid(
            "A520A1A4-1780-4FF6-BD18-167343C5AF16");
        public static string LocalAppDataLow
        {
            get { return SHGetKnownFolderPath(LocalAppDataLowGuid); }
        }


        /// <summary>
        /// 通过文件夹的KNOWNFOLDERID获得已知文件夹的完整路径。
        /// </summary>
        /// <param name="rfid">
        /// 一个指定文件夹KNOWNFOLDERID的引用。
        /// </param>
        /// <returns></returns>
        public static string SHGetKnownFolderPath(Guid rfid)
        {
            IntPtr pPath = IntPtr.Zero;
            string path = null;
            try
            {
                int hr = SHGetKnownFolderPath(rfid, 0, IntPtr.Zero, out pPath);
                if (hr != 0)
                {
                    throw Marshal.GetExceptionForHR(hr);
                }
                path = Marshal.PtrToStringUni(pPath);
            }
            finally
            {
                if (pPath != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pPath);
                    pPath = IntPtr.Zero;
                }
            }
            return path;
        }


        /// <summary>
        /// 通过文件夹的KNOWNFOLDERID获取已知文件夹的完整路径。
        /// </summary>
        /// <param name="rfid">
        /// 一个指定文件夹KNOWNFOLDERID的引用。
        /// </param>
        /// <param name="dwFlags">
        /// 特殊获取选项
        /// </param>
        /// <param name="hToken">
        /// 一个作为指定用户的访问令牌。如果此参数为空，此函数以当前用户查询
        /// 已知文件夹信息。
        /// </param>
        /// <param name="pszPath">
        /// 当此方法返回时，它包含了一个指向已知文件夹路径的字符串的指针。调用此
        /// 函数的进程需要负责在其不被使用时调用CoTaskMemFree释放此资源。
        /// </param>
        /// <returns>HRESULT</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            out IntPtr pszPath);
    }
}