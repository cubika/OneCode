'*************************** Module Header ******************************'
' 模块名:  PropertyNamingRule.vb
' 项目名:  VBCustomCodeAnalysisRule
' 版权(c)  Microsoft Corporation.
' 
' PropertyNamingRule类类继承了Microsoft.FxCop.Sdk.BaseIntrospectionRule类，重写了方法
'     public ProblemCollection Check(Member member).
' 
' 这个规则是用来检验属性的名字是不是以大写字符开始的。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************'

Imports Microsoft.FxCop.Sdk

Friend NotInheritable Class PropertyNamingRule
    Inherits BaseIntrospectionRule
    ''' <summary>
    ''' 定义这个规则名称，资源文件和资源程序集
    ''' </summary>
    Public Sub New()
        MyBase.New("PropertyNamingRule", "VBCustomCodeAnalysisRule.Rules",
                   GetType(PropertyNamingRule).Assembly)

    End Sub


    ''' <summary>
    '''如果他是一个方法，检查成员的名称
    ''' 这个属性的名称以一个大写字符开始。
    ''' </summary>
    Public Overrides Function Check(ByVal memb As Member) _
        As ProblemCollection
        If TypeOf memb Is PropertyNode Then
            Dim p As PropertyNode = TryCast(memb, PropertyNode)

            If p.Name.Name(0) < "A"c _
                OrElse p.Name.Name(0) > "Z"c Then
                Me.Problems.Add(New Problem(Me.GetNamedResolution(
                                            "UppercaseProperty",
                                            p.Name.Name,
                                            p.DeclaringType.FullName)))
            End If
        End If

        Return Me.Problems
    End Function


End Class
