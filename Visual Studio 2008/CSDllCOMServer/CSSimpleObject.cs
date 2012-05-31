/********************************* 模块头 *********************************\
* 模块名:      CSSimpleObject.cs
* 项目名:      CSDllCOMServer
* 版权 (c) Microsoft Corporation.
* 
* 这个示例着重于使用COM技术导出.Net Framework组件。这个允许我们为COM开发人员
* 编写一个.Net类型并且在非托管代码中使用此类型。此文件定义的COM组件，它实现了
* 一个显式定义接口：
* 
* CSSimpleObject - [Explicitly Define a Class Interface]
* 
* Program ID: CSDllCOMServer.CSExplicitInterfaceObject
* CLSID_CSExplicitInterfaceObject: 4B65FE47-2F9D-37B8-B3CB-5BE4A7BC0926
* IID_ICSExplicitInterfaceObject: 32DBA9B0-BE1F-357D-827F-0196229FA0E2
* DIID_ICSExplicitInterfaceObjectEvents: 95DB823B-E204-428c-92A3-7FB29C0EC576
* LIBID_CSDllCOMServer: F0998D9A-0E79-4F67-B944-9E837F479587
* 
* 属性：
*  // 包括get和set存取方法。
* public float FloatProperty
* 
* 方法：
* // HelloWorld 返回一个字符串“HelloWorld”
* public string HelloWorld();
* // GetProcessThreadID输出正在运行的线程ID和进程ID
* public void GetProcessThreadID(out uint processId, out uint threadId);
* 
* 事件：
* // FloatPropertyChanging在新的FloatProperty属性被设置之前触发。
* // 客户端可以通过参数Cancel来取消对FloatProperty的修改。
* void FloatPropertyChanging(float NewValue, ref bool Cancel);
* 
* -------
* 在构建导出到COM客户端的.Net组件时，我们不推荐使用ClassInterface。使用不同的
* 接口显式的描述哪些成员将会被导出，并且使用.Net组件实现这些接口是我们认为值得
* 推荐的方法。使用ClassInterface虽然是一个快速并且简单的方法导出.Net组件到一个
* COM客户端（参考CSImplicitInterfaceObject），但是这不是我们所推荐的方法。
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
using System.Runtime.InteropServices;
#endregion


namespace CSDllCOMServer
{
    #region Interfaces

    /// <summary>
    /// 这个public接口用于描述从类的COM接口
    /// </summary>
    [Guid("32DBA9B0-BE1F-357D-827F-0196229FA0E2")]          // IID
    [ComVisible(true)]
    // 默认使用双接口。这使客户端获得最佳早绑定及晚绑定。
    //[InterfaceType(ComInterfaceType.InterfaceIsDual)]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICSSimpleObject
    {
        #region Properties

        float FloatProperty { get; set; }

        #endregion

        #region Methods

        string HelloWorld();

        void GetProcessThreadID(out uint processId, out uint threadId);

        [ComVisible(false)]
        void HiddenFunction();

        #endregion
    }

    /// <summary>
    /// 此public接口描述了从类能触发的事件
    /// </summary>
    [Guid("95DB823B-E204-428c-92A3-7FB29C0EC576")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface ICSSimpleObjectEvents
    {
        #region Events

        [DispId(1)]
        void FloatPropertyChanging(float NewValue, ref bool Cancel);

        #endregion
    }

    #endregion

    [ClassInterface(ClassInterfaceType.None)]           // 没有 ClassInterface
    [ComSourceInterfaces(typeof(ICSSimpleObjectEvents))]
    [Guid("4B65FE47-2F9D-37B8-B3CB-5BE4A7BC0926")]      // CLSID
    //[ProgId("CSCOMServerDll.CustomCSSimpleObject2")]  // ProgID
    [ComVisible(true)]
    public class CSSimpleObject : ICSSimpleObject
    {
        #region Properties

        /// <summary>
        /// private成员将不列入类库，它们将不显示在COM客户端中。
        /// </summary>
        private float fField = 0;

        /// <summary>
        /// 一个public属性，它包括了get和set存取方法
        /// </summary>
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

        /// <summary>
        /// 一个public方法，它返回一个“HelloWorld”字符串。
        /// </summary>
        /// <returns>"HelloWorld"</returns>
        public string HelloWorld()
        {
            return "HelloWorld";
        }

        /// <summary>
        /// 一个public方法，它拥有2个返回值：当前进程ID和当前线程ID
        /// </summary>
        /// <param name="processId">[out] 当前进程ID</param>
        /// <param name="threadId">[out] 当前线程ID</param>
        public void GetProcessThreadID(out uint processId, out uint threadId)
        {
            processId = NativeMethod.GetCurrentProcessId();
            threadId = NativeMethod.GetCurrentThreadId();
        }

        /// <summary>
        /// 一个隐藏的方法(ComVisible = false)。
        /// </summary>
        public void HiddenFunction()
        {
            Console.WriteLine("HiddenFunction is called.");
        }
        
        #endregion

        #region Events

        [ComVisible(false)]
        public delegate void FloatPropertyChangingEventHandler(float NewValue, ref bool Cancel);

        /// <summary>
        /// 一个public事件，它在FloatProperty属性被赋予新值之前被触发。参数Cancel
        /// 用于取消对FloatProperty属性的修改。
        /// </summary>
        public event FloatPropertyChangingEventHandler FloatPropertyChanging;

        #endregion
    }

}