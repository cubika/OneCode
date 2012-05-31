/******************************* 模块头 ***********************************\
* 模块名:             CodeManager.cs
* 项目名:        CSASPNETHighlightCodeInPage
* 版权(c) Microsoft Corporation
* 
* 在这个文件中,我们使用一个散列表变量来存储不同的语言代码和与之相关包含
* 匹配选项的正则表达式.然后将样式对象添加到代码匹配字符串.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;

namespace CSASPNETHighlightCodeInPage
{
    /// <summary>
    /// 样式字符串和正则表达式的结构. 
    /// </summary>
    public struct RegexStruct
    {
        public string styleObject;
        public Regex regex;
    }

    /// <summary>
    /// 匹配字符串和样式对象.
    /// </summary>
    public class RegExp
    {
        /// <summary>
        /// 保存匹配字符串集合.
        /// </summary>
        private ArrayList _regexStructList = new ArrayList();

        /// <summary>
        /// </summary>
        /// <param name="styleObject">样式对象</param>
        /// <param name="reg">正则表达式</param>
        /// <param name="regOption">匹配选项</param>
        public void Add(string styleObject, string reg, RegexOptions regOption)
        {
            RegexStruct regexStruct = new RegexStruct();
            regexStruct.styleObject = styleObject;
            regexStruct.regex = new Regex(reg, regOption | RegexOptions.Compiled);
            _regexStructList.Add(regexStruct);
        }
        /// <summary>
        /// 返回匹配字符串,只读.
        /// </summary>
        public ArrayList regexStructList
        {
            get { return _regexStructList; }
        }
    }

    /// <summary>
    /// 高亮代码操作.
    /// </summary>
    public class CodeManager
    {
        /// <summary>
        /// 初始化散列表的变量,它用于存储不同语言的代码
        /// 及其相关的包含匹配选项的正则表达式.
        /// </summary>
        /// <returns></returns>
        public static Hashtable Init()
        {
            Hashtable hashTable = new Hashtable();
            RegExp regExp = new RegExp();

            // 添加VBScript语言信息到散列表变量.   
            #region VBScript语言
            regExp.Add("str", "\"([^\"\\n]*?)\"", RegexOptions.None);
            regExp.Add("note", "'[^\r\n]*", RegexOptions.None);
            regExp.Add("kw", @"\b(elseif|if|then|else|select|case|end|for|while"
                + "|wend|do|loop|until|abs|sgn|hex|oct|sqr|int|fix|round"
                + "|log|split|cint|sin|cos|tan|len|mid|left|right|lcase|ucase"
                + "|trim|ltrim|rtrim|replace|instr|instrrev|space|string"
                + "|strreverse|cstr|clng|cbool|cdate|csng|cdbl|date|time|now"
                + "|dateadd|datediff|dateserial|datevalue|year|month|day|hour"
                + "|minute|second|timer|timeserial|timevalue|weekday|monthname"
                + "|array|asc|chr|filter|inputbox|join|msgbox|lbound|ubound"
                + "|redim|randomize|rnd|isempty|mod|execute|not|and|or|xor"
                + "|const|dim|erase"
                + @"|class(?!\s*=))\b", RegexOptions.IgnoreCase);
            hashTable.Add("vbs", regExp);
            #endregion

            // 添加JavaScript语言信息到散列表变量.   
            #region JavaScript语言
            regExp = new RegExp();
            regExp.Add("str", "\"[^\"\\n]*\"|'[^'\\n]*'", RegexOptions.None);
            regExp.Add("note", @"\/\/[^\n\r]*|\/\*[\s\S]*?\*\/", RegexOptions.None);
            regExp.Add("kw", @"\b(break|delete|function|return|typeof|case|do|if"
                + "|switch|var|catch|else|in|this|void|continue|false|nstanceof"
                + "|throw|while|debugger|finally|new|true|with|default|for|null"
                + "|try|abstract|double|goto|native|static|boolean|enum|implements"
                + "|package|super|byte|export|import|private|synchronized|char"
                + "|extends|int|protected|throws|final|interface|public|transient"
                + "|const|float|long|short|volatile"
                + @"|class(?!\s*=))\b", RegexOptions.None);
            hashTable.Add("js", regExp);
            #endregion

            // 添加SqlServer语言信息到散列表变量.   
            #region SqlServer语言
            regExp = new RegExp();
            regExp.Add("sqlstr", "'([^'\\n]*?)*'", RegexOptions.None);
            regExp.Add("note", @"--[^\n\r]*|\/\*[\s\S]*?\*\/", RegexOptions.None);
            regExp.Add("sqlconnect", @"\b(all|and|between|cross|exists|in|join|like"
                + "|not|null|outer|or)\b", RegexOptions.IgnoreCase);
            regExp.Add("sqlfunc", @"\b(avg|case|checksum|current_timestamp|day|left"
                + "|month|replace|year)\b", RegexOptions.IgnoreCase);
            regExp.Add("kw", @"\b(action|add|alter|after|as|asc|bigint|bit|binary|by"
                + "|cascade|char|character|check|column|columns|constraint|create"
                + "|current_date|current_time|database|date|datetime|dec|decimal"
                + "|default|delete|desc|distinct|double|drop|end|else|escape|file"
                + "|first|float|foreign|from|for|full|function|global|grant|group"
                + "|having|hour|ignore|index|inner|insert|int|integer|into|if|is"
                + "|key|kill|load|local|max|minute|modify|numeric|no|on|option|order"
                + "|partial|password|precision|primary|procedure|privileges"
                + "|read|real|references|restrict|returns|revoke|rows|second|select"
                + "|set|shutdown|smallint|table|temporary|text|then|time"
                + "|timestamp|tinyint|to|use|unique|update|values|varchar|varying"
                + @"|varbinary|with|when|where)\b", RegexOptions.IgnoreCase);
            hashTable.Add("sql", regExp);
            #endregion

            // 添加C#语言信息到散列表变量.   
            #region  C#语言
            regExp = new RegExp();
            regExp.Add("str", "\"[^\"\\n]*\"", RegexOptions.None);
            regExp.Add("note", @"\/\/[^\n\r]*|\/\*[\s\S]*?\*\/", RegexOptions.None);
            regExp.Add("Var", @"(?<=\bclass\s+)([_a-z][_a-z0-9]*)(?=\s*[\{:])"
                + @"|(?<=\=\s*new\s+)([a-z_][a-z0-9_]*)(?=\s*\()"
                + @"|([a-z][a-z0-9_]*)(?=\s+[a-z_][a-z0-9_]*\s*=\s*new)",
                RegexOptions.IgnoreCase);
            regExp.Add("kw", @"\b(partial|abstract|event|get|set|value|new|struct|as"
                + "|null|switch|base|object|this|bool|false|operator|throw|break"
                + "|finally|out|byte|fixed|override|try|case|float|params|typeof"
                + "|catch|for|private|uint|char|foreach|protected|ulong|checked"
                + "|goto|public|unchecked|if|readonly|unsafe|const|implicit|ref"
                + "|ushort|continue|in|return|using|decimal|int|sbyte|virtual"
                + "|default|interface|sealed|volatile|delegate|internal|short|void"
                + "|do|is|sizeof|while|double|lock|stackalloc|else|long|static"
                + @"|enum|string|namespace|region|endregion|class(?!\s*=))\b",
                RegexOptions.None);
            regExp.Add("kwG", @"\b(EventArgs|Page|Label|TextBox|CheckBox|DropDownList"
                + @"|Control|Button|DayRenderEventArgs|Color(?!\s*=))\b",
                RegexOptions.None);
            hashTable.Add("cs", regExp);
            #endregion

            // 添加VB.NET语言信息到散列表变量.
            #region  VB.NET语言
            regExp = new RegExp();
            regExp.Add("str", "\"[^\"\\n]*\"", RegexOptions.None);
            regExp.Add("note", @"'[^\n\r]*", RegexOptions.None);
            regExp.Add("Var", @"(?<=\bclass\s+)([_a-z][_a-z0-9]*)(?=\s*[\{:])"
                + @"|(?<=\=\s*new\s+)([a-z_][a-z0-9_]*)(?=\s*\()"
                + @"|([a-z][a-z0-9_]*)(?=\s+[a-z_][a-z0-9_]*\s*=\s*new)",
                RegexOptions.IgnoreCase);
            regExp.Add("kw", @"\b(AddHandler|AddressOf|AndAlso|Alias|And|Ansi|As"
                + "|Assembly|Auto|Boolean|Class|CLng|CObj|Const|Char|CInt|CDbl"
                + "|ByRef|Byte|ByVal|Call|Case|Catch|CBool|CByte|CChar|CDate|CDec"
                + "|CShort|CSng|CStr|CType|Date|Decimal|Declare|Default|Delegate"
                + "|Dim|DirectCast|Do|Double|Each|Else|ElseIf|End|Handles|If"
                + "|Enum|Erase|Error|Event|Exit|False|Finally|For|Friend|Function"
                + "|Get|GetType|GoTo|Let|Lib|Like|Long|Loop|Me|Mod|Module|Nothing"
                + "|Implements|Imports|In|Inherits|Integer|Interface|Is|Public"
                + "|MustInherit|MustOverride|MyBase|MyClass|Namespace|New|Next|Not"
                + "|NotInheritable|NotOverridable|Object|On|Option|Optional|Or|OrElse"
                + "|Overloads|Overridable|Overrides|ParamArray|Preserve|Private"
                + "|RaiseEvent|ReadOnly|ReDim|RemoveHandler|Resume|Return|Property"
                + "|Select|Set|Shadows|Shared|Short|Single|Static|Step|Stop|String"
                + "|Structure|Sub|SyncLock|Then|Throw|Protected|TypeOf|Unicode|Try"
                + "|To|True|Until|Variant|When|While|With|WithEvents"
                + @"|WriteOnly|Xor(?!\s*=))\b", RegexOptions.None);
            regExp.Add("kwG", @"\b(EventArgs|Page|Label|TextBox|CheckBox|DropDownList"
                + @"|Control|Button|Nullable|DayRenderEventArgs|Color(?!\s*=))\b",
                RegexOptions.None);
            hashTable.Add("vb", regExp);
            #endregion

            // 添加CSS语法信息到散列表变量.
            #region CSS语法
            regExp = new RegExp();
            regExp.Add("note", @"\/\*[\s\S]*?\*\/", RegexOptions.None);
            regExp.Add("str", @"([\s\S]+)", RegexOptions.None);
            regExp.Add("kw", @"(\{[^\}]+\})", RegexOptions.None);
            regExp.Add("sqlstr", @"([a-z\-]+(?=\s*:))", RegexOptions.IgnoreCase);
            regExp.Add("black", @"([\{\}])", RegexOptions.None);
            hashTable.Add("css", regExp);
            #endregion

            // 添加HTML语言信息到散列表变量.
            #region  HTML语言
            regExp = new RegExp();
            regExp.Add("", "<%@\\s*page[\\s\\S]*?language=['\"](.*?)[\"']",
                RegexOptions.IgnoreCase);
            regExp.Add("", @"<!--([\s\S]*?)-->", RegexOptions.None);
            regExp.Add("", @"(<script[^>]*>)([\s\S]*?)<\/script>",
                RegexOptions.IgnoreCase);
            regExp.Add("", @"<%(?!@)([\s\S]*?)%>", RegexOptions.None);
            regExp.Add("", @"<\?php\b([\s\S]*?)\?>", RegexOptions.IgnoreCase);
            regExp.Add("", @"(<style[^>]*>)([\s\S]*?)<\/style>",
                RegexOptions.IgnoreCase);
            regExp.Add("", @"&([a-z]+;)", RegexOptions.None);
            regExp.Add("", @"'.*?'", RegexOptions.None);
            regExp.Add("", "\".*?\"", RegexOptions.None);
            regExp.Add("", @"<([^>]+)>", RegexOptions.None);
            hashTable.Add("html", regExp);
            #endregion
            return hashTable;
        }

        /// <summary>
        /// 替换引号或者单引号中的尖括号.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string NoteBrackets(Match m)
        {
            return "<span class='gray'>"
                + m.Groups[0].Value.Replace("<", "&lt;").Replace(">", "&gt;")
                + "</span>";
        }

        /// <summary>
        /// 替换尖括号.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string RetrieveBrackets(Match m)
        {
            return m.Groups[0].Value.Replace("<", "&lt;").Replace(">", "&gt;");
        }


        /// <summary>
        /// 高亮HTML代码
        /// </summary>
        /// <param name="codeString">代码字符串</param>
        /// <param name="hashTable">正则表达式集合</param>
        /// <returns>已高亮的代码</returns>
        public static string HighlightHTMLCode(string codeString,
            Hashtable hashTable)
        {
            string lang = "VB";
            RegExp regExp = (RegExp)hashTable["html"];
            Regex regex = ((RegexStruct)regExp.regexStructList[0]).regex,
                htmlR = ((RegexStruct)regExp.regexStructList[9]).regex;
            Match match = regex.Match(codeString);
            MatchCollection mc;
            ArrayList note = new ArrayList(),
                vb = new ArrayList(),
                js = new ArrayList(),
                cs = new ArrayList(),
                css = new ArrayList();
            int blockIndex = 0;

            // 获取页面默认语言.
            if (match.Groups[1].Value.Trim() != "") lang
                = match.Groups[1].Value.ToUpper().Trim();
            if (lang != "C#") lang = "VB";

            #region 替换字符
            codeString = codeString.Replace("\\\"", "__CharactersQuotes__")
                .Replace("\\'", "__CharactersSingleQuote__");
            #endregion

            #region 脚本标签
            regex = ((RegexStruct)regExp.regexStructList[2]).regex;
            mc = regex.Matches(codeString);
            foreach (Match m in mc)
            {
                if (m.Groups[1].Value.ToLower().IndexOf("runat") == -1)
                {
                    // JavaScript标签.
                    if (m.Groups[1].Value.ToLower().
                        IndexOf("vbscript") == -1)
                    {
                        blockIndex = js.Count;
                        js.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                            (m.Groups[2].Value.Trim() != "" ?
                            HighlightCode(m.Groups[2].Value, "js",
                            (RegExp)hashTable["js"]) : "")
                           + "<span class=\"kw\">&lt;/"
                           + "<span class=\"str\">script</span>&gt;</span> ");
                        codeString = regex.Replace(codeString, 
                            "__JS" + blockIndex + "__", 1);
                    }
                    else
                    {
                        // VBscript标签.
                        blockIndex = vb.Count;
                        vb.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                            (m.Groups[2].Value.Trim() != "" ?
                            HighlightCode(m.Groups[2].Value, "vbs",
                            (RegExp)hashTable["vbs"]) : "")
                           + "<span class=\"kw\">&lt;/"
                           + "<span class=\"str\">script</span>&gt;</span> ");
                        codeString = regex.Replace(codeString, 
                            "__VB" + blockIndex + "__", 1);
                    }
                }
                else
                {
                    // C#语言标签.
                    if (lang == "C#")
                    {
                        if (m.Groups[1].Value.ToLower().IndexOf("vb") == -1)
                        {
                            blockIndex = cs.Count;
                            cs.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                                (m.Groups[2].Value.Trim() != "" ?
                                HighlightCode(m.Groups[2].Value, "cs",
                                (RegExp)hashTable["cs"]) : "")
                                + "<span class=\"kw\">&lt;/"
                                + "<span class=\"str\">script</span>&gt;</span> ");
                            codeString = regex.Replace(codeString,
                                "__C#" + blockIndex + "__", 1);
                        }
                        else
                        {
                            // VBScript语言标签.
                            blockIndex = vb.Count;
                            vb.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                                (m.Groups[2].Value.Trim() != "" ?
                                HighlightCode(m.Groups[2].Value, "vbs", 
                                (RegExp)hashTable["vbs"]) : "")
                                + "<span class=\"kw\">&lt;/"
                                + "<span class=\"str\">script</span>&gt;</span> ");
                            codeString = regex.Replace(codeString,
                                "__VB" + blockIndex + "__", 1);
                        }
                    }
                    else
                    {
                        if (m.Groups[1].Value.ToLower().IndexOf("c#") != -1)
                        {
                            blockIndex = cs.Count;
                            cs.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                                (m.Groups[2].Value.Trim() != "" ?
                                HighlightCode(m.Groups[2].Value, "cs", 
                                (RegExp)hashTable["cs"]) : "")
                                + "<span class=\"kw\">&lt;/"
                                + "<span class=\"str\">script</span>&gt;</span> ");
                            codeString = regex.Replace(codeString,
                                "__C#" + blockIndex + "__", 1);
                        }
                        else
                        {
                            blockIndex = vb.Count;
                            vb.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                                (m.Groups[2].Value.Trim() != "" ?
                                HighlightCode(m.Groups[2].Value, "vbs",
                                (RegExp)hashTable["vbs"]) : "")
                                + "<span class=\"kw\">&lt;/"
                                + "<span class=\"str\">script</span>&gt;</span> ");
                            codeString = regex.Replace(codeString,
                                "__VB" + blockIndex + "__", 1);
                        }
                    }

                }
            }
            #endregion

            #region 样式标签
            regex = ((RegexStruct)regExp.regexStructList[5]).regex;
            mc = regex.Matches(codeString);
            blockIndex = 0;
            foreach (Match m in mc)
            {
                css.Add(htmlR.Replace(m.Groups[1].Value, HTMLEval) +
                        (m.Groups[2].Value.Trim() != "" ?
                        HighlightCode(m.Groups[2].Value, "css", 
                        (RegExp)hashTable["css"]) : "")
                       + "<span class=\"kw\">&lt;/"
                       + "<span class=\"str\">style</span>&gt;</span> ");
                codeString = regex.Replace(codeString,
                    "__CSS" + blockIndex + "__", 1);
                blockIndex++;
            }
            #endregion

            #region 注释标签
            regex = ((RegexStruct)regExp.regexStructList[1]).regex;
            mc = regex.Matches(codeString);
            blockIndex = 0;
            foreach (Match m in mc)
            {
                note.Add("<span class='note'>&lt;!--"
                    + m.Groups[1].Value.Replace("<", "&lt;") + "--&gt;</span>");
                codeString = regex.Replace(codeString,
                    "__Comments" + blockIndex + "__", 1);
                blockIndex++;
            }
            #endregion

            #region <%%>标签中的代码
            regex = ((RegexStruct)regExp.regexStructList[3]).regex;
            mc = regex.Matches(codeString);
            foreach (Match m in mc)
            {
                if (lang == "VB")
                {
                    blockIndex = vb.Count;
                    vb.Add("<span class='declare'>&lt;%</span>" +
                            (m.Groups[1].Value.Trim() != "" ?
                            HighlightCode(m.Groups[1].Value, "vbs",
                            (RegExp)hashTable["vbs"]) : "")
                            + "<span class='declare'>%&gt;</span>");
                }
                else
                {
                    blockIndex = cs.Count;
                    cs.Add("<span class='declare'>&lt;%</span>" +
                            (m.Groups[1].Value.Trim() != "" ?
                            HighlightCode(m.Groups[1].Value, "cs",
                            (RegExp)hashTable["cs"]) : "")
                            + "<span class='declare'>%&gt;</span>");
                }
                codeString = regex.Replace(codeString,
                    "__" + lang + blockIndex + "__", 1);
            }
            #endregion

            #region 替换 '&' 字符
            codeString = ((RegexStruct)regExp.regexStructList[6]).regex
                .Replace(codeString, "&amp;$1");
            #endregion

            #region HTML标签
            codeString = ((RegexStruct)regExp.regexStructList[7]).regex
                .Replace(codeString, RetrieveBrackets);
            codeString = ((RegexStruct)regExp.regexStructList[8]).regex
                .Replace(codeString, RetrieveBrackets);
            codeString = htmlR.Replace(codeString, HTMLEval);
            #endregion

            #region 将字符串替换回原值.
            int i;

            // 注释.
            for (i = 0; i < note.Count; i++) codeString = codeString.Replace("__Comments"
                + i + "__", note[i].ToString());
            codeString = codeString.Replace("__CharactersQuotes__", "\\\"").
                Replace("__CharactersSingleQuote__", "\\'");

            // CSS.
            for (i = 0; i < css.Count; i++) codeString = codeString.Replace("__CSS"
                + i + "__", css[i].ToString());

            // C#语言.
            for (i = 0; i < cs.Count; i++) codeString = codeString.Replace("__C#"
                + i + "__", cs[i].ToString());

            // VBScript语言或者vb语言.
            for (i = 0; i < vb.Count; i++) codeString = codeString.Replace("__VB"
                + i + "__", vb[i].ToString());

            // Javascript语言.
            for (i = 0; i < js.Count; i++) codeString = codeString.Replace("__JS"
                + i + "__", js[i].ToString());
            #endregion

            return codeString;
        }


        /// <summary>
        /// 根据语言高亮代码(除了HTML语言).
        /// </summary>
        /// <param name="codeString">代码字符串</param>
        /// <param name="language">代码语言</param>
        /// <param name="regExp">正则表达式对象</param>
        /// <returns>以高亮代码</returns>
        public static string HighlightCode(string codeString, 
            string language, RegExp regExp)
        {

            language = language.ToLower();
            codeString = codeString.Replace("<!--", "&lt;!--");
            RegexStruct regexStruct;
            ArrayList styleString = new ArrayList(),
                note = new ArrayList(),
                xmlnote = new ArrayList();
            MatchCollection mc;
            int blockIndex = 0;

            #region 替换字符
            if (language != "css")
                codeString = codeString.Replace("\\\"", "__CharactersQuotes__")
                .Replace("\\'", "__CharactersSingleQuote__");
            #endregion

            #region 替换字符串
            if (language != "css")
            {
                regexStruct = (RegexStruct)regExp.regexStructList[0];
                mc = regexStruct.regex.Matches(codeString);
                foreach (Match m in mc)
                {
                    styleString.Add("<span class='" + regexStruct.styleObject + "'>"
                        + m.Groups[0].Value.Replace("<", "&lt;")
                        + "</span>");
                    codeString = regexStruct.regex.Replace(codeString, 
                        "__StringVariables"+ blockIndex + "__", 1);
                    blockIndex++;
                }
            }
            #endregion

            #region 替换C#语言XML注释
            blockIndex = 0;
            if (language == "cs")
            {
                Regex regex = new Regex(@"((?<!/)///(?!/))([^\r\n]*)?"),
                    attri = new Regex(@"(<[^>]+>)");
                mc = regex.Matches(codeString);
                string tmp = "";
                foreach (Match m in mc)
                {
                    tmp = m.Groups[2].Value;
                    tmp = attri.Replace(tmp, NoteBrackets);
                    xmlnote.Add("<span class='note'>"
                        + "<span class='gray'>///</span>"
                        + tmp + "</span>");
                    codeString = regex.Replace(codeString,
                        "__XMLComments" + blockIndex + "__", 1);
                    blockIndex++;
                }
            }
            #endregion

            #region 替换注释
            regexStruct = (RegexStruct)regExp.regexStructList[language == "css" ? 0 : 1];
            mc = regexStruct.regex.Matches(codeString);
            blockIndex = 0;
            foreach (Match m in mc)
            {
                note.Add("<span class='" + regexStruct.styleObject + "'>"
                    + m.Groups[0].Value.Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    + "</span>");
                codeString = regexStruct.regex.Replace(codeString, 
                    "__Comments" + blockIndex + "__", 1);
                blockIndex++;
            }
            #endregion

            #region 其他替换
            int i = language == "css" ? 1 : 2;
            for (; i < regExp.regexStructList.Count; i++)
            {
                regexStruct = (RegexStruct)regExp.regexStructList[i];
                if (language == "cs" && regexStruct.styleObject == "Var") 
                    codeString = regexStruct.regex.Replace(codeString, 
                        "<span class='Var'>$1$2$3</span>");
                else
                    codeString = regexStruct.regex.Replace(codeString, "<span class='"
                        + regexStruct.styleObject+ "'>$1</span>");
            }
            #endregion

            #region 将字符串替换回原值.
            if (language != "css") for (i = 0; i < styleString.Count; i++)
                    codeString = codeString.Replace("__StringVariables" +
                i + "__", styleString[i].ToString());
            if (language == "cs") for (i = 0; i < xmlnote.Count; i++)
                    codeString = codeString.Replace("__XMLComments" +
                i + "__", xmlnote[i].ToString());
            for (i = 0; i < note.Count; i++)
                codeString = codeString.Replace("__Comments"
                + i + "__", note[i].ToString());
            if (language != "css")
            {
                // 替换包含注释的字符串.
                if (codeString.IndexOf("__XMLComments") != -1)
                    for (i = 0; i < styleString.Count; i++)
                        for (i = 0; i < xmlnote.Count; i++)
                            codeString = codeString.Replace("__XMLComments" + i
                                + "__",ClearHTMLTag(xmlnote[i].ToString()));
                if (codeString.IndexOf("__Comments") != -1)
                    for (i = 0; i < styleString.Count; i++)
                        for (i = 0; i < note.Count; i++)
                            codeString = codeString.Replace("__Comments" + i
                                + "__",ClearHTMLTag(note[i].ToString()));
                if (codeString.IndexOf("__StringVariables") != -1)
                    for (i = 0; i < styleString.Count; i++)
                        codeString = codeString.Replace("__StringVariables" + i
                            + "__", ClearHTMLTag(styleString[i].ToString()));

                if (codeString.IndexOf("__XMLComments") != -1)
                    for (i = 0; i < xmlnote.Count; i++)
                        codeString = codeString.Replace("__XMLComments" + i
                            + "__", xmlnote[i].ToString());
                codeString = codeString.Replace("__CharactersQuotes__", "\\\"")
                    .Replace("__CharactersSingleQuote__", "\\'");
            }
            #endregion
            return codeString;
        }


        /// <summary>
        /// 清除HTML标签.
        /// </summary>
        public static string ClearHTMLTag(string htmlString)
        {
            // 清除脚本标签.
            htmlString = Regex.Replace(htmlString,
                @"<script[^>]*?>.*?</script>",
                "", RegexOptions.IgnoreCase);

            // 清除HTML标签.
            htmlString = Regex.Replace(htmlString, @"<(.[^>]*)>",
                "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"([\r\n])[\s]+",
                "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"-->",
                "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*",
                "", RegexOptions.IgnoreCase);

            htmlString = Regex.Replace(htmlString, @"&(quot|#34);",
                "\"", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(amp|#38);",
                "&", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(lt|#60);",
                "<", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(gt|#62);",
                ">", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);",
                " ", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);",
                "\xa1", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(cent|#162);",
                "\xa2", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(pound|#163);",
                "\xa3", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(copy|#169);",
                "\xa9", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);",
                "", RegexOptions.IgnoreCase);

            htmlString.Replace("<", "");
            htmlString.Replace(">", "");
            htmlString.Replace("\r\n", "");
            htmlString = HttpContext.Current.Server
                .HtmlEncode(htmlString).Trim();
            return htmlString;
        }

        /// <summary>
        /// 替换HTML标签属性.
        /// </summary>
        /// <param name="m">匹配集合</param>
        /// <returns>已替换的代码</returns>
        private static string HTMLEval(Match m)
        {
            string tmp = m.Groups[1].Value;
            if (tmp.StartsWith("/"))
                return "<span class='kw'>&lt;/<span class='str'>"
                    + tmp.Substring(1) + "</span>&gt;</span>";
            else if (new Regex(@"^([_0-9a-z]+)\s*\/$",
                RegexOptions.IgnoreCase).IsMatch(tmp))
                return "<span class='kw'>&lt;<span class='str'>"
                    + tmp.Substring(0, tmp.Length - 1)
                    + "</span>&gt;</span>";
            else if (tmp.ToLower().StartsWith("!doctype"))
            {
                tmp = "<span class='kw'>" + m.Groups[0].Value.Substring(1)
                    + "</span>";
                tmp = new Regex(@"\b(html|public)\b",
                    RegexOptions.IgnoreCase).Replace(tmp, 
                    "<span class='sqlstr'>$1</span>");
                return "<span class='kw'>&lt;!" + tmp + "&gt;</span>";
            }
            else
            {
                Regex regex = new Regex("([a-z_][a-z_0-9\\.\\-]*)\\s*=\\s*\"([^\"]*)\"",
                    RegexOptions.IgnoreCase);
                tmp = regex.Replace(tmp,
                    "<span class=\"sqlstr\">$1</span><span class=\"kw\">=\"$2\"</span>");
                regex = new Regex("([a-z_][a-z_0-9\\.\\-]*)\\s*=\\s*'([^']*)'",
                    RegexOptions.IgnoreCase);
                tmp = regex.Replace(tmp,
                    "<span class=\"sqlstr\">$1</span><span class=\"kw\">='$2'</span>");
                regex = new Regex("([a-z_][a-z_0-9\\-]*)\\s*=\\s*(?!['\"])(\\w+)",
                    RegexOptions.IgnoreCase);
                tmp = regex.Replace(tmp,
                    "<span class=\"sqlstr\">$1</span><span class=\"kw\">=$2</span>");

                regex = new Regex(@"^([a-z_0-9\-]+)", RegexOptions.IgnoreCase);
                tmp = regex.Replace(tmp, "<span class='str'>$1</span>");
                if (tmp.StartsWith("%@"))
                    return "<span class='str'><span class='declare'>&lt;%</span>"
                        + "<span class='kw'>@</span>"
                        + tmp.Trim(new char[] { '%', '@' })
                        + "<span class='declare'>%&gt;</span></span>";
                return "<span class='kw'>&lt;" + tmp + "&gt;</span>";
            }
        }
        /// <summary>
        /// 将\r, \n 替换为 <br/>.
        /// </summary>
        /// <param name="codeString"></param>
        /// <returns></returns>
        public static string Encode(string codeString)
        {
            codeString = codeString.Replace("\r", "").Replace("\n", "<br/>");
            return Regex.Replace(codeString, "(?<!<span)( +)(?!class)", 
                GetSpace, RegexOptions.Compiled);
        }

        /// <summary>
        /// 将空格替换为&nbsp;.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string GetSpace(Match m)
        {
            return m.Groups[1].Value.Replace(" ", "&nbsp;");
        }
    }
}