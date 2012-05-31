/****************************** 模块头******************************\
*模块名:  FTPFileSystem.cs
* 项目:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
 * FTPFileSystem 类代表一个远程FTP服务器文件，当运行FTP列表协议方法得到一个详细的文件列表在一个FTP服务器上，
 * 这个服务器将相应许多信息记录，每个记录代表一个文件，依靠服务器的FTP目录列表格式，记录如下：
* 1. MSDOS
*    1.1. Directory
*         12-13-10  12:41PM  <DIR>  Folder A
*    1.2. File
*         12-13-10  12:41PM  [Size] File B  
*         
*   NOTE: The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit
*         years is not checked in IIS.
*        
* 2. UNIX
*    2.1. Directory
*         drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
*    2.2. File
*         -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
* 
*    NOTE: The date segment does not contains year.
 *   注意：日期部分不包括年
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

namespace CSFTPUpload
{

    public class FTPFileSystem
    {
        /// <summary>
        /// 原始字符串记录
        /// </summary>
        public string OriginalRecordString { get; set; }

        /// <summary>

        /// 
        /// </summary>
        public FTPDirectoryListingStyle DirectoryListingStyle { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// FTPFileSystem 实例名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>

        /// 指定是否FTPFileSystem 实例是一个目录
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// FTPFileSystem 实例最近修改的时间
        /// 
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
      
        ///假如不是一个目录FTPFileSystem 实例的大小
        /// </summary>
        public int Size { get; set; }

        private FTPFileSystem() { }

        /// <summary>

        /// 重写ToString（）方法展示更多友好信息
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t\t{2}",
                this.ModifiedTime.ToString("yyyy-MM-dd HH:mm"),
                this.IsDirectory ? "<DIR>" : this.Size.ToString(),
                this.Name);
        }

        /// <summary>
        
        /// 从记录字符串中指出FTP Directory Listing Style
        /// </summary>
        public static FTPDirectoryListingStyle GetDirectoryListingStyle(string recordString)
        {
            Regex regex = new System.Text.RegularExpressions.Regex(@"^[d-]([r-][w-][x-]){3}$");

            string header = recordString.Substring(0, 10);

           
            //如果类型是UNIX，开头如"drwxrwxrwx"
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
       
        /// 从记录字符串得到一个FTPFileSystem
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


            //如果是目录加“/”到路径
            fileSystem.Url = new Uri(baseUrl, fileSystem.Name + (fileSystem.IsDirectory ? "/" : string.Empty));

            return fileSystem;
        }

        /// <summary>

        /// 记录字符串如下：
        /// Directory: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
        /// File:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
        /// NOTE: The date segment does not contains year.
        /// </summary>
        static FTPFileSystem ParseUNIXRecordString(string recordString)
        {
            FTPFileSystem fileSystem = new FTPFileSystem();

            fileSystem.OriginalRecordString = recordString.Trim();
            fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX;

            // The segments is like "drwxrwxrwx", "",  "", "1", "owner", "", "", "", 
            // "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
            // "0", "Dec", "13", "11:25", "Folder", "A".
            string[] segments = fileSystem.OriginalRecordString.Split(' ');

            int index = 0;

            //许可部分如"drwxrwxrwx
            string permissionsegment = segments[index];

            
           //如果属性以“d”开始，意味是个目录
            fileSystem.IsDirectory = permissionsegment[0] == 'd';


            //跳过空部分
            while (segments[++index] == string.Empty) { }

          
            //跳过目录部分


            //跳过空部分
            while (segments[++index] == string.Empty) { }


            //跳过所有者部分


            //跳过空部分
            while (segments[++index] == string.Empty) { }


            //跳过组部分


            //跳过空部分
            while (segments[++index] == string.Empty) { }

            //如果文件流是个文件，大小大于0 
            fileSystem.Size = int.Parse(segments[index]);


            //跳过空部分
            while (segments[++index] == string.Empty) { }
            //月份部分
            string monthsegment = segments[index];

            //跳过空部分
            while (segments[++index] == string.Empty) { }

            // 天部分
            string daysegment = segments[index];

         
            //跳过空部分
            while (segments[++index] == string.Empty) { }

            // 时间部分.
            string timesegment = segments[index];

            fileSystem.ModifiedTime = DateTime.Parse(string.Format("{0} {1} {2} ",
                timesegment, monthsegment, daysegment));

         
            //跳过空部分
            while (segments[++index] == string.Empty) { }


            //在原始字符串中计算文件名索引
            int filenameIndex = 0;

            for (int i = 0; i < index; i++)
            {

                //在初始字符中' represents ' 
                if (segments[i] == string.Empty)
                {
                    filenameIndex += 1;
                }
                else
                {
                    filenameIndex += segments[i].Length + 1;
                }
            }

            //文件名可能包括许多部分因为名字包括''
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

            // The segments is like "12-13-10",  "", "12:41PM", "", "","", "",
            // "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
            string[] segments = fileSystem.OriginalRecordString.Split(' ');

            int index = 0;


            //如果四位年在IIS中没被选中，日期部分像"12-13-10"代替"12-13-2010"
            string dateSegment = segments[index];
            string[] dateSegments = dateSegment.Split(new char[] { '-' },
                StringSplitOptions.RemoveEmptyEntries);

            int month = int.Parse(dateSegments[0]);
            int day = int.Parse(dateSegments[1]);
            int year = int.Parse(dateSegments[2]);


            //如果年大于50小于100是19**
            if (year >= 50 && year < 100)
            {
                year += 1900;
            }


           //如果年小于50是20**
            else if (year < 50)
            {
                year += 2000;
            }


            //跳过空部分
            while (segments[++index] == string.Empty) { }

            // 时间部分.
            string timesegment = segments[index];

            fileSystem.ModifiedTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}",
                year, month, day, timesegment));


            //跳过空部分
            while (segments[++index] == string.Empty) { }

            //大小或目录部分，如果是“<DIR>”它意味一个目录，否则是文件大小。
            string sizeOrDirSegment = segments[index];

            fileSystem.IsDirectory = sizeOrDirSegment.Equals("<DIR>",
                StringComparison.OrdinalIgnoreCase);


            //如果fileSystem 是一个文件，大小大于0
            if (!fileSystem.IsDirectory)
            {
                fileSystem.Size = int.Parse(sizeOrDirSegment);
            }


            while (segments[++index] == string.Empty) { }

 
            //计算在原始字符串中文件名部分的索引
            int filenameIndex = 0;

            for (int i = 0; i < index; i++)
            {
                // 在原始字符串中"" represents ' 
                if (segments[i] == string.Empty)
                {
                    filenameIndex += 1;
                }
                else
                {
                    filenameIndex += segments[i].Length + 1;
                }
            }  
            //文件名包括许多部分因为名字能包含' ' 
            fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim();

            return fileSystem;
        }
    }
}
