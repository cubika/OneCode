================================================================================
  Windows应用程序: VBWinFormPassValueBetweenForms 概述
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示如何在多个窗体之间传递值.

这里有两种通用的方法可以实现在多个窗体之间传递值.

1. 使用一个属性.

   在目标窗体类中创建一个公共的属性，然后我们就可以通过设置这个属性的值的方式在多个窗体中传递值.

2. 使用一个方法.

   在目标窗体类中创建一个公共的方法，然后我们就可以通过把值作为参数传给这个方法的形式在多个窗体中传递值.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

   1. 创建两个窗体，分别命名为FrmPassValueBetweenForms和FrmPassValueBetweenForms2;
      
   2. 在FrmPassValueBetweenForms2中创建一个公共的属性命名为ValueToPassBetweenForms;
      
      private _valueToPassBetweenForms As String;

      Public Property ValueToPassBetweenForms() As String
        Get
            Return Me._valueToPassBetweenForms
        End Get
        Set(ByVal value As String)
            Me._valueToPassBetweenForms = value
        End Set
      End Property

      
   3. 在FrmPassValueBetweenForms2类中创建一个公共的方法命名为SetValueFromAnotherForm;
      
      Public Sub SetValueFromAnotherForm(ByVal val As String)
        Me._valueToPassBetweenForms = val
      End Sub
      
   4. 在FrmPassValueBetweenForms窗体中处理按钮的点击事件.
     在button1的点击事件中，通过设置FrmPassValueBetweenForms2中的SetValueFromAnotherForm
属性将值从FrmPassValueBetweenForms窗体传递给FrmPassValueBetweenForms2窗体.
     在button2的点击事件中，通过调用FrmPassValueBetweenForms2中的SetValueFromAnotherForm
方法将值从FrmPassValueBetweenForms窗体传递给FrmPassValueBetweenForms2窗体.


/////////////////////////////////////////////////////////////////////////////
参考:
   
1. Windows Forms General FAQ.
   http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/77a66f05-804e-4d58-8214-0c32d8f43191
   

/////////////////////////////////////////////////////////////////////////////
