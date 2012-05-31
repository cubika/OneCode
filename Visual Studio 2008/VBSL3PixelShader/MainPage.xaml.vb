'****************************** 模块头 ******************************\
' 模块名:	MainPage.xaml.cs
' 项目名:		CSSL3PixelShader
' 版权: (c) Microsoft Corporation.
' 
' 本示例内容包含了如何使用Silverlight3中新添加的像素着色器效果特性。
' 主要包含了2个方面：
' 
'1. 如何使用自带的效果，如投影效果。
'2. 如何创建自定义像素着色器效果，并在项目中使用自定义的效果。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

#Region "Imports directives"

Imports System.Windows.Threading
Imports System.Windows.Media.Effects

#End Region


Partial Public Class MainPage
    Inherits UserControl
    ' 计时器,用于递减_amplitude的值
    Private _timer As DispatcherTimer = New DispatcherTimer()
    ' 初始化自定义效果
    Private _effect As OvalWateryEffect = New OvalWateryEffect(New Uri("/VBSL3PixelShader;component/ovalwatery.ps", UriKind.Relative))

    Public Sub New()
        InitializeComponent()
        Me.ImageWithPixelShader.Effect = _effect
        ' 初始化计时器以及添加Tick事件
        _timer.Interval = TimeSpan.FromMilliseconds(50)
        AddHandler _timer.Tick, AddressOf _timer_Tick
    End Sub

    ''' <summary>
    ''' 该事件处理用于降低 Amplitude，并且
    ''' 每一次触发都会添加一个新的 OvalWateryEffect
    ''' </summary>
    Private Sub _timer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If Me._effect.Amplitude > 0.0 Then
            Me._effect.Amplitude -= 0.05
        Else
            Me._timer.Stop()
        End If

    End Sub

    ''' <summary>
    ''' 该事件处理获得当前的鼠标位置赋予一个 private 字段，并
    ''' 将计时器启动。
    ''' </summary>
    Private Sub UserControl_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        ' 将鼠标位置从控件的坐标转化成着色像素器需要的纹理贴图坐标
        Me._effect.Center = New Point(e.GetPosition(Me.ImageWithPixelShader).X / Me.ImageWithPixelShader.ActualWidth, e.GetPosition(Me.ImageWithPixelShader).Y / Me.ImageWithPixelShader.ActualHeight)
        Me._effect.Amplitude = 0.5
        _timer.Start()
    End Sub
End Class


''' <summary>
''' OvalWaterEffect Class 是一个自定义的像素着色器效果类。
''' </summary>
Public Class OvalWateryEffect
    Inherits ShaderEffect
    ''' <summary>
    '''  下列2个 DependencyProperties 是本自定义像素着色器效果的关键之处。
    ''' 他们在HLSL（高级着色语言）以及托管代码之间建立了一个桥梁。
    ''' 一旦属性的值被改变，PixShaderConstantCallback 事件会被触发。
    ''' 比如，在PixelShaderConstantCallback(1)中的1代表了下列HLSL代码中的C1。
    ''' 换句话来说,改变Amplitude属性的值，会影响到下列HLSL代码中 amplitude 变量的值。
    ''' sampler2D input : register(S0);
    ''' float2 center:register(C0);
    ''' float amplitude:register(C1);
    ''' float4 main(float2 uv : TEXCOORD) : COLOR
    ''' {
    ''' if(pow((uv.x-center.x),2)+pow((uv.y-center.y),2)<0.15)
    ''' {
    ''' uv.y = uv.y  + (sin(uv.y*100)*0.1*amplitude);
    ''' }
    ''' return tex2D( input , uv.xy);
    ''' }
    ''' </summary>
    Public Shared ReadOnly AmplitudeProperty As DependencyProperty = DependencyProperty.Register("Amplitude", GetType(Double), GetType(OvalWateryEffect), New PropertyMetadata(0.1, ShaderEffect.PixelShaderConstantCallback(1)))

    Public Shared ReadOnly CenterProperty As DependencyProperty = DependencyProperty.Register("Center", GetType(Point), GetType(OvalWateryEffect), New PropertyMetadata(New Point(0.5, 0.5), ShaderEffect.PixelShaderConstantCallback(0)))


    Public Sub New(ByVal uri As Uri)
        Dim u As Uri = uri
        Dim psCustom As PixelShader = New PixelShader()
        psCustom.UriSource = u
        PixelShader = psCustom

        MyBase.UpdateShaderValue(CenterProperty)
        MyBase.UpdateShaderValue(AmplitudeProperty)
    End Sub

    Public Property Amplitude() As Double
        Get
            Return CDbl(MyBase.GetValue(AmplitudeProperty))
        End Get
        Set(ByVal value As Double)
            MyBase.SetValue(AmplitudeProperty, value)
        End Set
    End Property

    Public Property Center() As Point
        Get
            Return CType(MyBase.GetValue(CenterProperty), Point)
        End Get
        Set(ByVal value As Point)
            MyBase.SetValue(CenterProperty, value)
        End Set
    End Property
End Class