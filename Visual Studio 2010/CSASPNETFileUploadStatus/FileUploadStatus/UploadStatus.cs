/********************************* 模块头 ***********************************\
* 模块名:    UploadStatus.cs
* 项目名:    CSASPNETFileUploadStatus
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
* 像ActiveX 控件,Flash 或者Silverlight.
* 
* 我们使用这个类来存储上传进度状态.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Threading;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.IO;

namespace CSASPNETFileUploadStatus
{
    #region ////////////事件委托////////////
    // 我们定义一个委托EventHandler来把事件保存到UploadStatus.
    public delegate void UploadStatusEventHandler(
                        object sender, UploadStatusEventArgs e);

    public class UploadStatusEventArgs : EventArgs
    {
        [ScriptIgnore]
        public HttpContext context
        {
            get;
            protected set;
        }
        public UploadStatusEventArgs(HttpContext ctx)
        {
            context = ctx;
        }
    }
    #endregion

    [Serializable]
    public class UploadStatus
    {
        #region ////////////私有变量////////////

        private enum DataUnit
        {
            Byte = 1,
            KB = 1024,
            MB = 1048576,
            GB = 1073741824
        }
        private enum TimeUnit
        {
            Seconds = 1,
            Minutes = 60,
            Hours = 3600,
            Day = 86400
        }

        //返回上传数据的单元.
        private DataUnit LoadedUnit
        {
            get
            {
                return GetDataUnit(LoadedLength);
            }
        }
        //返回全部内容数据的单元.
        private DataUnit ContentUnit
        {
            get
            {
                return GetDataUnit(ContentLength);
            }
        }

        //请求的内容
        [NonSerialized]
        private HttpContext Context;

        #endregion

        #region ////////////公有属性////////////

        /// <summary>
        /// 如果用户终止上传，它将会返回true.
        /// </summary>
        public bool Aborted
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取文件内容的长度.
        /// </summary>
        public long ContentLength
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取单元被格式化的文件的内容长度.
        /// </summary>
        public string ContentLengthString
        {
            get
            {
                decimal rslWithUnit = (decimal)ContentLength / (int)ContentUnit;
                return rslWithUnit.ToString("0.00") + " " + ContentUnit.ToString();
            }
        }
        /// <summary>
        /// 获取上传文件的内容长度
        /// </summary>
        public long LoadedLength
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取单元被格式化的上传 
        /// 文件内容的长度
        /// </summary>
        public string LoadedLengthString
        {
            get
            {
                decimal rslWithUnit = (decimal)LoadedLength / (int)LoadedUnit;
                return rslWithUnit.ToString("0.00") + " " + LoadedUnit.ToString();
            }
        }
        /// <summary>
        /// 获取上传开始时的时间.
        /// </summary>
        public DateTime StartTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取上传结束或终止时的时间.
        /// </summary>
        public DateTime EndTime
        {
            get;
            private set;
        }
        /// <summary>
        /// 检查上传是否结束.
        /// </summary>
        public bool IsFinished
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取上传内容的百分比
        /// </summary>
        public int LoadedPersentage
        {
            get
            {
                int percent = Convert.ToInt32(
                        Math.Ceiling(
                            (decimal)LoadedLength / (decimal)ContentLength
                            * 100
                        )
                    );
                return percent;
            }
        }
        /// <summary>
        /// 获取上传花费时间
        /// 单位时秒
        /// </summary>
        public double SpendTimeSeconds
        {
            get
            {
                DateTime calcTime = DateTime.Now;
                if (IsFinished || Aborted)
                {
                    calcTime = EndTime;
                }
                double spendtime = Math.Ceiling(calcTime.Subtract(StartTime).TotalSeconds);
                if (spendtime == 0 && IsFinished)
                {
                    spendtime = 1;
                }
                return spendtime;
            }
        }
        /// <summary>
        /// 获取单元被格式化的上传花费的时间
        /// </summary>
        public string SpendTimeString
        {
            get
            {
                double spent = SpendTimeSeconds;
                TimeUnit unit = GetTimeUnit(spent);
                double unitTime = spent / (int)GetTimeUnit(spent);
                return unitTime.ToString("0.0") + " " + unit.ToString();
            }
        }

        /// <summary>
        /// 获取上传速度
        /// 单位是 字节/秒
        /// </summary>
        public double UploadSpeed
        {
            get
            {
                double spendtime = SpendTimeSeconds;
                double speed = (double)LoadedLength / spendtime;
                return speed;
            }
        }
        /// <summary>
        /// 获取单元被格式化的上传速度.
        /// </summary>
        public string UploadSpeedString
        {
            get
            {
                double spendtime = SpendTimeSeconds;
                DataUnit unit = GetDataUnit((long)Math.Ceiling((double)LoadedLength / spendtime));
                double speed = UploadSpeed / (int)unit;
                return speed.ToString("0.0") + " " + unit.ToString() + "/seconds";
            }
        }
        /// <summary>
        /// 获取剩余时间
        /// 单位是秒
        /// </summary>
        public double LeftTimeSeconds
        {
            get
            {
                double remain = Math.Floor((ContentLength - LoadedLength) / UploadSpeed);
                return remain;
            }
        }
        /// <summary>
        /// 获取被格式化单元的剩余时间
        /// </summary>
        public string LeftTimeString
        {
            get
            {
                double remain = LeftTimeSeconds;
                TimeUnit unit = GetTimeUnit(remain);
                double newRemain = remain / (int)unit;
                return newRemain.ToString("0.0") + " " + unit.ToString();
            }
        }

        #endregion

        #region ////////////事件////////////
        public event UploadStatusEventHandler OnDataChanged;
        public event UploadStatusEventHandler OnFinish;
        #endregion

        #region ////////////构造函数////////////
        public UploadStatus()
        {
        }
        public UploadStatus(HttpContext ctx, long length)
        {
            Aborted = false;
            IsFinished = false;
            StartTime = DateTime.Now;
            Context = ctx;
            ContentLength = length;
            UpdateLoadedLength(0);
        }
        #endregion

        #region ////////////方法////////////
        private TimeUnit GetTimeUnit(double seconds)
        {
            if (seconds > (int)TimeUnit.Day)
            {
                return TimeUnit.Day;
            }
            if (seconds > (int)TimeUnit.Hours)
            {
                return TimeUnit.Hours;
            }
            if (seconds > (int)TimeUnit.Minutes)
            {
                return TimeUnit.Minutes;
            }
            return TimeUnit.Seconds;

        }
        private DataUnit GetDataUnit(long length)
        {
            if (length > Math.Pow(2D, 30))
            {
                return DataUnit.GB;
            }
            if (length > Math.Pow(2D, 20))
            {
                return DataUnit.MB;
            }
            if (length > Math.Pow(2D, 10))
            {
                return DataUnit.KB;
            }
            return DataUnit.Byte;
        }

        private void changeFinish()
        {
            if (OnFinish != null)
            {
                OnFinish(this, new UploadStatusEventArgs(Context));
            }
        }
        private void changeData()
        {
            if (Aborted)
            {
                return;
            }
            if (OnDataChanged != null)
            {
                OnDataChanged(this, new UploadStatusEventArgs(Context));
            }
            if (LoadedLength == ContentLength)
            {
                EndTime = DateTime.Now;
                IsFinished = true;
                changeFinish();
            }
        }
        public void Abort()
        {
            Aborted = true;
            EndTime = DateTime.Now;
        }
        public void UpdateLoadedLength(long length)
        {
            if (!IsFinished && !Aborted)
            {
                LoadedLength += length;
                changeData();
            }
        }



        #endregion
    }
}
