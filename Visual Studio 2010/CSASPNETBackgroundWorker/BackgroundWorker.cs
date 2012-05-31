/********************************** 模块头 ***********************************\
* 模块名:        BackgroundWorker.cs
* 项目名:        CSASPNETBackgroundWorker
* 版权(c) Microsoft Corporation
*
* BackgroundWorker类调用一个单独的线程的方法.它允许在被调用时传递参数给方法.
* 它可以让目标方法的报告的进展和结果.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Threading;

namespace CSASPNETBackgroundWorker
{
    /// <summary>
    /// 这个类是用来在一个单独的线程执行操作.
    /// </summary>
    public class BackgroundWorker
    {
        /// <summary>
        /// 这个线程是用来在后台运行操作.
        /// </summary>
        Thread _innerThread = null;

        #region Properties
        /// <summary>
        /// 显示当前进度的整数.
        /// 100表示操作完成.
        /// </summary>
        public int Progress 
        {
            get 
            {
                return _progress;
            }
        }
        int _progress = 0;

        /// <summary>
        /// 用来保存操作结果的对象.
        /// </summary>
        public object Result
        {
            get
            {
                return _result;
            }
        }
        object _result = null;

        /// <summary>
        /// 用来分辨当前后台工作是否运行的布尔值.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_innerThread != null)
                {
                    return _innerThread.IsAlive;
                }
                return false;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress">
        /// 改变其值以报告进度
        /// </param>
        /// <param name="_result">
        /// 作为结果保存的值
        /// </param>
        /// <param name="arguments">
        /// 传递到操作方法的参数
        /// </param>
        public delegate void DoWorkEventHandler(ref int progress, 
            ref object _result, params object[] arguments);

        public event DoWorkEventHandler DoWork;
        #endregion

        /// <summary>
        /// 开始执行后台操作.
        /// </summary>
        /// <param name="arguments">
        /// 将被传递到操作方法的参数
        /// </param>
        public void RunWorker(params object[] arguments)
        {
            if (DoWork != null)
            {
                _innerThread = new Thread(() =>
                {
                    _progress = 0;
                    DoWork.Invoke(ref _progress, ref _result, arguments);
                    _progress = 100;
                });
                _innerThread.Start();
            }
        }
    }
}