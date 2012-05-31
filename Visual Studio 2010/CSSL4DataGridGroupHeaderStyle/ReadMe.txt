=============================================================================
  SILVERLIGHT 应用程序 : CSSL4DataGridGroupHeaderStyle 项目概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

本项目创建了一个示例程序, 演示了定义不同级别的组标题样式,并根据组标头内容定义
组标题样式.


/////////////////////////////////////////////////////////////////////////////
先决条件:

Silverlight 4 Tools RTM for Visual Studio 2010
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=b3deb194-ca86-4fb6-a716-b67c2604a139&displaylang=en

Silverlight 4 Toolkit for Visual Studio 2010
http://silverlight.codeplex.com/


/////////////////////////////////////////////////////////////////////////////
实现方法:
 
1. 如何定义不同级别的组标题样式.
可以使用 DataGrid.RowGroupHeaderStyle 属性定义组标题样式, 并且可以根据样式标签将
各个样式应用到不同级别的组标题中. 先应用到最高级, 其次是第二级, 依此类推.

2. 如何改变一个级别中的组标题样式.
在本例中, 使用了 Control.Template 在 DataGrid.RowGroupHeaderStyle 中定义了
StackPanel. StackPanel 的 Background 绑定至了 GroupHeaderName, 并且 background
的值使用 IValueConverter 依据 GroupHeaderName 来进行设定.

    
/////////////////////////////////////////////////////////////////////////////
参考资料:

DataGrid.RowGroupHeaderStyles 属性
http://msdn.microsoft.com/en-us/library/system.windows.controls.datagrid.rowgroupheaderstyles.aspx

Control.Template 属性
http://msdn.microsoft.com/en-us/library/system.windows.controls.control.template.aspx

Style.TargetType 属性
http://msdn.microsoft.com/en-us/library/system.windows.style.targettype.aspx


/////////////////////////////////////////////////////////////////////////////
