/********************************** 模块头 ***********************************\
* 模块名:        DBProcess.cs
* 项目名:        CSExcelLikeGridView
* 版权(c) Microsoft Corporation
*
* 这是执行批插入, 更新或删除的UI模块.
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
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

namespace CSExcelLikeGridView
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DBProcess p = new DBProcess();
                GridView1.DataSource = p.GetDataTable(true);
                GridView1.DataBind();
            }
        }

        /// <summary>
        /// 此函数将会确认最近的修改并执行批保存.
        /// </summary>
        protected void btnSaveAll_Click(object sender, EventArgs e)
        {
            //默认值为false, 表示db未保存
            bool flag = false;

            DBProcess p = new DBProcess();
            DataTable dt = p.GetDataTable(false);

            // 改变状态并执行一个批更新
            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                if ((GridView1.Rows[i].FindControl("chkDelete") as CheckBox).Checked)
                {
                    dt.Rows[i].Delete();
                    flag = true;
                }
                else
                {
                    if (dt.Rows[i]["PersonName"].ToString()
                        != (GridView1.Rows[i].FindControl("tbName") as TextBox).Text)
                    {
                        if (dt.Rows[i].RowState == DataRowState.Unchanged)
                        {
                            dt.Rows[i].BeginEdit();
                        }
                        dt.Rows[i]["PersonName"] =
                            (GridView1.Rows[i].FindControl("tbName") as TextBox).Text;
                        if (dt.Rows[i].RowState == DataRowState.Unchanged)
                        {
                            dt.Rows[i].EndEdit();
                        }
                        flag = true;
                    }
                    if (dt.Rows[i]["PersonAddress"].ToString()
                        != (GridView1.Rows[i].FindControl("tbAddress") as TextBox).Text)
                    {
                        if (dt.Rows[i].RowState == DataRowState.Unchanged)
                        {
                            dt.Rows[i].BeginEdit();
                        }
                        dt.Rows[i]["PersonAddress"] =
                            (GridView1.Rows[i].FindControl("tbAddress") as TextBox).Text;
                        if (dt.Rows[i].RowState == DataRowState.Unchanged)
                        {
                            dt.Rows[i].EndEdit();
                        }
                        flag = true;
                    }
                }
            }

            p.BatchSave(dt);

            // 保存数据到db,不再需要保持状态颜色
            HidState.Value = "[]";

            dt = p.GetDataTable(true);
            GridView1.DataSource = dt;
            GridView1.DataBind();

            if (flag)
            {
                ClientScript.RegisterStartupScript
        (GetType(), "js", "alert('保存所有更改成功!');", true);
            }

        }

        /// <summary>
        ///  此函数处理下列场合:
        ///  记录每行每列的状态到HidState.
        /// </summary>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int i = 0;
            DBProcess p = new DBProcess();
            DataTable dt = p.GetDataTable(false);
            TextBox tb1 = null;
            TextBox tb2 = null;
            StringBuilder sbu = new StringBuilder();
            CheckBox chkbox = null;

            // 维持JSON状态.
            sbu.Append("[");

            for (; i < dt.Rows.Count; ++i)
            {
                // 循环单元格
                for (int j = 2; j < GridView1.HeaderRow.Cells.Count; ++j)
                {
                    tb1 = GridView1.Rows[i].FindControl("tbName") as TextBox;
                    tb2 = GridView1.Rows[i].FindControl("tbAddress") as TextBox;

                    sbu.Append("{'Index':'" + (i * GridView1.HeaderRow.Cells.Count + j));

                    //不添加, 表示无变更或添加
                    if (dt.Rows[i].RowState !=DataRowState.Added)
                    {
                        // 决定db中第一个单元格是否变更 
                        if (j == 2)
                        {
                            if (!tb1.Text.Equals(dt.Rows[i][j - 1, DataRowVersion.Original]))
                            {
                                dt.Rows[i].BeginEdit();
                                sbu.Append("','Color':'blue',");
                                dt.Rows[i][j - 1] = tb1.Text;
                            }
                            else
                            {
                                sbu.Append("','Color':'',");
                            }
                        }
                        else
                        {
                            // 决定db中第二个单元格是否变更 
                            if (!tb2.Text.Equals(dt.Rows[i][j - 1, DataRowVersion.Original]))
                            {
                                dt.Rows[i].BeginEdit();
                                sbu.Append("','Color':'blue',");
                                dt.Rows[i][j - 1] = tb2.Text;
                            }
                            else
                            {
                                sbu.Append("','Color':'',");
                            }
                        }
                        dt.Rows[i].EndEdit();
                    }

                    else
                    {
                        // 添加行标绿
                        if (dt.Rows[i].RowState == DataRowState.Added)
                        {
                            sbu.Append("','Color':'green',");
                        }
                        // 其他行保持原色
                        else
                        {
                            sbu.Append("','Color':'',");
                        }
                    }

                    // 保持Delete语句
                    chkbox = GridView1.Rows[i].FindControl("chkDelete") as CheckBox;
                    sbu.Append("'Deleted':'" + chkbox.Checked + "'},");
                }
            }

            DataRow r = dt.NewRow();
            r["PersonName"] = (GridView1.FooterRow.FindControl("tbNewName") as TextBox).Text;
            r["PersonAddress"] = (GridView1.FooterRow.FindControl("tbNewAddress") as TextBox).Text;
            dt.Rows.Add(r);
            sbu.Append("{'Index':'" + (i * GridView1.HeaderRow.Cells.Count
                            + 2) + "','Color':'green','Deleted':'false'},");
            sbu.Append("{'Index':'" + (i * GridView1.HeaderRow.Cells.Count
                            + 3) + "','Color':'green','Deleted':'false'}");
            sbu.Append("]");
            p.WriteDataTable(dt);
            HidState.Value = sbu.ToString();
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
}