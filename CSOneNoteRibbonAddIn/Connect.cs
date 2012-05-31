/****************************** 模块头 ************************************\
模块名:  Connect.cs
项目名:  CSOneNoteRibbonAddIn
版权 (c) Microsoft Corporation.

寄宿发生于外接程序上的事件通知，例如,当它们被加载、卸载、更新等等.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSOneNoteRibbonAddIn
{
    #region 引用的命名空间
    using System;
    using Extensibility;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.Office.Core;
    using System.Runtime.InteropServices.ComTypes;
    using System.IO;
    using CSOneNoteRibbonAddIn.Properties;
    using System.Drawing.Imaging;
    using OneNote = Microsoft.Office.Interop.OneNote; 
    #endregion

    #region 插件安装和信息设置的提示
    // 在运行时，外接程序向导准备注册表加载.
    // 在以后的时间，如果外接程序由于某些原因,变为不可用,例如:
    //   1) 您移动此项目到一台计算机上,该计算机并不是最初创建该项目的机器.
    //   2) 当显示一条消息询问您是否要删除该外接程序,您选择了"是".
    //   3) 注册表损坏.
    // 您将需要通过生成 CSOneNoteRibbonAddInSetup 项目，重新注册,在“解决方案资源管理器”
    // 中右击该项目,选择安装.
    #endregion

    /// <summary>
    ///  用于实现一个外接程序的对象.
    /// </summary>
    /// <seealso class='IDTExtensibility2' />
    [GuidAttribute("0BE84534-48A5-48A7-A9BD-0B5CAE7E12A0"),
    ProgId("CSOneNoteRibbonAddIn.Connect")]
    public class Connect : Object, Extensibility.IDTExtensibility2, IRibbonExtensibility
    {
        /// <summary>
        /// 实现该外接对象的构造器.
        ///	您可以将您自己的初始化代码添加到该方法中.
        /// </summary>
        public Connect()
        {
        }

        /// <summary>
        /// 实现 IDTExtensibility2 接口的OnConnection 方法.
        /// 当外接程序被加载时,接收通知.
        /// </summary>
        /// <param term='application'>
        ///  宿主应用程序的根对象。
        /// </param>
        /// <param term='connectMode'>
        ///  描述外接程序正在被加载的情况.
        /// </param>
        /// <param term='addInInst'>
        ///  表示此外接程序的对象.
        /// </param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, 
            object addInInst, ref System.Array custom)
        {
            MessageBox.Show("CSOneNoteRibbonAddIn OnConnection");
            applicationObject = application;
            addInInstance = addInInst;
        }

        /// <summary>
        ///  实现 IDTExtensibility2 接口的OnDisconnection 方法.
        ///  当外接程序被卸载时,接收通知.
        /// </summary>
        /// <param term='disconnectMode'>
        ///  描述外接程序正在被卸载的情况.
        /// </param>
        /// <param term='custom'>
        ///  宿主应用程序特定的参数的数组.
        /// </param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, 
            ref System.Array custom)
        {
            MessageBox.Show("CSOneNoteRibbonAddIn OnDisconnection");
            this.applicationObject = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 实现 IDTExtensibility2 接口的 OnAddInsUpdate 方法.   
        /// 当外接程序的集合被修改时,接收通知.
        /// </summary>
        /// <param term='custom'>
        /// 宿主应用程序特定的参数的数组.
        /// </param>
        /// <seealso class='IDTExtensibility2' />
        public void OnAddInsUpdate(ref System.Array custom)
        {
            MessageBox.Show("CSOneNoteRibbonAddIn OnAddInsUpdate");
        }

        /// <summary>
        ///  实现 IDTExtensibility2 接口的 OnStartupComplete 方法. 
        ///  当宿主应用程序的集合被修改时,接收通知.
        /// </summary>
        /// <param term='custom'>
        ///  宿主应用程序特定的参数的数组.
        /// </param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref System.Array custom)
        {
            MessageBox.Show("CSOneNoteRibbonAddIn OnStartupComplete");
        }

        /// <summary>
        ///  实现 IDTExtensibility2 接口的 OnBeginShutdown 方法.
        ///  当宿主应用程序被卸载时,接收通知.
        /// </summary>
        /// <param term='custom'>
        ///  宿主应用程序特定的参数的数组.
        /// </param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref System.Array custom)
        {
            MessageBox.Show("CSOneNoteRibbonAddIn OnBeginShutdown");

            if (this.applicationObject != null)
            {
                this.applicationObject = null;
            }
        }

        private object applicationObject;
        private object addInInstance;

        /// <summary>		     
        /// 从自定义功能区用户界面的 XML 自定义文件中,加载的 XML 标记 
        /// </summary>
        /// <param name="RibbonID"> RibbonX 用户界面的 ID </param>
        /// <returns>string</returns>
        public string GetCustomUI(string RibbonID)
        {
            return Properties.Resources.customUI;
        }

        /// <summary>
        ///  实现方法 customUI.xml 中的 OnGetImage
        /// </summary>
        /// <param name="imageName"> customUI.xml 中的图像名 </param>
        /// <returns> 装有图像数据的内存流 </returns>
        public IStream OnGetImage(string imageName)
        {
            MemoryStream stream = new MemoryStream();
            if (imageName == "showform.png")
            {
                Resources.ShowForm.Save(stream, ImageFormat.Png);
            }

            return new ReadOnlyIStreamWrapper(stream);
        }

        /// <summary>
        ///  用于显示窗体的方法
        /// </summary>
        /// <param name="control"> 表示传递给每个功能区用户界面 (UI) 控件的	
        /// 回调过程的对象. </param>
        public void ShowForm(IRibbonControl control)
        {
            OneNote.Window context = control.Context as OneNote.Window;
            CWin32WindowWrapper owner =
                new CWin32WindowWrapper((IntPtr)context.WindowHandle);
            TestForm form = new TestForm(applicationObject as OneNote.Application);
            form.ShowDialog(owner);

            form.Dispose();
            form = null;
            context = null;
            owner = null;           
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}