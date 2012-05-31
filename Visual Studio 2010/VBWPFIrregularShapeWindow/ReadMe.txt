================================================================================
       Windows 应用: VBWPFIrregularShapeWindow综述                     
===============================================================================
/////////////////////////////////////////////////////////////////////////////
概要:
这个例子阐述了怎样为WPF的用户界面创建一个形状不规则的窗体。
这个例子中的不规则窗体的形状包括了：矩形，圆形，三角形以图片为背景的形状等等。
在每一个WPF窗体中我们可以鼠标右击并且点击弹出菜单来最大化，最小化或关闭这种操作。此外还为这些不
规则的窗体添加了拖拽行为。

////////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在 VS2010中创建这个项目. 

步骤2. 你可以使用其它PNG格式的图片文件代替transparentPic.png 或者使用默认图片。

步骤3. 运行 VBWPFIrregularShapeWindow.exe.

步骤4. 分别点击 "椭圆形窗体","圆角窗体","三角形窗体","对话窗体","图片背景窗体" ，它们会显示相应
	  的不规则窗体。 


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. 每一个不规则窗体有一个绑定命令的类，该类为完成命令的事件处理程序绑定了一个路由命令。
2. 我们可以使用缩放变化来放大或减小对象的尺寸。并且这是两个相反的动作，所以当不同的命令在影响对象时一个有效而一个无效。

3. 如果你使用一个菜单项来影响一个对象，那么你应该设置菜单项，命令目标方法和按钮，否则菜单项对象将无效。

4. 这个例子提供了一个现成的样板来判断CommandBinding的对象：例如canExecute方法可以接受结果是真或假。

5. 这个例子使用了事件触发器和故事面板，例如：http://msdn.microsoft.com/en-us/library/ms745683.aspx#Y3189,
   如果你已经浏览了以上文章所提到的，你会发现另一种触发器，那就是事件触发器，这种触发器根据事件的发生开始一连串的行为。
   例如，事件触发器对象详细说明了鼠标是否点击下了按钮，椭圆和图片，高度属性的值在0.2秒内自动变到260。
   当鼠标从菜单项上移开时，该属性在1秒内变回到原始值。注意它是如何在鼠标离开时无需设置动画值的。
   这是因为动画是可以保持原始值的轨迹的。你只需要使用下面的代码，将其中的目标属性替换成按钮，路径，
   椭圆或是图片即可。
     <Window.Resources>
        <Style TargetType="Path">
            <Setter Property="Opacity" Value="0.5" />
            <Style.Triggers>

                <EventTrigger RoutedEvent="Mouse.MouseEnter">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard>
                                <DoubleAnimation
                  Duration="0:0:0.2"
                  Storyboard.TargetProperty="Height"
                  To="260"  />
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
                <EventTrigger RoutedEvent="Mouse.MouseLeave">
                    <EventTrigger.Actions>
                        <BeginStoryboard>
                            <Storyboard>
                                <DoubleAnimation
                  Duration="0:0:1"
                  Storyboard.TargetProperty="Height"  />
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger.Actions>
                </EventTrigger>
            </Style.Triggers>

        </Style>

    </Window.Resources>

	当然，你需要为按钮，路径，椭圆或图片加入高度属性并且设置它的值，例如：
	<Path Height="250" Name="Triangle" Stroke="Black" StrokeThickness="2" Grid.ColumnSpan="10" Grid.RowSpan="4">

6.为了让子窗口不和主窗口重合，该例子中所使用的代码如下：

	ellipseWindow.Left = this.Left + this.Width + width;
	ellipseWindow.Top = this.Top + this.Height + height;

/////////////////////////////////////////////////////////////////////////////
参考文献:

CommandBinding 类 http://msdn.microsoft.com/zh-cn/library/system.windows.input.commandbinding.aspx
RoutedUICommand 类 http://msdn.microsoft.com/zh-cn/library/ms752308.aspx
ScaleTransform 类http://msdn.microsoft.com/zh-cn/library/system.windows.media.scaletransform.aspx
Window.WindowState 属性  http://msdn.microsoft.com/zh-cn/library/system.windows.window.windowstate.aspx
样式设置和模板化 http://msdn.microsoft.com/en-us/library/ms745683.aspx#Y3189
/////////////////////////////////////////////////////////////////////////////



