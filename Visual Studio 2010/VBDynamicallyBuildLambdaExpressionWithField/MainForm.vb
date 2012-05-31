'********************************** 模块头 ******************************************'
' 模块名:  MainForm.vb
' 项目:    VBDynamicallyBuildLambdaExpressionWithField
' 版权 (c) Microsoft Corporation.
'
' 这个实例演示了如何动态创建lambda表达式,和将数据显示在 DataGridView 控件中.
' 
' 这个实例演示了如何将多个条件连结在一起，并动态生成LINQ到SQL. LINQ是非常好的方法，它用
' 类型安全、直观、极易表现的方式，来声明过滤器和查询数据. 例如，该应用
' 程序中的搜索功能，可以让客户找到满足多个列所定义条件的一切记录.' 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************************'

Imports DynamicCondition


Partial Public Class MainForm
    Private db As New NorthwindDataContext()

    ''' <summary>
    ''' 当Winform加载时，将字段列表加载到控件中.
    ''' </summary>
    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' 将字段列表加载到控件中.
        ConditionBuilder1.SetDataSource(db.Orders)
    End Sub

    ''' <summary>
    ''' 动态生成LINQ查询，并将数据填入DataGridView控件中.
    ''' </summary>
    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        ' 从控件中获取条件.
        Dim c = ConditionBuilder1.GetCondition(Of Order)()

        ' 筛选掉不符合条件的所有Orders.
        ' 注意，由于延迟执行，实际上尚未执行查询.
        Dim filteredQuery = db.Orders.Where(c)

        ' 我们现在可以执行其他任何操作（例如Order By或者Select)在filteredQuery中.
        Dim query = From row In filteredQuery
                    Order By row.OrderDate, row.OrderID
                    Select row

        ' 执行查询，并将结果显示在DataGridView控件中.
        dgResult.DataSource = query
    End Sub

    ''' <summary>
    ''' DefaultInstance属性.
    ''' </summary>
    Private Shared _defaultInstance As MainForm
    Public Shared ReadOnly Property DefaultInstance() As MainForm
        Get
            If _defaultInstance Is Nothing Then
                _defaultInstance = New MainForm()
            End If
            Return _defaultInstance
        End Get
    End Property
End Class

