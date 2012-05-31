========================================================================
    SILVERLIGHT 应用程序 : CSSL3PixelShader 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法：

本示例内容包含了如何使用Silverlight3中新添加的像素着色器效果特性
1. 如何使用自带的效果，如投影效果。
2. 如何创建自定义像素着色器效果，并在项目中使用自定义的效果。

/////////////////////////////////////////////////////////////////////////////
先决条件：

Silverlight 3 Tools for Visual Studio 2008 SP1
http://www.microsoft.com/downloads/details.aspx?displaylang=zh-cn&FamilyID=9442b0f2-7465-417a-88f3-5e7b5409e9dd

DirectX SDK: (如果希望自己创建.ps文件的话，需要下载安装该SDK)
http://www.microsoft.com/DOWNLOADS/details.aspx?FamilyID=24a541d6-0486-4453-8641-1eee9e21b282&displaylang=en

/////////////////////////////////////////////////////////////////////////////
创建步骤：

A. 创建一个.ps文件用于自定义像素着色器效果。(如果你希望直接使用本示例中自带的.ps文件,跳过该步)

步骤1. 创建一个新的.txt文件。并将下列HLSL(高级着色器语言)代码粘贴进去。

sampler2D input : register(S0);
float2 center:register(C0);
float amplitude:register(C1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	if(pow((uv.x-center.x),2)+pow((uv.y-center.y),2)<0.15)
	{
		uv.y = uv.y  + (sin(uv.y*100)*0.1*amplitude);
	}
	return tex2D( input , uv.xy);
}

步骤2. 保存修改过的.txt文件并关闭。将其重命名为ovalwatery.fx。

步骤3. 打开 Directx SDK 命令行提示工具，并运行下列命令：

fxc /T ps_2_0 /Fo "<OutputPath>\ovalwatery.ps" "<InputPath>\ovalwatery.fx"

<InputPath> 是 .fx 文件的路径.
<OutPutpat> 是你想要创建的 .ps 文件的路径.

B. 创建 Silverlight 工程

步骤1. 在Visual Studio 2008 sp1中创建一个 Visual C# Silverlight Application 工程，将其命名为CSSL3PicelShader

C. 将.ps文件添加到工程中去。

步骤1. 在解决方案窗口中右击项目节点，选择"添加->现有项"以添加.ps文件(在A的第3步中所创建的文件)。

步骤1. 在解决方案窗口中右击项目节点，选择"添加->现有项"以添加"Humpback Whale.jpg"。（你可以在本示例的文件夹中找到该jpg文件）

E. 编辑 xaml.

步骤1. 在解决方案窗口中双击 MaiPage.xaml 来查看 xaml 代码。将 <Grid> 元素用下列代码替换。

	<Grid x:Name="LayoutRoot">
		<Grid.RowDefinitions>
			<RowDefinition Height="1*"/>
			<RowDefinition Height="9*"/>
        </Grid.RowDefinitions>
        <TextBlock HorizontalAlignment="Center" Foreground="Red" FontSize="32" 
			Text="请点击图片">
            <TextBlock.Effect>
                <DropShadowEffect Color="Black">    
                </DropShadowEffect>
            </TextBlock.Effect>
		</TextBlock>
			<Image Grid.Row="1" Width="640" Height="480"  
				x:Name="ImageWithPixelShader" Source="Humpback Whale.jpg">
		</Image>
	</Grid>

上述代码主要添加了2个控件：

一个TextBlock用于展示如何使用自带的投影效果。
一个Image控件用于展示如何使用自定义的像素着色器效果。

步骤2. 用下列代码替换 <UserControl> 标签。

<UserControl x:Class="CSSL3PixelShader.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
    xmlns:c="clr-namespace:CSSL3PixelShader" 
    MouseLeftButtonDown="UserControl_MouseLeftButtonDown"
    mc:Ignorable="d" d:DesignWidth="640" d:DesignHeight="480">

这个步骤主要做了2件事：

为UserControl添加了鼠标左键按下事件：
MouseLeftButtonDown="UserControl_MouseLeftButtonDown"

定义了一个新的xmlns
xmlns:c="clr-namespace:CSSL3PixelShader"

F. 编辑 xaml.vb.

步骤1. 双击 MainPage.xaml.cs 来查看代码,在文件头部添加下列引用
Imports System.Windows.Threading
Imports System.Windows.Media.Effects

步骤2. 将 MainPage Class 用下列代码替换。

	Partial Public Class MainPage
    Inherits UserControl
    ' 计时器,用于递减_amplitude的指
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

Step3. Add a new class OvalWateryEffect after the MainPage class:

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


/////////////////////////////////////////////////////////////////////////////
参考资料：

HLSL(高级着色语言)编程指导
http://msdn.microsoft.com/en-us/library/bb509635(VS.85).aspx

Dependency Properties 概况
http://msdn.microsoft.com/en-us/library/cc221408(VS.95).aspx

像素着色器效果
http://msdn.microsoft.com/en-us/library/dd901594(VS.95).aspx


/////////////////////////////////////////////////////////////////////////////
