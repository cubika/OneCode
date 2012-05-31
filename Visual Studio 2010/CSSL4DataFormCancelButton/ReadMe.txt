========================================================================
    SILVERLIGHT应用程序 : CSSL4DataFormCancelButton项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

本示例演示了如何在使用DataForm控件编辑时显示取消按钮

/////////////////////////////////////////////////////////////////////////////
演示:

用户请按照以下步骤测试本示例：
	1. 请确认你已经安装了Silverlight 4 Tools for Visual Studio 2010 和
	   Silverlight 4 Toolkit.
	   本示例使用的是Silverlight 4 Toolkit - April 2010 
	   (http://silverlight.codeplex.com/releases/view/43528)
	   
	   用户最好直接安装Silverlight_4_Toolkit_April_2010.msi。
	   在安装完成后，所有相关的dll文件都会被安装到GAC中去。  
	   否则，用户需要将Silverlight_4_Toolkit_April_2010.zip解压后，找到
	   System.Windows.Controls.Data.DataForm.Toolkit.dll，并以此替代示
	   例用到的引用。
 
	   System.Windows.Controls.Data.DataForm.Toolkit.dll存放的路径是
	   April 2010 Silverlight Toolkit\April 2010 Silverlight Toolkit\Bin。
	   (右键单击项目名-->添加引用 -->在上面的文件夹下找到并选择文件)
	2. 使用Visual Studio 打开CSSL4DataFormCancelButton并编译。
	3. 运行项目。
    4. 在默认情况下，取消按钮是被禁止的。但在修改或添加操作时，
	   取消按钮可被触发。


/////////////////////////////////////////////////////////////////////////////
先决条件:

本示例需要安装Silverlight Tools 和 Toolkit。
Silverlight 4 Tools RTM for Visual Studio 2010
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=b3deb194-ca86-4fb6-a716-b67c2604a139&displaylang=en


Silverlight 4 Toolkit for Visual Studio 2010
http://silverlight.codeplex.com/releases/view/43528


/////////////////////////////////////////////////////////////////////////////
实现方法:

如何在DataForm中编辑时实现显示取消按钮的功能？
	1. 创建一个继承IEditableObject接口的类，
	   并实现BeginEdit，CancelEdit，EndEdit三个方法。
	2. 在编辑前保存当前值。
	3. 在CancelEdit方法中恢复原始值。
    
/////////////////////////////////////////////////////////////////////////////
参考资料:

IEditableObject 接口
http://msdn.microsoft.com/en-us/library/system.componentmodel.ieditableobject_members%28v=VS.95%29.aspx
 
DataAnnotation 类
http://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations%28VS.95%29.aspx

RangeAttribute 类
http://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.rangeattribute%28v=VS.95%29.aspx

INotifyPropertyChanged 接口
http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged%28VS.95%29.aspx


/////////////////////////////////////////////////////////////////////////////
