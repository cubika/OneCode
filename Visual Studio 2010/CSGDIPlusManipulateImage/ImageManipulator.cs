/****************************** 模块头 ******************************\
* 模块名:  ImageManipulator.cs
* 项目名:	  CSGDIPlusManipulateImage
* 版权(c)  Microsoft Corporation.
* 
* 类 ImageManipulator 使用 GDI+ 来操作图像. 它提供了公共方法来拉伸, 旋转, 翻转以及
* 倾斜图像, 支持任意角度旋转. 这个类同时提供方法在 System.Windows.Forms.Control 的
* 子类对象上绘制图像.
* 
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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CSGDIPlusManipulateImage
{
    public class ImageManipulator : IDisposable
    {

        // 指示资源是否已经被释放.
        bool disposed = false;

        /// <summary>
        /// 需处理的图像.
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// 图像的实际大小. 当旋转, 倾斜图像的时候, 图像的实际大小是不变的.
        /// </summary>
        public Size ContentSize { get; private set; }

        /// <summary>
        /// 已旋转的角度. 0~360.
        /// </summary>
        public float RotatedAngle { get; private set; }

        /// <summary>
        /// 已倾斜的单位.
        /// </summary>
        public int SkewedUnit { get; private set; }



        public ImageManipulator(Bitmap img)
        {
            if (img == null)
            {
                throw new ArgumentNullException("图像不能为空.");
            }

            // 初始化属性值.
            this.Image = img;
            this.ContentSize = img.Size;
            RotatedAngle = 0;
        }

        /// <summary>
        /// 重新设置图像的大小.
        /// 这个方法使用指定的尺寸创建一个新的Bitmap实例.
        /// </summary>
        public void ResizeImage(Size size)
        {
            Bitmap newImg = null;
            try
            {

                // 使用指定的尺寸创建一个新的Bitmap实例.
                newImg = new Bitmap(Image, size);

                // 释放原始图像.
                this.Image.Dispose();

                // 设置 this.Image 为新的位图.
                this.Image = newImg;

                this.ContentSize = this.Image.Size;
            }
            catch
            {

                // 如果有任何异常, 释放新位图.
                if (newImg != null)
                {
                    newImg.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// 使用指定的系数拉伸图像.
        /// 步骤:
        /// 1 计算新的尺寸.
        /// 2 使用指定系数初始化一个矩阵. 这个矩阵用于图像的转换.
        /// 3 将原始图片绘制到新位图上, 然后转化就会生效.
        /// </summary>
        public void Scale(float xFactor, float yFactor, InterpolationMode mode)
        {
            Bitmap newImg = null;
            Graphics g = null;
            Matrix mtrx = null;

            try
            {
                // 使用指定的尺寸创建一个新的Bitmap实例.
                newImg = new Bitmap(Convert.ToInt32(this.Image.Size.Width * xFactor),
                    Convert.ToInt32(this.Image.Size.Height * yFactor));

                // 从位图获得Graphics对象.
                g = Graphics.FromImage(newImg);

                // 设置 Interpolation Mode. 
                g.InterpolationMode = mode;

                // 使用指定系数初始化一个矩阵. 这个矩阵用于图像的转换. 
                mtrx = new Matrix(xFactor, 0, 0, yFactor, 0, 0);

                // 使用矩阵转换图像.
                g.MultiplyTransform(mtrx, MatrixOrder.Append);

                // 将原始图片绘制到新位图上, 然后转化就会生效.
                g.DrawImage(this.Image, 0, 0);

                // 释放原始图像.
                this.Image.Dispose();

                // 设置 this.Image 为新的位图.
                this.Image = newImg;

                this.ContentSize = this.Image.Size;
            }
            catch
            {

                // 如果有任何异常, 释放新位图.
                if (newImg != null)
                {
                    newImg.Dispose();
                }

                throw;
            }
            finally
            {

                // 释放 Graphics 和 Matrix 对象.
                if (g != null)
                {
                    g.Dispose();
                }

                if (mtrx != null)
                {
                    mtrx.Dispose();
                }
            }
        }

        /// <summary>
        /// 选择, 翻转图像.
        /// </summary>
        /// <param name="type">
        /// 一个 System.Drawing.RotateFlipType 的成员指示旋转或翻转的类型.
        /// </param>
        public void RotateFlip(RotateFlipType type)
        {
            // 选择, 翻转图像.
            this.Image.RotateFlip(type);

            // 计算旋转的角度. 
            switch (type)
            {
                // Rotate180FlipXY 等价于 RotateNoneFlipNone;
                case RotateFlipType.RotateNoneFlipNone:
                    break;

                // Rotate270FlipXY 等价于 Rotate90FlipNone;
                case RotateFlipType.Rotate90FlipNone:
                    this.RotatedAngle += 90;
                    break;

                // RotateNoneFlipXY 等价于 Rotate180FlipNone;
                case RotateFlipType.Rotate180FlipNone:
                    this.RotatedAngle += 180;
                    break;

                // Rotate90FlipXY 等价于 Rotate270FlipNone;
                case RotateFlipType.Rotate270FlipNone:
                    this.RotatedAngle += 270;
                    break;

                // Rotate180FlipY 等价于 RotateNoneFlipX;
                case RotateFlipType.RotateNoneFlipX:
                    this.RotatedAngle = 180 - this.RotatedAngle;
                    break;

                // Rotate270FlipY 等价于 Rotate90FlipX;
                case RotateFlipType.Rotate90FlipX:
                    this.RotatedAngle = 90 - this.RotatedAngle;
                    break;

                // Rotate180FlipX 等价于 RotateNoneFlipY;
                case RotateFlipType.RotateNoneFlipY:
                    this.RotatedAngle = 360 - this.RotatedAngle;
                    break;

                // Rotate270FlipX 等价于 Rotate90FlipY;
                case RotateFlipType.Rotate90FlipY:
                    this.RotatedAngle = 270 - this.RotatedAngle;
                    break;

                default:
                    break;
            }

            // 旋转的角度是0~360度.
            if (RotatedAngle >= 360)
            {
                RotatedAngle -= 360;
            }
            if (RotatedAngle < 0)
            {
                RotatedAngle += 360;
            }
        }

        /// <summary>
        /// 旋转图像至任意角度.
        /// 步骤:
        /// 1 计算实际需要的大小.
        /// 2 将图像的中心点移至(0,0).
        /// 3 旋转指定角度.
        /// 4 将图像的中心点移至新图像的中心.
        /// </summary>
        public void RotateImg(float angle)
        {
            RectangleF necessaryRectangle = RectangleF.Empty;

            // 计算实际需要的大小.
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddRectangle(
                    new RectangleF(0f, 0f, ContentSize.Width, ContentSize.Height));
                using (Matrix mtrx = new Matrix())
                {
                    float totalAngle = angle + this.RotatedAngle;
                    while (totalAngle >= 360)
                    {
                        totalAngle -= 360;
                    }
                    this.RotatedAngle = totalAngle;
                    mtrx.Rotate(RotatedAngle);

                    // 实际需要的大小.
                    necessaryRectangle = path.GetBounds(mtrx);
                }
            }

            Bitmap newImg = null;
            Graphics g = null;

            try
            {

                // 使用指定的尺寸创建一个新的Bitmap实例.
                newImg = new Bitmap(Convert.ToInt32(necessaryRectangle.Width),
                 Convert.ToInt32(necessaryRectangle.Height));

                // 从位图获得Graphics对象.
                g = Graphics.FromImage(newImg);

                // 将图像的中心点移至(0,0).
                g.TranslateTransform(-this.Image.Width / 2, -this.Image.Height / 2);

                // 旋转指定角度.
                g.RotateTransform(angle, MatrixOrder.Append);

                // 将图像的中心点移至新图像的中心.
                g.TranslateTransform(newImg.Width / 2, newImg.Height / 2,
                    MatrixOrder.Append);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(this.Image, 0, 0);

                // 释放原始图像.
                this.Image.Dispose();

                // 设置 this.Image 为新的位图.
                this.Image = newImg;

            }
            catch
            {

                // 如果有任何异常, 释放新位图.
                if (newImg != null)
                {
                    newImg.Dispose();
                }

                throw;
            }
            finally
            {

                // 释放 Graphics 对象.
                if (g != null)
                {
                    g.Dispose();
                }

            }
        }

        /// <summary>
        /// 通过指定原始图像的左上角、右上角和左下角的目标点可旋转、反射和扭曲图像。 这三个目标点确定
        /// 将原始矩形图像映射为平行四边形的仿射变换。
        /// 例如, 原始的图像所在的矩形是 {[0,0], [100,0], [100,50],[50, 0]}, 倾斜 -10个单元后,
        /// 结果是 {[-10,0], [90,0], [100,50],[50, 0]}. 
        /// </summary>
        /// <param name="unit">
        /// 倾斜的单元. 
        /// </param>
        public void Skew(int unit)
        {
            RectangleF necessaryRectangle = RectangleF.Empty;
            int totalUnit = 0;

            // 计算实际需要的大小.
            using (GraphicsPath path = new GraphicsPath())
            {
                Point[] newPoints = null;

                totalUnit = SkewedUnit + unit;

                newPoints = new Point[] 
                {

                    new Point(totalUnit, 0),
                    new Point(totalUnit+this.ContentSize.Width, 0),
                    new Point(this.ContentSize.Width, this.ContentSize.Height),
                    new Point(0, this.ContentSize.Height),
                };
                path.AddLines(newPoints);
                necessaryRectangle = path.GetBounds();
            }


            Bitmap newImg = null;
            Graphics g = null;

            try
            {

                // 使用指定的尺寸创建一个新的Bitmap实例.
                newImg = new Bitmap(Convert.ToInt32(necessaryRectangle.Width),
                 Convert.ToInt32(necessaryRectangle.Height));

                // 从位图获得Graphics对象.
                g = Graphics.FromImage(newImg);

                // 移动图像.
                if (totalUnit <= 0 && SkewedUnit <=0)
                {
                    g.TranslateTransform(-unit, 0, MatrixOrder.Append);
                }

                g.InterpolationMode = InterpolationMode.HighQualityBilinear;

                Point[] destinationPoints = new Point[] 
                {
                    new Point(unit, 0),
                    new Point(unit+this.Image.Width, 0),
                    new Point(0, this.Image.Height),
                };
                
                g.DrawImage(this.Image, destinationPoints);

                // 释放原始图像.
                this.Image.Dispose();

                // 设置 this.Image 为新的位图.
                this.Image = newImg;
           
                SkewedUnit = totalUnit;             
            }
            catch
            {

                // 如果有任何异常, 释放新位图.
                if (newImg != null)
                {
                    newImg.Dispose();
                }

                throw;
            }
            finally
            {

                // 释放 Graphics 对象.
                if (g != null)
                {
                    g.Dispose();
                }

            }

        }



        /// <summary>
        /// 将图像绘制在控件上.
        /// </summary>
        public void DrawControl(Control control, Point adjust, Pen penToDrawBounds)
        {

            // 为控件创建Graphics对象.
            Graphics graphicsForPanel = control.CreateGraphics();

            // 清理控件的图像, 并设置背景颜色.
            graphicsForPanel.Clear(System.Drawing.SystemColors.ControlDark);

            // 将图像绘制在控件的中心.
            Point p = new Point((control.Width - this.Image.Size.Width) / 2,
                (control.Height - this.Image.Size.Height) / 2);

            // 调整位置.
            graphicsForPanel.TranslateTransform(adjust.X, adjust.Y);

            graphicsForPanel.DrawImage(this.Image, p);

            // 绘制边框.
            if (penToDrawBounds != null)
            {

                var unit = GraphicsUnit.Pixel;
                var rec = this.Image.GetBounds(ref unit);

                graphicsForPanel.DrawRectangle(penToDrawBounds, rec.X + p.X, rec.Y + p.Y, rec.Width, rec.Height);
                graphicsForPanel.DrawLine(penToDrawBounds,
                    rec.X + p.X, rec.Y + p.Y,
                    rec.X + p.X + rec.Width, rec.Y + p.Y + rec.Height);

                graphicsForPanel.DrawLine(penToDrawBounds,
                   rec.X + p.X + rec.Width, rec.Y + p.Y,
                   rec.X + p.X, rec.Y + p.Y + rec.Height);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // 释放托管资源.
                if (this.Image != null)
                {
                    this.Image.Dispose();
                }

            }
            disposed = true;
        }
    }
}
