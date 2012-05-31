=============================================================================
      动态链接库 : CppCLINETAssemblyWrapper 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

文件中的代码声明了C++的类CSSimpleObjectWrapper封装在.NET类库CSClassLibrary中定义
的类CSSimpleObject。本地的C++应用程序可以引用这个封装类并链接DLL去间接调用.NET类。

  CppCallNETAssemblyWrapper (本地C++应用程序)
          -->
      CppCLINETAssemblyWrapper (C++/CLI 封装)
              -->
          CSClassLibrary (.NET 程序集)


/////////////////////////////////////////////////////////////////////////////
实例关系:
(当前实例和其他实例之间的关系在Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppCLINETAssemblyWrapper -> CSClassLibrary
C++/CLI 实例模块CppCLINETAssemblyWrapper封装了在C#类库CSClassLibrary中定义的.NET类。
这个封装类可以被任意本地C++应用程序调用，从而间接操作.NET类。

/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在Visual Studio 2008中，创建一个Visual C++ / CLR / 类库项目并命名为CppCLINETAssemblyWrapper。
项目导向创建一个默认空的C++/CLI类：

    namespace CppCLINativeDllWrapper {

	    public ref class Class1
	    {
		    // 在此为您的类添加方法
	    };
    }

步骤2. 在C++/CLI类库项目中应用CSClassLibrary。

步骤3. 为配置输C++/CLI类库出标记。这个标记可以被导入并被本地C++应用程序调用。

在项目预处理定义中添加CPPCLINETASSEMBLYWRAPPER_EXPORTS(请参考 C/C++ / 预处理界面
在项目属性页面中）。所有在DLL中的文件都需要在CPPCLINETASSEMBLYWRAPPER_EXPORTS标记下编译。
这个标记不应该被定义在任何使用这个DLL的项目中。 

在头文件CppCLINETAssemblyWrapper.h中, 添加如下定义:

    #ifdef CPPCLINETASSEMBLYWRAPPER_EXPORTS
    #define SYMBOL_DECLSPEC __declspec(dllexport)
    #else
    #define SYMBOL_DECLSPEC	__declspec(dllimport)
    #endif

任何其他项目，其源文件中包含这个头文件可以看到SYMBOL_DECLSPEC类和方法被从一个DLL中到导入，
尽管这个DLL辨识这个用宏定义的标记作为导出。

因为这个头文件可能被包含在其他任意本地C++项目中，所以这个文件应该只包含本地C++类型，引用和关键字。

步骤4. 为C#类库上的CSClassLibrary进行封装的类CSSimpleObjectWrapper进行设计。

在头文件CppCLINETAssemblyWrapper.h中, 声明类：

    class SYMBOL_DECLSPEC CSSimpleObjectWrapper
    {
    public:
        CSSimpleObjectWrapper(void);
        virtual ~CSSimpleObjectWrapper(void);

        // 属性
        float get_FloatProperty(void);
        void set_FloatProperty(float fVal);

        // 方法
        HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

        // 静态方法
        static int GetStringLength(PWSTR pszString);

    private:
        void *m_impl;
    };

这个类包含了一个指向封装.NET对象的本地C++指针(void *m_impl;)。它在构造函数
CSSimpleObjectWrapper(void)中进行初始化，以及在析构函数(virtual ~CSSimpleObjectWrapper(void);)
中，释放封装对象。 

    CSSimpleObjectWrapper::CSSimpleObjectWrapper(void)
    {
        // 实例化C#的类CSSimpleObject.
        CSSimpleObject ^ obj = gcnew CSSimpleObject();

        // 绑定CSSimpleObject的.NET对象, 并且在m_impl中
        // 记录绑定对象的地址
        m_impl = GCHandle::ToIntPtr(GCHandle::Alloc(obj)).ToPointer(); 
    }

    CSSimpleObjectWrapper::~CSSimpleObjectWrapper(void)
    {
        // 获取与GCHandle相关联的绑定对象的地址 
        // 并且释放GCHandle。在这点上， CSSimpleObject对象对GC是适当的。
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        h.Free();
    }

CSSimpleObjectWrapper的公共成员方法封装了C#的CSSimpleObject中的方法。
它们通过CSSimpleObject对象的指针m_impl重新使用CSSimpleObject的调用。
marshaling类型在托管和本地代码中产生。

    float CSSimpleObjectWrapper::get_FloatProperty(void)
    {
        // 从内存地址中获得绑定对象CSSimpleObject
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

		// 重新使用绑定对象CSSimpleObject中的相同属性的调用
        return obj->FloatProperty;
    }

    void CSSimpleObjectWrapper::set_FloatProperty(float fVal)
    {
        // 在内存地址中获得绑定对象CSSimpleObject.
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

        // 重新使用绑定对象CSSimpleObject中的相同属性的调用
        obj->FloatProperty = fVal;
    }

    HRESULT CSSimpleObjectWrapper::ToString(PWSTR pszBuffer, DWORD dwSize)
    {
        // 从内存地址中获得绑定对象CSSimpleObject
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

        String ^ str;
        HRESULT hr;
        try
        {
            // 重新使用绑定对象CSSimpleObject中的相同属性的调用
            str = obj->ToString();
        }
        catch (Exception ^ e)
        {
            hr = Marshal::GetHRForException(e);
        }

        if (SUCCEEDED(hr))
        {
            // 从System::String到PCWSTR的转换.
            marshal_context ^ context = gcnew marshal_context();
            PCWSTR pszStr = context->marshal_as<const wchar_t*>(str);
            hr = StringCchCopy(pszBuffer, dwSize, pszStr == NULL ? L"" : pszStr);
            delete context; // 这个将会通过pszStr对内存进行释放。
        }

        return hr;
    }

    int CSSimpleObjectWrapper::GetStringLength(PWSTR pszString)
    {
        return CSSimpleObject::GetStringLength(gcnew String(pszString));
    }


/////////////////////////////////////////////////////////////////////////////
参考:

Using C++ Interop (Implicit PInvoke)
http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx


/////////////////////////////////////////////////////////////////////////////