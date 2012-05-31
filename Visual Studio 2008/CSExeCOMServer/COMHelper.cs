/********************************* 模块头 **********************************\
* 模块名:      COMHelper.cs
* 项目名:      CSExeCOMServer
* 版权 (c) Microsoft Corporation.
* 
* COMHelper提供了用于注册和注销COM服务器的helper函数。它还封装了一些在.NET环境下
* 使用的Native COM API。
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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
#endregion


internal class COMHelper
{
    /// <summary>
    /// 注册组件为本地服务器
    /// </summary>
    /// <param name="t"></param>
    public static void RegasmRegisterLocalServer(Type t)
    {
        GuardNullType(t, "t");   // 检查参数

        // 打开组件的CLSID键值
        using (RegistryKey keyCLSID = Registry.ClassesRoot.OpenSubKey(
            @"CLSID\" + t.GUID.ToString("B"), /*可写*/true))
        {
            // 在注册之后移除自动生成的InprocServer32键值
            // (REGASM将其设置在这里，但是我们要制作进程外的服务器组件)
            keyCLSID.DeleteSubKeyTree("InprocServer32");

            // 在CLSID键中创建"LocalServer32"
            using (RegistryKey subkey = keyCLSID.CreateSubKey("LocalServer32"))
            {
                subkey.SetValue("", Assembly.GetExecutingAssembly().Location,
                    RegistryValueKind.String);
            }
        }
    }

    /// <summary>
    /// 注销组件
    /// </summary>
    /// <param name="t"></param>
    public static void RegasmUnregisterLocalServer(Type t)
    {
        GuardNullType(t, "t");   //检查参数

        // 删除组件的CLSID键
        Registry.ClassesRoot.DeleteSubKeyTree(@"CLSID\" + t.GUID.ToString("B"));
    }

    private static void GuardNullType(Type t, String param)
    {
        if (t == null)
        {
            throw new ArgumentException("The CLR type must be specified.", param);
        }
    }
}

internal class COMNative
{
    /// <summary>
    /// CoInitializeEx()用于设置每个线程的单元模式。
    /// </summary>
    /// <param name="pvReserved">必须为空（NULL）</param>
    /// <param name="dwCoInit">
    /// 该线程的并发模式和初始化选项
    /// </param>
    /// <returns></returns>
    [DllImport("ole32.dll")]
    public static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

    /// <summary>
    /// CoUninitialize() 用于卸载一个COM线程。
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern void CoUninitialize();

    /// <summary>
    /// 如果注册了一个拥有OLE的EXE类对象，它就可以被其他程序连接。EXE对象程序需要
    /// 在启动时执行CoRegisterClassObject。并且它仍然需要组册其内部对象以供相同的
    /// EXE程序或这个程序使用的其他代码块，例如动态链接库（DLL）。
    /// </summary>
    /// <param name="rclsid">用于注册的CLSID</param>
    /// <param name="pUnk">
    /// 指向可以被发布的类对象上的IUnknow接口。
    /// </param>
    /// <param name="dwClsContext">
    /// 可执行代码的主体内容。
    /// </param>
    /// <param name="flags">
    /// 如何连接至类对象。
    /// </param>
    /// <param name="lpdwRegister">
    /// 指向一个用于确认该类对象已经被注册的值。
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// 在CoRegisterClassObject中使用P/Invoke注册COM对象是不被支持的。
    /// </remarks>
    [DllImport("ole32.dll")]
    public static extern int CoRegisterClassObject(
        ref Guid rclsid,
        [MarshalAs(UnmanagedType.Interface)] IClassFactory pUnk,
        CLSCTX dwClsContext,
        REGCLS flags,
        out uint lpdwRegister);

    /// <summary>
    /// 注意：OLE是一个类对象。在此之前它和CoRegisterClassObject函数一起注册。
    /// 现在已经不需要再使用了。
    /// </summary>
    /// <param name="dwRegister">
    /// CoRegisterClassObject函数之前所返回的标识变量
    /// </param>
    /// <returns></returns>
    [DllImport("ole32.dll")]
    public static extern UInt32 CoRevokeClassObject(uint dwRegister);

    /// <summary>
    /// 一个服务器可以注册多个类对象时，此函数被执行。其用于通知SCM关于所有的已注
    /// 册类和允许所有这些类对象的激活请求。
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// 此服务器可以使用CoResumeClassObjects而注册多个类对象。 在第一次执行
    /// CoRegisterClassObject后，为每一个服务器支持的CLSID指定
    /// REGCLS_LOCAL_SERVER或REGCLS_SUSPENDED。这个函数使OLE通知SCM所有的已注册类
    /// 以及在服务器进程中开始激活请求。
    /// 
    /// 此方法中，无论多少个已注册的CLSID，对SCM仅使用一次执行代码， 这大大缩短了
    /// 总体注册的时间和服务器启动时间。另外一个好处是，如果服务器有多重单元，即不
    /// 同的已注册CLSID在不同的单元中，或则是无限制的线程服务器， 在服务器执行
    /// CoResumeClassObjects之前不会有新的激活请求。这使得服务器在不得不处理激活和
    /// 关闭请求之前注册所有其CLSID和正常启动。
    /// </remarks>
    [DllImport("ole32.dll")]
    public static extern int CoResumeClassObjects();

    /// <summary>
    /// IClassFactory的接口ID
    /// </summary>
    public const string IID_IClassFactory =
        "00000001-0000-0000-C000-000000000046";

    /// <summary>
    /// IUnknown的接口ID
    /// </summary>
    public const string IID_IUnknown =
        "00000000-0000-0000-C000-000000000046";

    /// <summary>
    /// IDispatch的接口ID
    /// </summary>
    public const string IID_IDispatch =
        "00020400-0000-0000-C000-000000000046";

    /// <summary>
    /// 此类不支持聚合（aggregation）或则远程操作此类对象
    /// </summary>
    public const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);

    /// <summary>
    /// 没所支持的接口
    /// </summary>
    public const int E_NOINTERFACE = unchecked((int)0x80004002);
}

/// <summary>
/// 所有在系统注册表中注册的和拥有你所分配CLSID的类必须实现这个接口。
/// 这样这个类的对象才能被创建。
/// http://msdn.microsoft.com/en-us/library/ms694364.aspx
/// </summary>
[ComImport(), ComVisible(false), 
InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
Guid(COMNative.IID_IClassFactory)]
internal interface IClassFactory
{
    /// <summary>
    /// 创建一个卸载对象
    /// </summary>
    /// <param name="pUnkOuter"></param>
    /// <param name="riid">
    /// 此引用变量用于确认这个接口被用于和新创建的对象间的通讯。如果pUnkOuter为NULL， 
    /// 这个变量为初始化接口时所频繁使用的IID。
    /// </param>
    /// <param name="ppvObject">
    /// 用于接收于riid中请求的接口指针的指针变量的地址。
    /// </param>
    /// <returns>S_OK为成功。</returns>
    [PreserveSig]
    int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

    /// <summary>
    /// 锁定内存中已打开的对象应用程序。
    /// </summary>
    /// <param name="fLock">
    /// 如果为TRUE，增加锁定计数器；
    /// 如果为FALSE，减少锁定计数器。
    /// </param>
    /// <returns>S_OK为成功。</returns>
    [PreserveSig]
    int LockServer(bool fLock);
}

/// <summary>
/// 在CLSCTX枚举中的值是用于在在执行激活时确认在一个对象中那一段可执行代码被执行。
/// 这些值还被用于执行CoRegisterClassObject时确认在一个有效的类对象中请求构造实例的
/// 可执行代码块。
/// </summary>
[Flags]
internal enum CLSCTX : uint
{
    INPROC_SERVER = 0x1,
    INPROC_HANDLER = 0x2,
    LOCAL_SERVER = 0x4,
    INPROC_SERVER16 = 0x8,
    REMOTE_SERVER = 0x10,
    INPROC_HANDLER16 = 0x20,
    RESERVED1 = 0x40,
    RESERVED2 = 0x80,
    RESERVED3 = 0x100,
    RESERVED4 = 0x200,
    NO_CODE_DOWNLOAD = 0x400,
    RESERVED5 = 0x800,
    NO_CUSTOM_MARSHAL = 0x1000,
    ENABLE_CODE_DOWNLOAD = 0x2000,
    NO_FAILURE_LOG = 0x4000,
    DISABLE_AAA = 0x8000,
    ENABLE_AAA = 0x10000,
    FROM_DEFAULT_CONTEXT = 0x20000,
    ACTIVATE_32_BIT_SERVER = 0x40000,
    ACTIVATE_64_BIT_SERVER = 0x80000
}

/// <summary>
/// 在REGCLS枚举中定义了用于CoRegisterClassObject中控制连接类型的值。
/// 这个值用于确定如何连接到一个类对象的连接方法。
/// </summary>
[Flags]
internal enum REGCLS : uint
{
    SINGLEUSE = 0,
    MULTIPLEUSE = 1,
    MULTI_SEPARATE = 2,
    SUSPENDED = 4,
    SURROGATE = 8,
}