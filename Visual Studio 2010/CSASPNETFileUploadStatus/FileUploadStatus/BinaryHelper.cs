/****************************** 模块头 ******************************\
* 模块名:    BinaryHelper.cs
* 项目名:    CSASPNETFileUploadStatus
* 版权 (c) Microsoft Corporation
*
* 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
* 像ActiveX 控件,Flash 或者Silverlight.
* 
* 这个类是用来过滤二进制数据进而获得文件数据. 
* 所有这些静态的方法对于二进制数据的处理都是有帮助的. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
namespace CSASPNETFileUploadStatus
{
    internal class BinaryHelper
    {

        // 从一个原始的二进制数组中复制部分数据.
        public static byte[] Copy(byte[] source, int index, int length)
        {
            byte[] result = new byte[length];
            Array.ConstrainedCopy(source, index, result, 0, length);
            return result;
        }

        // 把两个二进制数组合并在一起.
        public static byte[] Combine(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return null;
            }
            else if (a == null || b == null)
            {
                return a ?? b;
            }
            byte[] newData = new byte[a.Length + b.Length];
            Array.ConstrainedCopy(a, 0, newData, 0, a.Length);
            Array.ConstrainedCopy(b, 0, newData, a.Length, b.Length);
            return newData;

        }

        // 检查两个二进制数组在同一个索引
        // 里是否有相同的数据 .
        public static bool Equals(byte[] source, byte[] compare)
        {
            if (source.Length != compare.Length)
            {
                return false;
            }
            if (SequenceIndexOf(source, compare, 0) != 0)
            {
                return false;
            }
            return true;
        }

        // 在二进制数组里获取部分数据.
        public static byte[] SubData(byte[] source, int startIndex)
        {
            byte[] result = new byte[source.Length - startIndex];
            Array.ConstrainedCopy(source, startIndex, result, 0, result.Length);
            return result;
        }

        // 在二进制数组里获取部分数据.
        public static byte[] SubData(byte[] source, int startIndex, int length)
        {
            byte[] result = new byte[length];
            Array.ConstrainedCopy(source, startIndex, result, 0, length);
            return result;
        }

        // 在原数组里所有数据和位置都和另
        // 一个数组相同的时候获取原数组索引.
        public static int SequenceIndexOf(byte[] source, byte[] compare)
        {
            return SequenceIndexOf(source, compare, 0);
        }
        public static int SequenceIndexOf(byte[] source, byte[] compare, int startIndex)
        {
            int result = -1;
            int sourceLen = source.Length;
            int compareLen = compare.Length;
            if (startIndex < 0)
            {
                return -1;
            }

            for (int i = startIndex; i < sourceLen - compareLen + 1; i++)
            {
                if (source[i] == compare[0] &&
                    source[i + compareLen - 1] == compare[compareLen - 1])
                {
                    int t = 0;
                    for (int j = 0; j < compare.Length; j++)
                    {
                        t++;
                        if (compare[j] != source[i + j])
                            break;
                    }
                    if (t == compareLen)
                    {
                        result = i;
                        break;
                    }
                }
            }
            return result;
        }

    }
}
