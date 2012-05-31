================================================================================
       Windows应用程序: VBWinFormBindToNestedProperties 概述
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示了如何将数据源中的嵌套的属性绑定到DataGridView控件中的列上.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 为子属性创建一个继承自PropertyDescriptor类的自定义的PropertyDescriptor命名为
SubPropertyDescriptor.

2. 创建一个继承自CustomTypeDescriptor类的子类，命名为MyCustomTypeDescriptor，可以增加一个额外
的PropertyDescriptor到指定类型原有的PropertyDescriptorCollection中.

3. 创建一个继承自TypeDescriptionProvider的子类，命名为MyTypeDescriptionProvider，来使用刚刚
继承自CustomTypeDescriptor类的子类MyCustomTypeDescriptor.

4. 在那个指定的具有复杂属性的类型上增加一个TypeDescriptionProviderAttribute属性.


/////////////////////////////////////////////////////////////////////////////
参考:

ICustomTypeDescriptor, Part 2
http://msdn.microsoft.com/en-us/magazine/cc163804.aspx
   

/////////////////////////////////////////////////////////////////////////////
