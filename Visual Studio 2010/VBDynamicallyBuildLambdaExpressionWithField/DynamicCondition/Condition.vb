'******************************** 模块头 *************************************************'
' 模块名:   Condition.vb
' 项目名:  VBDynamicallyBuildLambdaExpressionWithField
' 版权 (c) Microsoft Corporation.
'
' Condition.vb 文件定义了一个与 System.Linq.Expressions 命名空间相关的 LINQ 运算符.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************************************'

Imports System.Linq.Expressions

Public Module DynamicQuery
#Region "Condition Classes"

    Public MustInherit Class Condition

#Region "Fields"

        ' 用来确保通过多个查询,得到了特定的 ParameterExpression 的同一实例.
        Private Shared paramTable As New Dictionary(Of String, ParameterExpression)()

        ' 将在运行时,传递到 LINQ to SQL 的表达式树.
        Protected Friend LambdaExpr As LambdaExpression

        ' 枚举所有可以执行的不同比较.
        Public Enum Compare As Integer
            [Or] = ExpressionType.Or
            [And] = ExpressionType.And
            [Xor] = ExpressionType.ExclusiveOr
            [Not] = ExpressionType.Not
            Equal = ExpressionType.Equal
            [Like] = ExpressionType.TypeIs + 1
            NotEqual = ExpressionType.NotEqual
            [OrElse] = ExpressionType.OrElse
            [AndAlso] = ExpressionType.AndAlso
            LessThan = ExpressionType.LessThan
            GreaterThan = ExpressionType.GreaterThan
            LessThanOrEqual = ExpressionType.LessThanOrEqual
            GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual
        End Enum
#End Region

        ''' <summary>
        ''' 构造一个Condition，用T表示元素类型，S表示值类型.
        ''' </summary>
        Public Shared Function Create(Of T, S)(ByVal dataSource As IEnumerable(Of T), ByVal propertyName As String, ByVal condType As Compare, ByVal value As S) As Condition(Of T, S)

            Return New Condition(Of T, S)(propertyName, condType, value)
        End Function

        ''' <summary>
        ''' 构造一个Condition，用T表示元素类型，S表示值类型.
        ''' 在运行时才能知道值类型的情况下，这是很有用的.
        ''' </summary>
        Public Shared Function Create(Of T)(ByVal dataSource As IEnumerable(Of T), ByVal propertyName As String, ByVal condType As Compare, ByVal value As Object, ByVal valueType As Type) As Condition(Of T)

            Return New Condition(Of T)(propertyName, condType, value, valueType)
        End Function


        ''' <summary>
        ''' 创造一个由两个其他条件组合而成的条件.
        ''' </summary>
        ''' <typeparam name="T"> 条件针对时执行的类型</typeparam>
        ''' <param name="cond1">第一个条件</param>
        ''' <param name="condType">使用在条件上的运算符</param>
        ''' <param name="cond2">第二个条件</param>
        ''' <returns>一个新的条件,是根据指定的运算符，将两个条件组合而得</returns>
        ''' <remarks></remarks>
        Public Shared Function Combine(Of T)(ByVal cond1 As Condition(Of T), ByVal condType As Compare, ByVal cond2 As Condition(Of T)) As Condition(Of T)
            Return Condition(Of T).Combine(cond1, condType, cond2)
        End Function

        ''' <summary>
        ''' 根据指定的运算符，组合多个条件.
        ''' </summary>
        Public Shared Function Combine(Of T)(ByVal cond1 As Condition(Of T), ByVal condType As Compare, ByVal ParamArray conditions() As Condition(Of T)) As Condition(Of T)
            Return Condition(Of T).Combine(cond1, condType, conditions)
        End Function


#Region "Protected Methods"
        ''' <summary>
        ''' 根据指定的运算符（condType），将两个表达式组合起来.
        ''' </summary>
        Protected Shared Function CombineExpression(ByVal left As Expression, ByVal condType As Compare, ByVal right As Expression) As Expression

            '基于运算符，连结表达式.
            Select Case condType
                Case Compare.Or
                    Return Expression.Or(left, right)
                Case Compare.And
                    Return Expression.And(left, right)
                Case Compare.Xor
                    Return Expression.ExclusiveOr(left, right)
                Case Compare.Equal
                    Return Expression.Equal(left, right)
                Case Compare.OrElse
                    Return Expression.OrElse(left, right)
                Case Compare.AndAlso
                    Return Expression.AndAlso(left, right)
                Case Compare.NotEqual
                    Return Expression.NotEqual(left, right)
                Case Compare.LessThan
                    Return Expression.LessThan(left, right)
                Case Compare.GreaterThan
                    Return Expression.GreaterThan(left, right)
                Case Compare.LessThanOrEqual
                    Return Expression.LessThanOrEqual(left, right)
                Case Compare.GreaterThanOrEqual
                    Return Expression.GreaterThanOrEqual(left, right)
                Case Compare.Like

                    '对于 Like 运算符，我们对 VB 运行时中的 LikeString 方法调用进行了编码. 
                    Dim m = GetType(CompilerServices.Operators).GetMethod("LikeString")

                    Return Expression.Call(m, left, right, Expression.Constant(CompareMethod.Binary))

                Case Else
                    Throw New ArgumentException("Not a valid Condition Type", "condType", Nothing)
            End Select
        End Function

        ''' <summary>
        ''' 由于这两个的类型参数必须相同，我们可以将通常为Func(Of T, T, Boolean)的，转换为Func(Of T, Boolean).
        ''' </summary>
        Protected Shared Function CombineFunc(Of T)(ByVal d1 As Func(Of T, Boolean), ByVal condType As Compare, ByVal d2 As Func(Of T, Boolean)) As Func(Of T, Boolean)

            '返回组合 d1 和 d2 委托的一个委托.
            Select Case condType
                Case Compare.Or
                    Return Function(x) d1(x) Or d2(x)
                Case Compare.And
                    Return Function(x) d1(x) And d2(x)
                Case Compare.Xor
                    Return Function(x) d1(x) Xor d2(x)
                Case Compare.Equal
                    Return Function(x) d1(x) = d2(x)
                Case Compare.OrElse
                    Return Function(x) d1(x) OrElse d2(x)
                Case Compare.AndAlso
                    Return Function(x) d1(x) AndAlso d2(x)
                Case Compare.NotEqual
                    Return Function(x) d1(x) <> d2(x)
                Case Compare.LessThan
                    Return Function(x) Integer.Parse(d1(x).ToString()) < Integer.Parse(d2(x).ToString())
                Case Compare.GreaterThan
                    Return Function(x) Integer.Parse(d1(x).ToString()) > Integer.Parse(d2(x).ToString())
                Case Compare.LessThanOrEqual
                    Return Function(x) Integer.Parse(d1(x).ToString()) <= Integer.Parse(d2(x).ToString())
                Case Compare.GreaterThanOrEqual
                    Return Function(x) Integer.Parse(d1(x).ToString()) >= Integer.Parse(d2(x).ToString())
                Case Else
                    Throw New ArgumentException("Not a valid Condition Type", "condType")
            End Select
        End Function

        ''' <summary>
        ''' 确保，对于给定的类型 t ,我们得到了 ParameterExpression 的同一实例.
        ''' </summary>
        Protected Shared Function GetParamInstance(ByVal dataType As Type) As ParameterExpression

            '参数匹配是按引用，而不是按名称，所以我们将该实例缓存到一个字典中.
            If Not (paramTable.ContainsKey(dataType.Name)) Then
                paramTable.Add(dataType.Name, Expression.Parameter(dataType, dataType.Name))
            End If

            Return paramTable(dataType.Name)
        End Function
#End Region

    End Class

    Public Class Condition(Of T)
        Inherits Condition

        ' 包含编译好的表达式树，可以在本地运行的委托.
        Friend del As Func(Of T, Boolean)

#Region "Constructors"

        Friend Sub New()
        End Sub

        Friend Sub New(ByVal propName As String, ByVal condType As Compare, ByVal value As Object, ByVal valueType As Type)
            ' 拆分数组，以处理处理嵌套属性的访问.
            Dim s = propName.Split("."c)

            ' 为 propName 获得 PropertyInfo 实例.
            Dim pInfo = GetType(T).GetProperty(s(0))
            Dim paramExpr = GetParamInstance(GetType(T))
            Dim callExpr = Expression.MakeMemberAccess(paramExpr, pInfo)

            ' 为每一个指定的成员，构造一个附加的 MemberAccessExpression.
            ' 例如，如果用户表示 "myCustomer.Order.OrderID = 4"，为此我
            ' 们需要一个附加的 MemberAccessExpression."
            For i = 1 To s.GetUpperBound(0)
                pInfo = pInfo.PropertyType.GetProperty(s(i))
                callExpr = Expression.MakeMemberAccess(callExpr, pInfo)
            Next i

            'ConstantExpression,表示运算符左边的值.
            Dim valueExpr = Expression.Constant(value, valueType)

            Dim b As Expression = CombineExpression(callExpr, condType, valueExpr)
            LambdaExpr = Expression.Lambda(Of Func(Of T, Boolean))(b, New ParameterExpression() {paramExpr})

            ' 将 lambda 表达式编译到一个委托中.
            del = CType(LambdaExpr.Compile(), Func(Of T, Boolean))
        End Sub

#End Region

#Region "Methods"

        ' 根据指定的运算符，组合两个条件.
        Friend Shared Function Combine(ByVal cond1 As Condition(Of T), ByVal condType As Compare, ByVal cond2 As Condition(Of T)) As Condition(Of T)
            Dim c As New Condition(Of T)()

            Dim b As Expression = CombineExpression(cond1.LambdaExpr.Body, condType, cond2.LambdaExpr.Body)

            Dim paramExpr = New ParameterExpression() {GetParamInstance(GetType(T))}

            ' 构建 Lambda 表达式，编译委托.
            c.LambdaExpr = Expression.Lambda(Of Func(Of T, Boolean))(b, paramExpr)
            c.del = Condition.CombineFunc(cond1.del, condType, cond2.del)

            Return c
        End Function

        ' 根据指定的运算符，组合多个条件.
        Friend Shared Function Combine(ByVal cond1 As Condition(Of T), ByVal condType As Compare, ByVal ParamArray conditions() As Condition(Of T)) As Condition(Of T)
            Dim finalCond = cond1
            For Each c In conditions
                finalCond = Condition.Combine(finalCond, condType, c)
            Next c

            Return finalCond
        End Function

        ' 在本地，而不是远程，运行查询.
        Public Function Matches(ByVal row As T) As Boolean
            Return del(row) ' 将行传递到委托,看是否匹配.
        End Function

#End Region

#Region "Overloaded Operators"

        ' 运算符重载 - 允许像"(condition1 Or condition2) And condition3"这样的语法.
        Public Shared Operator And(ByVal c1 As Condition(Of T), ByVal c2 As Condition(Of T)) As Condition(Of T)
            Return Condition.Combine(c1, Compare.And, c2)
        End Operator

        Public Shared Operator Or(ByVal c1 As Condition(Of T), ByVal c2 As Condition(Of T)) As Condition(Of T)
            Return Condition.Combine(c1, Compare.Or, c2)
        End Operator

        Public Shared Operator Xor(ByVal c1 As Condition(Of T), ByVal c2 As Condition(Of T)) As Condition(Of T)
            Return Condition.Combine(c1, Compare.Xor, c2)
        End Operator

#End Region

    End Class

    ' 表示一个像"object.Property = value"这样的条件.
    ' 在本例中，对象是 T 类型的，值是 S 类型的.
    '
    ' 尽管对此的大多数逻辑已经包含在基类中，定义第二个泛型参数，意味着用
    ' 户不需要传递一个System.Type的参数——它可以被推断出来.
    Public Class Condition(Of T, S)
        Inherits Condition(Of T)
        Friend Sub New(ByVal propName As String, ByVal condType As Compare, ByVal value As S)
            MyBase.New(propName, condType, value, GetType(S))
        End Sub
    End Class

#End Region

#Region "Extension Methods"

    ''' <summary>
    ''' 根据指定的条件，过滤一个 IQueryable(Of T).
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Where(Of T)(ByVal source As IQueryable(Of T), ByVal condition_Renamed As Condition(Of T)) As IQueryable(Of T)

        Dim callExpr = Expression.Call(GetType(Queryable), "Where", New Type() {source.ElementType}, source.Expression, Expression.Quote(condition_Renamed.LambdaExpr))

        Return CType(source.Provider.CreateQuery(callExpr), IQueryable(Of T))
    End Function

    ''' <summary>
    ''' 根据指定的条件，过滤一个 IEnumerable(Of T).
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Where(Of T)(ByVal source As IEnumerable(Of T), ByVal condition_Renamed As Condition(Of T)) As IEnumerable(Of T)
        Return source.Where(condition_Renamed.del)
    End Function

    ''' <summary>
    '''  可以取消，任何实现 IEnumerable(Of T) 类型的扩展方法. 
    '''  它构造了一个用 T 作为元素类型，用 S 作为值类型的 Condition.
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function CreateCondition(Of T, S)(ByVal dataSource As IEnumerable(Of T), ByVal propName As String, ByVal condType As DynamicQuery.Condition.Compare, ByVal value As S) As Condition(Of T, S)

        Return Condition.Create(dataSource, propName, condType, value)
    End Function
#End Region
End Module
