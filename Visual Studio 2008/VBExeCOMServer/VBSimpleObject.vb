'********************************* 模块头 **********************************'
' 模块名:    VBSimpleObject.vb
' 项目民:      VBExeCOMServer
' Copyright (c) Microsoft Corporation.
' 
' 定义COM类，VBSimpleObject， 和其ClassFactory，VBSimpleObjectClassFactory
' 
' (在编写您自己的COM服务器时，请生成新的GUID)
' Program ID: VBExeCOMServer.VBSimpleObject
' CLSID_VBSimpleObject: 3CCB29D4-9466-4f3c-BCB2-F5F0A62C2C3C
' IID__VBSimpleObject: 5EECE765-6416-467c-8D5E-C227F69E7EB7
' DIID___VBSimpleObjectEvents: 10C862E3-37E6-4e36-96FE-3106477235F1
' 
' 属性：
' 包括访问方法get和put 
' FloatProperty As Single
' 
' 方法：
' ' HelloWorld 返回一个字符串"HelloWorld"
' Function HelloWorld() As String
' ' GetProcessThreadID输出正在运行的进程ID和线程ID
' Sub GetProcessThreadID(ByRef processId As UInteger, 
'                        ByRef threadId As UInteger)
' 
' 事件：
' 'FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
' '客户端可以通过参数Cancel来取消对FloatProperty的修改。
' Event FloatPropertyChanging(ByVal NewValue As Single, 
'                             ByRef Cancel As Boolean)
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
Imports System.ComponentModel

#End Region


<ComClass(VBSimpleObject.ClassId, VBSimpleObject.InterfaceId, _
          VBSimpleObject.EventsId), ComVisible(True)> _
Public Class VBSimpleObject
    Inherits ReferenceCountedObject

#Region "COM Registration"

    Public Const ClassId As String _
    = "3CCB29D4-9466-4f3c-BCB2-F5F0A62C2C3C"
    Public Const InterfaceId As String _
    = "5EECE765-6416-467c-8D5E-C227F69E7EB7"
    Public Const EventsId As String _
    = "10C862E3-37E6-4e36-96FE-3106477235F1"


    '这些程序描述了服务器所需要的额外的COM注册过程


    <ComRegisterFunction(), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Shared Sub Register(ByVal t As Type)
        Try
            COMHelper.RegasmRegisterLocalServer(t)
        Catch ex As Exception
            Console.WriteLine(ex.Message) ' 记录错误
            Throw ex ' 再次抛出此异常
        End Try
    End Sub

    <EditorBrowsable(EditorBrowsableState.Never), ComUnregisterFunction()> _
    Public Shared Sub Unregister(ByVal t As Type)
        Try
            COMHelper.RegasmUnregisterLocalServer(t)
        Catch ex As Exception
            Console.WriteLine(ex.Message) ' 记录错误
            Throw ex ' 再次抛出异常
        End Try
    End Sub

#End Region

#Region "Properties"

    Private fField As Single = 0

    ''' <summary>
    ''' 包括访问方法get和put的public属性
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
    ''' 一个返回字符串“HelloWorld”的public方法
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HelloWorld() As String
        Return "HelloWorld"
    End Function

    ''' <summary>
    ''' 一个返回当前进程ID和线程ID的public方法
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
    ''' 一个public事件。它在FloatProperty属性被赋予新值前被触发。参数Cancel
    ''' 允许客户端取消对FloatProperty所做的更改。
    ''' </summary>
    ''' <param name="NewValue"></param>
    ''' <param name="Cancel"></param>
    ''' <remarks></remarks>
    Public Event FloatPropertyChanging(ByVal NewValue As Single, _
                                       ByRef Cancel As Boolean)

#End Region

End Class


''' <summary>
''' 为VBSimpleObject创造的一个工厂类
''' </summary>
Friend Class VBSimpleObjectClassFactory
    Implements IClassFactory

    Public Function CreateInstance(ByVal pUnkOuter As IntPtr, ByRef riid As Guid, _
                                   <Out()> ByRef ppvObject As IntPtr) As Integer _
                                   Implements IClassFactory.CreateInstance
        ppvObject = IntPtr.Zero

        If (pUnkOuter <> IntPtr.Zero) Then
            ' pUnkOuter变量为非空，并且此对象不支持聚合（aggregation）。
            Marshal.ThrowExceptionForHR(COMNative.CLASS_E_NOAGGREGATION)
        End If

        If ((riid = New Guid(VBSimpleObject.ClassId)) OrElse _
            (riid = New Guid(COMNative.IID_IDispatch)) OrElse _
            (riid = New Guid(COMNative.IID_IUnknown))) Then
            ' 创建一个.NET对象的实例
            ppvObject = Marshal.GetComInterfaceForObject( _
            New VBSimpleObject, GetType(VBSimpleObject).GetInterface("_VBSimpleObject"))
        Else
            ' 被ppvObject所指向的对象不被此riid的接口支持。
            Marshal.ThrowExceptionForHR(COMNative.E_NOINTERFACE)
        End If

        Return 0  ' S_OK
    End Function


    Public Function LockServer(ByVal fLock As Boolean) As Integer _
    Implements IClassFactory.LockServer
        Return 0  ' S_OK
    End Function

End Class


''' <summary>
''' ReferenceCountedObject 类.
''' </summary>
''' <remarks></remarks>
<ComVisible(False)> _
Public Class ReferenceCountedObject

    Public Sub New()
        ' 增加COM服务器中的锁定计数器。
        ExeCOMServer.Instance.Lock()
    End Sub

    Protected Overrides Sub Finalize()
        Try
            ' 减少COM服务器中的锁定服务器。
            ExeCOMServer.Instance.Unlock()
        Finally
            MyBase.Finalize()
        End Try
    End Sub

End Class