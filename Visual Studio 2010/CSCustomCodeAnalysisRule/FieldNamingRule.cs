/****************************** Module Header ******************************\
* 模块名:  FieldNamingRule.cs
* 项目名:  CSCustomCodeAnalysisRule
* 版权(c)  Microsoft Corporation.
* 
*FieldNamingRule类继承了Microsoft.FxCop.Sdk.BaseIntrospectionRule类，重写了方法
*     public ProblemCollection Check(Member member).
* 
* 这个规则是用来检验字段的名字是不是以小写字符开始的。
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using Microsoft.FxCop.Sdk;

namespace CSCustomCodeAnalysisRule
{
    sealed class FieldNamingRule : BaseIntrospectionRule
    {
        /// <summary>
        /// 定义这个规则名称，资源文件和资源程序集
        /// </summary>
        public FieldNamingRule()
            : base(
                "FieldNamingRule",
                "CSCustomCodeAnalysisRule.Rules",
                typeof(FieldNamingRule).Assembly)
        {

        }

        /// <summary>
        ///如果他是一个字段，检查成员的名称
        /// 如果这个字段不是一个事件或者一个静态成员，他的名字应该以一个小写字符开始。
        /// </summary>
        public override ProblemCollection Check(Member member)
        {
            if (member is Field)
            {
                Field field = member as Field;

                if (!(field.Type is DelegateNode)
                    && !field.IsStatic)
                {
                    if (field.Name.Name[0] < 'a' || field.Name.Name[0] > 'z')
                    {
                        this.Problems.Add(new Problem(
                           this.GetNamedResolution(
                           "LowercaseField", 
                           field.Name.Name, 
                           field.DeclaringType.FullName)));
                    }
                }

            }

            return this.Problems;
        }

    }
}
