================================================================================
	   Windows应用程序: CSCheckEXEType 概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要:
这个实例演示了如何查看一个可执行文件的类型. 对于给定的EXE或者DLL文件,
本实例做如下检测:

1. 通过PE中子系统标记确定EXE类型(控制台或者窗体程序, 也可以是其他EXE类型)
2. 是.NET程序集吗? 
    如果不是, 
        - 检测该EXE的位数（32位还是64位）
    如果是, 
        - 检测该EXE的位数（Any CPU，32位，64位）
        - 检测其.NET编译运行时版本 
        - 打印程序集全名
		(例如： System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL)
   
////////////////////////////////////////////////////////////////////////////////
演示:
步骤1. 在VS2010中生成这个项目.

步骤2. 运行 VBCheckEXEType.exe. 这个应用程序将显示如下帮助文本信息.
请键入EXE文件路径:
<直接回车退出>

步骤3. 键入一个有效的Native可执行文件路径，并按下回车键，如： 
       "D:\NativeConsole32.exe", 你会看到如下信息.

控制台应用程序: 是
.NET应用程序: 否
32位 应用程序: 是

步骤4. 键入一个有效的.NET可执行文件路径，并按下回车键，如：
       "D:\NetWinFormAnyCPU.exe", 你会看到如下信息.

控制台应用程序: 否
.NET应用程序: 是
.NET编译运行时: v4.0.30319
全名: NetWinFormAnyCPU, Version=1.0.0.0, Culture=neutral, PublicKeyToken=neutral, processorArchitecture=MSIL


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

首先, 创建一个名为ExecutableFile的类，其代表一个可执行文件。它可以这个映像文件中获取这个映像文件头部, 
可选映像文件头部 和数据目录. 
根据这些头部, 我们能知道这是否是一个控制台应用程序, 是否是一个.NET应用程序以及是否是32位Native应用程序. 

注意: 32位和64位应用程序的 IMAGE_OPTIONAL_HEADER 结构体是不同的. 
      其不同点是64位应用程序没有 BaseOfData 字段，并且这些字段
      ImageBase/SizeOfStackReserve/SizeOfStackCommit/SizeOfHeapReserve/SizeOfHeapCommit 的数据类型是 UInt64. 

其次, 为了生成.NET 应用程序的全显示名, 我们可以使用 fusion API. 

  Char[] buffer = new Char[1024];

  // 获取 IReferenceIdentity 接口.
  Fusion.IReferenceIdentity referenceIdentity =
     Fusion.NativeMethods.GetAssemblyIdentityFromFile(ExeFilePath,
     ref Fusion.NativeMethods.ReferenceIdentityGuid) as Fusion.IReferenceIdentity;
  Fusion.IIdentityAuthority IdentityAuthority = Fusion.NativeMethods.GetIdentityAuthority();  
  
  IdentityAuthority.ReferenceToTextBuffer(0, referenceIdentity, 1024, buffer);

  string fullName = new string(buffer);

第三，检测已编译的.NET运行时的版本，我们可以使用宿主API。

  object metahostInterface=null;
  Hosting.NativeMethods.CLRCreateInstance(
      ref Hosting.NativeMethods.CLSID_CLRMetaHost,
      ref Hosting.NativeMethods.IID_ICLRMetaHost, 
      out metahostInterface);

  if (metahostInterface == null || !(metahostInterface is Hosting.IClrMetaHost))
  {
      throw new ApplicationException("不能得到IClrMetaHost接口.");
  }

  Hosting.IClrMetaHost ClrMetaHost = metahostInterface as Hosting.IClrMetaHost;
  StringBuilder buffer=new StringBuilder(1024);
  uint bufferLength=1024;          
  ClrMetaHost.GetVersionFromFile(this.ExeFilePath, buffer, ref bufferLength);
  string runtimeVersion = buffer.ToString(); 
/////////////////////////////////////////////////////////////////////////////
参考:

An In-Depth Look into the Win32 Portable Executable File Format
http://msdn.microsoft.com/en-us/magazine/cc301805.aspx

Exploring pe file headers using managed code
http://blogs.msdn.com/b/kstanton/archive/2004/03/31/105060.aspx

Getting the full display name of an assembly given the path to the manifest file
http://blogs.msdn.com/b/junfeng/archive/2005/09/13/465373.aspx

IReferenceIdentity Interface
http://msdn.microsoft.com/en-us/library/ms231949.aspx

IIdentityAuthority Interface
http://msdn.microsoft.com/en-us/library/ms231265(VS.80).aspx

GetIdentityAuthority Function
http://msdn.microsoft.com/en-us/library/ms231607(VS.80).aspx

GetAssemblyIdentityFromFile Function
http://msdn.microsoft.com/en-us/library/ms230508.aspx

CLRCreateInstance Function
http://msdn.microsoft.com/en-us/library/dd537633.aspx

ICLRMetaHost::GetVersionFromFile Method
http://msdn.microsoft.com/en-us/library/dd233127.aspx