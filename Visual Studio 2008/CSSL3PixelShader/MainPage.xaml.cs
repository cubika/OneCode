/****************************** 模块头 ******************************\
* 模块名:	MainPage.xaml.cs
* 项目名:		CSSL3PixelShader
* 版权: (c) Microsoft Corporation.
* 
* 本示例内容包含了如何使用Silverlight3中新添加的像素着色器效果特性。
* 主要包含了2个方面：
* 
*1. 如何使用自带的效果，如投影效果。
*2. 如何创建自定义像素着色器效果，并在项目中使用自定义的效果。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Effects;
#endregion


namespace CSSL3PixelShader
{
    public partial class MainPage : UserControl
    {

        // 计时器用于递减_amplitude的值
        private DispatcherTimer _timer = new DispatcherTimer();
        // 初始化自定义效果
        OvalWateryEffect _effect = new OvalWateryEffect(
                    new Uri(@"/CSSL3PixelShader;component/ovalwatery.ps",
                        UriKind.Relative));

        public MainPage()
        {
            InitializeComponent();
            this.ImageWithPixelShader.Effect = _effect;
            // 初始化计时器以及添加Tick事件
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        /// <summary>
        /// 该事件处理用于降低 Amplitude，并且
        /// 每一次触发都会添加一个新的 OvalWateryEffect
        /// </summary>
        void _timer_Tick(object sender, EventArgs e)
        {
            if (this._effect.Amplitude > 0.0)
            {
                this._effect.Amplitude -= 0.05;
            }
            else
            {
                this._timer.Stop();
            }

        }

        /// <summary>
        /// 该事件处理获得当前的鼠标位置赋予一个 private 字段，并
        /// 将计时器启动。
        /// </summary>
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 将鼠标位置从控件的坐标转化成着色像素器需要的纹理贴图坐标
            this._effect.Center = new Point(e.GetPosition(this.ImageWithPixelShader).X /
            this.ImageWithPixelShader.ActualWidth, e.GetPosition(
            this.ImageWithPixelShader).Y / this.ImageWithPixelShader.ActualHeight);
            this._effect.Amplitude = 0.5;
            _timer.Start();
        }
    }


    /// <summary>
    /// OvalWaterEffect Class 是一个自定义的像素着色器效果类。
    /// </summary>
    public class OvalWateryEffect : ShaderEffect
    {
        /// <summary>
        /// 下列2个 DependencyProperties 是本自定义像素着色器效果的关键之处。
        /// 他们在HLSL（高级着色语言）以及托管代码之间建立了一个桥梁。
        /// 一旦属性的值被改变，PixShaderConstantCallback 事件会被触发。
        /// 比如，在PixelShaderConstantCallback(1)中的1代表了下列HLSL代码中的C1。
        /// 换句话来说,改变Amplitude属性的值，会影响到下列HLSL代码中 amplitude 变量的值。
        /// sampler2D input : register(S0);
        /// float2 center:register(C0);
        /// float amplitude:register(C1);
        /// float4 main(float2 uv : TEXCOORD) : COLOR
        /// {
        /// if(pow((uv.x-center.x),2)+pow((uv.y-center.y),2)<0.15)
        /// {
        /// uv.y = uv.y  + (sin(uv.y*100)*0.1*amplitude);
        /// }
        /// return tex2D( input , uv.xy);
        /// }
        /// </summary>
        public static readonly DependencyProperty AmplitudeProperty =
            DependencyProperty.Register("Amplitude", typeof(double), typeof(OvalWateryEffect),
            new PropertyMetadata(0.1, ShaderEffect.PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(OvalWateryEffect),
            new PropertyMetadata(new Point(0.5, 0.5), ShaderEffect.PixelShaderConstantCallback(0)));


        public OvalWateryEffect(Uri uri)
        {
            Uri u = uri;
            PixelShader psCustom = new PixelShader();
            psCustom.UriSource = u;
            PixelShader = psCustom;

            base.UpdateShaderValue(CenterProperty);
            base.UpdateShaderValue(AmplitudeProperty);
        }

        public double Amplitude
        {
            get
            {
                return (double)base.GetValue(AmplitudeProperty);
            }
            set
            {
                base.SetValue(AmplitudeProperty, value);
            }
        }

        public Point Center
        {
            get
            {
                return (Point)base.GetValue(CenterProperty);
            }
            set
            {
                base.SetValue(CenterProperty, value);
            }
        }
    }
}
