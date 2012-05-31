'****************************** 模块头 **************************************'
' 模块名:	MainModule.vb
' 项目名:	VBAutomateWord
' 版权(c)   Microsoft Corporation.
' 
' VBAutomateWord 案例阐述了如何使用Visual Basic.NET代码生成 Microsoft 
' Word 实例,构建新的Word文档, 插入段落, 表格,保存文档,关闭Word应用程序
' 及释放非托管COM资源的过程.
' 
' Office 自动化建立在组件对象模型（COM）的基础上.当从托管代码中调用Office相关
' 的COM对象时, 将自动生成一个运行库可调用包装（RCW）. RCW掌管.NET应用程序与COM
' 对象间的调用, 它保存对COM对象进行引用的数量值.如果RCW上并非所有的引用都被释放,
' 那么Office的COM对象将不会退出, 这也将导致Office应用程序在自动化运行后无法终止.
' 为了确保Office应用程序完全退出, 示例提供了两种解决方案.
' 
' Solution1.AutomateWord 阐述了通过Microsoft Word主要的互用组件（PIA）自动化Word
' 应用程序, 并且将每个COM访问对象分配给一个新的变量, 使用户最终可以通过调用
' Marshal.FinalReleaseComObject方法释放这些变量的过程. 
' 
' Solution2.AutomateWord阐述了通过Microsoft Word主要的互用组件（PIA）自动化Word
' 应用程序，在自动化方法退出堆栈后执行垃圾收集器（此时RCW对象不再被引用）,从而
' 清除RCW并释放COM对象的过程.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*******************************************************************************'


Module MainModule

    <STAThread()> _
    Sub Main()

        ' Solution1.AutomateWord 阐述了通过Microsoft Word主要的互用组件（PIA）
        ' 自动化Word应用程序, 并且将每个COM访问对象分配给一个新的变量, 使用户
        ' 最终可以通过调用Marshal.FinalReleaseComObject方法释放这些变量的过程.

        Solution1.AutomateWord()

        Console.WriteLine()

        ' Solution2.AutomateWord阐述了通过Microsoft Word主要的互用组件（PIA）
        ' 自动化Word应用程序, 在自动化方法退出堆栈后执行垃圾收集器（此时RCW
        ' 对象不再被引用）,从而清除RCW并释放COM对象的过程.

        Solution2.AutomateWord()

    End Sub

End Module
