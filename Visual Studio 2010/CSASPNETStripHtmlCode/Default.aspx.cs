/*********************************** 模块头 ********************************\
* 模块名: Default.aspx.cs
* 项目名: CSASPNETStripHtmlCode
* 版权 (c) Microsoft Corporation
*
* 这个页面从SourcePage.aspx中搜索完整的html代码.
* 用户可以获取和解析html代码的许多部分,像:纯文本, 
* 图片,链接,脚本代码,等等.
* 这个实例代码可以用在许多应用程序中.例如： 
* 搜索引擎,搜索引擎需要检查网页的截断信息,像标题,
* 纯文本,图片等等.
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CSASPNETStripHtmlCode
{
    public partial class Default : System.Web.UI.Page
    {
        string strUrl = String.Empty;
        string strWholeHtml = string.Empty;
        const string MsgPageRetrieveFailed = "对不起,网页运行失败！";
        bool flgPageRetrieved = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            strUrl = this.Page.Request.Url.ToString().Replace("Default","SourcePage");         
            tbResult.Text = string.Empty;
        }

        protected void btnRetrieveAll_Click(object sender, EventArgs e)
        {
            strWholeHtml = this.GetWholeHtmlCode(strUrl);
            if (flgPageRetrieved)
            {
                tbResult.Text = strWholeHtml;
            }
            else
            {
                tbResult.Text = MsgPageRetrieveFailed;
            }
        }

        /// <summary>
        /// 用WebRequest和WebRespond从SourcePage.aspx中检索完整的html代码 
        /// 我们把html代码的格式转换为uft-8.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetWholeHtmlCode(string url)
        {
            string strHtml = string.Empty;
            StreamReader strReader = null;
            HttpWebResponse wrpContent = null;       
            try
            {
                HttpWebRequest wrqContent = (HttpWebRequest)WebRequest.Create(strUrl);
                wrqContent.Timeout = 300000;
                wrpContent = (HttpWebResponse)wrqContent.GetResponse();
                if (wrpContent.StatusCode != HttpStatusCode.OK)
                {
                    flgPageRetrieved = false;
                    strHtml = "对不起,网页运行失败";
                }
                if (wrpContent != null)
                {
                    strReader = new StreamReader(wrpContent.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                    strHtml = strReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                flgPageRetrieved = false;
                strHtml = e.Message;
            }
            finally 
            {
                if (strReader != null)
                    strReader.Close();
                if (wrpContent != null)
                    wrpContent.Close();
            }
            return strHtml;
        }

        /// <summary>
        /// 从html代码里搜索纯文本,这个纯文本只包括html的 
        /// Body标记.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRetrievePureText_Click(object sender, EventArgs e)
        {
            strWholeHtml = this.GetWholeHtmlCode(strUrl);
            if (flgPageRetrieved)
            {
                string strRegexScript = @"(?m)<body[^>]*>(\w|\W)*?</body[^>]*>";
                string strRegex = @"<[^>]*>";
                string strMatchScript = string.Empty;
                Match matchText = Regex.Match(strWholeHtml, strRegexScript, RegexOptions.IgnoreCase);
                strMatchScript = matchText.Groups[0].Value;
                string strPureText = Regex.Replace(strMatchScript, strRegex, string.Empty, RegexOptions.IgnoreCase);
                tbResult.Text = strPureText;
            }
            else
            {
                tbResult.Text = MsgPageRetrieveFailed;
            }
        }

        /// <summary>
        /// 从html代码中检索脚本代码.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRetrieveSriptCode_Click(object sender, EventArgs e)
        {
            strWholeHtml = this.GetWholeHtmlCode(strUrl);
            if (flgPageRetrieved)
            {
                string strRegexScript = @"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>";
                string strRegex = @"<[^>]*>";
                string strMatchScript = string.Empty;
                MatchCollection matchList = Regex.Matches(strWholeHtml, strRegexScript, RegexOptions.IgnoreCase);
                StringBuilder strbScriptList = new StringBuilder();
                foreach (Match matchSingleScript in matchList)
                {
                    string strSingleScriptText = Regex.Replace(matchSingleScript.Value, strRegex, string.Empty, RegexOptions.IgnoreCase);
                    strbScriptList.Append(strSingleScriptText + "\r\n");
                }
                tbResult.Text = strbScriptList.ToString();
            }
            else
            {
                tbResult.Text = MsgPageRetrieveFailed;
            }
        }

        /// <summary>
        /// 从html代码中检索图片信息.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRetrieveImage_Click(object sender, EventArgs e)
        {
            strWholeHtml = this.GetWholeHtmlCode(strUrl);
            if (flgPageRetrieved)
            {
                string strRegexImg = @"(?is)<img.*?>";
                MatchCollection matchList = Regex.Matches(strWholeHtml, strRegexImg, RegexOptions.IgnoreCase);
                StringBuilder strbImageList = new StringBuilder();

                foreach (Match matchSingleImage in matchList)
                {
                    strbImageList.Append(matchSingleImage.Value + "\r\n");
                }
                tbResult.Text = strbImageList.ToString();
            }
            else
            {
                tbResult.Text = MsgPageRetrieveFailed;
            }
        }

        /// <summary>
        /// 从html代码中检索链接.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRetrievelink_Click(object sender, EventArgs e)
        {
            strWholeHtml = this.GetWholeHtmlCode(strUrl);
            if (flgPageRetrieved)
            {

                string strRegexLink = @"(?is)<a .*?>";
                MatchCollection matchList = Regex.Matches(strWholeHtml, strRegexLink, RegexOptions.IgnoreCase);
                StringBuilder strbLinkList = new StringBuilder();

                foreach (Match matchSingleLink in matchList)
                {
                    strbLinkList.Append(matchSingleLink.Value + "\r\n");
                }
                tbResult.Text = strbLinkList.ToString();
            }
            else
            {
                tbResult.Text = MsgPageRetrieveFailed;
            }
        }

    }
}