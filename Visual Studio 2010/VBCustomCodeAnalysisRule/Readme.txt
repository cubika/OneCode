================================================================================
				Windows应用程序: VBCustomCodeAnalysisRule 项目概述                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
摘要：
这个示例演示了如何去创建，部署和运行自定义代码分析规则。
在Visual Studio 2010 旗舰版和Visual Studio 2010终极版中，你能够使用代码分析去发现在你的代码中的
潜在的问题。在这个例子中的这些规则常常用来去检查文本属性和方法的名称。

/////////////////////////////////////////////////////////////////////////////
设置和移除：

--------------------------------------
在开发环境中

A. 安装

导航到输出文件夹
1. 复制VBCustomCodeAnalysisRule.dll 到
   C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\
   FxCop\Rules [对64位操作系统] 或者 C:\Program Files\Microsoft Visual Studio 10.0\Team Tools\
   Static Analysis Tools\FxCop\Rules [对32位操作系统]

2. 复制 VBCustomCodeAnalysisRule.ruleset 到
   C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\
   Rule Sets [对64位操作系统] 或者 C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\
   Static Analysis Tools\Rule Sets [对32位操作系统]

B.移除

删除上面的两个文件


--------------------------------------
在部署环境中

A. 安装

安装VBCustomCodeAnalysisRuleSetup.msi，这个文件在VBCustomCodeAnalysisRuleSetup安装项目的输出目录当中。

B. 移除
 
卸载VBCustomCodeAnalysisRuleSetup.msi，这个文件在VBCustomCodeAnalysisRuleSetup安装项目的输出目录当中。



////////////////////////////////////////////////////////////////////////////////
演示：

步骤1.在Visual Studio 2010 旗舰版或者Visual Studio 2010终极版中， 打开这个项目
        
步骤2.构建这个解决方案

步骤3.在解决方案浏览器中，右键点击这个项目的VBCustomCodeAnalysisRuleSetup ，选择安装
	注意：在安装和移除部分，你也可以按照下面的步骤来复制这些文件

步骤4.打开另一个VS2010进程，创建一个VB的类库项目TestCA.vbproj
	添加下面的类。
	      Public Class ClassA

              Private WrongFieldName As Integer
              Private rightFieldName As Integer
          
              Public Property wrongPropertyName() As Integer
              Public Property RightPropertyName() As Integer
          
              Public Sub wrongMethodName()
              End Sub
              Public Sub RightMethodName()
              End Sub
          
          End Class

步骤5.打开TestCA项目的属性页，在代码分析标签中，选择VBCustomCodeAnalysisRule.

步骤6.在VS中，点击分析=>运行代码分析在TestCA上。在错误列表窗口中，你将要得到下面的警告。
CCAR0001 : Naming: 在TextCA.ClassA中，WrongFieldName字段的名称应该以小写字符开始
CCAR0002 :Naming: 在TextCA.ClassA中，wrongMethodName 方法的名称应该以大写字符开始	
CCAR0003 : Naming: 在TextCA.ClassA中，wrongPropertyName 属性的名称应该以大写字符开始

/////////////////////////////////////////////////////////////////////////////
代码逻辑：

A.创建项目，添加引用

在Visual Sturdio 2010中，创建一个Visual Basic/窗口/类库项目，命名为 "VBCustomCodeAnalysisRule". 
添加RxCopSdk.dll和Microsoft.Cci.Dll文件的引用。这两个程序集位于FxCop文件夹中，FxCop10.0位于
Windows SDK7.1中，你可以通过下面的连接下载它。
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=35aeda01-421d-4ba5-b44b-543dc8c33a20&displaylang=en

B.执行代码分析规则，字段命名规则，方法命名规则，属性命名规则

一个自定义代码分析规则是一个继承Microsoft.FxCop.Sdk.BaseIntrospectionRule类的一个密封类。
重写了public ProblemCollection Check(Member member)方法，实现检查这些成员。

    sealed class PropertyNamingRule : BaseIntrospectionRule
    {
        public PropertyNamingRule()
            : base( "PropertyNamingRule", "VBCustomCodeAnalysisRule.Rules",
                typeof(FieldNamingRule).Assembly)
        {}
      
        public override ProblemCollection Check(Member member)
        {         
            if (member is PropertyNode)
            {
                PropertyNode property = member as PropertyNode;

                if (property.Name.Name[0] < 'A' || property.Name.Name[0] > 'Z')
                {
                    this.Problems.Add(new Problem(
                       this.GetNamedResolution(
                       "UppercaseProperty", 
                       property.Name.Name,
                       property.DeclaringType.FullName)));
                }
            }

            return this.Problems;
        }
    }


C.在Rules.xml文件中，定义规则。

在程序集中，这个文件为所有的这些规则定义了元数据。在Visual Studio中，添加一个名字是Rrules.xml的文件然后
在属性窗口中，标记这个文件作为一个“Embedded Resource”，这个XML文件像这样：


<Rules>
  <Rule TypeName="FieldNamingRule" Category="Naming" CheckId="CCAR0001">
    <Name>字段名称应该以小写字符开始</Name>
    <Description>在类中，字段名称应该以小写字符开始  </Description>
    <Resolution Name="LowercaseField"> 字段名称 {0} 在类中 {1} 应该一小写字符开始 </Resolution>
    <MessageLevel Certainty="99">Warning</MessageLevel>
    <Message Certainty="99">Warning</Message>
    <FixCategories>NonBreaking</FixCategories>
    <Owner></Owner>
    <Url></Url>
    <Email></Email>
  </Rule>
  ...

   
D.定义一个新的规则集。

在Visual Studio 2010中，一个新的特性被叫做是规则集。在分析过程中，规则集合是一个新的配置途径，
这种配置途径下的规则应该是运行的。一个规则集可以这样描述

<?xml version="1.0" encoding="utf-8"?>
<RuleSet Name="VBCustomCodeAnalysisRule" Description=" " ToolsVersion="10.0">
  <Rules AnalyzerId="Microsoft.Analyzers.ManagedCodeAnalysis" 
         RuleNamespace="Microsoft.Rules.Managed">
    <Rule Id="CCAR0001" Action="Warning" />
    <Rule Id="CCAR0002" Action="Warning" />
    <Rule Id="CCAR0003" Action="Warning" />
  </Rules>
</RuleSet>

E.在安装项目中部署规则

为了添加一个部署的项目，在文件菜单中，注意添加选项然后点击新建项目。在这个新添加的项目对话框中，展开其
他项目类型的节点，展开这个安装和部署项目，点击Visual Studio安装程序，然后点击安装项目。在名称对话框中，
类型设置为VBCustomCodeAnalysisRuleSetup。点击OK，实现创建这个项目。

1右键点击这个安装程序，选择视图/文件系统。

2在文件系统窗口中，右键点击目标机器的文件系统，选择添加专用的文件夹=>Program Files文件夹。

3在Program Files文件夹下，创建“Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools
 \Rule Sets”这个文件夹，添加VbCustomCodeAnalys的主要的输出目录到这个文件夹中。
	
4在Program Files文件夹下，创建“Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools
 \Rule Sets”这个文件夹。添加VBCustomCodeAnalysisRule.ruleset 这个文件到这个文件夹中，
构建这个安装项目，如果构建成功，你将要得到一个.msi文件和一个setup.exe文件。

你能对你的用户描述这些，来实现安装和卸载这些规则。
/////////////////////////////////////////////////////////////////////////////
参考资料:
http://msdn.microsoft.com/en-us/library/microsoft.build.evaluation.project.aspx
http://www.binarycoder.net/fxcop/html/doccomments.html
/////////////////////////////////////////////////////////////////////////////