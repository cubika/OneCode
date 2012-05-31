/******************************* 模块头 *********************************\
模块名:     <Person.cs>
项目名:     <CSASPNETMVCDataView>
版权 (c) Microsoft Corporation.

个人信息模块模型文件.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CSASPNETMVCDataView.Models
{
    public class Person
    {
        [Required(ErrorMessage = "请填写ID.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "请填写姓名.")]
        public string Name { get; set; }

        [Range(1, 200, ErrorMessage = "1到200之间的数字.")]
        public int Age { get; set; }

        [RegularExpression(@"(^189\d{8}$)|(^13\d{9}$)|(^15\d{9}$)", 
            ErrorMessage = "无效电话号码. 以13,15,189开头11位数字. 例. 13800138000")]
        public string Phone { get; set; }

        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", 
            ErrorMessage = "无效电子邮件地址.")]
        public string Email { get; set; }
    }
}
