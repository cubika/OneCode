/****************************** 模块 标识 ******************************\
* 模块名:    ScoreCartForSchoolLinqToEntities.cs
* 项目:      CSADONETDataServiceSL3Client
* 版权       (c) Microsoft Corporation.
* 
* ScoreCardForSchoolLinqToEntities.cs 展示了如何用创建一个类来为 UI 作数据源。
* 它是用来连接自动生成的 ADO.NET Data Service客户端代码所获得数据以及 UI 的桥梁。
* 这里还使用了一些小技巧，例如，Display 以及 Editable 属性的使用。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
\***************************************************************************/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CSADONETDataServiceSL3Client.SchoolLinqToEntitiesService;

namespace CSADONETDataServiceSL3Client
{
    // ScoreCardForSchoolLinqToEntities 对象的集合就是 DataGrid 控件的数据源。
    public class ScoreCardForSchoolLinqToEntities 
    {   
        // 为了进行更新操作，维护一个CourseGrade对象的引用
        [Display(AutoGenerateField = false)]
        public CourseGrade CourseGrade
        { 
            get; 
            set; 
        }
        [Editable(false)]
        public string PersonName
        {
            get;
            set;
        }
        [Editable(false)]
        public string Course
        {
            get;
            set;
        }
        private decimal? _grade;
        public decimal? Grade
        {
            get { return _grade; }
            set {
                // 当Grade被改变时，更新内存中的CourseGrade数据
                if (CourseGrade != null) 
                {
                CourseGrade.Grade = value;
                }
            _grade = value;
            }
        }

       
    }
}
