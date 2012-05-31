=============================================================================
      控制台应用程序 : CppCallNETAssemblyWrapper 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个代码示例演示了一个本地C++应用程序通过C++/CLI封装类对托管代码进行封装后的调用。

  CppCallNETAssemblyWrapper (本地C++应用程序)
          -->
      CppCLINETAssemblyWrapper (C++/CLI的封装)
              -->
          CSClassLibrary (.NET程序集)


/////////////////////////////////////////////////////////////////////////////
演示:

以后步骤演示了.NET和本地代码的混合示例

步骤1. 在Visual Studio 2008中，在你成功的编译CSClassLibrary，CppCLINETAssemblyWrapper,
以及CppCallNETAssemblyWrapper示例后，你将获得到应用程序：CppCallNETAssemblyWrapper.exe
和两个DLL文件：CppCLINETAssemblyWrapper.dll和CSClassLibrary.dll。它们的关系是：
CppCallNETAssemblyWrapper调用CppCLINETAssemblyWrapper.dll。而CppCLINETAssemblyWrapper.dll
又调用定义了公共类的CSClassLibrary.dll。

步骤2. 在指令框中运行CppCallNETAssemblyWrapper。 程序在控制台输出以下信息：

    模块"CSClassLibrary" 没有被加载
    Class: CSSimpleObject::FloatProperty = 1.20
    Class: CSSimpleObject::ToString => "1.20"
    模块"CSClassLibrary" 被加载

这个信息显示了.NET程序集CSClassLibrary没有被加载直到CppCallNETAssemblyWrapper
引用了一个被包含在DLL中的标记。通过C++/CLI封装类被定义在CppCLINETAssemblyWrapper.dll
中的CSSimpleObjectWrapper，CppCallNETAssemblyWrapper创建了一个CSSimpleObject的实例
并且调用了它的属性FloatProperty和方法ToString。


/////////////////////////////////////////////////////////////////////////////
示例关系:
(当前实例和其他实例之间的关系在Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppCallNETAssemblyWrapper -> CppCLINETAssemblyWrapper -> CSClassLibrary
本地C++示例应用程序CppCallNETAssemblyWrapper通过C++/CLI封装类CppCLINETAssemblyWrapper
调用定义在C#类库CSClassLibrary。


/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 隐式链接导出封装类CSSimpleObjectWrapper的C++/CLI类库CppCLINETAssemblyWrapper 

  首先，你需要在项目属性 / 链接器 / 输入 / 附加目录中输入LIB文件名，从而导入DLL所产生的LIB
  文件。你可以在属性 / 链接器 / 常规 / 附加库目录中配置LIB文件的搜索路径
  
  其次，引入定义了封装类的头文件
  
    #include "CppCLINETAssemblyWrapper.h"
  
  你可以在属性 / C/C++ / 常规 / 附加引用目录中配置头文件的搜索路径

步骤2. 通过CppCLINETAssemblyWrapper间接访问.NET类CSSimpleObject，
以此来调用封装来CSSimpleObjectWrapper，例如：

    CSSimpleObjectWrapper obj;

    obj.set_FloatProperty(1.2F);
    float fProp = obj.get_FloatProperty();
    wprintf(L"Class: CSSimpleObject::FloatProperty = %.2f\n", fProp);


/////////////////////////////////////////////////////////////////////////////
参考:

Using C++ Interop (Implicit PInvoke)
http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx


/////////////////////////////////////////////////////////////////////////////
