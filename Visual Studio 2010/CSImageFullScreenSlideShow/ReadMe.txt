=============================================================================
           Windows 应用程序: CSImageFullScreenSlideShow 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:
	
该实例演示了如何在 Windows 窗体应用程序中显示图像幻灯片。它也演示了如何在全屏模式下
以幻灯片形式放映图像。


/////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 在 Visual Studio 2010中生成并运行该示例项目.

步骤2. 准备一些图像文件. 点击“打开文件...”按钮，选择包含图像文件的路径. 

步骤3. 点击"上一张"按钮和"下一张"按钮使图像文件按顺序显示.

步骤4. 左键单击"设置"按钮,为Timer控件选择显示图像文件的时间间隔，以便定期显示图像.
       最后，左键单击"开始幻灯片放映"按钮，使图像文件一张接一张地显示.

步骤5. 左键单击"全屏"按钮，在全屏模式下显示图像.按"ESC"键退出全屏模式.


/////////////////////////////////////////////////////////////////////////////
实现:

1. 当用户选择图像文件的根文件夹时, 该实例用基于堆栈的枚举方法，枚举文件夹中的图像
   文件.此方法在这篇MSDN文章中有所阐述：  
   http://msdn.microsoft.com/en-us/library/bb513869.aspx
   该实例没有使用
        Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
   来枚举文件，因为当用户对根文件中的某一个目录或文件没有访问权限时，它将中断.

        public static string[] GetFiles(string path, string searchPattern)
        {
            string[] patterns = searchPattern.Split(';');
            List<string> files = new List<string>();
            foreach (string filter in patterns)
            {
                // 	遍历目录树 ,忽视 DirectoryNotFoundException 
				//  或者 UnauthorizedAccessException 异常.
                // http://msdn.microsoft.com/en-us/library/bb513869.aspx

                // 用于保存文件中待检查的子文件夹名称的数据结构.
                Stack<string> dirs = new Stack<string>(20);

                if (!Directory.Exists(path))
                {
                    throw new ArgumentException();
                }
                dirs.Push(path);

                while (dirs.Count > 0)
                {
                    string currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir);
                    }

                    // 如果我们没有一个文件夹或者文件的访问权限,一个 
                    // UnauthorizedAccessException 异常将抛出. 
                    // 它可能允许忽视异常并且继续枚举剩下的文件和文件夹,或者不可能.
                    // 有时候也可能引起DirectoryNotFound异常（但可能性不大）.
                    // 当调用Directory.Exists之后, currentDir被另一个应用程序或线
                    // 程删除时,该异常将发生.                    
                    // 选择捕捉哪一个异常，完全取决于你打算执行的特定任务，和你对此                    
                    // 代码将运行的系统有多少确定性. 
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    try
                    {
                        files.AddRange(Directory.GetFiles(currentDir, filter));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    // 将子目录推入栈以便遍历.
                    // 这也可能在处理文件前发生.
                    foreach (string str in subDirs)
                    {
                        dirs.Push(str);
                    }
                }
            }

            return files.ToArray();
        }

2. 该实例在PictureBox显示图像. 

        /// <summary>
        /// 在PictureBox显示图像.
        /// </summary>
        public static void ShowImage(string path, PictureBox pct)
        {
            pct.ImageLocation = path;
        }

        /// <summary>
        /// 显示上一张图像.
        /// </summary>
        private void ShowPrevImage()
        {
            ShowImage(this.imageFiles[(--this.selected) % this.imageFiles.Length], this.pictureBox);
        }

        /// <summary>
        /// 显示下一张图像.
        /// </summary>
        private void ShowNextImage()
        {
            ShowImage(this.imageFiles[(++this.selected) % this.imageFiles.Length], this.pictureBox);
        }

        一个定时器(timer)用于自动地以幻灯片形式放映这些图像.

        /// <summary>
        /// 定期显示下一张图像.
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            ShowNextImage();
        }

3. 该实例提供了一个辅助类'FullScreen',以便在全屏模式下以幻灯片形式放映图像. 
   FullScreen.cs包括两个公共方法:
   
        EnterFullScreen - 用来使Windows窗体在全屏模式下显示.
        LeaveFullScreen - 用来使Windows窗体回到原始状态.

        /// <summary>
        /// 将窗口最大化至全屏.
        /// </summary>
        public void EnterFullScreen(Form targetForm)
        {
            if (!IsFullScreen)
            {
                Save(targetForm);  // 保存原始的窗体状态.

                targetForm.WindowState = FormWindowState.Maximized;
                targetForm.FormBorderStyle = FormBorderStyle.None;
                targetForm.TopMost = true;
                targetForm.Bounds = Screen.GetBounds(targetForm);

                IsFullScreen = true;
            }
        }

        /// <summary>
        /// 离开全屏模式,回到原始的窗体状态.
        /// </summary>
        public void LeaveFullScreen(Form targetForm)
        {
            if (IsFullScreen)
            {
                // 回到原始的窗体状态.
                targetForm.WindowState = winState;
                targetForm.FormBorderStyle = brdStyle;
                targetForm.TopMost = topMost;
                targetForm.Bounds = bounds;

                IsFullScreen = false;
            }
        }


/////////////////////////////////////////////////////////////////////////////
参考:

How to: Iterate Through a Directory Tree (C# Programming Guide)
http://msdn.microsoft.com/en-us/library/bb513869.aspx

Screen.GetBounds Method 
http://msdn.microsoft.com/en-us/library/system.windows.forms.screen.getbounds.aspx


/////////////////////////////////////////////////////////////////////////////
