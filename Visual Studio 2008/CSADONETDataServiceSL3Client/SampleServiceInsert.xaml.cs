/****************************** 模块 标识 ******************************\
* 模块名:    SampleServiceInsert.xaml.cs
* 项目:      CSADONETDataServiceSL3Client
* 版权       (c) Microsoft Corporation.
* 
* SampleServiceInsert.cs 展示如何在Silverlight中调用 ASP.NET Data Service
* 进行查询与增加。
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
using CSADONETDataServiceSL3Client.SampleService;
using System.Data.Services.Client;

namespace CSADONETDataServiceSL3Client
{
    public partial class SampleServiceInsert : UserControl
    {
        // ADO.NET Data Service 的 URL。
        private const string _sampleUri =
             "http://localhost:8888/Samples.svc";
        // returnedCategory => _context ={via async REST call}=> ADO.NET Data Service
        private SampleProjects _context;
        public SampleServiceInsert()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SampleServiceInsert_Loaded);
        }

        void SampleServiceInsert_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 本方法中，我们向ADO.NET Data Service发送了一个异步的REST请求，来获得Category
        /// 记录。
        /// </summary>
        private void LoadData()
        {
            _context = new SampleProjects(new Uri(_sampleUri));
            DataServiceQuery<Category> query = (DataServiceQuery<Category>)(
                from c in _context.Categories
                select c);

            query.BeginExecute(OnCategoryQueryComplete, query);
        }

        /// <summary>
        /// Category查询请求的回调方法。
        /// </summary>
        /// <param name="result"></param>
        private void OnCategoryQueryComplete(IAsyncResult result)
        {
            Dispatcher.BeginInvoke(() =>
            {

                DataServiceQuery<Category> query =
                       result.AsyncState as DataServiceQuery<Category>;
                try
                {
                    var returnedCategory =
                        query.EndExecute(result);

                    if (returnedCategory != null)
                    {

                        this.mainDataGrid.ItemsSource = returnedCategory.ToList();
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
        /// 本事件中，我们向 ADO.NET Data Service发送一个异步的REST请求
        /// 来增加一条Category记录。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            string categorynameforinsert = this.categoryTextBox.Text;
            _context.AddToCategories(new Category() { CategoryName = categorynameforinsert });
            _context.BeginSaveChanges(OnChangesSaved, _context);
        } 
        
        /// <summary>
        /// 增加请求的回调方法。
        /// </summary>
        /// <param name="result"></param>
        private void OnChangesSaved(IAsyncResult result)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SampleProjects svcContext = result.AsyncState as SampleProjects;

                try
                {
                    // 完成保存改变操作显示结果。
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
