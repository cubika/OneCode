/**************************************** 模块头 *****************************************\
* 模块名:    Default.aspx.cs
* 项目名:    CSASPNETAddControlDynamically
* 版权 (c) Microsoft Corporation
*
* 这个项目演示了如何向ASP.NET页面动态添加控件. 它虚构了客户需要输入多于一个且
* 无上限的地址信息的情景. 因此我们使用按钮添加新地址TextBox.当用户输入完所有地址, 
* 我们也使用按钮在数据库更新这些信息, 在本示例中运行为显示这些地址.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;
using System.Web.UI.WebControls;

namespace CSASPNETAddControlDynamically
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            // 使用ViewState储存地址总数.
            if (ViewState["AddressCount"] == null)
            {
                ViewState["AddressCount"] = 0;
            }

            // 获得地址总数.
            int addresscount = (int)ViewState["AddressCount"];

            // 递归添加地址输入组件.
            for (int i = 0; i < addresscount; i++)
            {
                AddAdress((i + 1).ToString());
            }
        }
        
        protected void btnAddAddress_Click(object sender, EventArgs e)
        {
            if (ViewState["AddressCount"] != null)
            {
                int btncount = (int)ViewState["AddressCount"];

                // 添加一个新组件到pnlAddressContainer.
                AddAdress((btncount + 1).ToString());
                ViewState["AddressCount"] = btncount + 1;
            }
            else
            {
                Response.Write("出错了");
                Response.End();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int addresscount = (int)ViewState["AddressCount"];

            // 在页面上显示所有地址.
            // 这里模仿我们将这些地址写入数据库.
            for (int i = 0; i < addresscount; i++)
            {
                TextBox tb = pnlAddressContainer.FindControl("TextBox" + (i + 1).ToString()) as TextBox;
                string address = tb.Text == "" ? "空白" : tb.Text;
                Response.Write("地址" + (i + 1).ToString() + "是" + address + ".<br />");
            }

            // 清除ViewState.
            ViewState["AddressCount"] = null;
        }

        protected void AddAdress(string id)
        {
            // 用以显示地址编号的Label.
            Label lb = new Label();
            lb.Text = "地址" + id + ": ";

            // 用以输入地址的TextBox.
            TextBox tb = new TextBox();
            tb.ID = "TextBox" + id;

            if (id != "1")
            {
                // 可以尝试不带判定条件下的这句代码.
                // 单击Save按钮后会有很奇怪的行为.
                tb.Text = Request.Form[tb.ID];
            }

            // 用以检查地址的Button.
            // 同时演示如何绑定事件到一个动态控件上.
            Button btn = new Button();
            btn.Text = "检查";
            btn.ID = "Button" + id;

            // 使用+=运算符绑定事件.
            btn.Click += new EventHandler(ClickEvent);

            Literal lt = new Literal() { Text = "<br />" };

            // 作为一个组件添加这些控件到pnlAddressContainer.
            pnlAddressContainer.Controls.Add(lb);
            pnlAddressContainer.Controls.Add(tb);
            pnlAddressContainer.Controls.Add(btn);
            pnlAddressContainer.Controls.Add(lt);
        }

        protected void ClickEvent(object sender, EventArgs e)
        {
            // 从sender获得按钮实例.
            Button btn = sender as Button;

            // 通过FindControl()方法获得TextBox实例和它的值.
            TextBox tb = pnlAddressContainer.FindControl(btn.ID.Replace("Button", "TextBox")) as TextBox;
            string address = tb.Text == "" ? "空白" : tb.Text;

            // 弹出一个消息框显示对应的地址.
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<script type=\"text/javascript\">");
            sb.Append("alert(\"地址" + btn.ID.Replace("Button", "") + "是" + address + ".\");");
            sb.Append("</script>");

            if (!ClientScript.IsClientScriptBlockRegistered(this.GetType(), "AlertClick"))
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "AlertClick", sb.ToString());
            }
        }
    }
}