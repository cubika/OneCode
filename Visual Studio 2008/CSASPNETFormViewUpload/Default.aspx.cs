/****************************** 模块头 ******************************\
* 模块名:	Default.aspx.cs
* 项目名:		CSASPNETFormViewUpload
* Copyright (c) Microsoft Corporation.
* 
* 本页面填充了一个从SQL Server数据库中读取数据的FromView控件 
* 同时还提供了处理数据所需的UI.
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
#endregion Using directives

namespace CSASPNETFormViewUpload
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 检查页面是否初次被访问.
            if (!IsPostBack)
            {
                // 启用FormView分页选项 
                // 同时设定PageButton计数.
                fvPerson.AllowPaging = true;
                fvPerson.PagerSettings.PageButtonCount = 15;

                // 填充FormView控件.
                BindFormView();
            }
        }

        private void BindFormView()
        {
            // 从Web.config获取链接字符串. 
            // 当我们使用Using语句时, 
            // 不需要显式释放代码中的对象, 
            // using语句会处理他们.
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                // 新建一个DataSet对象.
                DataSet dsPerson = new DataSet();

                // 新建一个SELECT查询.
                string strSelectCmd = "SELECT PersonID,LastName,FirstName FROM Person";

                // 新建一个SqlDataAdapter对象
                // SqlDataAdapter表示一组数据命令和一个数据库链接
                // 用以填充DataSet与 
                // 更新SQL Server数据库. 
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);

                // 打开数据链接
                conn.Open();

                // 以查询的结果按行填充DataSet中名为Person的DataTable
                // 
                da.Fill(dsPerson, "Person");


                // 绑定FormView控件.
                fvPerson.DataSource = dsPerson;
                fvPerson.DataBind();
            }
        }

        // FormView.PageIndexChanging事件
        protected void fvPerson_PageIndexChanging(object sender, FormViewPageEventArgs e)
        {
            // 设定新显示页面的索引. 
            fvPerson.PageIndex = e.NewPageIndex;

            // 重新绑定FormView控件,显示新页面的数据.
            BindFormView();
        }

        // FormView.ItemInserting事件
        protected void fvPerson_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            // 从Web.config获取链接字符串. 
            // 当我们使用Using语句时, 
            // 不需要显式释放代码中的对象, 
            // using语句会处理他们.
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                // 新建一个命令对象.
                SqlCommand cmd = new SqlCommand();

                // 将数据链接关联到命令.
                cmd.Connection = conn;

                // 设定命令文本
                // SQL语句或者存储过程名字. 
                cmd.CommandText = "INSERT INTO Person ( LastName, FirstName, Picture ) VALUES ( @LastName, @FirstName, @Picture )";

                // 设定命令类型
                // CommandType.Text 表示原始SQL语句; 
                // CommandType.StoredProcedure 表示存储过程.
                cmd.CommandType = CommandType.Text;

                // 从FormView控件的InsertItemTemplate中获取名和姓. 
                // 
                string strLastName = ((TextBox)fvPerson.Row.FindControl("tbLastName")).Text;
                string strFirstName = ((TextBox)fvPerson.Row.FindControl("tbFirstName")).Text;

                // 向SqlCommand添加参数并设值.
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName;

                FileUpload uploadPicture = (FileUpload)fvPerson.FindControl("uploadPicture");

                if (uploadPicture.HasFile)
                {
                    // 向SqlCommand添加图片参数.
                    // 如果已指定一张图片，将此参数设为此图片的按字节的值 
                    // . 
                    cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = uploadPicture.FileBytes;
                }
                else
                {
                    // 向SqlCommand添加图片参数.
                    // 如果未指定图片，将此参数的值设为 
                    // NULL.
                    cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = DBNull.Value;
                }

                // 打开数据链接.
                conn.Open();

                // 执行命令.
                cmd.ExecuteNonQuery();
            }

            // 将FormView控件切换到只读显示模式. 
            fvPerson.ChangeMode(FormViewMode.ReadOnly);

            // 重新绑定FormView控件以显示插入后的数据.
            BindFormView();
        }

        // FormView.ItemUpdating 事件
        protected void fvPerson_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            // 从Web.config获取链接字符串. 
            // 当我们使用Using语句时, 
            // 不需要显式释放代码中的对象, 
            // using语句会处理他们.
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                // 新建一个命令对象.
                SqlCommand cmd = new SqlCommand();

                // 将数据链接关联到命令.
                cmd.Connection = conn;

                // 设定命令文本
                // SQL语句或者存储过程名字. 
                cmd.CommandText = "UPDATE Person SET LastName = @LastName, FirstName = @FirstName, Picture = ISNULL(@Picture,Picture) WHERE PersonID = @PersonID";

                // 设定命令类型
                // CommandType.Text 表示原始SQL语句; 
                // CommandType.StoredProcedure 表示存储过程.
                cmd.CommandType = CommandType.Text;

                // 从FormView控件的EditItemTemplate中获取人物ID、名和姓. 
                // 
                string strPersonID = ((Label)fvPerson.Row.FindControl("lblPersonID")).Text;
                string strLastName = ((TextBox)fvPerson.Row.FindControl("tbLastName")).Text;
                string strFirstName = ((TextBox)fvPerson.Row.FindControl("tbFirstName")).Text;

                // 向SqlCommand添加参数并设值.
                cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID;
                cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName;
                cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName;

                // 找到FormView控件的EditItemTemplate中的FileUpload控件. 
                // 
                FileUpload uploadPicture = (FileUpload)fvPerson.FindControl("uploadPicture");

                if (uploadPicture.HasFile)
                {
                    // 向SqlCommand添加图片参数.
                    // 如果已指定一张图片，将此参数设为此图片的按字节的值.
                    //  
                    cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = uploadPicture.FileBytes;
                }
                else
                {
                    // 向SqlCommand添加图片参数.
                    // 如果未指定图片，将此参数的值设为 
                    // NULL.
                    cmd.Parameters.Add("@Picture", SqlDbType.VarBinary).Value = DBNull.Value;
                }

                // 打开数据链接.
                conn.Open();

                // 执行命令.
                cmd.ExecuteNonQuery();
            }

            // 将FormView控件切换到只读显示模式. 
            fvPerson.ChangeMode(FormViewMode.ReadOnly);

            // 重新绑定FormView控件以显示更新后的数据.
            BindFormView();
        }

        // FormView.ItemDeleting 事件
        protected void fvPerson_ItemDeleting(object sender, FormViewDeleteEventArgs e)
        {
            // 从Web.config获取链接字符串. 
            // 当我们使用Using语句时, 
            // 不需要显式释放代码中的对象, 
            // using语句会处理他们.
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                // 新建一个命令对象.
                SqlCommand cmd = new SqlCommand();

                // 将数据链接关联到命令.
                cmd.Connection = conn;

                // 设定命令文本
                // SQL语句或者存储过程名字. 
                cmd.CommandText = "DELETE FROM Person WHERE PersonID = @PersonID";

                // 设定命令类型
                // CommandType.Text 表示原始SQL语句; 
                // CommandType.StoredProcedure 表示存储过程.
                cmd.CommandType = CommandType.Text;

                // 从FormView控件的ItemTemplate中获取PersonID.
                // 
                string strPersonID = ((Label)fvPerson.Row.FindControl("lblPersonID")).Text;

                // 向SqlCommand添加参数并设值.
                cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID;

                // 打开数据链接.
                conn.Open();

                // 执行命令.
                cmd.ExecuteNonQuery();
            }

            // 重新绑定FormView控件以显示删除后的数据.
            BindFormView();
        }

        // FormView.ModeChanging 事件
        protected void fvPerson_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            // 将FormView control切换到新模式
            fvPerson.ChangeMode(e.NewMode);

            // 重新绑定FormView控件以新模式显示数据.
            BindFormView();
        }   
    }
}
