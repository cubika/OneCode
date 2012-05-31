/****************************** 模块头 ******************************\
* 模块名:    UploadFileCollection.cs
* 项目名:    CSASPNETFileUploadStatus
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
* 像ActiveX 控件,Flash 或者Silverlight.
* 
* 这个类用来过滤请求实体的文件数据并且把它们
* 存储在UploadFileCollection. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Text;
using System.Text.RegularExpressions;

namespace CSASPNETFileUploadStatus
{
    internal class FileUploadDataManager
    {
        private bool isFinished = true;
        private byte[] draft = null;
        private bool isFile = false;

        private readonly byte[] _backSpace = null;
        private readonly byte[] _doubleBackSpace = null;
        private readonly byte[] _boundary = null;
        private readonly byte[] _endTag = null;

        public FileUploadDataManager(string boundary)
        {
            _boundary = ASCIIEncoding.ASCII.GetBytes("--" + boundary);
            _backSpace = ASCIIEncoding.ASCII.GetBytes("\r\n");
            _doubleBackSpace = ASCIIEncoding.ASCII.GetBytes("\r\n\r\n");
            _endTag = ASCIIEncoding.ASCII.GetBytes("--" + boundary + "--\r\n");
            FilterResult = new UploadFileCollection();
            draft = null;
            isFile = false;
            isFinished = true;
        }

        public UploadFileCollection FilterResult
        {
            private set;
            get;
        }

        public void AppendData(byte[] data)
        {
            FilterFileDataFromBodyEntity(data);
            if (data == null)
                return;
        }

        private void AppendToLastEntity(byte[] data)
        {
            if (!isFinished && isFile)
            {
                UploadFile lastFile =
                    FilterResult[FilterResult.Count - 1];
                lastFile.AppendData(data);
            }
        }

        // 过滤请求数据来获取文件的二进制数据.
        // 这里是过滤数据的算法逻辑. 
        // 在我们讲到算法之前将会有很多内容要 
        // 讲（可能要一本书）.所以我只在这里开
        // 个头，但是忽略详细的解释.
        private void FilterFileDataFromBodyEntity(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            if (draft != null)
            {
                byte[] temp = BinaryHelper.Combine(draft,
                                BinaryHelper.Copy(data, 0, _boundary.Length));
                int entity_st = BinaryHelper.SequenceIndexOf(temp, _boundary, 0);
                int entity_ed = BinaryHelper.SequenceIndexOf(temp, _boundary,
                    entity_st + _boundary.Length + 2);

                if (isFile && !isFinished)
                {
                    if (entity_st == 0)
                    {
                        int header_st = entity_st + _boundary.Length + 2;
                        int header_ed = BinaryHelper.SequenceIndexOf(temp,
                            _doubleBackSpace, header_st);
                        int body_st = header_ed + 4;
                        if (entity_ed == -1)
                        {
                            AppendToLastEntity(BinaryHelper.SubData(draft, body_st));
                            draft = null;
                        }
                        else
                        {
                            AppendToLastEntity(BinaryHelper.SubData(draft,
                                body_st, entity_ed - body_st - 2));
                            isFinished = true;
                            isFile = false;
                            draft = BinaryHelper.SubData(draft, entity_ed);
                        }
                    }
                    else
                    {
                        AppendToLastEntity(draft);
                        draft = null;
                    }
                }

                // 当需要添加新数据时，
                // 把这两个二进制数组合并成一个.
                data = BinaryHelper.Combine(draft, data);
                draft = null;
            }
            while (true)
            {
                // 找到边界
                int entity_st = BinaryHelper.SequenceIndexOf(data, _boundary, 0);

                // 如果当前加载的数据包含边界
                if (entity_st > -1)
                {
                    if (isFile && !isFinished)
                    {
                        AppendToLastEntity(BinaryHelper.SubData(data, 0,
                            entity_st - 2));
                        data = BinaryHelper.SubData(data, entity_st);
                        isFile = false;
                        isFinished = true;
                        continue;
                    }

                    int entity_ed = BinaryHelper.SequenceIndexOf(data, _boundary,
                        entity_st + _boundary.Length + 2);
                    int header_st = entity_st + _boundary.Length + 2;
                    int header_ed = BinaryHelper.SequenceIndexOf(data,
                        _doubleBackSpace, header_st);
                    int body_st = header_ed + 4;

                    if (body_st < 4)
                    {
                        // 如果实体的头部不完整，
                        // 设置这个实体作为数据，
                        // 卸载掉函数来请求更多的数据.
                        draft = data;
                        return;
                    }
                    else
                    {
                        // 如果实体的头部完整 
                        if (!isFile && isFinished)
                        {
                            // 把实体头部的数据转换为UTF8编码
                            string headerInEntity = ASCIIEncoding.UTF8.GetString(
                                BinaryHelper.SubData(data, header_st, header_ed - header_st));
                            // 如果这是一个文件实体，头部包会含关键字："filename".
                            if (headerInEntity.IndexOf("filename") > -1)
                            {
                                // 在实体的头部使用正则表达式来 
                                // 获取元数据的关键字值.
                                Regex detailsReg =
                                    new Regex("Content-Disposition: form-data; name=\"([^\"]*)\";" +
                                        " filename=\"([^\"]*)\"Content-Type: ([^\"]*)");
                                Match regMatch =
                                    detailsReg.Match(headerInEntity.Replace("\r\n", ""));
                                string controlName = regMatch.Groups[1].Value;
                                string clientPath = regMatch.Groups[2].Value;
                                string contentType = regMatch.Groups[3].Value;
                                if (string.IsNullOrEmpty(clientPath))
                                {
                                    isFile = false;
                                }
                                else
                                {
                                    isFile = true;
                                    // 为文件实体创建一个新的实例
                                    UploadFile up = new UploadFile(clientPath, contentType);
                                    FilterResult.Add(up);
                                    isFinished = false;
                                }
                            }
                            else
                            {
                                isFile = false;
                            }
                        }

                    }
                    if (entity_ed > -1)
                    {
                        // 如果我们在第一个边界之后又发现另一边界，
                        // 那表示实体块在那里结束了.
                        // 只是文件实体的时候，我们才需要在实体的
                        // 主体里获取数据.
                        if (isFile)
                        {
                            AppendToLastEntity(BinaryHelper.SubData(data,
                                body_st, entity_ed - body_st - 2));
                            isFinished = true;
                            isFile = false;
                        }
                        // 移除当前的实体处理数据并且
                        // 循环到下一个数据.
                        data = BinaryHelper.SubData(data, entity_ed);
                        if (BinaryHelper.Equals(data, _endTag))
                        {
                            data = null;
                            draft = null;
                            return;
                        }
                        continue;
                    }
                    else
                    {
                        // 如果我们不能找到结束标记,我们 
                        // 必须要把这些数据移到草稿并 
                        // 请求添加新数据.
                        draft = data;
                        return;
                    }
                }
                else
                {
                    // 如果我们不能找到结束标记,我们 
                    // 必须要把这些数据移到草稿并 
                    // 请求添加新数据.
                    draft = data;
                    return;
                }

            }


        }
    }
}
