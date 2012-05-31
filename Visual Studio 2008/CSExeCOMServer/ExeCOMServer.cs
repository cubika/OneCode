/********************************* 模块头 **********************************\
* 模块名:      ExeCOMServer.cs
* 项目名:      CSExeCOMServer
* 版权 (c) Microsoft Corporation.
* 
* ExeCOMServer封装了C#中线程外COM服务器的框架。这个类实现了单态（Singleton）设计
* 模式，并且它是线程安全的。执行CSExeCOMServer.Instance.Run()以启动服务器。如果
* 服务器正在运行，此函数将立即返回。在Run方法中，它注册了COM服务器所公开的COM类中类
* 组件。并且启动消息循环以等待锁定计数器回落至0。当锁定计数器为0时，它撤销注册并退
* 出服务器。
* 
* 当一个COM对象被创建时，服务器的锁定计数器将会增加。当一个对象被释放时（被GC时），
* 锁定计数器会减少。为确保COM对象会被即时的垃圾回收（GC），ExeCOMServer在服务器启动后
* 每隔5秒钟触发一次垃圾回收。
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
using System.Text;
using System.Threading;
#endregion


namespace CSExeCOMServer
{
    sealed internal class ExeCOMServer
    {
        #region Singleton Pattern

        private ExeCOMServer()
        {
        }

        private static ExeCOMServer _instance = new ExeCOMServer();
        public static ExeCOMServer Instance
        {
            get { return _instance; }
        }

        #endregion


        private object syncRoot = new Object();  // 锁定中的线程同步
        private bool _bRunning = false;  // 是否服务器正在运行

        // 运行消息循环的线程ID
        private uint _nMainThreadID = 0;

        // 服务器中的锁定计数器（记录已激活的COM对象数目）
        private int _nLockCnt = 0;
        
        // 用于每5秒触发GC的定时器
        private Timer _gcTimer;

        /// <summary>
        /// 这个方法实现了在COM服务器启动后每5秒触发一次GC。
        /// </summary>
        /// <param name="stateInfo"></param>
        private static void GarbageCollect(object stateInfo)
        {
            GC.Collect();   // GC
        }

        private uint _cookieSimpleObj;

        /// <summary>
        /// PreMessageLoop负责注册COM类工厂，并且初始化COM服务器的关键成员变量
        /// （比如_nMainThreadID和_nLockCnt）
        /// </summary>
        private void PreMessageLoop()
        {
            /////////////////////////////////////////////////////////////////
            // 注册COM类工厂
            // 

            Guid clsidSimpleObj = new Guid(CSSimpleObject.ClassId);

            // 注册CSSimpleObject类对象 
            int hResult = COMNative.CoRegisterClassObject(
                ref clsidSimpleObj,                 // 即将被注册的CLSID
                new CSSimpleObjectClassFactory(),   // 类工厂
                CLSCTX.LOCAL_SERVER,                // 执行代码块
                REGCLS.MULTIPLEUSE | REGCLS.SUSPENDED,
                out _cookieSimpleObj);
            if (hResult != 0)
            {
                throw new ApplicationException(
                    "CoRegisterClassObject failed w/err 0x" + hResult.ToString("X"));
            }

            // 注册其他类对象
            // ...

            // 通知SCM所有的已注册类以及在服务器进程中开始激活请求。
            hResult = COMNative.CoResumeClassObjects();
            if (hResult != 0)
            {
                // 在失败时撤销 CSSimpleObject的注册
                if (_cookieSimpleObj != 0)
                {
                    COMNative.CoRevokeClassObject(_cookieSimpleObj);
                }

                // 撤销其他类的注册
                // ...

                throw new ApplicationException(
                    "CoResumeClassObjects failed w/err 0x" + hResult.ToString("X"));
            }


            /////////////////////////////////////////////////////////////////
            // 初始化成员变量

            // 记录COM服务器当前运行中的线程ID。这样，服务器可以知道哪里可以发送
            // WM_QUIT消息，以便退出消息循环。
            _nMainThreadID = NativeMethod.GetCurrentThreadId();

            // 记录在服务器中激活的COM对象的数目。当_nLockCnt为0时，服务器可以关闭。
            _nLockCnt = 0;

            // 启动GC计时器以触发每5秒一次的GC。
            _gcTimer = new Timer(new TimerCallback(GarbageCollect), null,
                5000, 5000);
        }

        /// <summary>
        /// RunMessageLoop运行标准的消息循环。该消息循环再收到WM_QUIT时退出。
        /// </summary>
        private void RunMessageLoop()
        {
            MSG msg;
            while (NativeMethod.GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                NativeMethod.TranslateMessage(ref msg);
                NativeMethod.DispatchMessage(ref msg);
            }
        }

        /// <summary>
        /// PostMessageLoop被用于撤销对服务器可见的COM类并且执行清理。
        /// </summary>
        private void PostMessageLoop()
        {
            /////////////////////////////////////////////////////////////////
            // 撤销COM类的注册

            // 撤销CSSimpleObject的注册
            if (_cookieSimpleObj != 0)
            {
                COMNative.CoRevokeClassObject(_cookieSimpleObj);
            }

            // 撤销其他类
            // ...


            /////////////////////////////////////////////////////////////////
            // 执行清理

            // 关闭GC计时器
            if (_gcTimer != null)
            {
                _gcTimer.Dispose();
            }

            //等待其他进程关闭
            Thread.Sleep(1000);
        }

        /// <summary>
        /// 运行COM服务器。如果服务器正在运行，此函数直接返回。
        /// </summary>
        /// <remarks>这个方法是线程安全的</remarks>
        public void Run()
        {
            lock (syncRoot) // 确认线程安全
            {
                // 如果服务器正在运行，直接返回
                if (_bRunning)
                    return;

                // 用于确认服务器是否正在运行
                _bRunning = true;
            }

            try
            {
                // 执行PreMessageLoop以执行初始化成员变量和注册类工厂。
                PreMessageLoop();

                // 执行消息循环。
                RunMessageLoop();

                // 执行PostMessageLoop以便撤销注销
                PostMessageLoop();
            }
            finally
            {
                _bRunning = false;
            }
        }

        /// <summary>
        /// 增加锁定计数
        /// </summary>
        /// <returns>在数值增加后返回新的锁定计数器</returns>
        /// <remarks>此方法是线程安全的</remarks>
        public int Lock()
        {
            return Interlocked.Increment(ref _nLockCnt);
        }

        /// <summary>
        /// 减少锁定计数。当锁定计时器的值为0时，发送WM_QUIT消息，以便关闭COM服务器
        /// </summary>
        /// <returns>在数值减少后返回新的锁定计数器</returns>
        public int Unlock()
        {
            int nRet = Interlocked.Decrement(ref _nLockCnt);

            // 档计数为0时，终止服务器。
            if (nRet == 0)
            {
                // 为主线程发送一个WM_QUIT消息
                NativeMethod.PostThreadMessage(_nMainThreadID, 
                    NativeMethod.WM_QUIT, UIntPtr.Zero, IntPtr.Zero); 
            }

            return nRet;
        }

        /// <summary>
        /// 返回当前锁定计数器
        /// </summary>
        /// <returns></returns>
        public int GetLockCount()
        {
            return _nLockCnt;
        }
    }
}