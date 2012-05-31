'********************************* 模块头 *********************************'
' 模块名:      VBSimpleObject.vb
' 项目名:      VBDllCOMServer
' 版权 (c) Microsoft Corporation.
' 
' 这个VB.NET示例着重于使用COM技术导出.Net Framework组件。这个允许我们为COM开发人
' 员编写一个.Net类型并且在非托管代码中使用此类型。此示例使用了ComClassAttribute属
' 性使编译器添加元数据，这将允许一个类作为一个COM对象被导出。这个属性简化了从
' Visual Basic中导出COM对象的过程。如果没有标记为ComClassAttribute，你需要好几个
' 步骤从Visual Basic中创建一个COM组件。一旦类被标记为ComClassAttribute，编译器会
' 自动执行这些额外的操作。
' 
' VBDllCOMServer导出了以下项目：
'
'   Program ID: VBDllCOMServer.VBSimpleObject
'   CLSID_VBSimpleObject: 805303FE-B5A6-308D-9E4F-BF500978AEEB
'   IID__VBSimpleObject: 90E0BCEA-7AFA-362A-A75E-6D07C1C6FC4B
'   DIID___VBSimpleObject: 72D3EFB2-0D88-4BA7-A26B-8FFDB92FEBED (EventID)
'   LIBID_VBDllCOMServer: A0CB2839-B70C-4035-9B11-2FF27E08B7DF
' 
'   属性：
'     ' 包括get和set存取方法。
'     FloatProperty As Single
' 
'   方法：
'     ' HelloWorld 返回一个字符串“HelloWorld”
'     Function HelloWorld() As String
'
'     ' GetProcessThreadID输出正在运行的线程ID和进程ID
'     Sub GetProcessThreadID(ByRef processId As UInteger, 
'                            ByRef threadId As UInteger)
' 
'   事件：
'     'FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
'     '客户端可以通过参数Cancel来取消对FloatProperty的修改。
'
'     Event FloatPropertyChanging(ByVal NewValue As Single, 
'                                 ByRef Cancel As Boolean)
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Runtime.InteropServices

#End Region


<ComClass(VBSimpleObject.ClassId, VBSimpleObject.InterfaceId, _
          VBSimpleObject.EventsId), ComVisible(True)> _
Public Class VBSimpleObject

#Region "COM Registration"

    Public Const ClassId As String _
    = "805303FE-B5A6-308D-9E4F-BF500978AEEB"
    Public Const InterfaceId As String _
    = "90E0BCEA-7AFA-362A-A75E-6D07C1C6FC4B"
    Public Const EventsId As String _
    = "72D3EFB2-0D88-4ba7-A26B-8FFDB92FEBED"

#End Region

#Region "Properties"

    Private fField As Single = 0

    ''' <summary>
    ''' 一个public属性，它包括了get和set存取方法
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FloatProperty() As Single
        Get
            Return Me.fField
        End Get
        Set(ByVal value As Single)
            Dim cancel As Boolean = False
            ' 触发FloatPropertyChanging事件
            RaiseEvent FloatPropertyChanging(value, cancel)
            If Not cancel Then
                Me.fField = value
            End If
        End Set
    End Property

#End Region

#Region "Methods"

    ''' <summary>
    ''' 一个public方法，它返回一个“HelloWorld”字符串。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HelloWorld() As String
        Return "HelloWorld"
    End Function

    ''' <summary>
    ''' 一个public方法，它拥有2个返回值：当前进程ID和当前线程ID
    ''' </summary>
    ''' <param name="processId"></param>
    ''' <param name="threadId"></param>
    ''' <remarks></remarks>
    Public Sub GetProcessThreadID(ByRef processId As UInteger, ByRef threadId As UInteger)
        processId = NativeMethod.GetCurrentProcessId
        threadId = NativeMethod.GetCurrentThreadId
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' 一个public事件，它在FloatProperty属性被赋予新值之前被触发。参数Cancel
    ''' 用于取消对FloatProperty属性的修改。
    ''' </summary>
    ''' <param name="NewValue"></param>
    ''' <param name="Cancel"></param>
    ''' <remarks></remarks>
    Public Event FloatPropertyChanging(ByVal NewValue As Single, _
                                       ByRef Cancel As Boolean)

#End Region

End Class
