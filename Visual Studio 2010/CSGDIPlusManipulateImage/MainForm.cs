/****************************** 模块头 ******************************\
* 模块名:  MainForm.cs
* 项目名:	  CSGDIPlusManipulateImage
* 版权(c)  Microsoft Corporation.
* 
* 这是程序的主窗体, 用来初始化用户界面以及处理事件.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CSGDIPlusManipulateImage
{
    public partial class MainForm : Form
    {
        Pen pen = null;

        ImageManipulator imgManipulator = null;

        Point adjustment = Point.Empty;

        public MainForm()
        {
            InitializeComponent();

            // 从本地文件加载位图.
            Bitmap img = new Bitmap("OneCodeIcon.png");

            // 初始化 imgManipulator.
            imgManipulator = new ImageManipulator(img);

            // 向下拉框中添加所有的InterpolationMode.
            for (int i = 0; i <= 7; i++)
            {
                cmbInterpolationMode.Items.Add((InterpolationMode)(i));
            }

            cmbInterpolationMode.SelectedIndex = 0;

        }

        /// <summary>
        /// 处理按钮 btnRotateLeft, btnRotateRight, btnFlipVertical 和 
        /// btnFlipHorizontal 的点击事件.
        /// </summary>
        private void btnRotateFlip_Click(object sender, EventArgs e)
        {
            Button rotateFlipButton = sender as Button;

            if (rotateFlipButton == null)
            {
                return;
            }

            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

            switch (rotateFlipButton.Name)
            {
                case "btnRotateLeft":
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
                case "btnRotateRight":
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case "btnFlipVertical":
                    rotateFlipType = RotateFlipType.RotateNoneFlipY;
                    break;
                case "btnFlipHorizontal":
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                    break;
            }

            // 旋转或翻转图片.
            imgManipulator.RotateFlip(rotateFlipType);

            // 重绘 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);
        }

        /// <summary>
        ///  处理按钮 btnRotateAngle 的点击事件.
        /// </summary>
        private void btnRotateAngle_Click(object sender, EventArgs e)
        {
            float angle = 0;

            // 验证输入值.
            float.TryParse(tbRotateAngle.Text, out angle);

            if (angle > 0 && angle < 360)
            {
                // 旋转图像.
                imgManipulator.RotateImg(angle);

                // 重绘 pnlImage.
                imgManipulator.DrawControl(this.pnlImage, adjustment, pen);

            }
        }

        /// <summary>
        /// 处理按钮 btnMoveUp, btnMoveDown, btnMoveLeft 和 btnMoveRight
        /// 的点击事件.
        /// </summary>
        private void btnMove_Click(object sender, EventArgs e)
        {
            Button moveButton = sender as Button;
            if (moveButton == null)
            {
                return;
            }

            int x = 0;
            int y = 0;

            switch (moveButton.Name)
            {
                case "btnMoveUp":
                    y = -10;
                    break;
                case "btnMoveDown":
                    y = 10;
                    break;
                case "btnMoveLeft":
                    x = -10;
                    break;
                case "btnMoveRight":
                    x = 10;
                    break;
            }
            adjustment = new Point(adjustment.X + x, adjustment.Y + y);

            // 重绘 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);

        }

        private void pnlImage_Paint(object sender, PaintEventArgs e)
        {

            // 首次绘制 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);

        }

        /// <summary>
        /// 处理按钮 btnAmplify 和 btnMicrify 的点击事件.
        /// </summary>
        private void btnAmplify_Click(object sender, EventArgs e)
        {
            Button btnScale = sender as Button;
            if (btnScale.Name == "btnAmplify")
            {
                imgManipulator.Scale(2, 2, (InterpolationMode)cmbInterpolationMode.SelectedItem);
            }
            else
            {
                imgManipulator.Scale(0.5f, 0.5f, (InterpolationMode)cmbInterpolationMode.SelectedItem);
            }
            
            // 重绘 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);
        }


        /// <summary>
        /// 处理按钮 btnSkewLeft 和 btnSkewRight 的点击事件.
        /// </summary>
        private void btnSkew_Click(object sender, EventArgs e)
        {
            Button btnSkew = sender as Button;

            switch(btnSkew.Name)
            {
                case "btnSkewLeft":
                    imgManipulator.Skew(-10);
                    break;
                case "btnSkewRight":
                    imgManipulator.Skew( 10);
                    break;
            }

            // 重绘 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);
        }


        /// <summary>
        /// 重置图像.
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {

            imgManipulator.Dispose();

            // 从本地文件加载位图.
            Bitmap img = new Bitmap("OneCodeIcon.png");

            // 初始化 imgManipulator.
            imgManipulator = new ImageManipulator(img);

            // 重绘 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);

        }

        /// <summary>
        /// 处理 chkDrawBounds 的点击事件. 
        /// </summary>
        private void chkDrawBounds_CheckedChanged(object sender, EventArgs e)
        {
            // 如果 pen 不为空, 则绘制图像的边框.
            if (chkDrawBounds.Checked)
            {
                pen = Pens.Blue;
            }
            else
            {
                pen = null;
            }

            // 重绘 pnlImage.
            imgManipulator.DrawControl(this.pnlImage, adjustment, pen);
        }

       
    }
}
