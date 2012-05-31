'/********************************* 模块头 **********************************\
'* 模块名:  App.xaml.vb
'* 项目名:  AzureBingMaps
'* 版权 (c) Microsoft Corporation.
'* 
'* App后台代码.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Windows
Imports System.Windows.Navigation
Imports Microsoft.Phone.Controls
Imports Microsoft.Phone.Shell

Partial Public Class App
    Inherits Application
    Private Shared m_viewModel As MainViewModel = Nothing
    Friend Shared DataSource As New DataSource()

    ''' <summary>
    ''' 用来绑定视图的静态ViewModel.
    ''' </summary>
    ''' <returns>MainViewModel对象.</returns>
    Public Shared ReadOnly Property ViewModel() As MainViewModel
        Get
            ' 有需要之前延迟视图模型的创建
            If m_viewModel Is Nothing Then
                m_viewModel = New MainViewModel()
            End If

            Return m_viewModel
        End Get
    End Property

    ''' <summary>
    ''' 提供Phone应用程序的根帧的快速访问.
    ''' </summary>
    ''' <returns>Phone应用程序的根帧.</returns>
    Public Property RootFrame() As PhoneApplicationFrame
        Get
            Return m_RootFrame
        End Get
        Private Set(ByVal value As PhoneApplicationFrame)
            m_RootFrame = value
        End Set
    End Property
    Private m_RootFrame As PhoneApplicationFrame

    ''' <summary>
    ''' Application对象构造器.
    ''' </summary>
    Public Sub New()
        ' 未捕获异常的全局句柄. 
        AddHandler UnhandledException, AddressOf Application_UnhandledException

        ' 调试时显示图形化存档信息.
        If System.Diagnostics.Debugger.IsAttached Then
            ' 显示当前帧率指针.

            ' 显示应用程序在每一帧中重绘的区域.
            'Application.Current.Host.Settings.EnableRedrawRegions = true;

            ' 启用非产品级分析可视化模式, 
            ' 用彩色的覆盖显示了一个页面中正在使用GPU加速区域.
            'Application.Current.Host.Settings.EnableCacheVisualization = true;
            Application.Current.Host.Settings.EnableFrameRateCounter = True
        End If

        ' 标准Silverlight初始化
        InitializeComponent()

        ' Phone相关初始化
        InitializePhoneApplication()
    End Sub

    ' 应用程序启动时执行的代码（例如 开始）
    ' 此代码将不会在应用程序重新激活时执行
    Private Sub Application_Launching(ByVal sender As Object, ByVal e As LaunchingEventArgs)
        DataSource.LoadDataAsync()
    End Sub

    ' 应用程序被激活时执行的代码（转到前台）
    ' 此代码将不会在应用程序首次加载时执行
    Private Sub Application_Activated(ByVal sender As Object, ByVal e As ActivatedEventArgs)
        ' 确保应用程序状态已被正确恢复
        If Not App.ViewModel.IsDataLoaded Then
            App.ViewModel.LoadData()
        End If
    End Sub

    ' 应用程序非活动时执行的代码（转到后台）
    ' 此代码将不会在应用程序关闭时执行
    Private Sub Application_Deactivated(ByVal sender As Object, ByVal e As DeactivatedEventArgs)
        ' 确保所需应用程序状态被正确保存.
    End Sub

    ' 应用程序关闭时执行的代码 (例如用户点击Back)
    ' 此代码将不会在应用程序非活动时执行
    Private Sub Application_Closing(ByVal sender As Object, ByVal e As ClosingEventArgs)
    End Sub

    ' 导航失败时执行的代码 
    Private Sub RootFrame_NavigationFailed(ByVal sender As Object, ByVal e As NavigationFailedEventArgs)
        If System.Diagnostics.Debugger.IsAttached Then
            ' 导航失败;中断进入调试器
            System.Diagnostics.Debugger.Break()
        End If
    End Sub

    ' 处理未捕获的异常的代码
    Private Sub Application_UnhandledException(ByVal sender As Object, ByVal e As ApplicationUnhandledExceptionEventArgs) Handles Me.UnhandledException
        If System.Diagnostics.Debugger.IsAttached Then
            ' 发生未捕获的异常; 中断进入调试器
            System.Diagnostics.Debugger.Break()
        End If
    End Sub

#Region "Phone application initialization"

    ' 避免重复初始化
    Private phoneApplicationInitialized As Boolean = False

    ' 请不要向这个方法添加其他代码
    Private Sub InitializePhoneApplication()
        If phoneApplicationInitialized Then
            Return
        End If

        ' 建立框架，但暂不设置RootVisual，这使直到应用程序准备渲染前启动画面仍然有效.
        RootFrame = New PhoneApplicationFrame()
        AddHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication

        ' 处理浏览失败
        AddHandler RootFrame.NavigationFailed, AddressOf RootFrame_NavigationFailed

        ' 确保我们不再初始化
        phoneApplicationInitialized = True
    End Sub

    ' 请不要向这个方法添加其他代码
    Private Sub CompleteInitializePhoneApplication(ByVal sender As Object, ByVal e As NavigationEventArgs)
        ' 设置RootVisual允许应用程序渲染
        If Not ReferenceEquals(RootVisual, RootFrame) Then
            RootVisual = RootFrame
        End If

        ' 删除不再被使用的句柄
        RemoveHandler RootFrame.Navigated, AddressOf CompleteInitializePhoneApplication
    End Sub

#End Region
End Class