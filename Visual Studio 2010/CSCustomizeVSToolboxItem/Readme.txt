================================================================================
		Visual Studio Package项目: CSCustomizeVSToolboxItem                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要：

如果你向vs2010的toolbox添加一个新的栏目，那么显示的名称
和新栏目的控件提示信息默认是一样的。这个例子说明了怎样通过
客户端控件向Visual Studio的工具箱里添加栏目。

必要条件：

Visual Studio 2010 Premium 或 Visual Studio 2010 Ultimate. Visual Studio 2010 SDK.

注意：先安装VS SDK再安装VS2010 SP1.

示例：

步骤1. 在Visual Studio 2010 Professional或更高版本中打开本项目。

步骤2. 打开该项目的属性页并选择Debug栏目。选中start Action的
       Start external program项，并浏览选中devenv.exe(默认路径
       是C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe)，
       并添加"/rootsuffix Exp" (非引用)到Command line arguments。

步骤3. 构建此解决方案。

步骤4. 按F5,然后Microsoft Visual Studio 2010的一个试验示例将
       被启动。

步骤5. 在刚启动的VS的试验实例中，新建一个解决方案或打开一个已存在的
       解决方案并开始编辑一个.txt文件。

步骤6. 打开工具箱窗口，你将会发现有一个"CS Custom Toolbox Tab"工具栏目，并且
       该工具栏目包含一个"CS Custom Toolbox Item"条目。把鼠标指针移到这个条目上，你会看到
       下面的提示信息：
          
           CS Custom Toolbox Tooltip
           CS Custom Toolbox Description

步骤7. 拖拽这个新的工具箱控件放到.txt文件中，"cs hello world"将被添加到文件中。

/////////////////////////////////////////////////////////
代码逻辑：

A. 新建一个VSIX项目。

1. 选择Visual Studio Package项目模板。在命名框中键入
   项目名称并点击确定。

   在新建项目对话框中Visual Studio Package项目模板在
   这些区域有效：
   在Visual Basic的拓展名下，该项目的语言默认是Visual Basic。
   在C#的拓展名下，该项目的语言默认是C#。
   在其他项目类型拓展名下，该项目的语言默认是C++。

2. 在“选择一种编程语言”页面，可选择C#。让模板生成一个
    key.snk文件来标记程序集。或者，点击浏览来选择你自己的
    密钥文件。模板会复制你的密钥文件并把它命名为key.snk。

3. 在“基本VSPackage信息”页面，指明你的VSPackage的详细信息。

4. 点击完成来完成创建。

5. 可选择性的选择集成测试项目和单元测试项目来为你的解决方案创建测试项目。

    更多信息，可参阅
    演练：通过使用Visual Studio模板包创建一个菜单命令。
    http://msdn.microsoft.com/en-us/library/bb164725.aspx

    注意：命令菜单不必被选中。

B. 在CSCustomizeVSToolboxItemPackage中重写Initialize方法。
    在此方法中，得到IVsToolbox2和IVsActivityLog服务。

C. 添加自定义工具箱项。

    验证ToolboxTab和的ToolboxItem是否存在，如果不存在，添加自定义的工具箱项目。
    
     using (var stream = SaveStringToStreamRaw(FormatTooltipData(toolboxTooltipString, toolboxDescriptionString)))
   {
       var toolboxData = new Microsoft.VisualStudio.Shell.OleDataObject();
       toolboxData.SetData("VSToolboxTipInfo", stream);
       toolboxData.SetData(DataFormats.Text, "Hello world");

       TBXITEMINFO[] itemInfo = new TBXITEMINFO[1];
       itemInfo[0].bstrText = toolboxItemString;
       itemInfo[0].hBmp = IntPtr.Zero;
       itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

       ErrorHandler.ThrowOnFailure(vsToolbox2.AddItem(toolboxData, itemInfo, toolboxTabString));
   }

   

/////////////////////////////////////////////////////////////////////////////
 参考：

 演练：动态配置自定义的ToolboxItem
 http://msdn.microsoft.com/en-us/library/bb165910.aspx

 演练：自动加载工具箱项
 http://msdn.microsoft.com/en-us/library/bb166237.aspx

 部署VSIX
 http://msdn.microsoft.com/en-us/library/ff363239.aspx
 /////////////////////////////////////////////////////////////////////////////





