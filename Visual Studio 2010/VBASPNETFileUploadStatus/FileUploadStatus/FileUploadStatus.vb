'************************************ 模块头 ******************************\
' 模块名:    UploadStatus.vb
' 项目名:    VBASPNETFileUploadStatus
' 版权 (c) Microsoft Corporation
'
' 本项目阐述了在不使用第三方组件时实现显示上传的状态和进程
' 像ActiveX 控件,Flash 或者Silverlight.
' 
' 我们使用这个类来存储上传进度状态.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************/


Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Web
Imports System.Web.Caching
Imports System.Threading
Imports System.Runtime.Serialization
Imports System.Web.Script.Serialization
Imports System.IO


#Region ""
' 我们定义一个委托EventHandler来把事件保存到UploadStatus.
Public Delegate Sub UploadStatusEventHandler(ByVal sender As Object, ByVal e As UploadStatusEventArgs)

Public Class UploadStatusEventArgs
    Inherits EventArgs
    <ScriptIgnore()> _
    Public Property context() As HttpContext
        Get
            Return m_context
        End Get
        Protected Set(ByVal value As HttpContext)
            m_context = value
        End Set
    End Property
    Private m_context As HttpContext
    Public Sub New(ByVal ctx As HttpContext)
        context = ctx
    End Sub
End Class
#End Region

<Serializable()> _
Public Class UploadStatus

    Private Enum DataUnit
        [Byte] = 1
        KB = 1024
        MB = 1048576
        GB = 1073741824
    End Enum
    Private Enum TimeUnit
        Seconds = 1
        Minutes = 60
        Hours = 3600
        Day = 86400
    End Enum

    ' 返回上传的数据单元.
    Private ReadOnly Property LoadedUnit() As DataUnit
        Get
            Return GetDataUnit(LoadedLength)
        End Get
    End Property
    ' 返回整个内容数据单元.
    Private ReadOnly Property ContentUnit() As DataUnit
        Get
            Return GetDataUnit(ContentLength)
        End Get
    End Property

    ' 请求内容
    Private Context As HttpContext





    ''' <summary>
    ''' 如果用户终止上传，它将会返回true.
    ''' </summary>
    Public Property Aborted() As Boolean
        Get
            Return _aborted
        End Get
        Private Set(ByVal value As Boolean)
            _aborted = value
        End Set
    End Property
    Private _aborted As Boolean

    ''' <summary>
    ''' 获取文件内容的长度.
    ''' </summary>
    Public Property ContentLength() As Long
        Get
            Return _contentLength
        End Get
        Private Set(ByVal value As Long)
            _contentLength = value
        End Set
    End Property
    Private _contentLength As Long
    ''' <summary>
    ''' 获取单元被格式化的文件的内容长度.
    ''' </summary>
    Public ReadOnly Property ContentLengthString() As String
        Get
            Dim rslWithUnit As Decimal = CDec(ContentLength) / CInt(ContentUnit)
            Return rslWithUnit.ToString("0.00") & " " & ContentUnit.ToString()
        End Get
    End Property
    ''' <summary>
    ''' 获取上传文件的内容长度
    ''' </summary>
    Public Property LoadedLength() As Long
        Get
            Return _loadedLength
        End Get
        Private Set(ByVal value As Long)
            _loadedLength = value
        End Set
    End Property
    Private _loadedLength As Long
    ''' <summary>
    ''' 获取单元被格式化的上传
    ''' 文件内容的长度
    ''' </summary>
    Public ReadOnly Property LoadedLengthString() As String
        Get
            Dim rslWithUnit As Decimal = CDec(LoadedLength) / CInt(LoadedUnit)
            Return rslWithUnit.ToString("0.00") & " " & LoadedUnit.ToString()
        End Get
    End Property
    ''' <summary>
    ''' 获取上传开始时的时间.
    ''' </summary>
    Public Property StartTime() As DateTime
        Get
            Return _startTime
        End Get
        Private Set(ByVal value As DateTime)
            _startTime = value
        End Set
    End Property
    Private _startTime As DateTime
    ''' <summary>
    ''' 获取上传结束或终止时的时间.
    ''' </summary>
    Public Property EndTime() As DateTime
        Get
            Return _endTime
        End Get
        Private Set(ByVal value As DateTime)
            _endTime = value
        End Set
    End Property
    Private _endTime As DateTime
    ''' <summary>
    ''' 检查上传是否结束.
    ''' </summary>
    Public Property IsFinished() As Boolean
        Get
            Return _isFinished
        End Get
        Private Set(ByVal value As Boolean)
            _isFinished = value
        End Set
    End Property
    Private _isFinished As Boolean
    ''' <summary>
    ''' 获取上传内容的百分比
    ''' </summary>
    Public ReadOnly Property LoadedPersentage() As Integer
        Get
            Dim percent As Integer = Convert.ToInt32(Math.Ceiling(CDec(LoadedLength) / CDec(ContentLength) * 100))
            Return percent
        End Get
    End Property
    ''' <summary>
    ''' 获取上传花费时间
    ''' 单位时秒
    ''' </summary>
    Public ReadOnly Property SpendTimeSeconds() As Double
        Get
            Dim calcTime As DateTime = DateTime.Now
            If IsFinished OrElse Aborted Then
                calcTime = EndTime
            End If
            Dim spendtime As Double = Math.Ceiling(calcTime.Subtract(StartTime).TotalSeconds)
            If spendtime = 0 AndAlso IsFinished Then
                spendtime = 1
            End If
            Return spendtime
        End Get
    End Property
    ''' <summary>
    ''' 获取单元被格式化的上传花费的时间
    ''' </summary>
    Public ReadOnly Property SpendTimeString() As String
        Get
            Dim spent As Double = SpendTimeSeconds
            Dim unit As TimeUnit = GetTimeUnit(spent)
            Dim unitTime As Double = spent / CInt(GetTimeUnit(spent))
            Return unitTime.ToString("0.0") & " " & unit.ToString()
        End Get
    End Property

    ''' <summary>
    ''' 获取上传速度
    ''' 单位是 字节/秒
    ''' </summary>
    Public ReadOnly Property UploadSpeed() As Double
        Get
            Dim spendtime As Double = SpendTimeSeconds
            Dim speed As Double = CDbl(LoadedLength) / spendtime
            Return speed
        End Get
    End Property
    ''' <summary>
    ''' 获取单元被格式化的上传速度.
    ''' </summary>
    Public ReadOnly Property UploadSpeedString() As String
        Get
            Dim spendtime As Double = SpendTimeSeconds
            Dim unit As DataUnit = GetDataUnit(CLng(Math.Truncate(Math.Ceiling(CDbl(LoadedLength) / spendtime))))
            Dim speed As Double = UploadSpeed / CInt(unit)
            Return speed.ToString("0.0") & " " & unit.ToString() & "/seconds"
        End Get
    End Property
    ''' <summary>
    ''' 获取剩余时间
    ''' 单位是秒
    ''' </summary>
    Public ReadOnly Property LeftTimeSeconds() As Double
        Get
            Dim remain As Double = Math.Floor((ContentLength - LoadedLength) / UploadSpeed)
            Return remain
        End Get
    End Property
    ''' <summary>
    ''' 获取被格式化单元的剩余时间
    ''' </summary>
    Public ReadOnly Property LeftTimeString() As String
        Get
            Dim remain As Double = LeftTimeSeconds
            Dim unit As TimeUnit = GetTimeUnit(remain)
            Dim newRemain As Double = remain / CInt(unit)
            Return newRemain.ToString("0.0") & " " & unit.ToString()
        End Get
    End Property




    Public Event OnDataChanged As UploadStatusEventHandler
    Public Event OnFinish As UploadStatusEventHandler



    Public Sub New()
    End Sub
    Public Sub New(ByVal ctx As HttpContext, ByVal length As Long)
        Aborted = False
        IsFinished = False
        StartTime = DateTime.Now
        Context = ctx
        ContentLength = length
    End Sub



    Private Function GetTimeUnit(ByVal seconds As Double) As TimeUnit
        If seconds > CInt(TimeUnit.Day) Then
            Return TimeUnit.Day
        End If
        If seconds > CInt(TimeUnit.Hours) Then
            Return TimeUnit.Hours
        End If
        If seconds > CInt(TimeUnit.Minutes) Then
            Return TimeUnit.Minutes
        End If
        Return TimeUnit.Seconds

    End Function
    Private Function GetDataUnit(ByVal length As Long) As DataUnit
        If length > Math.Pow(2.0, 30) Then
            Return DataUnit.GB
        End If
        If length > Math.Pow(2.0, 20) Then
            Return DataUnit.MB
        End If
        If length > Math.Pow(2.0, 10) Then
            Return DataUnit.KB
        End If
        Return DataUnit.[Byte]
    End Function

    Private Sub changeFinish()
        RaiseEvent OnFinish(Me, New UploadStatusEventArgs(Context))
    End Sub
    Private Sub changeData()
        If Aborted Then
            Return
        End If
        RaiseEvent OnDataChanged(Me, New UploadStatusEventArgs(Context))
        If LoadedLength = ContentLength Then
            EndTime = DateTime.Now
            IsFinished = True
            changeFinish()
        End If
    End Sub
    Public Sub Abort()
        Aborted = True
        EndTime = DateTime.Now
    End Sub
    Public Sub UpdateLoadedLength(ByVal length As Long)
        If Not IsFinished AndAlso Not Aborted Then
            LoadedLength += length
            changeData()
        End If
    End Sub




End Class

