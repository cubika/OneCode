/****************************** Module Header ******************************\
* 模块名称:  INTERNET_OPTION.cs
* 项目名称:	CSWebBrowserWithProxy
* Copyright (c) Microsoft Corporation.
* 
* 以下枚举包含4个在InternetQueryOption和InternetSetOption中要使用的WinINet常量。
* 你将在 http://msdn.microsoft.com/en-us/library/aa385328(VS.85).aspx 中了解到
* 所有的常量。
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

namespace CSWebBrowserWithProxy
{
    public enum INTERNET_OPTION
    {
        //设置一个INTERNET_PER_CONN_OPTION_LIST数据结构来确定一个特定连接的详细特征。
        INTERNET_OPTION_PER_CONNECTION_OPTION = 75,

        //通知系统设置的改变，使系统能在下次正确的使用这些设置接入Internet。
        INTERNET_OPTION_SETTINGS_CHANGED = 39,

        //通过一个注册一个句柄，使代理数据能被重读。
        INTERNET_OPTION_REFRESH = 37

    }
}
