/********************************* 模块头 **********************************\
* 模块名:      CSSimpleObject.cs
* 项目名:      CSExeCOMServer
* 版权 (c) Microsoft Corporation.
* 
* 定义COM类，CSSimpleObject， 和其ClassFactory，CSSimpleObjectClassFactory
* 
* （在编写您自己的COM服务器时，请生成新的GUID）
* Program ID: CSExeCOMServer.CSSimpleObject
* CLSID_CSSimpleObject: DB9935C1-19C5-4ed2-ADD2-9A57E19F53A3
* IID_ICSSimpleObject: 941D219B-7601-4375-B68A-61E23A4C8425
* DIID_ICSSimpleObjectEvents: 014C067E-660D-4d20-9952-CD973CE50436
* 
* 属性：
*   // 包括访问方法get和put 
*   float FloatProperty
* 
* 方法：
*   // HelloWorld 返回一个字符串"HelloWorld"
*   string HelloWorld();
* 
*   // GetProcessThreadID输出正在运行的进程ID和线程ID
*   void GetProcessThreadID(out uint processId, out uint threadId);
* 
* 事件：
*    // FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
*    // 客户端可以通过参数Cancel来取消对FloatProperty的修改。
* void FloatPropertyChanging(float NewValue, ref bool Cancel);
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
using System.Runtime.InteropServices;
using System.ComponentModel;
#endregion


namespace CSExeCOMServer
{
    #region Interfaces

    [Guid(CSSimpleObject.InterfaceId), ComVisible(true)]
    public interface ICSSimpleObject
    {
        #region Properties

        float FloatProperty { get; set; }

        #endregion

        #region Methods

        string HelloWorld();

        void GetProcessThreadID(out uint processId, out uint threadId);

        #endregion
    }

    [Guid(CSSimpleObject.EventsId), ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICSSimpleObjectEvents
    {
        #region Events

        [DispId(1)]
        void FloatPropertyChanging(float NewValue, ref bool Cancel);

        #endregion
    }

    #endregion

    [ClassInterface(ClassInterfaceType.None)]           // No ClassInterface
    [ComSourceInterfaces(typeof(ICSSimpleObjectEvents))]
    [Guid(CSSimpleObject.ClassId), ComVisible(true)]
    public class CSSimpleObject : ReferenceCountedObject, ICSSimpleObject
    {
        #region COM Component Registration

        internal const string ClassId =
            "DB9935C1-19C5-4ed2-ADD2-9A57E19F53A3";
        internal const string InterfaceId =
            "941D219B-7601-4375-B68A-61E23A4C8425";
        internal const string EventsId =
            "014C067E-660D-4d20-9952-CD973CE50436";

        // 这些程序描述了服务器所需要的额外的COM注册过程

        [EditorBrowsable(EditorBrowsableState.Never)]
        [ComRegisterFunction()]
        public static void Register(Type t)
        {
            try
            {
                COMHelper.RegasmRegisterLocalServer(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // 记录错误
                throw ex; // 再次抛出此异常
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [ComUnregisterFunction()]
        public static void Unregister(Type t)
        {
            try
            {
                COMHelper.RegasmUnregisterLocalServer(t);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // 记录错误
                throw ex; // 再次抛出异常
            }
        }

        #endregion

        #region Properties

        private float fField = 0;

        public float FloatProperty
        {
            get { return this.fField; }
            set
            {
                bool cancel = false;
                // 触发FloatPropertyChanging事件
                if (null != FloatPropertyChanging)
                    FloatPropertyChanging(value, ref cancel);
                if (!cancel)
                    this.fField = value;
            }
        }

        #endregion

        #region Methods

        public string HelloWorld()
        {
            return "HelloWorld";
        }

        public void GetProcessThreadID(out uint processId, out uint threadId)
        {
            processId = NativeMethod.GetCurrentProcessId();
            threadId = NativeMethod.GetCurrentThreadId();
        }

        #endregion

        #region Events

        [ComVisible(false)]
        public delegate void FloatPropertyChangingEventHandler(float NewValue, ref bool Cancel);
        public event FloatPropertyChangingEventHandler FloatPropertyChanging;

        #endregion
    }

    /// <summary>
    /// 为CSSimpleObject创造的一个工厂类。
    /// </summary>
    internal class CSSimpleObjectClassFactory : IClassFactory
    {
        public int CreateInstance(IntPtr pUnkOuter, ref Guid riid, 
            out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;

            if (pUnkOuter != IntPtr.Zero)
            {
                // pUnkOuter变量为非空，并且此对象不支持聚合（aggregation）。
                Marshal.ThrowExceptionForHR(COMNative.CLASS_E_NOAGGREGATION);
            }

            if (riid == new Guid(CSSimpleObject.ClassId) ||
                riid == new Guid(COMNative.IID_IDispatch) ||
                riid == new Guid(COMNative.IID_IUnknown))
            {
                // 创建一个.NET对象的实例
                ppvObject = Marshal.GetComInterfaceForObject(
                    new CSSimpleObject(), typeof(ICSSimpleObject));
            }
            else
            {
                // 被ppvObject所指向的对象不被此riid的接口支持。
                Marshal.ThrowExceptionForHR(COMNative.E_NOINTERFACE);
            }

            return 0;   // S_OK
        }

        public int LockServer(bool fLock)
        {
            return 0;   // S_OK
        }
    }

    /// <summary>
    /// ReferenceCountedObject 类.
    /// 
    /// </summary>
    [ComVisible(false)]
    public class ReferenceCountedObject
    {
        public ReferenceCountedObject()
        {
            // 增加COM服务器中的锁定计数器。
            ExeCOMServer.Instance.Lock();
        }

        ~ReferenceCountedObject()
        {
            //减少COM服务器中的锁定服务器。
            ExeCOMServer.Instance.Unlock();
        }
    }
}
