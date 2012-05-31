/****************************** 模块头  ******************************\
* 模块名:  XMLViewerSettings.cs
* 项目名:	    CSRichTextBoxSyntaxHighlighting 
* 版权(c)  Microsoft Corporation.
* 
* 这个XMLViewerSettings类定义了一些在XmlViewer类要使用的颜色，也
* 定义了一些用来在RTF中指定颜色顺序的常量
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

using System.Drawing;
using System.Text;

namespace CSRichTextBoxSyntaxHighlighting 
{
    public class XMLViewerSettings
    {
        public const int ElementID = 1;
        public const int ValueID = 2;
        public const int AttributeKeyID = 3;
        public const int AttributeValueID = 4;
        public const int TagID = 5;

        /// <summary>
        /// 一个xml元素名称的颜色
        /// </summary>
        public Color Element { get; set; }

        /// <summary>
        /// 一个xml元素值的颜色
        /// </summary>
        public Color Value { get; set; }

        /// <summary>
        /// 在xml元素中，属性键的颜色
        /// </summary>
        public Color AttributeKey { get; set; }

        /// <summary>
        /// 在xml元素中，属性值的颜色
        /// </summary>
        public Color AttributeValue { get; set; }

        /// <summary>
        /// 标签和运算符的颜色，例如，  "<,/> 和 =".
        /// </summary>
        public Color Tag { get; set; }

        /// <summary>
        /// 转化设置给RTF颜色的定义
        /// </summary>
        public string ToRtfFormatString()
        {
            // RTF颜色定义格式
            string format = @"\red{0}\green{1}\blue{2};";

            StringBuilder rtfFormatString = new StringBuilder();

            rtfFormatString.AppendFormat(format, Element.R, Element.G, Element.B);
            rtfFormatString.AppendFormat(format, Value.R, Value.G, Value.B);
            rtfFormatString.AppendFormat(format,
                AttributeKey.R, 
                AttributeKey.G, 
                AttributeKey.B);
            rtfFormatString.AppendFormat(format, AttributeValue.R, 
                AttributeValue.G, AttributeValue.B);
            rtfFormatString.AppendFormat(format, Tag.R, Tag.G, Tag.B);

            return rtfFormatString.ToString();

        }
    }
}
