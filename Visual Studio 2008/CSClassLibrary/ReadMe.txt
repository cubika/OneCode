=============================================================================
          类库应用程序 : CSClassLibrary Project 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示一个我们在其他应用程序所需使用的C#类库. 这个类库阐述了一个
被命名为CSSimpleObject的简单的类. 
这个类包含了:

构造函数:
    public CSSimpleObject();

实例字段和属性:
    private float fField;
    public float FloatProperty

实例方法:
    public override string ToString();

共享 (静态) 方法:
    public static int GetStringLength(string str);

实例事件:
    // 事件在设置FloatProperty的存储器时，被触发
    public event PropertyChangingEventHandler FloatPropertyChanging;

创建类库的进程非常简单。


/////////////////////////////////////////////////////////////////////////////
实例关系:

CSReflection -> CSClassLibrary
CSReflection 动态加载地 程序集, CSClassLibrary.dll, 和 
示例, 审查和使用它的类型

CppHostCLR -> CSClassLibrary
CppHostCLR 创建 CLR, 在CSClassLibrary.dll 中实例化一个被声明的类型和调用它的方法。

CSClassLibrary - VBClassLibrary
它们是用不同的语言编写的，功能相同的类库。


/////////////////////////////////////////////////////////////////////////////
演示:

A. 创建工程

步骤. 在Visual Studio 2008中创建一个命名为CSClassLibrary的Visual C#类库项目。

B. 往项目中添加一个名为CSSimpleObject的类并定义它的字段，属性，方法和事件。

步骤1. 在解决方案管理器中添加一个新的类，并命名为CSSimpleObject。

步骤2. 编辑CSSimpleObject.cs并添加字段，属性，方法和事件

C. 用强命名对程序集命名(可选)

强命名被用来存储共享程序集在全局程序集缓存(GAC)。这个可以帮助避免DLL"灾难"，
强命名同样可以保护程序集被非法入侵（替换或注入）。一个强命名由程序集的身份-
它简单的文本名字，版本号以及本地信息（如果有），加上一份公共密钥和数字签名所组成。
这是产生从程序集使用相应的私钥。 

步骤1. 右键点击项目并打开属性页面。

步骤2. 转入签名页面并选择签名程序集选项。 

步骤3. 在选择强命名密钥文件下拉框，选择新的。当出现创建强命名密钥对话框，
在密钥文件名输入框内输入期望的名字。如果有必要，我们可以用一个密码保护
强命名密钥文件。选择确定。

/////////////////////////////////////////////////////////////////////////////
参考:

MSDN: Creating Assemblies
http://msdn.microsoft.com/en-us/library/b0b8dk77.aspx

How to: Sign an Assembly with a Strong Name
http://msdn.microsoft.com/en-us/library/xc31ft41.aspx

How to: Create and Use C# DLLs (C# Programming Guide)
http://msdn.microsoft.com/en-us/library/3707x96z.aspx


/////////////////////////////////////////////////////////////////////////////
