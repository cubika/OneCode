/****************************** 模块头 ******************************\
* 模块名称:  CSCustomizeVSToolboxItemPackage.cs
* 项目:	    CSCustomizeVSToolboxItem
* 版权 (c) Microsoft Corporation.
* 
* CSCustomizeVSToolboxItemPackage类继承
* Microsoft.VisualStudio.Shell.Package类。它重写了Initialize方法。

* 如果您添加了一个新的项目到VS2010工具箱中，显示名称和新
* 项目的提示信息默认是相同的。这个例子说明了怎样通过
* 客户端控件向Visual Studio的工具箱里添加栏目。

*This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.CSCustomizeVSToolboxItem
{
    /// <summary>
    /// 这是被当前程序集展现用于实现封装的类。
    /// 一个类被认为是Visual Studio的一个有效包的最低要求是
    /// 实现IVsPackage接口并用壳注册自己。
    /// 本包运用了在管理软件包框架（MPF）中定义的助手类来
    /// 做这些事：它继承自Package类，该类提供IVsPackage接口
    /// 的实现并使用在框架中定义的注册属性来注册它自己及它的壳
    /// 的组成部分
    /// </summary>
    [DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\10.0")]
    // 这个属性告诉PkgDef创建工具（CreatePkgDef.exe），这个类是
    // 一个包。
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // 此软件包将会自动加载如果有解决方案在VS中。
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    // 这个属性是用来注册所需的信息在
    // 帮助/关于Visual Studio的对话框中显示这个软件包。
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidCSCustomizeVSToolboxItemPkgString)]
    // 注册VSPackage作为提供ToolboxItem对象需要ProvideToolboxItemsAttribute类。
    [ProvideToolboxItems(1)]
    public sealed class CSCustomizeVSToolboxItemPackage : Package
    {
        // 定义标签，栏目，工具提示信息，描述和拖放文本。
        const string toolboxTabString = "CS Custom Toolbox Tab";
        const string toolboxItemString = "CS Custom Toolbox Item";
        const string toolboxTooltipString = "CS Custom Toolbox Tooltip";
        const string toolboxDescriptionString = "CS Custom Toolbox Description";
        const string toolboxItemTextString = "CS Hello world!";

        // IVsToolbox2服务.
        IVsToolbox2 vsToolbox2;

        // IVsActivityLog服务.
        IVsActivityLog vsActivityLog;

        // 内存流存储提示数据.
        Stream tooltipStream;

        /// <summary>
        /// 包的默认构造器。
        /// 在此方法中，你可以放置任何不需要
        /// 任何Visual Studio服务的初始化代码，因为在
        /// 这个时候包对象已经被创建但还未在Visual Studio环境中选址。
        /// 初始化方法用来进行所有其他的初始化工作。
        /// </summary>
        public CSCustomizeVSToolboxItemPackage()
        {
            Trace.WriteLine("Entering constructor for: {0}", this.ToString());
        }



        /////////////////////////////////////////////////////////////////////////////
        // 重写包的实现
        #region Package Members

        /// <summary>
        /// 包的初始化；这个方法将在包被选址后调用，所以你可以把所有
        /// 基于Visual Studio提供服务的初始化代码放在这儿。
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ToolboxInitialized += new EventHandler(ToolboxItemPackage_ToolboxInitialized);
            this.ToolboxUpgraded += new EventHandler(ToolboxItemPackage_ToolboxUpgraded);
            // 初始化服务.
            vsActivityLog = GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            vsToolbox2 = GetService(typeof(SVsToolbox)) as IVsToolbox2;

            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
                string.Format("Entering initializer for: {0}", this.ToString()));



            // 添加工具箱项，如果它不存在.
            try
            {
                if (!VerifyToolboxTabExist())
                {
                    AddToolboxTab();
                }

                if (!VerifyToolboxItemExist())
                {
                    AddToolboxItem();
                }
             
            }
            catch (Exception ex)
            {
                LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, ex.Message);
                LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, ex.StackTrace);
            }
        }

        void ToolboxItemPackage_ToolboxUpgraded(object sender, EventArgs e)
        {
            // 抛出新的NotImplementedException()异常;
        }

        void ToolboxItemPackage_ToolboxInitialized(object sender, EventArgs e)
        {
            // 抛出新的NotImplementedException()异常;
        }

        /// <summary>
        /// 使用IVsActivityLog服务记录VS日志.
        /// </summary>
        void LogEntry(__ACTIVITYLOG_ENTRYTYPE type, string message)
        {

            if (vsActivityLog != null)
            {
                int hr = vsActivityLog.LogEntry((UInt32)type, this.ToString(), message);
                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        /// <summary>
        /// 检查工具箱栏目是否存在.
        /// </summary>
        bool VerifyToolboxTabExist()
        {
            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
              string.Format("Entering VerifyToolboxTabExist for: {0}", this.ToString()));

            bool exist = false;

            IEnumToolboxTabs tabs;
            uint num;

            ErrorHandler.ThrowOnFailure(vsToolbox2.EnumTabs(out tabs));
            string[] rgelt = new string[1];
            for (int i = tabs.Next(1, rgelt, out num);
                (ErrorHandler.Succeeded(i) && (num > 0)) && (rgelt[0] != null);
                i = tabs.Next(1, rgelt, out num))
            {
                if (rgelt[0] == toolboxTabString)
                {
                    exist = true;
                    break;
                }
            }

            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
             string.Format("VerifyToolboxTabExist {0}: {1}", toolboxTabString, exist));

            return exist;
        }

        /// <summary>
        /// 添加工具箱栏目. 
        /// </summary>
        void AddToolboxTab()
        {
            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
             string.Format("Entering AddToolboxTab for: {0}", toolboxTabString));
            ErrorHandler.ThrowOnFailure(vsToolbox2.AddTab(toolboxTabString));
        }

        /// <summary>
        /// 检查工具箱栏目是否存在.
        /// </summary>
        bool VerifyToolboxItemExist()
        {
            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
             string.Format("Entering VerifyToolboxItemExist for: {0}", this.ToString()));

            bool exist = false;
            IEnumToolboxItems items;
            uint num;
            ErrorHandler.ThrowOnFailure(vsToolbox2.EnumItems(toolboxTabString, out items));
            var rgelt = new Microsoft.VisualStudio.OLE.Interop.IDataObject[1];
            for (int i = items.Next(1, rgelt, out num);
                (ErrorHandler.Succeeded(i) && (num > 0)) && (rgelt[0] != null);
                i = items.Next(1, rgelt, out num))
            {
                string displayName;
                var hr = (vsToolbox2 as IVsToolbox3).GetItemDisplayName(rgelt[0], out displayName);
                ErrorHandler.ThrowOnFailure(hr);

                if (displayName.Equals(toolboxItemString, StringComparison.OrdinalIgnoreCase))
                {
                    exist = true;
                    break;
                }
            }

            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
            string.Format("VerifyToolboxItemExist {0}: {1}", toolboxItemString, exist));

            return exist;
        }

        /// <summary>
        /// 添加工具箱栏目. 
        /// </summary>
        void AddToolboxItem()
        {
            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION,
            string.Format("Entering AddToolboxItem for: {0}", toolboxItemString));
            var toolboxData = new Microsoft.VisualStudio.Shell.OleDataObject();
            tooltipStream = FormatTooltipData(toolboxTooltipString, toolboxDescriptionString);
            // 设置提示信息.
            toolboxData.SetData("VSToolboxTipInfo", tooltipStream);
            // 设置拖放文本.
            toolboxData.SetData(DataFormats.Text, toolboxItemTextString);
            TBXITEMINFO[] itemInfo = new TBXITEMINFO[1];
            itemInfo[0].bstrText = toolboxItemString;
            itemInfo[0].hBmp = IntPtr.Zero;
            itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;
            ErrorHandler.ThrowOnFailure(
                vsToolbox2.AddItem(toolboxData, itemInfo, toolboxTabString));
        }

        /// <summary>
        /// 排版工具提示.
        /// </summary>
        private Stream FormatTooltipData(string tooltip, string description)
        {
            const string NameHeader = "Name:";
            const string DescriptionHeader = "Description:";
            char ch = (char)(1 + NameHeader.Length + tooltip.Length);
            string str = ch.ToString() + NameHeader + tooltip;
            if (!string.IsNullOrEmpty(description))
            {
                ch = (char)(1 + DescriptionHeader.Length + description.Length);
                str += ch.ToString() + DescriptionHeader + description;
            }
            str += '\0';
            return SaveStringToStream(str);
        }

        private Stream SaveStringToStream(string value)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(value);
            MemoryStream stream = null;
            if ((bytes != null) && (bytes.Length > 0))
            {
                stream = new MemoryStream((int)bytes.Length);
                stream.Write(bytes, 0, (int)bytes.Length);
                stream.Flush();
            }
            else
            {
                stream = new MemoryStream();
            }
            stream.WriteByte((byte)0);
            stream.WriteByte((byte)0);
            stream.Flush();
            stream.Position = 0L;
            return stream;

        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && this.tooltipStream != null)
            {
                this.tooltipStream.Dispose();
            }
        }

    }
}
