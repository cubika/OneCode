'/****************************** 模块 标识 ******************************
'* 模块名:    ScoreCardForSchoolLinqToSQL.vb
'* 项目:      VBADONETDataServiceSL3Client
'* 版权       (c) Microsoft Corporation.
'* 
'* ScoreCardForSchoolLinqToSQL.vb 展示了如何用创建一个类来为 UI 作数据源。
'* 它是用来连接自动生成的 ADO.NET Data Service客户端代码所获得数据以及 UI 的桥梁。
'* 这里还使用了一些小技巧，例如，Display 以及 Editable 属性的使用。 
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'\**************************************************************************

Imports System
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.ComponentModel.DataAnnotations
Imports VBADONETDataServiceSL3Client.SchoolLinqToSQLService


' ScoreCardForSchoolLinqToSQL 对象的集合是 DataGrid 控件的数据源。
Public Class ScoreCardForSchoolLinqToSQL
    ' 为了进行删除操作，维护一个CourseGrade对象的引用。
    Private _CourseGrade As CourseGrade
    <Display(AutoGenerateField:=False)> _
    Public Property CourseGrade() As CourseGrade
        Get
            Return _CourseGrade
        End Get
        Set(ByVal value As CourseGrade)
            _CourseGrade = value
        End Set
    End Property
    Private _PersonName As String
    <Editable(False)> _
    Public Property PersonName() As String
        Get
            Return _PersonName
        End Get
        Set(ByVal value As String)
            _PersonName = value
        End Set
    End Property
    Private _Course As String
    <Editable(False)> _
    Public Property Course() As String
        Get
            Return _Course
        End Get
        Set(ByVal value As String)
            _Course = value
        End Set
    End Property

    Private _Grade As System.Nullable(Of Decimal)
    <Editable(False)> _
    Public Property Grade() As System.Nullable(Of Decimal)
        Get
            Return _Grade
        End Get
        Set(ByVal value As System.Nullable(Of Decimal))
            _Grade = value
        End Set
    End Property
End Class
