=============================================================================
      动态链接库 : CppCLINativeDllWrapper 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个C++/CLI代码示例展示了利用C++/CLI封装类，使得.NET代码可以调用本地C++ DLL模块中
导出的类和方法。

  CSCallNativeDllWrapper/VBCallNativeDllWrapper (任意.NET 应用)
          -->
      CppCLINativeDllWrapper (C++/CLI 封装)
              -->
          CppDynamicLinkLibrary (本地C++ DLL 模块)
在此次代码示例中，CSimpleObjectWrapper类封装了本地C++类CSimpleObject，NativeMethods类
封装了被CppDynamicLinkLibrary.dll导出的全局函数。

当涉及到与本地模块相互操作时，这些被Visual C++/CLI支持的能共同使用的特性提供了一个比其他
.NET语言更大的优势。除了传统的明确的P/Invoke，C++/CLI允许隐式P/Invoke，这也被称为C++ Interop，
或者几乎隐式的混合了托管代码和本地代码的It Just Work（IJW）。这些特性提供了更好的安全性，更
方便地代码，更强劲的编程以及更能减少对本地API的修改。如果本地代码是可用的并且允许任何.NET应用
去通过封装访问本地C++的类和方法，你可以使用技术去编译封装本地C++类和方法。


/////////////////////////////////////////////////////////////////////////////
代码关系:
(当前实例和其他实例之间的关系在Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppCLINativeDllWrapper -> CppDynamicLinkLibrary
C++/CLI示例模块CppCLINativeDllWrapper封装了被本地C++示例模块CppDynamicLinkLibrary导出的
类和方法。封装的类被任意.NET代码间接地调用本地C++的类和方法。


/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在Visual Studio 2008中，创建一个Visual C++ / CLR / 类库项目，并命名为
CppCLINativeDllWrapper。使用项目向导生成一个默认的空C++/CLI项目：

    namespace CppCLINativeDllWrapper {

	    public ref class Class1
	    {
		    // 在此添加类的方法
	    };
    }

步骤2. 引用本地C++ DLL CppDynamicLinkLibrary.

  操作1. 你需要在项目属性 / 链接器 / 输入 / 附加目录中输入LIB文件名，从而导入DLL所产生的LIB
  文件。你可以在属性 / 链接器 / 常规 / 附加库目录中配置LIB文件的搜索路径

  操作2. 从项目快捷菜单中选择引用。在属性页面对话框中，展开常规属性节点，选择引用，再选择添加
  新引用。。。按键。在添加引用对话框中，罗列了所有你可以引用的类库。在项目标签页中罗列所有当前
  的解决方案和它们所包含的类库。如果CppDynamicLinkLibrary项目在当前解决方案中，选择CppDynamicLinkLibrary
  在点击确定

步骤3. 引入定义了类和方法的头文件

    #include "CppDynamicLinkLibrary.h"

你可以在属性 / C/C++ / 常规 / 附加引用目录中配置头文件的搜索路径

步骤4. 设计封装本地C++类CSimpleObject的C++/CLI类CSimpleObjectWrapper。

    public ref class CSimpleObjectWrapper
    {
    public:
        CSimpleObjectWrapper(void);
        virtual ~CSimpleObjectWrapper(void);

        // 属性
        property float FloatProperty
        {
            float get(void);
            void set(float value);
        }

        // 方法
        virtual String ^ ToString(void) override;

        // 静态方法
        static int GetStringLength(String ^ str);

    protected:
        !CSimpleObjectWrapper(void);

    private:
        CSimpleObject *m_impl;
    };

类封装了一个本地C++类CSimpleObject的实例。这个实例被私有成员变量m_impl的地址所指向。
它在构造函数CSimpleObjectWrapper(void)中初始化。在析构函数(virtual ~CSimpleObjectWrapper(void);)
中释放，在(!CSimpleObjectWrapper(void);)中终结。

    CSimpleObjectWrapper::CSimpleObjectWrapper(void)
    {
        m_impl = new CSimpleObject();
    }
    
    CSimpleObjectWrapper::~CSimpleObjectWrapper(void)
    {
        if (m_impl)
        {
            delete m_impl;
            m_impl = NULL;
        }
    }

    CSimpleObjectWrapper::!CSimpleObjectWrapper(void)
    {
        if (m_impl)
        {
            delete m_impl;
            m_impl = NULL;
        }
    }

CSimpleObjectWrapper的公共成员属性和方法封装了本地C++类中的属性和方法。
它们重新使用调用它们通过CSSimpleObject对象的指针m_impl重新使用CSSimpleObject的调用。
marshaling类型在托管和本地代码中产生。

    float CSimpleObjectWrapper::FloatProperty::get(void)
    {
        return m_impl->get_FloatProperty();
    }

    void CSimpleObjectWrapper::FloatProperty::set(float value)
    {
        m_impl->set_FloatProperty(value);
    }

    String ^ CSimpleObjectWrapper::ToString(void)
    {
        wchar_t szStr[100];
        HRESULT hr = m_impl->ToString(szStr, ARRAYSIZE(szStr));
        if (FAILED(hr))
        {
            Marshal::ThrowExceptionForHR(hr);
        }
        // 转换 PWSTR 到 System::String 并返回.
        return gcnew String(szStr);
    }

    int CSimpleObjectWrapper::GetStringLength(System::String ^ str)
    {
        // 转换 System::String 到 PCWSTR, 并调用C++函数.
        marshal_context ^ context = gcnew marshal_context();
        PCWSTR pszString = context->marshal_as<const wchar_t*>(str);
        int length = CSimpleObject::GetStringLength(pszString);
        delete context;
        return length;
    }

步骤5. 设计封装本地C++ DLL模块导出函数的C++/CLI的类NativeMethods。

    /// <summary>
    /// 回调函数'PFN_COMPARE'  
    /// </summary>
    /// <remarks>
    /// 委托类型有 UnmanagedFunctionPointerAttribute. 使用这个特性，你可以定义 
    /// 调用本地函数指针类型的转换。在本地API的头文件中，回调函数 PFN_COMPARE 
    /// 被定义为__stdcall (CALLBACK)。所以这里我们可以使用
    /// CallingConvention::StdCall.
    /// </remarks>
    [UnmanagedFunctionPointer(CallingConvention::StdCall)]
    public delegate int CompareCallback(int a, int b);


    /// <summary>
    /// C++/CLI类封装了本地C++模块CppDynamicLinkLibrary.dll所导出的函数。
    /// </summary>
    public ref class NativeMethods
    {
    public:
        static int GetStringLength1(String ^ str);
        static int GetStringLength2(String ^ str);
        static int Max(int a, int b, CompareCallback ^ cmpFunc);
    };
所有在NativeMethods中的方法被定义为静态为了能够被CppDynamicLinkLibrary导出为全局函数。
它们重新调用本地DLL

    int NativeMethods::GetStringLength1(String ^ str)
    {
        // 转换 System::String 到 PCWSTR, 并调用C++函数.
        marshal_context ^ context = gcnew marshal_context();
        PCWSTR pszString = context->marshal_as<const wchar_t*>(str);
        int length = ::GetStringLength1(pszString);
        delete context;
        return length;
    }

    int NativeMethods::GetStringLength2(String ^ str)
    {
        // 转换 System::String 到 PCWSTR, 并调用C++函数.
        marshal_context ^ context = gcnew marshal_context();
        PCWSTR pszString = context->marshal_as<const wchar_t*>(str);
        int length = ::GetStringLength2(pszString);
        delete context;
        return length;
    }

    int NativeMethods::Max(int a, int b, CompareCallback ^ cmpFunc)
    {
        // 转换委托给一个函数指针
        IntPtr pCmpFunc = Marshal::GetFunctionPointerForDelegate(cmpFunc);
        return ::Max(a, b, static_cast<::PFN_COMPARE>(pCmpFunc.ToPointer()));
    }


/////////////////////////////////////////////////////////////////////////////
参考:

Using C++ Interop (Implicit PInvoke)
http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx

How to: Wrap Native Class for Use by C#
http://msdn.microsoft.com/en-us/library/ms235281.aspx


/////////////////////////////////////////////////////////////////////////////