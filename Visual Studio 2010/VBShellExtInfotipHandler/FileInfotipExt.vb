'****************************** 模块头 ******************************'
' 模块名称:  FileInfotipExt.vb
' 项目名称:      VBShellExtInfotipHandler
' 版权(c) Microsoft Corporation.
' 
' FileInfotipExt.vb 文件定义了一个信息提示处理程序， 它继承了 IPersistFile 和接口 IQueryInfo 

' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes


<ClassInterface(ClassInterfaceType.None), _
Guid("44BDEF95-C00F-493E-A55B-17937DB1F04E"), ComVisible(True)> _
Public Class FileInfotipExt
    Implements IPersistFile, IQueryInfo


    Private selectedFile As String


#Region "Shell Extension Registration"

    <ComRegisterFunction()> _
    Public Shared Sub Register(ByVal t As Type)
        Try
            ShellExtReg.RegisterShellExtInfotipHandler(t, ".vb")
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw
        End Try
    End Sub


    <ComUnregisterFunction()> _
    Public Shared Sub Unregister(ByVal t As Type)
        Try
            ShellExtReg.UnregisterShellExtInfotipHandler(t, ".vb")
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw
        End Try
    End Sub

#End Region


#Region "IPersistFile Members"

    Public Sub GetClassID(<Out()> ByRef pClassID As Guid) _
        Implements IPersistFile.GetClassID
        Throw New NotImplementedException
    End Sub

    Public Sub GetCurFile(<Out()> ByRef ppszFileName As String) _
        Implements IPersistFile.GetCurFile
        Throw New NotImplementedException
    End Sub

    Public Function IsDirty() As Integer _
        Implements IPersistFile.IsDirty
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 打开指定文件并根据文件的内容初始化对象 
    ''' contents.
    ''' </summary>
    ''' <param name="pszFileName">
    ''' 文件的绝对路径
    ''' </param>
    ''' <param name="dwMode">
    '''文件打开时的访问权限 
    ''' </param>
    Public Sub Load(ByVal pszFileName As String, ByVal dwMode As Integer) _
        Implements IPersistFile.Load
        ' pszFileName 包含文件的绝对路径信息
        Me.selectedFile = pszFileName
    End Sub

    Public Sub Save(ByVal pszFileName As String, ByVal fRemember As Boolean) _
        Implements IPersistFile.Save
        Throw New NotImplementedException
    End Sub

    Public Sub SaveCompleted(ByVal pszFileName As String) _
        Implements IPersistFile.SaveCompleted
        Throw New NotImplementedException
    End Sub

#End Region


#Region "IQueryInfo Members"

    ''' <summary>
    ''' 获取文本提示信息 .
    ''' </summary>
    ''' <param name="dwFlags">
    ''' Flags  是直接处理检索到的文本提示信息. flags 的值通常为0.
    ''' </param>
    ''' <returns>
    '''返回一个包含提示信息的字符串.
    ''' </returns>
    Public Function GetInfoTip(ByVal dwFlags As UInt32) As String _
        Implements IQueryInfo.GetInfoTip

        ' 先准备信息提示文本.本例子的信息提示文本时由文件路径和代码行数 

        Dim lineNum As Integer = 0
        Using reader As StreamReader = File.OpenText(Me.selectedFile)
            Do While (Not reader.ReadLine Is Nothing)
                lineNum += 1
            Loop
        End Using

        Return "File: " & Me.selectedFile & Environment.NewLine & _
            "Lines: " & lineNum.ToString & Environment.NewLine & _
            "- Infotip displayed by VBShellExtInfotipHandler"
    End Function

    ''' <summary>
    ''' 获取item 的标志信息.本例子方法在本例子中没用到
    ''' </summary>
    ''' <returns>返回item的标志信息</returns>
    Public Function GetInfoFlags() As Integer _
        Implements IQueryInfo.GetInfoFlags
        Throw New NotImplementedException
    End Function

#End Region

End Class