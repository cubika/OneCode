/****************************** 模块 标识 ******************************\
* 模块名:    SchoolLinqToSQLServiceDelete.xaml.cs
* 项目:      CSADONETDataServiceSL3Client
* 版权       (c) Microsoft Corporation.
* 
* SchoolLinqToSQLServiceDelete.cs 展示如何在Silverlight中调用 ASP.NET Data Service
* 进行查询与删除操作。
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
using CSADONETDataServiceSL3Client.SchoolLinqToSQLService;
using System.Data.Services.Client;

namespace CSADONETDataServiceSL3Client
{
    public partial class SchoolLinqToSQLServiceDelete : UserControl
    {
        // DataGrid 控件的数据源。
        private List<ScoreCardForSchoolLinqToSQL> _collection = new List<ScoreCardForSchoolLinqToSQL>();
        // ADO.NET Data Service 的 URL
        private const string _schoolLinqToSQLUri =
            "http://localhost:8888/SchoolLinqToSQL.svc";
        // collection => returnedCourseGrade => _context ={via async REST call}=> ADO.NET Data Service
        private SchoolLinqToSQLDataContext _context;

        public SchoolLinqToSQLServiceDelete()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SchoolLinqToSQLServiceDelete_Loaded);
        }

        void SchoolLinqToSQLServiceDelete_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 本方法中，发送一个异步的REST请求到ADO.NET Data Service来获得 CourseGrade 数据
        /// 我们扩展了 Person 以及 Course，所以关联记录也会被返回到客户端。
        /// </summary>
        private void LoadData()
        {
            _context = new SchoolLinqToSQLDataContext(new Uri(_schoolLinqToSQLUri));
            DataServiceQuery<CourseGrade> query = (DataServiceQuery<CourseGrade>)(
                from c in _context.CourseGrades.Expand("Person").Expand("Course")
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
                                      select new ScoreCardForSchoolLinqToSQL()
                                      {
                                          CourseGrade = c,
                                          Course = c.Course.Title,
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
        /// 本事件句柄处理的是删除按钮控件的点击事件。
        /// 事件句柄中通过调用 ADO.NET Data Service 删除与删除按钮在同一行的数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            ScoreCardForSchoolLinqToSQL s = b.DataContext as ScoreCardForSchoolLinqToSQL;
            if (s != null) {
                CourseGrade recordfordelete = s.CourseGrade;
                _context.DeleteObject(recordfordelete);
                _context.BeginSaveChanges(SaveChangesOptions.None,
                    OnChangesSaved, _context);
            }
        }  
        
        /// <summary>
        /// 删除操作的回调事件。
        /// </summary>
        /// <param name="result"></param>
        private void OnChangesSaved(IAsyncResult result)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SchoolLinqToSQLDataContext svcContext = result.AsyncState as SchoolLinqToSQLDataContext;

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
                    // 你也可以删除下边的代码行，并通过刷新本页或者直接查看数据库来看操作结果。
                    LoadData();
                }
            });

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
