/********************************* 模块头 **********************************\
* 模块名:                ExportConfigControlAttribute.cs
* 项目:                  ConfigControl.Contract
* Copyright (c) Microsoft Corporation.
* 
* ExportConfigControlAttribute
* IConfigAttributes
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace ConfigControl.Contract
{
    /// <summary>
    /// 输出 IConfigControl 类型特性
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportConfigControlAttribute : ExportAttribute
    {
        public ExportConfigControlAttribute()
            : base(typeof(IConfigControl))
        { }
        public string Name { set; get; }
        public Type PropertyValueType { set; get; }
    }

    public interface IConfigAttributes
    {
        string Name { get; }
        Type PropertyValueType { get; }
    }
}
