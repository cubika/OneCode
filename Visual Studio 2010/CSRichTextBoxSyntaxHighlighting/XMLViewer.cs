/****************************** 模块头 ******************************\
* 模块名:  XMLViewer.cs
* 项目名:	    CSRichTextBoxSyntaxHighlighting 
* 版权(c)  Microsoft Corporation.
* 
* XMLViewer类继承自System.Windows.Forms.RichTextBox，这个类是以规范的格式来显示xml文件
* 
* RichTextBox使用RTF格式来显示测试。XMLViewer类通过使用在XMLViewSettings中规定的一些
* 格式来实现xml到RTF的转换，然后设置RTF属性为这个值。
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
using System.Text;
using System.Xml.Linq;
using System.Drawing;

namespace CSRichTextBoxSyntaxHighlighting 
{
    public class XMLViewer : System.Windows.Forms.RichTextBox
    {
        private XMLViewerSettings settings;
        /// <summary>
        /// 格式设置
        /// </summary>
        public XMLViewerSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new XMLViewerSettings
                    {
                        AttributeKey = Color.Red,
                        AttributeValue = Color.Blue,
                        Tag = Color.Blue,
                        Element = Color.DarkRed,
                        Value = Color.Black,
                    };
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        /// <summary>
        /// 通过XMLViewerSettings中的一个规范格式，xml文件转换到RTF
        /// 然后设置RTF属性的值
        /// </summary>
        /// <param name="includeDeclaration">
        /// 指定是否包括声明
        /// </param>
        public void Process(bool includeDeclaration)
        {
            try
            {
                // RTF包含头和内容两个部分，colortbl作为头的一部分，{1}将要用被内容替换
                string rtfFormat = @"{{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052
{{\fonttbl{{\f0\fnil Courier New;}}}}
{{\colortbl ;{0}}}
\viewkind4\uc1\pard\lang1033\f0 
{1}}}";

                // 从Text属性中获取XDocument
                var xmlDoc = XDocument.Parse(this.Text, LoadOptions.None);

                StringBuilder xmlRtfContent = new StringBuilder();

                // 如果includeDeclaration是真，并且Document包含声明，那么将这个声明添加
                // 到RTF的内容中。
                if (includeDeclaration && xmlDoc.Declaration != null)
                {

                    // 在XMLViewerSettings中的常量，用来指定这个在RTF中colortbl的顺序
                    xmlRtfContent.AppendFormat(@"
\cf{0} <?\cf{1} xml \cf{2} version\cf{0} =\cf0 ""\cf{3} {4}\cf0 "" 
\cf{2} encoding\cf{0} =\cf0 ""\cf{3} {5}\cf0 ""\cf{0} ?>\par",
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.ElementID,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.AttributeValueID,
                        xmlDoc.Declaration.Version,
                        xmlDoc.Declaration.Encoding);
                }

                // 得到根元素的RTF
                string rootRtfContent = ProcessElement(xmlDoc.Root, 0);

                xmlRtfContent.Append(rootRtfContent);

                //构造完整的RTF，设置RTF的属性给这个值。
                this.Rtf = string.Format(rtfFormat, Settings.ToRtfFormatString(),
                    xmlRtfContent.ToString());


            }
            catch (System.Xml.XmlException xmlException)
            {
                throw new ApplicationException(
                    "Please check the input Xml. Error:" + xmlException.Message,
                    xmlException);
            }
            catch
            {
                throw;
            }
        }

        // 获取xml元素中的RTF
        private string ProcessElement(XElement element, int level)
        {

            // viewer不支持有命名空间的XML文件
            if (!string.IsNullOrEmpty(element.Name.Namespace.NamespaceName))
            {
                throw new ApplicationException(
                    "This viewer does not support the Xml file that has Namespace.");
            }

            string elementRtfFormat = string.Empty;
            StringBuilder childElementsRtfContent = new StringBuilder();
            StringBuilder attributesRtfContent = new StringBuilder();

            // 构造indent字符串
            string indent = new string(' ', 4 * level);

            // 如果元素有子元素和值, 添加这个元素到RTF中
            //  {{0}} 将要被替换为这些属性， {{1}} 将要被替换为子元素或值
            if (element.HasElements || !string.IsNullOrWhiteSpace(element.Value))
            {
                elementRtfFormat = string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} >\par
{{1}}
{0}\cf{1} </\cf{2} {3}\cf{1} >\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);

                // 构造RTF子元素
                if (element.HasElements)
                {
                    foreach (var childElement in element.Elements())
                    {
                        string childElementRtfContent =
                            ProcessElement(childElement, level + 1);
                        childElementsRtfContent.Append(childElementRtfContent);
                    }
                }

                // 如果 !string.IsNullOrWhiteSpace(element.Value), 然后构造RTF的值
                else
                {
                    childElementsRtfContent.AppendFormat(@"{0}\cf{1} {2}\par",
                        new string(' ', 4 * (level + 1)),
                        XMLViewerSettings.ValueID,
                        CharacterEncoder.Encode(element.Value.Trim()));
                }
            }

            // 元素只有这些属性. {{0}} 将要被这些属性替换.
            else
            {
                elementRtfFormat =
                    string.Format(@"
{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} />\par",
                    indent,
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    element.Name);
            }

            // 构造RTF的属性
            if (element.HasAttributes)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    string attributeRtfContent = string.Format(
                        @" \cf{0} {3}\cf{1} =\cf0 ""\cf{2} {4}\cf0 """,
                        XMLViewerSettings.AttributeKeyID,
                        XMLViewerSettings.TagID,
                        XMLViewerSettings.AttributeValueID,
                        attribute.Name,
                       CharacterEncoder.Encode(attribute.Value));
                    attributesRtfContent.Append(attributeRtfContent);
                }
                attributesRtfContent.Append(" ");
            }

            return string.Format(elementRtfFormat, attributesRtfContent,
                childElementsRtfContent);
        }

    }
}
