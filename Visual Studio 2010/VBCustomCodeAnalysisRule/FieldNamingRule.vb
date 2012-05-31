'*************************** Module Header ******************************'
' 模块名:  FieldNamingRule.vb
' 项目名:  VBCustomCodeAnalysisRule
' 版权(c)  Microsoft Corporation.
' 
' FieldNamingRule类继承了Microsoft.FxCop.Sdk.BaseIntrospectionRule类，重写了方法
'     public ProblemCollection Check(Member member).
' 
' 这个规则是用来检验字段的名字是不是以小写字符开始的。
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

Friend NotInheritable Class FieldNamingRule
    Inherits BaseIntrospectionRule
    ''' <summary>
    ''' 定义这个规则名称，资源文件和资源程序集
    ''' </summary>
    Public Sub New()
        MyBase.New("FieldNamingRule", "VBCustomCodeAnalysisRule.Rules",
                   GetType(FieldNamingRule).Assembly)

    End Sub

    ''' <summary>
    ''' 如果他是一个字段，检查成员的名称
    ''' 如果这个字段不是一个事件或者一个静态成员，他的名字应该以一个小写字符开始。
    ''' </summary>
    Public Overrides Function Check(ByVal memb As Member) _
        As ProblemCollection
        If TypeOf memb Is Field Then
            Dim fld As Field = TryCast(memb, Field)

            If Not (TypeOf fld.Type Is DelegateNode) _
                AndAlso (Not fld.IsStatic) Then

                If fld.Name.Name(0) < "a"c _
                    OrElse fld.Name.Name(0) > "z"c Then

                    Me.Problems.Add(New Problem(
                                    Me.GetNamedResolution("LowercaseField",
                                                          fld.Name.Name,
                                                          fld.DeclaringType.FullName)))
                End If
            End If

        End If

        Return Me.Problems
    End Function

End Class

