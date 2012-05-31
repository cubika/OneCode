=============================================================================
        动态连接库 : CppDynamicLinkLibrary 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

一个动态链接库（DLL）是一个包含函数和可以被其他模块（应用程序或DLL）使用的数据
的模块。这个Win32 DLL代码示例展示了导出数据，函数和类。

DLL代码示例导出数据，函数和类

    // 全局数据
    int g_nVal1
    int g_nVal2

    // 普通函数
    int __cdecl GetStringLength1(PCWSTR pszString);
    int __stdcall GetStringLength2(PCWSTR pszString);

    // 回调函数
    int __stdcall Max(int a, int b, PFN_COMPARE cmpFunc)

    // 类
    class CSimpleObject
    {
    public:
        CSimpleObject(void);  // 构造函数
        virtual ~CSimpleObject(void);  // 析构函数
          
        // 属性
        float get_FloatProperty(void);
        void set_FloatProperty(float newVal);

        // 方法
        HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

        // 静态方法
        static int GetStringLength(PCWSTR pszString);

    private:
        float m_fField;
    };
两种方法可以从示例DLL中到处标记

1. 用.DEF 文件导出标记

一个模块定义(.DEF)文件是一个包含了一个或者多个模块描述DLL各种数据的文本文件。
当编译DLL时，可以创建和使用一个.DEF文件。使用这种方法，我们可以从DLL按序号而
不是按名称导出函数。
 

2. 用 __declspec(dllexport)标记导出

__declspec(dllexport)添加了导出指令添加到对象文件，这样我们就可以不需要.DEF文件。
当试图导出带修饰的C++函数名称时，这种是最便利的方法。


/////////////////////////////////////////////////////////////////////////////
代码关系:
(当前实例和其他实例之间的关系在Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppImplicitlyLinkDll -> CppDynamicLinkLibrary
CppImplicitlyLinkDll 隐式链接（静态加载）DLL，并使用它的标记

CppDelayloadDll -> CppDynamicLinkLibrary
CppDelayloadDll 延迟加载DLL并使用它的标记。

CppLoadLibrary -> CppDynamicLinkLibrary
CppLoadLibrary 动态加载DLL并使用它的标记。

CSLoadLibrary -> CppDynamicLinkLibrary
.NET可执行文件CSLoadLibrary,动态加载本地DLL并使用它的标记。通过API：LoadLibrary, GetProcAddress and FreeLibrary.

CSPInvokeDll -> CppDynamicLinkLibrary
.NET可执行文件CSPInvokeDll通过P/Invoke，动态加载本地DLL并使用它的标记。.


/////////////////////////////////////////////////////////////////////////////
演示:

A. 创建项目

步骤1. 在Visual Studio 2008，创建一个 Visual C++ / Win32 / Win32 项目，并命名
为 “CppDynamicLinkLibrary"。

步骤2. 在WIn32应用程序向导的应用程序设置页面中，选择应用程序类型为DLL，并点击
输出标记选项。点击完成。

B. 用.DEF文件从DLL中导出标记。
http://msdn.microsoft.com/en-us/library/d91k01sh.aspx
一个模块定义(.DEF)文件是一个包含了一个或者多个模块描述DLL各种数据的文本文件。
它提供了关于导出，属性，和其他与被链接程序相关的信息的链接器信息。

步骤1. 在头文件中声明需要被导出的数据和方法，并且在Cpp文件中定义它们。

    int g_nVal1;
    int /*__cdecl*/ GetStringLength1(PCWSTR pszString)
    int __stdcall GetStringLength1(PCWSTR pszString)
    int __stdcall Max(int a, int b, PFN_COMPARE cmpFunc)

步骤2. 添加一个名字为CppDynamicLinkLibrary的.DEF文件。在文件中的第一条语句必须是
LIBRARY语句。这条语句定义了.DEF文件时属于一个DLL。LIBRARY语句遵循DLL的名字。链接
器在DLL的导入库中辨认这个名字。接着，EXPORTS语句罗列了由DLL导出的名称和可选
的数据和函数。

    LIBRARY   CppDynamicLinkLibrary
    EXPORTS
       GetStringLength1     @1
       Max					@2
       g_nVal1				DATA

步骤3. 为了能够使链接器导入.DEF文件到DLL项目中，右键点击项目，打开它的属性对话框。
在链接器/输入页面设置模块定义（/DEF）的值为"CppDynamicLinkLibrary.def".

C. 用__declspec(dllexport)标记导出

步骤1. 在头文件创建一个以下的块#ifdef，使得从DLL导入导出更简单（这里应该是自动添加，
如果你选择”导出标记“选项，当你创建项目时）。所有在DLL中的文件都需要用定义在命令行中的
CPPDYNAMICLINKLIBRARY_EXPORTS标记来编译（详见 项目属性中的C/C++ /预处理页面）。这个
标记不应该被定义在其他需要使用这个DLL的项目中。这样那些其他项目的源文件包含这个文件
可以看见SYMBOL_DECLSPEC函数被从一个DLL中引入，尽管这个DLL看见标记被定义在这个宏定义中。

	#ifdef CPPDYNAMICLINKLIBRARY_EXPORTS
	#define SYMBOL_DECLSPEC __declspec(dllexport)
	#else
	#define SYMBOL_DECLSPEC __declspec(dllimport)
	#endif

步骤2. 在头文件中声明被导出的数据，函数和类。添加SYMBOL_DECLSPEC在签名中。对于这些数据
和函数访问可能从C语言模块或动态的任何链接可执行文件，添加在开始EXTERN_C（即用extern“C“）
来指定C链接器。这消除了C++类型安全的命名（又叫做名饰）。初始化数据和执行的。cpp文件中的
函数和类。

    EXTERN_C SYMBOL_DECLSPEC int g_nVal2;
    EXTERN_C SYMBOL_DECLSPEC int /*__cdecl*/ GetStringLength2(PCWSTR pszString);
    EXTERN_C SYMBOL_DECLSPEC int __stdcall GetStringLength2(PCWSTR pszString);
    class SYMBOL_DECLSPEC CSimpleObject
    {
        ...
    };


/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: Exporting from a DLL
http://msdn.microsoft.com/en-us/library/z4zxe9k8.aspx

MSDN: Exporting from a DLL Using DEF Files
http://msdn.microsoft.com/en-us/library/d91k01sh.aspx

MSDN: Exporting from a DLL Using __declspec(dllexport)
http://msdn.microsoft.com/en-us/library/a90k134d.aspx

MSDN: Creating and Using a Dynamic Link Library (C++)
http://msdn.microsoft.com/en-us/library/ms235636.aspx

HOWTO: How To Export Data from a DLL or an Application
http://support.microsoft.com/kb/90530

Dynamic-link library
http://en.wikipedia.org/wiki/Dynamic_link_library


/////////////////////////////////////////////////////////////////////////////