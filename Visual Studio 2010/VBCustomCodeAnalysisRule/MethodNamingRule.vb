'*************************** Module Header ******************************'
' 模块名:  MethodNamingRule.vb
' 项目名:  VBCustomCodeAnalysisRule
' 版权(c)  Microsoft Corporation.
' 
' MethodNamingRule类 类继承了Microsoft.FxCop.Sdk.BaseIntrospectionRule类，重写了方法
' and override the method 
'     public ProblemCollection Check(Member member).
' 
' 这个规则是用来检验方法的名字是不是以大写字符开始的.
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

Friend NotInheritable Class MethodNamingRule
    Inherits BaseIntrospectionRule
    ''' <summary>
    ''' 定义这个规则名称，资源文件和资源程序集
    ''' </summary>
    Public Sub New()
        MyBase.New("MethodNamingRule", "VBCustomCodeAnalysisRule.Rules",
                   GetType(MethodNamingRule).Assembly)

    End Sub

    ''' <summary>
    ''' 如果他是一个方法，检查成员的名称
    '''如果这个方法不是一个构造方法或者一个访问器，他的名字应该以一个大写字符开始。
    ''' </summary>
    Public Overrides Function Check(ByVal memb As Member) _
        As ProblemCollection
        If TypeOf memb Is Method _
            AndAlso Not (TypeOf memb Is InstanceInitializer) _
            AndAlso Not (TypeOf memb Is StaticInitializer) Then

            Dim mthd As Method = TryCast(memb, Method)

            If (Not mthd.IsAccessor) _
                AndAlso (mthd.Name.Name(0) < "A"c _
                         OrElse mthd.Name.Name(0) > "Z"c) Then
                Me.Problems.Add(New Problem(Me.GetNamedResolution(
                                            "UppercaseMethod",
                                            mthd.Name.Name,
                                            mthd.DeclaringType.FullName)))
            End If
        End If
        Return Me.Problems
    End Function


End Class
