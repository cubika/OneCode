/****************************** 模块头 ******************************\
* 模块名:  FTPFileSystem.cs
* 项目名:	    CSFTPDownload
* 版权(c)  Microsoft Corporation.
* 
* 这个类表示远程FTP服务上的一个文件。当运行FTP LIST 协议方法来获得一个文件的详细列
* 表时，这个服务将响应一些信息的记录。每一个记录代表一个文件，依赖于服务上的FTP目录列
* 表的类型

* 如该记录：
* 1. MSDOS
*    1.1. Directory
*         12-13-10  12:41PM  <DIR>  Folder A
*    1.2. File
*         12-13-10  12:41PM  [Size] File B  
*         
*   	 
*   注意: 日期段，如“12-13-10“而不是”12-13-2010“，如果年是四位数在IIS或者FTP服务中
*         是不被检查的。
*        
* 2. UNIX
*    2.1. Directory
*         drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
*    2.2. File
*         -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
* 
*    注意: 日期段不包含年.
* 
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
using System.Text.RegularExpressions;
using System.Text;

namespace CSFTPDownload
{

    public class FTPFileSystem
    {
        /// <summary>
        /// 起初记录的字符串.
        /// </summary>
        public string OriginalRecordString { get; set; }

        /// <summary>
        /// MSDOS or UNIX.
        /// </summary>
        public FTPDirectoryListingStyle DirectoryListingStyle { get; set; }

        /// <summary>
        /// 这个服务的路径。
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// 这个FTPFileSystem实例的名字.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///指定这个FTPFileSystem实例是一个目录.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// 这个FTPFileSystem实例最后修改的时间.
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        ///
        /// 如果它不是一个目录，这个 FTPFileSystem 实例的大小.
        /// </summary>
        public int Size { get; set; }

        private FTPFileSystem() { }

        /// <summary>
        /// 重写ToString方法来显示一个更友好的信息.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t\t{2}",
                this.ModifiedTime.ToString("yyyy-MM-dd HH:mm"),
                this.IsDirectory ? "<DIR>" : this.Size.ToString(),
                this.Name);
        }

        /// <summary>
        /// 从recordString中找到FTP目录列表的样式
        /// </summary>
        public static FTPDirectoryListingStyle GetDirectoryListingStyle(string recordString)
        {
            Regex regex = new System.Text.RegularExpressions.Regex(@"^[d-]([r-][w-][x-]){3}$");

            string header = recordString.Substring(0, 10);

            //如果这个样式是UNIX, 这个头部分如 "drwxrwxrwx".
            if (regex.IsMatch(header))
            {
                return FTPDirectoryListingStyle.UNIX;
            }
            else
            {
                return FTPDirectoryListingStyle.MSDOS;
            }
        }

        /// <summary>
        /// 从recordString中获得一个FTPFileSystem
        /// </summary>
        public static FTPFileSystem ParseRecordString(Uri baseUrl, string recordString, FTPDirectoryListingStyle type)
        {
            FTPFileSystem fileSystem = null;

            if (type == FTPDirectoryListingStyle.UNIX)
            {
                fileSystem = ParseUNIXRecordString(recordString);
            }
            else
            {
                fileSystem = ParseMSDOSRecordString(recordString);
            }

            // 如果它是一个目录我们将 "/"添加到Url中，
          
            fileSystem.Url = new Uri(baseUrl, fileSystem.Name + (fileSystem.IsDirectory ? "/" : string.Empty));

            return fileSystem;
        }

        /// <summary>
        /// 这个recordString是像：
        /// 目录: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
        /// 文件:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
        /// 注意: 这个日期设置不能包括年.
        /// </summary>
        static FTPFileSystem ParseUNIXRecordString(string recordString)
        {
            FTPFileSystem fileSystem = new FTPFileSystem();

            fileSystem.OriginalRecordString = recordString.Trim();
            fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX;

            // 这个设置如 "drwxrwxrwx", "",  "", "1", "owner", "", "", "", 
            // "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
            // "0", "Dec", "13", "11:25", "Folder", "A".
            string[] segments = fileSystem.OriginalRecordString.Split(' ');

            int index = 0;

            //这个权限设置如： "drwxrwxrwx".
            string permissionsegment = segments[index];

            // 如果这个属性第一个字母是 'd', 它意味着是一个目录.
            fileSystem.IsDirectory = permissionsegment[0] == 'd';

            // 跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 跳过部分的目录.

            // 跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 跳过这组的部分.

            //跳过空的部分 .
            while (segments[++index] == string.Empty) { }

            //跳过组的这部分.

            //跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 如果这个文件流是一个文件, 它的大小大于0. 
            fileSystem.Size = int.Parse(segments[index]);

            // 跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 月的部分.
            string monthsegment = segments[index];

            // 跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 日期部分.
            string daysegment = segments[index];

            // 跳过这个空的部分.
            while (segments[++index] == string.Empty) { }

            // 时间部分.
            string timesegment = segments[index];

            fileSystem.ModifiedTime = DateTime.Parse(string.Format("{0} {1} {2} ",
                timesegment, monthsegment, daysegment));

            // 跳过空的部分
            while (segments[++index] == string.Empty) { }

            //在原始字符串中计算文件名索引的部分
           
            int filenameIndex = 0;

            for (int i = 0; i < index; i++)
            {
                // 在原始字符串中的"" 表示' '.
                if (segments[i] == string.Empty)
                {
                    filenameIndex += 1;
                }
                else
                {
                    filenameIndex += segments[i].Length + 1;
                }
            }
            // 这个名可能包含一些空字符.          
            fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim();

            return fileSystem;
        }

        /// <summary>
        /// 12-13-10  12:41PM       <DIR>          Folder A
        /// </summary>
        /// <param name="recordString"></param>
        /// <returns></returns>
        static FTPFileSystem ParseMSDOSRecordString(string recordString)
        {
            FTPFileSystem fileSystem = new FTPFileSystem();

            fileSystem.OriginalRecordString = recordString.Trim();
            fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.MSDOS;

            // 这个部分如 "12-13-10",  "", "12:41PM", "", "","", "",
            // "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
            string[] segments = fileSystem.OriginalRecordString.Split(' ');

            int index = 0;

            //这个数据部分如 "12-13-10" instead of "12-13-2010" 如果年是
            //四位数就不在ISS中查找
            string dateSegment = segments[index];
            string[] dateSegments = dateSegment.Split(new char[] { '-' },
                StringSplitOptions.RemoveEmptyEntries);

            int month = int.Parse(dateSegments[0]);
            int day = int.Parse(dateSegments[1]);
            int year = int.Parse(dateSegments[2]);

            //如果year大于等于50并且小雨100，将意味着这个年时19**
          
            if (year >= 50 && year < 100)
            {
                year += 1900;
            }

            //如果year小于50，它意味着年时20** 
          
            else if (year < 50)
            {
                year += 2000;
            }

            //跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 时间部分.
            string timesegment = segments[index];

            fileSystem.ModifiedTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}",
                year, month, day, timesegment));

            // 跳过空的部分.
            while (segments[++index] == string.Empty) { }

            // 这个部分的大小和目录 .
            //如果这部分是 "<DIR>", 它意味着是一个目录,否则它是一个文件的大小。
          
            string sizeOrDirSegment = segments[index];

            fileSystem.IsDirectory = sizeOrDirSegment.Equals("<DIR>",
                StringComparison.OrdinalIgnoreCase);

            //如果这个文件流是一个文件,这个大小是大于0的. 
            if (!fileSystem.IsDirectory)
            {
                fileSystem.Size = int.Parse(sizeOrDirSegment);
            }

            // 掉过空的部分.
            while (segments[++index] == string.Empty) { }

            // 在原始字符串中计算文件名索引的部分.
            int filenameIndex = 0;

            for (int i = 0; i < index; i++)
            {
                // 在原始字符串中的"" 表示' '.
                if (segments[i] == string.Empty)
                {
                    filenameIndex += 1;
                }
                else
                {
                    filenameIndex += segments[i].Length + 1;
                }
            }
            //这个名可能包含一些空字符.          
            fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim();

            return fileSystem;
        }
    }
}
