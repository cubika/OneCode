/**************************************** 模块头 *****************************************\
* 模块名:    Default.aspx.cs
* 项目名:    CSASPNETImagEditUpload
* 版权 (c) Microsoft Corporation
*
* 本项目演示了如何插入,编辑或更新一个图片并将其保存到Sql数据库中.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************************/

using System.Collections.Generic;
using System.Web.UI.WebControls;
using System;
using System.Data.SqlClient;
using System.IO;

namespace CSAspNetImageEditUpload
{
    public partial class _Default : System.Web.UI.Page
    {
        // 检查常用图片静态类型.
        private static List<string> imgytpes = new List<string>()
        {
            ".BMP",".GIF",".JPG",".PNG"
        };

        /// <summary>
        /// 读取所有记录到GridView.
        /// 如果存在记录, 默认选择第一条记录显示在FormView;
        /// 否则, 改变formview为插入模式可以使数据被插入.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvPersonOverView.DataBind();
        
                if (gvPersonOverView.Rows.Count > 0)
                {
                    gvPersonOverView.SelectedIndex = 0;
                    fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
                    fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
                }
                else
                {
                    fvPersonDetails.ChangeMode(FormViewMode.Insert);
                    fvPersonDetails.DefaultMode = FormViewMode.Insert;
                }
            }
        }


        /// <summary>
        /// 验证数据是否满足图片类型.
        /// </summary>
        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (args.Value != null && args.Value != "")
            {
                args.IsValid = imgytpes.IndexOf(System.IO.Path.GetExtension(args.Value).ToUpper()) >= 0;
            }

        }

        /// <summary>
        /// 在检查验证图片类型之后,
        /// 通过e.Values参数关联图片类型和图片字节集合
        /// 然后插入.
        /// </summary>
        protected void fvPersonDetails_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            object obj = Session["insertstate"];

            if (obj == null || (bool)obj)
            {
                CustomValidator cv = fvPersonDetails.FindControl("cmvImageType") as CustomValidator;

                cv.Validate();
                e.Cancel = !cv.IsValid;

                FileUpload fup = (FileUpload)fvPersonDetails.FindControl("fupInsertImage");

                if (cv.IsValid && fup.PostedFile.FileName.Trim() != "")
                {
                    e.Values["PersonImage"] = File.ReadAllBytes(fup.PostedFile.FileName);
                    e.Values["PersonImageType"] = fup.PostedFile.ContentType;
                }

            }
            else
            {
                e.Cancel = true;
                gvPersonOverView.DataBind();
                fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
                fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
            }
        }

        /// <summary>
        /// 在检查验证图片类型之后,
        /// 通过e.Values参数关联图片类型和图片字节集合
        /// 然后更新.
        /// </summary>
        protected void fvPersonDetails_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            CustomValidator cv = fvPersonDetails.FindControl("cmvImageType") as CustomValidator;

            cv.Validate();
            e.Cancel = !cv.IsValid;

            FileUpload fup = (FileUpload)fvPersonDetails.FindControl("fupEditImage");

            if (cv.IsValid && fup.PostedFile.FileName.Trim() != "")
            {
                e.NewValues["PersonImage"] = File.ReadAllBytes(fup.PostedFile.FileName);
                e.NewValues["PersonImageType"] = fup.PostedFile.ContentType;
            }
        }

        /// <summary>
        /// 更新成功后, 重新绑定数据,默认选择第一个.
        /// </summary>
        protected void fvPersonDetails_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            gvPersonOverView.DataBind();
            gvPersonOverView.SelectedIndex = gvPersonOverView.SelectedRow.RowIndex;
        }

        /// <summary>
        /// 插入成功后, 重新绑定数据,默认选择第一个,
        /// 改变FormView模式为只读(查看用).
        /// </summary>
        protected void fvPersonDetails_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            gvPersonOverView.DataBind();
            gvPersonOverView.SelectedIndex = gvPersonOverView.Rows.Count - 1;
            fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
            fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
        }

        /// <summary>
        /// 删除成功后 , 重新绑定数据.
        /// </summary>
        protected void fvPersonDetails_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
            gvPersonOverView.DataBind();

            // 如果还有记录
            if (gvPersonOverView.Rows.Count > 0)
            {
                // 取出被删除记录的行索引
                int delindex = (int)ViewState["delindex"];

                // 如果第一条记录被删除,移动到下一条记录.
                if (delindex == 0)
                {
                    gvPersonOverView.SelectedIndex = 0;
                }
                
                // 如果是最后一条记录被删除, 移动到前一条记录.
                else if (delindex == gvPersonOverView.Rows.Count)
                {
                    gvPersonOverView.SelectedIndex = gvPersonOverView.Rows.Count - 1;
                }

                // 否则, 删除后移动到下一条记录.
                else
                {
                    gvPersonOverView.SelectedIndex = delindex;
                }

            }

            // 如果没有记录, 变更为插入模式以插入数据.
            else
            {
                fvPersonDetails.ChangeMode(FormViewMode.Insert);
                fvPersonDetails.DefaultMode = FormViewMode.Insert;
            }
        }

        /// <summary>
        /// 当GridView的SelectedRowIndex改变时在FormView中显示图片和详细信息.
        /// </summary>
        protected void gvPersonOverView_SelectedIndexChanged(object sender, EventArgs e)
        {
            fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
            fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
        }

        /// <summary>
        /// 在ViewState中保持行索引用来Item_Deleted.
        /// </summary>
        protected void fvPersonDetails_ItemDeleting(object sender, FormViewDeleteEventArgs e)
        {
            ViewState["delindex"] = gvPersonOverView.SelectedIndex;
        }

        /// <summary>
        /// 在Session中保持insertState防止刷新页面后的重复插入.
        /// </summary>
        protected void fvPersonDetails_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            Session["insertstate"] = (e.NewMode == FormViewMode.Insert);
        }


    }
}