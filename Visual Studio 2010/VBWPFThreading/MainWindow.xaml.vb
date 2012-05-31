'****************************** Module Header ******************************'
' 模块:      MainWindow.xaml.vb
' 项目:      VBWPFThreading
' 版权 (c) Microsoft Corporation.
'
' 此示例演示了两种WPF线程模型。第一种是先将一个长时间运行的线程划分成许多小
' 片段，然后由WPF dispatcher按优先级逐个执行队列中的片段。后台执行的工作项不
' 会影响到前台界面处理，所以看起来像是后台运行的工作项是在另一个线程中执行的。
' 但实际上，所有的工作都是在同一个线程中完成的。如果为单线程图形用户界面应用
' 程序，并希望在UI线程中进行复杂处理的时候同事能保持用户界面的响应，这个技巧
' 非常实用。
'
' 第二种模型类似于WinForm中的多线程模型。后台工作在另一个线程中执行，并调用
' Dispatcher.BeginInvoke 方法来更新用户界面。
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System
Imports System.Windows
Imports System.Windows.Threading
Imports System.Threading

''' <summary> 
''' Interaction logic for MainWindow.xaml 
''' </summary> 
Partial Public Class MainWindow
    Inherits Window

#Region "Long-Running Calculation in UI Thread"

    Public Delegate Sub NextPrimeDelegate()
    Private num As Long = 3
    Private continueCalculating As Boolean = False
    Private fNotAPrime As Boolean = False

    Private Sub btnPrimeNumber_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If continueCalculating Then
            continueCalculating = False
            btnPrimeNumber.Content = "重新开始"
        Else
            continueCalculating = True
            btnPrimeNumber.Content = "停止"
            btnPrimeNumber.Dispatcher.BeginInvoke(DispatcherPriority.Normal, New NextPrimeDelegate(AddressOf CheckNextNumber))
        End If
    End Sub

    Public Sub CheckNextNumber()
        ' 重置标记
        fNotAPrime = False

        For i As Long = 3 To Math.Sqrt(num)
            If num Mod i = 0 Then
                ' 设置“不是素数”标记为真
                fNotAPrime = True
                Exit For
            End If
        Next

        ' 如果是素数
        If Not fNotAPrime Then
            tbPrime.Text = num.ToString()
        End If

        num += 2
        If continueCalculating Then
            btnPrimeNumber.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle, New NextPrimeDelegate(AddressOf Me.CheckNextNumber))
        End If
    End Sub

#End Region


#Region "Blocking Operation in Worker Thread"

    Private Delegate Sub NoArgDelegate()
    Private Delegate Sub OneArgDelegate(ByVal arg As Int32())

    Private Sub btnRetrieveData_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.btnRetrieveData.IsEnabled = False
        Me.btnRetrieveData.Content = "连接服务器"

        Dim fetcher As New NoArgDelegate(AddressOf Me.RetrieveDataFromServer)
        fetcher.BeginInvoke(Nothing, Nothing)
    End Sub

    ''' <summary> 
    ''' 在后台线程中获取数据
    ''' </summary> 
    Private Sub RetrieveDataFromServer()
        ' 为网络连接延迟5秒
        Thread.Sleep(5000)

        ' 生成要展示的随机数据
        Dim rand As New Random()
        Dim data As Int32() = {rand.[Next](1000), rand.[Next](1000), rand.[Next](1000), rand.[Next](1000)}

        ' 在UI线程中安排更新操作
        Me.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, New OneArgDelegate(AddressOf UpdateUserInterface), data)
    End Sub

    ''' <summary> 
    ''' 将新数据更新到用户界面，此方法在UI线程中执行。
    ''' </summary> 
    ''' <param name="data"></param> 
    Private Sub UpdateUserInterface(ByVal data As Int32())
        Me.btnRetrieveData.IsEnabled = True
        Me.btnRetrieveData.Content = "从服务器获取数据"
        Me.tbData1.Text = data(0).ToString()
        Me.tbData2.Text = data(1).ToString()
        Me.tbData3.Text = data(2).ToString()
        Me.tbData4.Text = data(3).ToString()
    End Sub

#End Region

End Class