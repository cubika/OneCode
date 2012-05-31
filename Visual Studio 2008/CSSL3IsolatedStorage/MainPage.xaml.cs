/****************************** 模块头 ******************************\
* 模块名:                 MainPage.xaml.cs
* 项目名:                     CSSL3IsolatedStorage
* 版权 (c) Microsoft Corporation.
* 
* 独立存储器示例后台文件。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
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
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Threading;

namespace CSSL3IsolatedStorage
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // 初始化应用程序
        IsoDirectory _isoroot;
        IsolatedStorageFile _isofile;
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 加载独立存储器文件
            _isofile = IsolatedStorageFile.GetUserStoreForApplication();
            
            // 显示独立存储器文件信息
            RefreshAvailableSize();

            // 加载独立存储器树目录查看模型
            _isoroot = LoadIsoRoot();

            // 显示树目录
            tvIsolatedStorage.ItemsSource = new ObservableCollection<IsoFile> { _isoroot };

            // 从IsolatedStorageSettings取得上次登陆时间
            if (IsolatedStorageSettings.ApplicationSettings.Contains("lastLogin"))
            {
                var date = IsolatedStorageSettings.ApplicationSettings["lastLogin"].ToString();
                tbAppInfo.Text = "上次本应用程序运行时间为: "+date;
            }
            else
                tbAppInfo.Text = "没有上次本应用程序运行时间。";
            // 保存登陆时间到IsolatedStorageSettings
            IsolatedStorageSettings.ApplicationSettings["lastLogin"] = DateTime.Now;

            // 更新操作面板
            UpdateOperationPanel();
        }

        #region 独立存储器树目录查看模型方法

        // Helper 方法: 取得父目录
        IsoDirectory GetParentDir(IsoDirectory root, IsoFile child)
        {
            if (string.IsNullOrEmpty(child.FilePath))
                return null;
            else
            {
                string[] dirs = child.FilePath.Split('/');
                IsoDirectory cur = root;
                for (int i = 1; i < dirs.Length - 1; i++)
                {
                    IsoDirectory next = cur.Children.FirstOrDefault(dir => dir.FileName == dirs[i]) as IsoDirectory;
                    if (next != null)
                        cur = next;
                    else
                        return null;
                }
                return cur;
            }
        }

        // 加载独立存储器查看模型
        IsoDirectory LoadIsoRoot()
        {
            var root = new IsoDirectory("Root", null);
            AddFileToDirectory(root, _isofile);
            return root;
        }

        // 用递归方法增加目录/文件
        void AddFileToDirectory(IsoDirectory dir, IsolatedStorageFile isf)
        {
            string[] childrendir, childrenfile;
            if (string.IsNullOrEmpty(dir.FilePath))
            {
                childrendir = isf.GetDirectoryNames();
                childrenfile = isf.GetFileNames();
            }
            else
            {
                childrendir = isf.GetDirectoryNames(dir.FilePath + "/");
                childrenfile = isf.GetFileNames(dir.FilePath + "/");
            }

            // 增加目录实体
            foreach (var dirname in childrendir)
            {
                var childdir = new IsoDirectory(dirname, dir.FilePath + "/" + dirname);
                AddFileToDirectory(childdir, isf);
                dir.Children.Add(childdir);
            }

            // 增加文件实体
            foreach (var filename in childrenfile)
            {
                dir.Children.Add(new IsoFile(filename, dir.FilePath + "/" + filename));
            }
        }
        #endregion

        #region 在worker线程中复制流

        // 创建workerthread来复制流
        void CopyStream(Stream from, Stream to)
        {

            BackgroundWorker bworker = new BackgroundWorker();
            bworker.WorkerReportsProgress = true;
            bworker.DoWork += new DoWorkEventHandler(bworker_DoWork);
            bworker.ProgressChanged += new ProgressChangedEventHandler(bworker_ProgressChanged);
            bworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bworker_RunWorkerCompleted);

            bworker.RunWorkerAsync(new Stream[] { from, to });

            // 显示“复制”面板
            gdDisable.Visibility = Visibility.Visible;
            spCopyPanel.Visibility = Visibility.Visible;
            gdPlayerPanel.Visibility = Visibility.Collapsed;
        }

        // 处理工作完成事件
        void bworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 关闭“复制”面板
            gdDisable.Visibility = Visibility.Collapsed;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        // 显示进度
        void bworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbCopyProgress.Value = e.ProgressPercentage;
        }

        // 在workerthread线程中复制流
        void bworker_DoWork(object sender, DoWorkEventArgs e)
        {
            var param = e.Argument as Stream[];

            byte[] buffer = new byte[65536];
            int pos = 0;
            int progress = -1;
            while (true)
            {
                int icount = param[0].Read(buffer, pos, buffer.Length);
                param[1].Write(buffer, 0, icount);
                if (icount < buffer.Length)
                    break;

                int curprogress = (int)(param[1].Length * 100 / param[0].Length);
                if (curprogress > progress)
                {
                    progress = curprogress;
                    ((BackgroundWorker)sender).ReportProgress(progress);
                }
            }

            // 在用户界面线程中关闭线程
            Dispatcher.BeginInvoke(delegate
            {
                param[0].Close();
                param[1].Close();
                RefreshAvailableSize(); 
            });
        }

        #endregion

        #region 树目录视图和操作按钮事件处理器

        void RefreshAvailableSize()
        {
            tbQuotaAvailable.Text = string.Format("当前存储器空间大小是: {0}KB, {1}KB可用。 这个配额能通过用户操作增加，例如鼠标点击操作。",
                _isofile.Quota / 1024, _isofile.AvailableFreeSpace / 1024);
        }

        // 更新操作面板
        void UpdateOperationPanel()
        {
            var item = tvIsolatedStorage.SelectedItem;
            if (item == null)
            {
                spOperationPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                spOperationPanel.Visibility = Visibility.Visible;
                if (item is IsoDirectory)
                {
                    bnAddDir.Visibility = Visibility.Visible;
                    bnAddFile.Visibility = Visibility.Visible;
                    bnDelete.Visibility = Visibility.Visible;
                    bnSave.Visibility = Visibility.Collapsed;
                    bnPlay.Visibility = Visibility.Collapsed;
                }
                else if (item is IsoFile)
                {
                    bnAddDir.Visibility = Visibility.Collapsed;
                    bnAddFile.Visibility = Visibility.Collapsed;
                    bnDelete.Visibility = Visibility.Visible;
                    bnSave.Visibility = Visibility.Visible;
                    bnPlay.Visibility = Visibility.Visible;
                }
            }
        }

        // 增加空间配额
        private void bnIncreaseQuota_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isofile.IncreaseQuotaTo(_isofile.Quota + 1024 * 1024 * 10))
                {
                    MessageBox.Show("增加空间配额失败。");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            RefreshAvailableSize();
        }

        // 增加目录
        private void bnAddDir_Click(object sender, RoutedEventArgs e)
        {
            var item = tvIsolatedStorage.SelectedItem as IsoDirectory;
            if (item != null)
            {
                string newfoldename = "Folder_" + Guid.NewGuid();
                string newfolderpath = item.FilePath + "/" + newfoldename;

                try
                {
                    // 检查是否目录已经存在
                    if (_isofile.DirectoryExists(newfolderpath))
                    {
                        MessageBox.Show("目录已经存在:" + newfolderpath);
                    }
                    else
                    {
                        _isofile.CreateDirectory(newfolderpath);
                        item.Children.Add(new
                        IsoDirectory(newfoldename, newfolderpath));
                    }
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show("由于路径长度限制，目录深度设置为应该小于4。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("增加目录失败。\n详细: " + ex.Message);
                }
            }
        }

        // 增加文件
        private void bnAddFile_Click(object sender, RoutedEventArgs e)
        {
            bool overrideflag = false;
            var selecteddir = tvIsolatedStorage.SelectedItem as IsoDirectory;
            if(selecteddir==null)
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filename = ofd.File.Name;
                string filepath = selecteddir.FilePath + "/" + filename;
                IsoFile file = new IsoFile(filename, filepath);

                try
                {
                    // 检查文件名是否和目录名一样
                    if (_isofile.GetDirectoryNames(filepath).Length > 0)
                    {
                        MessageBox.Show(string.Format("文件名“{0}”不允许。", filename));
                        return;
                    }
                    // 检查文件是否已经存在
                    else if (_isofile.GetFileNames(filepath).Length > 0)
                    {
                        // 显示提示框，问是否覆盖文件
                        var mbresult = MessageBox.Show(string.Format("覆盖当前文件: {0} ?", filename), "覆盖警告", MessageBoxButton.OKCancel);
                        if (mbresult != MessageBoxResult.OK)
                            return;
                        else
                            overrideflag = true;
                    }
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show("增加文件失败。\n文件路径太长。");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                // 检查是否有足够空间
                if (_isofile.AvailableFreeSpace < ofd.File.Length)
                {
                    MessageBox.Show("独立存储器空间不够。");
                    return;
                }

                Stream isostream = null;
                Stream filestream = null; 
                try
                {
                    // 创建isolatedstorage流
                    isostream = _isofile.CreateFile(filepath);
                    // 打开文件流
                    filestream = ofd.File.OpenRead();
                    // 复制
                    // 注意：这里不会捕捉复制过程中的异常。
                    CopyStream(filestream, isostream);

                    // 检查覆盖
                    if (!overrideflag)
                        selecteddir.Children.Add(file);
                }
                catch(Exception ex) {
                    if (isostream != null) isostream.Close();
                    if (filestream != null) filestream.Close();
                    MessageBox.Show(ex.Message);
                }
            }
        }
   
        // 删除
        private void bnDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = tvIsolatedStorage.SelectedItem as IsoFile;
            if (item != null)
            {
                // 根目录
                if (string.IsNullOrEmpty(item.FilePath))
                {
                    MessageBox.Show("不能删除根目录");
                    return;
                }

                try
                {
                    if (item is IsoDirectory)
                    {
                        _isofile.DeleteDirectory(item.FilePath);
                    }
                    else
                    {
                        _isofile.DeleteFile(item.FilePath);
                    }
                    var isodirparent = GetParentDir(_isoroot, item);
                    if (isodirparent != null)
                        isodirparent.Children.Remove(item);
                }
                catch (PathTooLongException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            RefreshAvailableSize();
        }

        // 保存到本地
        private void bnSave_Click(object sender, RoutedEventArgs e)
        {
            var item = tvIsolatedStorage.SelectedItem as IsoFile;
            if (item != null)
            {
                try
                {
                    SaveFileDialog sfd1 = new SaveFileDialog();

                    // 设置文件过滤
                    var substr = item.FileName.Split('.');
                    if (substr.Length >= 2)
                    {
                        string defaultstr = "*." + substr[substr.Length - 1];
                        sfd1.Filter = string.Format("({0})|{1}|(*.*)|*.*", defaultstr, defaultstr);
                    }
                     else 
                        sfd1.Filter = "(*.*)|*.*";

                    // 显示保存对话框
                    var result = sfd1.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        // 打开isolatedstorage流
                        var filestream = sfd1.OpenFile();
                        // 创建文件流
                        var isostream = _isofile.OpenFile(item.FilePath, FileMode.Open, FileAccess.Read);
                        // 复制
                        CopyStream(isostream, filestream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // 关闭播放面板
        private void bnClosePlayer_Click(object sender, RoutedEventArgs e)
        {
            gdDisable.Visibility = Visibility.Collapsed;
            mePlayer.Stop();
            mePlayer.Source = null;

            if (currentplaystream != null)
                currentplaystream.Close();

       }

        // 播放
        Stream currentplaystream = null;
        private void bnPlay_Click(object sender, RoutedEventArgs e)
        {
            var item = tvIsolatedStorage.SelectedItem as IsoFile;
            if (item != null)
            {
                try
                {
                    var stream = _isofile.OpenFile(item.FilePath, FileMode.Open, FileAccess.Read);

                    // 显示播放面板
                    gdDisable.Visibility = Visibility.Visible;
                    spCopyPanel.Visibility = Visibility.Collapsed;
                    gdPlayerPanel.Visibility = Visibility.Visible;

                    mePlayer.SetSource(stream);
                    currentplaystream = stream;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // 当选择树目录的子项改变时，刷新操作面板
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateOperationPanel();
        }
        #endregion

    }

    // Isolatedstoarge文件对象
    public class IsoFile
    {
        public string FilePath{set;get;}
        public string FileName { set; get; }

        public Stream ContentStream
        {
            private set;
            get;
        }
        public IsoFile(string strFilename, string strPath)
        {
            FileName = strFilename;
            FilePath = strPath;
        }
    }

    // Isolatedstorage目录对象
    public class IsoDirectory:IsoFile
    {
        public ObservableCollection<IsoFile> Children
        { 
            private set;
            get;
        }
        public IsoDirectory(string strFilename, string strPath)
            : base(strFilename, strPath)
        {
            Children = new ObservableCollection<IsoFile>();
        }
    }

    // 图像转换器
    // 根据实体类型，返回不同的图像
    public class ImageConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IsoDirectory)
                return new BitmapImage(new Uri("/Images/dir.png", UriKind.Relative));
            else
                return new BitmapImage(new Uri("/Images/File.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
