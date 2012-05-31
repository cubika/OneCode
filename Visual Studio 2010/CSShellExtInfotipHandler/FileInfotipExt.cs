/********************************** 模块头 **********************************\
模块名称:  FileInfotipExt.cs
项目名称:      CSShellExtInfotipHandler
版权 (c) Microsoft Corporation.

 FileInfotipExt.cs 文件定义了一个信息提示处理程序， 它继承了 IPersistFile 和接口 IQueryInfo 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace CSShellExtInfotipHandler
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("B8D98AB4-376B-45D0-9CF6-8BF22A588989"), ComVisible(true)]
    public class FileInfotipExt : IPersistFile, IQueryInfo
    {
        // 被选择文件的名字
        private string selectedFile = null;


        #region Shell Extension Registration

        [ComRegisterFunction()]
        public static void Register(Type t)
        {
            try
            {
                ShellExtReg.RegisterShellExtInfotipHandler(t, ".cs");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                throw;  
            }
        }

        [ComUnregisterFunction()]
        public static void Unregister(Type t)
        {
            try
            {
                ShellExtReg.UnregisterShellExtInfotipHandler(".cs");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                throw;  
            }
        }

        #endregion


        #region IPersistFile Members

        public void GetClassID(out Guid pClassID)
        {
            throw new NotImplementedException();
        }

        public void GetCurFile(out string ppszFileName)
        {
            throw new NotImplementedException();
        }

        public int IsDirty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打开指定文件并根据内容初始化对象。
        /// </summary>
        /// <param name="pszFileName">
        /// 被打开的文件的绝对路径。
        /// </param>
        /// <param name="dwMode">
        /// 被打开的文件的访问权限 
        /// </param>
        public void Load(string pszFileName, int dwMode)
        {
            // pszFileName 包含被打开文件的绝对路径信息.
            this.selectedFile = pszFileName;
        }

        public void Save(string pszFileName, bool fRemember)
        {
            throw new NotImplementedException();
        }

        public void SaveCompleted(string pszFileName)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IQueryInfo Members

        /// <summary>
        ///获取文本信息提示
        /// </summary>
        /// <param name="dwFlags">
        /// Flags是直接处理检索的文本提示信息，通常默认值为0 
        /// </param>
        /// <returns>
        /// 返回一个包含提示信息的字符串
        /// </returns>
        public string GetInfoTip(uint dwFlags)
        {
            //先准备信息提示文本.本例子的信息提示文本时由文件路径和代码行数 
            int lineNum = 0;
            using (StreamReader reader = File.OpenText(this.selectedFile))
            {
                while (reader.ReadLine() != null)
                {
                    lineNum++;
                }
            }

            return string.Format("File: {0}\nLines: {1}\n" +
                "- Infotip displayed by CSShellExtInfotipHandler",
                this.selectedFile, lineNum.ToString());
        }

        /// <summary>
        /// 获取item的标志信息.本例子没有使用这个方法 。
        /// </summary>
        /// <returns>返回item的标志信息.</returns>
        public int GetInfoFlags()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}