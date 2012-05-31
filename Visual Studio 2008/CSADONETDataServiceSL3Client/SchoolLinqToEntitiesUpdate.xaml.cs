/****************************** 模块 标识 ******************************\
* 模块名:    SchoolLinqToEntitiesUpdate.xaml.cs
* 项目:      CSADONETDataServiceSL3Client
* 版权       (c) Microsoft Corporation.
* 
* SchoolLinqToEntitiesUpdate.cs 展示如何在Silverlight中调用 ADO.NET Data Service
* 来进行查询与更新。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data.Services.Client;
using CSADONETDataServiceSL3Client.SchoolLinqToEntitiesService;
using System.Windows.Browser;
using System.Globalization;
using System.IO;
using System.Net.Browser;

namespace CSADONETDataServiceSL3Client
{
    public partial class SchoolLinqToEntitiesUpdate : UserControl
    {
        // DataGrid 控件的数据源。
        private List<ScoreCardForSchoolLinqToEntities> _collection = new List<ScoreCardForSchoolLinqToEntities>();
        // ADO.NET Data Service 的 URL。
        private const string _schoolLinqToEntitiesUri =
           "http://localhost:8888/SchoolLinqToEntities.svc";
        // _collection => returnedCourseGrade => _entities ={via async REST call}=> ADO.NET Data Service
        private SQLServer2005DBEntities _entities;

        public SchoolLinqToEntitiesUpdate()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();

        }

        /// <summary>
        /// 本方法中，发送一个异步的REST请求到ADO.NET Data Service来获得 CourseGrade 数据
        /// 我们扩展了 Person 以及 Course，所以外部记录也会被返回到客户端。
        /// </summary>
        private void LoadData()
        {
            _entities = new SQLServer2005DBEntities(new Uri(_schoolLinqToEntitiesUri));
            DataServiceQuery<CourseGrade> query = (DataServiceQuery<CourseGrade>)(
                from c in _entities.CourseGrade.Expand("Person").Expand("Course")
                select c);

            query.BeginExecute(OnCourseGradeQueryComplete, query);
        }

        /// <summary>
        /// CourseGrade查询的回调方法。
        /// </summary>
        /// <param name="result"></param>
        private void OnCourseGradeQueryComplete(IAsyncResult result)
        {
            Dispatcher.BeginInvoke(() =>
            {

                DataServiceQuery<CourseGrade> query =
                       result.AsyncState as DataServiceQuery<CourseGrade>;
                try
                {
                    var returnedCourseGrade =
                        query.EndExecute(result);

                    if (returnedCourseGrade != null)
                    {
                        _collection = (from c in returnedCourseGrade.ToList()
                                       select new ScoreCardForSchoolLinqToEntities()
                                       {
                                           // 由于在服务器端有一个查询拦截器(如下)，所以服务器端只能返回ID大于4000的Course数据
                                           // [QueryInterceptor("Course")]
                                           // public Expression<Func<Course, bool>> QueryCourse()
                                           // {
                                           //     // LINQ lambda expression to filter the course objects
                                           //     return c => c.CourseID > 4000;
                                           // }

                                           CourseGrade = c,
                                           Course = c.Course == null ? "只有 Course ID>4000 才会被显示" :
                                               c.Course.Title,
                                           Grade = c.Grade,
                                           PersonName = string.Format("{0} {1}",
                                           c.Person.FirstName, c.Person.LastName)
                                       }).ToList();

                        this.mainDataGrid.ItemsSource = _collection;
                    }
                }
                catch (DataServiceQueryException ex)
                {
                    this.messageTextBlock.Text = string.Format("错误: {0} - {1}",
                        ex.Response.StatusCode.ToString(), ex.Response.Error.Message);
                }
            });

        }

        /// <summary>
        /// 本事件句柄处理的是DataGrid控件的RowEdited事件。
        /// 在事件中调用ADO.NET Data Service，更新了被编辑的Grade数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainDataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            ScoreCardForSchoolLinqToEntities s = e.Row.DataContext as ScoreCardForSchoolLinqToEntities;
            if (s != null)
            {
                CourseGrade recordforupdate = s.CourseGrade;
                _entities.UpdateObject(recordforupdate);
                _entities.BeginSaveChanges(SaveChangesOptions.ReplaceOnUpdate,
                    OnChangesSaved, _entities);

            }

        }

        /// <summary>
        /// 更新操作的回调方法。
        /// </summary>
        /// <param name="result"></param>
        private void OnChangesSaved(IAsyncResult result)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SQLServer2005DBEntities svcContext = result.AsyncState as SQLServer2005DBEntities;

                try
                {
                    // 完成保存改变操作并显示结果。
                    WriteOperationResponse(svcContext.EndSaveChanges(result));
                }
                catch (DataServiceRequestException ex)
                {
                    // 显示错误信息。
                    WriteOperationResponse(ex.Response);
                }
                catch (InvalidOperationException ex)
                {
                    messageTextBlock.Text = ex.Message;
                }
                finally
                {
                    // 重新加载绑定的集合来刷新Grid，显示操作的结果。 
                    // 你也可以删除下边的代码行，并通过刷新本也或者直接查看数据库来看操作结果。
                    LoadData();
                }
            }
    );

        }

        /// <summary>
        /// 本方法用以显示 ADO.NET Data Service 返回的详细信息。
        /// </summary>
        /// <param name="response"></param>
        private void WriteOperationResponse(DataServiceResponse response)
        {
            messageTextBlock.Text = string.Empty;
            int i = 1;

            if (response.IsBatchResponse)
            {
                messageTextBlock.Text = string.Format("批处理返回状态码: {0}\n",
                    response.BatchStatusCode);
            }
            foreach (ChangeOperationResponse change in response)
            {
                messageTextBlock.Text +=
                    string.Format("\t更改 {0} 状态码: {1}\n",
                    i.ToString(), change.StatusCode.ToString());
                if (change.Error != null)
                {
                    string.Format("\t更改 {0} 错误: {1}\n",
                        i.ToString(), change.Error.Message);
                }
                i++;
            }
        }

       
    }
}
