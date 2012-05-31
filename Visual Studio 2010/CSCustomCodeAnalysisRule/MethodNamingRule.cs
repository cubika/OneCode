/****************************** Module Header ******************************\
* 模块名:  MethodNamingRule.cs
* 项目名:  CSCustomCodeAnalysisRule
* 版权(c)  Microsoft Corporation.
* 
* MethodNamingRule类 类继承了Microsoft.FxCop.Sdk.BaseIntrospectionRule类，重写了方法
*     public ProblemCollection Check(Member member).
* 
* 这个规则是用来检验方法的名字是不是以大写字符开始的.
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
    sealed class MethodNamingRule : BaseIntrospectionRule
    {
        /// <summary>
        /// 定义这个规则名称，资源文件和资源程序集
        /// </summary>
        public MethodNamingRule()
            : base(
                "MethodNamingRule",
                "CSCustomCodeAnalysisRule.Rules",
                typeof(MethodNamingRule).Assembly)
        {

        }

        /// <summary>
        /// 如果他是一个方法，检查成员的名称
        /// 如果这个方法不是一个构造方法或者一个访问器，他的名字应该以一个大写字符开始。
        /// </summary>
        public override ProblemCollection Check(Member member)
        {         
            if (member is Method
                && !(member is InstanceInitializer)
                && !(member is StaticInitializer))
            {
                Method method = member as Method;
                if (!method.IsAccessor
                    && (method.Name.Name[0] < 'A' || method.Name.Name[0] > 'Z'))
                {
                    this.Problems.Add(new Problem(
                       this.GetNamedResolution(
                       "UppercaseMethod",
                       method.Name.Name,
                       method.DeclaringType.FullName)));
                }
            }
            return this.Problems;
        }


    }
}
