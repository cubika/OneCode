/****************************** 模块头 ******************************\
* 模块名:  CharacterEncoder.cs
* 项目名:	    CSRichTextBoxSyntaxHighlighting 
* 版权 (c)  Microsoft Corporation.
* 
* CharacterEncoder类提供一个Static方法，去编码在XML和RTF中的一个特殊的字符，例如 '<', '>', '"', '&', ''', '\',
*  '{' and '}' 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Text;

namespace CSRichTextBoxSyntaxHighlighting
{
    public class CharacterEncoder
    {
        public static string Encode(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
            {
                return string.Empty;
            }

            StringBuilder encodedText = new StringBuilder();
            for (int i = 0; i < originalText.Length; i++)
            {
                switch (originalText[i])
                {
                    case '"':
                        encodedText.Append("&quot;");
                        break;
                    case '&':
                        encodedText.Append(@"&amp;");
                        break;
                    case '\'':
                        encodedText.Append(@"&apos;");
                        break;
                    case '<':
                        encodedText.Append(@"&lt;");
                        break;
                    case '>':
                        encodedText.Append(@"&gt;");
                        break;

                    // 字符 '\' 应该被转换为 @"\\" or "\\\\" 
                    case '\\':
                        encodedText.Append(@"\\");
                        break;

                    // 字符 '{' 应该被转换为 @"\{" or "\\{" 
                    case '{':
                        encodedText.Append(@"\{");
                        break;

                    // 字符 '}' 应该被转化为 @"\}" or "\\}" 
                    case '}':
                        encodedText.Append(@"\}");
                        break;
                    default:
                        encodedText.Append(originalText[i]);
                        break;
                }

            }
            return encodedText.ToString();
        }
    }
}
