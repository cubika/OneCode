/****************************** Module Header ******************************\
* 模块名:  PropertyNamingRule.cs
* 项目名:	    CSCustomCodeAnalysisRule
* 版权(c) Microsoft Corporation.
* 
* PropertyNamingRule类类继承了Microsoft.FxCop.Sdk.BaseIntrospectionRule类，重写了方法
*     public ProblemCollection Check(Member member).
* 
* 这个规则是用来检验属性的名字是不是以大写字符开始的。
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
    sealed class PropertyNamingRule : BaseIntrospectionRule
    {
        /// <summary>
        /// 定义这个规则名称，资源文件和资源程序集
        /// </summary>
        public PropertyNamingRule()
            : base(
                "PropertyNamingRule",
                "CSCustomCodeAnalysisRule.Rules",
                typeof(FieldNamingRule).Assembly)
        {

        }

        /// <summary>
        /// 如果他是一个方法，检查成员的名称
        /// 这个属性的名称以一个大写字符开始。
        /// </summary>
        public override ProblemCollection Check(Member member)
        {         
            if (member is PropertyNode)
            {
                PropertyNode property = member as PropertyNode;

                if (property.Name.Name[0] < 'A' || property.Name.Name[0] > 'Z')
                {
                    this.Problems.Add(new Problem(
                       this.GetNamedResolution(
                       "UppercaseProperty", 
                       property.Name.Name,
                       property.DeclaringType.FullName)));
                }
            }

            return this.Problems;
        }


    }
}
