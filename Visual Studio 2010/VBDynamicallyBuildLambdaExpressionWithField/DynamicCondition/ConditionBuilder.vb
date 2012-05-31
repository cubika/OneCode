'******************************** 模块头 *************************************************'
' 模块名:  ConditionBuilder.vb
' 项目名:  VBDynamicallyBuildLambdaExpressionWithField
' 版权 (c) Microsoft Corporation.
'
' ConditionBuilder.vb file 为第一个Condition定义了一个UserControl.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Imports System.ComponentModel

''' <summary>
''' Designer 元数据
''' </summary>
<Designer(GetType(ConditionBuilderDesigner))>
Partial Public Class ConditionBuilder

    Public Sub New()
        InitializeComponent()
    End Sub


#Region "Properties"
    Private Const cName As String = "ConditionLine"

    ''' <summary>
    ''' 枚举，用于条件类型的定义.
    ''' </summary>
    Public Enum Compare As Integer
        [And] = DynamicCondition.DynamicQuery.Condition.Compare.And
        [Or] = DynamicCondition.DynamicQuery.Condition.Compare.Or
    End Enum

    Private _lines As Integer = 1
    Private _type As Type
    Private _operatorType As Compare = Compare.And

    ''' <summary>
    '''  要显示的 ConditionLine 控件数目.
    ''' </summary>
    Public Property Lines() As Integer
        Get
            Return _lines
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then
                Throw New ArgumentException("Lines cannot be less than 1")
            End If

            If value > _lines Then
                ' 构造新的 ConditionLine.
                For i = _lines To value - 1
                    Dim cLine As ConditionLine = New ConditionLine With {.Name = cName & (i + 1), .Left = ConditionLine1.Left, .Width = ConditionLine1.Width, .Top = ConditionLine1.Top + i * (ConditionLine1.Height + 1), .OperatorType = CType(_operatorType, DynamicQuery.Condition.Compare), .Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right}

                    Me.Controls.Add(cLine)
                Next i

            ElseIf value < _lines Then

                ' 去除多余的 ConditionLine.
                For i = value To _lines
                    Me.Controls.RemoveByKey(cName & (i + 1))
                Next i

            End If
            _lines = value
        End Set
    End Property

    ''' <summary>
    ''' 用于每一个 ConditionLine 的默认运算符 (And/Or).
    ''' </summary>
    Public Property OperatorType() As Compare
        Get
            Return _operatorType
        End Get
        Set(ByVal value As Compare)
            _operatorType = value
            For i = 1 To _lines
                GetConditionLine(cName & i).OperatorType = CType(value, DynamicQuery.Condition.Compare)
            Next i
        End Set
    End Property
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' 用 dataSource 中含有的列来填充下拉列表. dataSource 可以是
    ''' IEnumerable(Of T) (本地), 或者是 IQueryable(Of T) (远程).
    ''' </summary>
    Public Sub SetDataSource(Of T)(ByVal dataSource As IEnumerable(Of T))
        _type = GetType(T)

        ' 筛选掉不是内部类型的所有属性.
        ' 例如，一个 Customers 对象可能拥有 EntityRef（Of Order）类型的一个Orders 属性，但是在
        ' 列表中显示该属性意义并不大.
        ' 注意， 根本的 Condition API 确实支持嵌套属性的访问，只是 ConditionBuilder 控件没有
        ' 为用户提供一个机制来指定它.  
        Dim props = From p In _type.GetProperties()
                    Where DynamicCondition.ConditionLine.GetSupportedTypes().Contains(p.PropertyType)
                    Select p

        ' 将列装载到每一个 ConditionLine.
        For i = 1 To _lines
            GetConditionLine(cName & i).DataSource = CType(props.ToArray().Clone(), System.Reflection.PropertyInfo())
        Next i
    End Sub



    ''' <summary>
    ''' 使用此方法来得到，用于表示用户输入 ConditionBuilder 的所有数据一个的Condition对象.
    ''' </summary>
    Public Function GetCondition(Of T)() As DynamicQuery.Condition(Of T)

        ' 这只用来推断类型，因此无需将其实例化.
        Dim dataSrc As T = Nothing
        Dim finalCond = GetConditionLine(cName & "1").GetCondition(Of T)(dataSrc)

        ' 从每一个 ConditionLine 提取 Condition,然后将它与 finalCond 结合起来. 
        For i = 2 To _lines
            Dim cLine = GetConditionLine(cName & i)
            finalCond = DynamicCondition.DynamicQuery.Condition.Combine(Of T)(finalCond, cLine.OperatorType, cLine.GetCondition(Of T)(dataSrc))
        Next i

        Return finalCond
    End Function

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' 接受 "ConditionLine2" ，返回该控件的实际实例.
    ''' </summary>
    Private Function GetConditionLine(ByVal name As String) As ConditionLine
        Return CType(Me.Controls(name), ConditionLine)
    End Function

    ''' <summary>
    ''' 当加载 ConditionBuilder 时执行. 
    ''' </summary>
    Private Sub ConditionBuilder_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.ConditionLine1.lb.Visible = False
    End Sub
#End Region

End Class
