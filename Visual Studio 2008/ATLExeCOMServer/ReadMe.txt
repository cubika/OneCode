========================================================================
                活动模板库 : ATLExeCOMServer 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：
    示例ATLDllCOMServer演示了在Visual Studio 2008中如何使用活动模板库（ATL）
    向导创建一个进程外组件对象模型服务器（COM Server）。使用ATL是旨在开发高
    效的，灵活的，轻量级的COM组件以及简化组件开发过程。ATLExeCOMServer叙述了
    一个ATL单线程单元（STA）中的一个简单对象，包括其属性、方法及事件。



（注意：在编写您自己的COM服务器时，请为其生成新的GUID）
  Program ID: ATLExeCOMServer.ATLSimpleObjectSTA
  CLSID_ATLSimpleObjectSTA: 9465BE08-C9FA-4DDF-A56E-57B92BCDEEB0
  IID_IATLSimpleObjectSTA: 47E764FE-D065-4BDE-8943-30F46664B02C
  DIID__IATLSimpleObjectSTAEvents: 6EE998B7-61C8-4D54-B27A-F623E8C1EA64
  LIBID_ATLExeCOMServerLib: C7902493-E23D-4487-824F-4AB97BD8B86D
  AppID: B711EE75-FDA3-4B0E-BFAA-67CB305D62AE

  属性：
    // 包括访问方法get和put 
    FLOAT FloatProperty

  方法：
    // HelloWorld 返回一个BSTR值"HelloWorld"
    HRESULT HelloWorld([out,retval] BSTR* pRet);
 
    // GetProcessThreadID输出正在运行的进程ID和线程ID
    HRESULT GetProcessThreadID([out] LONG* pdwProcessId, [out] LONG* pdwThreadId);

  事件：
    // FloatPropertyChanging：在属性FloatProperty被更新前会触发此事件。
    // 其中参数[in, out] Cancel允许客户机程序撤销对属性FloatProperty的修改
 
    
    // FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
    // 客户端可以通过[in，out]参数Cancel来取消对FloatProperty的修改。

    void FloatPropertyChanging(
      [in] FLOAT NewValue, [in, out] VARIANT_BOOL* Cancel);

此示例中的主要代码是由Visual Studio生成的。在本文档的“创建过程”章节中，您可
以找到建立这样的COM服务器的详细步骤。


/////////////////////////////////////////////////////////////////////////////

项目间关系


MFCCOMClient -> ATLExeCOMServer
MFCCOMClient演示了如何使用进程外组件。

ATLExeCOMServer - ATLDllCOMServer - ATLCOMService
所有的COM组件均使用ATL。ATLDllCOMServer是一个进程内组件，其形式为动态链
接库（DLL）。ATLExeCOMServer是一个进程外组件，其形式为作为本地服务器的可执行
文件（EXE）。ATLCOMService是一个进程外组件，其形式为提供本地Windows启动时后
台运行的服务（EXE）。


/////////////////////////////////////////////////////////////////////////////
生成：

由于ATLExeCOMServer组件需要在注册表HKCR中注册，在生成此组件时，请以管理员身份运
行Visual Studio。

/////////////////////////////////////////////////////////////////////////////
部署：

A、安装

ATLCOMService.exe /Regserver
此命令注册该COM服务器。

B、移除

ATLCOMService.exe /Unregserver
此命令将该COM服务器从注册表中移除


/////////////////////////////////////////////////////////////////////////////
创建过程：

A、创建项目

步骤1：在Visual Studio 2008中创建一个Visual C++ ATL项目，并把它命名为ATLExeCOMServer。

步骤2：在ATL项目向导的“应用程序设置”页面中，把服务器类别选择为可执行文件（EXE）
并且允许合并代理/存根（stub）代码

B、添加一个ATL简单对象

步骤1、在解决方案资源管理器中，向项目内添加一个ATL简单对象

步骤2、在ATL简单对象向导中，指定简称为ATLSimpleObjectSTA，并且选择线程模式为单
元，选择接口为双重。最后在支持项中选择连接点。至此在文件ATLExeCOMServer.idl中创
建了_IATLSimpleObjectSTAEvents接口。连接点选项是使组件支持事件的先决条件。

C、为ATL简单对象添加属性

步骤1、在类视图中找到IATLSimpleObjectSTA接口。在此接口上点击鼠标右键，在目录
中选择添加，然后选择添加属性。

步骤2、在添加属性向导中，定义属性类型为FLOAT，属性名为FloatProperty。选择Get函
数和Put函数。因此ATLSimpleObjectSTA可以使用get_FloatProperty和put_FloatProperty
方法来获取和设置FloatProperty属性。

步骤3、向CATLSimpleObjectSTA类中添加一个float变量，m_fField。

	protected:
		//用于记录FloatProperty属性
		float m_fField;
		
编写get和put访问方法，使FloatProperty属性能够读写m_fField。

	STDMETHODIMP CATLSimpleObjectSTA::get_FloatProperty(FLOAT* pVal)
	{
		*pVal = m_fField;
		return S_OK;
	}

	STDMETHODIMP CATLSimpleObjectSTA::put_FloatProperty(FLOAT newVal)
	{
		m_fField = newVal;
		return S_OK;
	}

D、为ATL简单对象添加方法

步骤1、在类视图中找到IATLSimpleObjectSTA接口。在此接口上点击鼠标右键，在目录
中选择添加，然后选择添加方法。

步骤2、在添加方法向导中，把方法名定义为HelloWorld。添加一个名为pRet的变量，其参数
属性为retval， 参数类型为BSTR*。

步骤3、编写HelloWorld方法的主体

	STDMETHODIMP CATLSimpleObjectSTA::HelloWorld(BSTR* pRet)
	{
		//为string分配内存
		*pRet = ::SysAllocString(L"HelloWorld");
		if (pRet == NULL)
			return E_OUTOFMEMORY;

		// 客户端现在负责为pbstr释放内存
		return S_OK;
	}

通过几乎相同的步骤，我们可以添加GetProcessThreadID方法。此方法被用于获得正在执
行的进程ID和线程ID。

HRESULT GetProcessThreadID([out] LONG* pdwProcessId, [out] LONG* pdwThreadId);

E、为ATL简单对象添加事件。

注意：连接点选项是使组件支持事件的先决条件，参考创建过程B中的步骤2。

步骤1、在类视图中展开ATLExeCOMServer和ATLExeCOMServerLib并找到_IATLSimpleObjectSTAEvents。

步骤2、在_IATLSimpleObjectSTAEvents上点击鼠标右键，在目录中选择添加，然后选择
添加方法。

步骤3、设置返回类型为void，并把方法名设置为FloatPropertyChanging。首先添加一个
参数属性为in，参数类型为FLOAT的参数NewValue。然后再添加一个参数属性为in和out，
参数类型为VARIANT_BOOL*的参数Cancel。在点击完成之后，我们可以在ATLExeCOMServer.idl
中找到如下调度接口_IATLSimpleObjectSTAEvents：


	dispinterface _IATLSimpleObjectSTAEvents
	{
		properties:
		methods:
			[id(1), helpstring("method FloatPropertyChanging")] void 
			FloatPropertyChanging(
			[in] FLOAT NewValue, [in,out] VARIANT_BOOL* Cancel);
	};

步骤4、我们可以通过以下2种方法创建生成类型库。通过重新生成项目或者在解决方案管理
器中在ATLExeCOMServer.idl点击鼠标右键然后点击编译。注意：我们必须在设置连接点之前
编译IDL文件。

步骤5、在类视图中右键点击CATLSimpleObjectSTA类，在弹出菜单上单击添加，然后点击
添加连接点。在实现连接点向导中，在源接口中选择_IATLSimpleObjectSTAEvents，双击
令其加入到实现连接点， 然后点击完成。这将会生成一个连接点的代理类，如在此例中在
_IATLSimpleObjectSTAEvents_CP.h文件中生成CProxy_IATLSimpleObjectSTAEvents。此外
还创建一个以Fire_[eventname]命名的函数，这个函数在客户端被用于响应事件。

步骤6、在put_FloatProperty中引发事件

	STDMETHODIMP CATLSimpleObjectSTA::put_FloatProperty(FLOAT newVal)
	{
		//引发事件，FloatPropertyChanging
		VARIANT_BOOL cancel = VARIANT_FALSE; 
		Fire_FloatPropertyChanging(newVal, &cancel);

		if (cancel == VARIANT_FALSE)
		{
			m_fField = newVal;	//保存新值
		}  //除此之外，不做任何事情
		return S_OK;
	}

F、配置和生成ATL COM服务器项目

步骤1、右键点击ATLExeCOMServer项目，选择属性。在属性页中，转到生成事件，生成后
事件，确认在命令行里有以下命令："$(TargetPath)" /RegServer。


/////////////////////////////////////////////////////////////////////////////
参考资料：


ATL 教程
http://msdn.microsoft.com/en-us/library/599w5e7x.aspx


/////////////////////////////////////////////////////////////////////////////
