================================================================================
       WINDOWS FORMS应用程序: VBWinFormGroupRadioButtons项目概述
       
                        RadioButton 例子
                        
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
概要:

这个例子展示了怎样在不同的容器中组织RadioButtons.
   

/////////////////////////////////////////////////////////////////////////////
演示:

当你选中4个RadioButton中的一个，其他的将自动不选中。换句话说虽然这4个RadioButton在不同的容器中他们也是一组的。

/////////////////////////////////////////////////////////////////////////////
实现:

1. 放4个Radiobutton在不同的容器中，使用在 MainForm.Designer.cs文件中的RadioButton_CheckedChanged方法处理它们的CheckedChanged 事件。
   
2. 首先使用 radTmp Radiobutton存储老的 RadioButton,  选中RadioButton然后指定radTmp到这个RadioButton。
   
3. 不选中那个老的RadioButton，然后当每个CheckedChanged 事件发生时指定radTmp到新选中的 RadioButton 。


/////////////////////////////////////////////////////////////////////////////
参考:

1. Windows Forms一般常见问题FAQ.
 http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/77a66f05-804e-4d58-8214-0c32d8f43191
   
2. Windows Forms RadioButton控件：
   http://msdn.microsoft.com/en-us/library/f5h102xz.aspx
   

/////////////////////////////////////////////////////////////////////////////

   

/////////////////////////////////////////////////////////////////////////////
