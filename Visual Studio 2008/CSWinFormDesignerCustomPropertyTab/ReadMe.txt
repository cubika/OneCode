================================================== ============================== 
Windows窗体应用程序：CSWinFormDesignerCustomPropertyTab项目概况 
        
================================================== ============================== 

////////////////////////////////////////////////// /////////////////////////// 
用途： 

CSWinFormDesignerCustomPropertyTab代码演示如何创建自定义的
Windows窗体设计时自定义属性选项卡。 


////////////////////////////////////////////////// /////////////////////////// 
演示： 
1、生成该项目。 

2、双击MainForm.cs打开主窗体 

3、打开Visual Studio工具箱，你应该找到UC_CustomPropertyTab控件。 
    
4、将UC_CustomPropertyTab拖到主窗体上。 

5、在UC_CustomPropertyTab属性窗口中，你可以看到一个红色图标的“C”。 
   这就是自定义属性选项卡。 

////////////////////////////////////////////////// /////////////////////////// 
创建过程： 

1、创建一个UserControl; 

2、添加对System.Designer.dll引用; 

3、创建一个bool类型的属性在类CustomTabDisplayAttribute的派生类中，
   取任何您喜欢的名字，例如： 
   “CustomTabDisplayAttribute”; 

4、创建一个自定义属性选项卡从PropertiesTab类派生; 

5、重写PropertiesTab.CanExtend属性，使其只接受我们的 
   用户控件类型“UC_CustomPropertyTab”; 
    
6、重写PropertiesTab.GetProperties方法返回的属性 
   被标记为Browserable（false）和CustomTabDisplay（true）; 

7、重写PropertiesTab.TabName属性给自定义属性 
   标签一个名称; 
    
8、创建UserControl的一些特性，标记的属性要 
   显示自定义属性选项卡与Browserable（false）和 
   CustomTabDisplay（true）。 
    
9、将UserControl加上设计器的标记，使其使用上述步骤创建的自定义 
   控件设计服务。 
     
10、创建一个位图文件，将其命名为与自定义类型 
    属性选项卡同名，在这个示例中，位图文件的名称应该是 
    “CustomTab”; 

11、在位图上绘制一些内容。 

12、在解决方案的资源管理器中选择这个位图文件，并切换到属性 
    窗口，在“生成操作”字段选择“嵌入的资源”。
    
13、生成该项目。 

14、UserControl将出现在工具箱中。 

15、把UC_CustomPropertyTab拖到主窗体，你可以在Visual Studio属性窗体中看到“自定义选项卡”。



////////////////////////////////////////////////// /////////////////////////// 
参考资料： 

1. How do I add a custom tab to the property grid when it is displaying my object's properties?
http://windowsclient.net/blogs/faqs/archive/2006/05/26/how-do-i-add-a-custom-tab-to-the-property-grid-when-it-is-displaying-my-object-s-properties.aspx

2. Windows Forms FAQs
http://windowsclient.net/blogs/faqs/archive/tags/Custom+Designers/default.aspx

////////////////////////////////////////////////// ///////////////////////////