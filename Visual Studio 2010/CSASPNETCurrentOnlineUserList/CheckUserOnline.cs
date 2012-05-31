/**************************************** 模块头 *****************************************\
* 模块名:    CheckUserOnline.cs
* 项目名:    CSASPNETCurrentOnlineUserList
* 版权 (c) Microsoft Corporation
*
* 本类用来添加JavaScript代码到页面.JavaScript函数可以检查用户活动时间
* 同时发送一个请求到CheckUserOnlinePage.aspx页面.
* 本项目将根据最近活动时间自动删除离线用户记录.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************************/

using System;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Configuration;

namespace CSASPNETCurrentOnlineUserList
{
    [Description("CheckUserOnline"), DefaultProperty(""),
    ToolboxData("<{0}:CheckUserOnline runat=server />")]
    public class CheckUserOnline : System.Web.UI.WebControls.PlaceHolder
    {
        /// <summary>
        /// 刷新时间间隔,默认值为25.
        /// </summary>
        public virtual int RefreshTime
        {
            get
            {
                object _obj1 = this.ViewState["RefreshTime"];
                if (_obj1 != null) { return int.Parse(((string)_obj1).Trim()); }
                return 25;
            }
            set
            {
                this.ViewState["RefreshTime"] = value;
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            // 自web.config通过xmlhttp获得来访地址.
            string _refreshUrl = (string)ConfigurationManager.AppSettings["refreshUrl"];
            string _scriptString = @" <script language=""JavaScript"">";
            _scriptString += writer.NewLine;
            _scriptString += @"   window.attachEvent(""onload"", " + this.ClientID;
            _scriptString += @"_postRefresh);" + writer.NewLine;
            _scriptString += @"   var " + this.ClientID + @"_xmlhttp=null;";
            _scriptString += writer.NewLine;
            _scriptString += @"   function " + this.ClientID + @"_postRefresh(){";
            _scriptString += writer.NewLine;
            _scriptString += @"    var " + this.ClientID;
            _scriptString += @"_xmlhttp = new ActiveXObject(""Msxml2.XMLHTTP"");";
            _scriptString += writer.NewLine;
            _scriptString += @"    " + this.ClientID;
            _scriptString += @"_xmlhttp.Open(""POST"", """ + _refreshUrl + @""", false);";
            _scriptString += writer.NewLine;
            _scriptString += @"    " + this.ClientID + @"_xmlhttp.Send();";
            _scriptString += writer.NewLine;
            _scriptString += @"    var refreshStr= " + this.ClientID;
            _scriptString += @"_xmlhttp.responseText;";
            _scriptString += writer.NewLine;

            _scriptString += @"    try {";
            _scriptString += writer.NewLine;
            _scriptString += @"     var refreshStr2=refreshStr;";
            _scriptString += writer.NewLine;
            _scriptString += @"    }";
            _scriptString += writer.NewLine;
            _scriptString += @"    catch(e) {}";
            _scriptString += writer.NewLine;
            _scriptString += @"    setTimeout(""";
            _scriptString += this.ClientID + @"_postRefresh()"",";
            _scriptString += this.RefreshTime.ToString() + @"000);";
            _scriptString += writer.NewLine;
            _scriptString += @"   }" + writer.NewLine;
            _scriptString += @"<";
            _scriptString += @"/";
            _scriptString += @"script>" + writer.NewLine;
            writer.Write(writer.NewLine);
            writer.Write(_scriptString);
            writer.Write(writer.NewLine);
            base.Render(writer);
        }
    }
}