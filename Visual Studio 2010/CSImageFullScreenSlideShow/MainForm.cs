/********************************** 模块头 ******************************************\
* 模块名:  MainForm.cs
* 项目名:  CSImageFullScreenSlideShow
* 版权 (c) Microsoft Corporation.
*
* 该实例演示了如何在全屏模式中显示幻灯片，如何修改放映图像的时间间隔.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;


namespace CSImageFullScreenSlideShow
{
    public partial class MainForm : Form
    {
        private string[] imageFiles = null;

        // 图像索引.
        private int selected = 0;
        private int begin = 0;
        private int end = 0;

        private FullScreen fullScreen = new FullScreen();

        public MainForm()
        {
            InitializeComponent();

            this.btnPrevious.Enabled = false;
            this.btnNext.Enabled = false;
            this.btnImageSlideShow.Enabled = false;
        }

        /// <summary>
        /// 选择图像文件夹.
        /// </summary>
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (this.imageFolderBrowserDlg.ShowDialog() == DialogResult.OK)
            {
                this.imageFiles = GetFiles(this.imageFolderBrowserDlg.SelectedPath,
                    "*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff;*.gif");

                this.selected = 0;
                this.begin = 0;
                this.end = imageFiles.Length;

                if (this.imageFiles.Length == 0)
                {
                    MessageBox.Show("找不到任何图像");

                    this.btnPrevious.Enabled = false;
                    this.btnNext.Enabled = false;
                    this.btnImageSlideShow.Enabled = false;
                }
                else
                {
                    ShowImage(imageFiles[selected], pictureBox);

                    this.btnPrevious.Enabled = true;
                    this.btnNext.Enabled = true;
                    this.btnImageSlideShow.Enabled = true;
                }
            }
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            string[] patterns = searchPattern.Split(';');
            List<string> files = new List<string>();
            foreach (string filter in patterns)
            {
                // 	遍历目录树 ,忽视 DirectoryNotFoundException 
                //  或者 UnauthorizedAccessException 异常.
                //  http://msdn.microsoft.com/en-us/library/bb513869.aspx

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

        /// <summary>
        /// 点击"上一张"按钮导航至上一张图像. 
        /// </summary>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (this.imageFiles == null || this.imageFiles.Length == 0)
            {
                MessageBox.Show("请选择要以幻灯片形式放映的图像!");
                return;
            }
            ShowPrevImage();
        }

        /// <summary>
        /// 点击"下一张"按钮导航至下一张图像. 
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.imageFiles == null || this.imageFiles.Length == 0)
            {
                MessageBox.Show("请选择要以幻灯片形式放映的图像!");
                return;
            }
            ShowNextImage();
        }

        /// <summary>
        /// 以幻灯片形式放映图像.
        /// </summary>
        private void btnImageSlideShow_Click(object sender, EventArgs e)
        {
            if (this.imageFiles == null || this.imageFiles.Length == 0)
            {
                MessageBox.Show("请选择要以幻灯片形式放映的图像!");
                return;
            }

            if (timer.Enabled == true)
            {
                this.timer.Enabled = false;
                this.btnOpenFolder.Enabled = true;
                this.btnImageSlideShow.Text = "开始幻灯片放映";
            }
            else
            {
                this.timer.Enabled = true;
                this.btnOpenFolder.Enabled = false;
                this.btnImageSlideShow.Text = "停止幻灯片放映";
            }
        }

        /// <summary>
        /// 定期显示下一张图像.
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            ShowNextImage();
        }

        /// <summary>
        /// 显示子窗体来更改Timer控件的设置. 
        /// </summary>
        private void btnSetting_Click(object sender, EventArgs e)
        {
            Settings frmSettings = new Settings(ref timer);
            frmSettings.ShowDialog();
        }

        /// <summary>
        /// 进入或者离开全屏模式.
        /// </summary>
        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            if (!this.fullScreen.IsFullScreen)
            {
                // 隐藏按钮，最大化幻灯片放映面板. 
                this.gbButtons.Visible = false;
                this.pnlSlideShow.Dock = DockStyle.Fill;

                fullScreen.EnterFullScreen(this);
            }
        }

        /// <summary>
        /// 响应"ESC"按键.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (this.fullScreen.IsFullScreen)
                {
                    // 显示按钮,还原幻灯片放映面板.
                    this.gbButtons.Visible = true;
                    this.pnlSlideShow.Dock = DockStyle.Top;

                    fullScreen.LeaveFullScreen(this);
                }
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

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
    }
}
