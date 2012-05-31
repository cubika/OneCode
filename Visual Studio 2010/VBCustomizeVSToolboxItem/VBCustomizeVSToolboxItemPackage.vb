'****************************** 模块头 ******************************\
' 模块名称:  VBCustomizeVSToolboxItemPackage.vb
' 项目:	    VBCustomizeVSToolboxItem
' 版权 (c) Microsoft Corporation.
' 
' VBCustomizeVSToolboxItemPackage类继承
' Microsoft.VisualStudio.Shell.Package类。它重写了Initialize方法。
' 
' 如果您添加了一个新的项目到VS2010工具箱中，显示名称和新
' 项目的提示信息默认是相同的。这个例子说明了怎样通过
' 客户端控件向Visual Studio的工具箱里添加栏目。

' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Globalization
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

''' <summary>
''' 这是被当前程序集展现用于实现封装的类。
''' 一个类被认为是Visual Studio的一个有效包的最低要求是
''' 实现IVsPackage接口并用壳注册自己。
''' 本包运用了在管理软件包框架（MPF）中定义的助手类来
''' 做这些事：它继承自Package类，该类提供IVsPackage接口
''' 的实现并使用在框架中定义的注册属性来注册它自己及它的壳
''' 的组成部分
''' </summary>
<DefaultRegistryRoot("Software\Microsoft\VisualStudio\10.0"), _
PackageRegistration(UseManagedResourcesOnly:=True), _
ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string), _
InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400), _
Guid(GuidList.guidVBCustomizeVSToolboxItemPkgString), _
ProvideToolboxItems(1)> _
Public NotInheritable Class VBCustomizeVSToolboxItemPackage
    Inherits Package

    ' 定义标签，栏目，工具提示信息，描述和拖放文本。
    Const toolboxTabString As String = "VB Custom Toolbox Tab"
    Const toolboxItemString As String = "VB Custom Toolbox Item"
    Const toolboxTooltipString As String = "VB Custom Toolbox Tooltip"
    Const toolboxDescriptionString As String = "VB Custom Toolbox Description"
    Const toolboxItemTextString As String = "VB Hello world!"

    ' IVsToolbox2服务.
    Dim vsToolbox2 As IVsToolbox2
    ' IVsActivityLog服务.
    Dim vsActivityLog As IVsActivityLog
    ' 内存流存储提示数据.
    Dim tooltipStream As Stream

    ''' <summary>
    ''' 包的默认构造器。
    ''' 在此方法中，你可以放置任何不需要
    ''' 任何Visual Studio服务的初始化代码，因为在
    ''' 这个时候包对象已经被创建但还未在Visual Studio环境中选址。
    ''' 初始化方法用来进行所有其他的初始化工作。
    ''' </summary>
    Public Sub New()
        Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", Me.GetType().Name))
    End Sub



    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' 重写包的实现
#Region "Package Members"

    ''' <summary>
    ''' 包的初始化；这个方法将在包被选址后调用，所以你可以把所有
    ''' 基于Visual Studio提供服务的初始化代码放在这儿。
    ''' </summary>
    Protected Overrides Sub Initialize()
        MyBase.Initialize()

        ' 初始化服务.
        vsActivityLog = TryCast(GetService(GetType(SVsActivityLog)), IVsActivityLog)
        vsToolbox2 = TryCast(GetService(GetType(SVsToolbox)), IVsToolbox2)

        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("Entering initializer for: {0}", Me.ToString()))

        ' 添加工具箱项，如果它不存在.
        Try
            If Not VerifyToolboxTabExist() Then
                AddToolboxTab()
            End If

            If Not VerifyToolboxItemExist() Then
                AddToolboxItem()
            End If

            vsToolbox2.UpdateToolboxUI()
        Catch ex As Exception
            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, ex.Message)
            LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' 使用IVsActivityLog服务记录VS日志.
    ''' </summary>
    Private Sub LogEntry(ByVal type As __ACTIVITYLOG_ENTRYTYPE, ByVal message As String)

        If vsActivityLog IsNot Nothing Then
            Dim hr As Integer = vsActivityLog.LogEntry(CUInt(type), Me.ToString(), message)
            ErrorHandler.ThrowOnFailure(hr)
        End If
    End Sub

    ''' <summary>
    ''' 检查工具箱栏目是否存在.
    ''' </summary>
    Private Function VerifyToolboxTabExist() As Boolean
        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("Entering VerifyToolboxTabExist for: {0}", Me.ToString()))

        Dim exist As Boolean = False

        Dim tabs As IEnumToolboxTabs = Nothing
        Dim num As UInteger

        ErrorHandler.ThrowOnFailure(vsToolbox2.EnumTabs(tabs))
        Dim rgelt(0) As String
        Dim i As Integer = tabs.Next(1, rgelt, num)
        Do While (ErrorHandler.Succeeded(i) AndAlso (num > 0)) AndAlso (rgelt(0) IsNot Nothing)
            If rgelt(0) = toolboxTabString Then
                exist = True
                Exit Do
            End If
            i = tabs.Next(1, rgelt, num)
        Loop

        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("VerifyToolboxTabExist {0}: {1}", toolboxTabString, exist))

        Return exist
    End Function

    ''' <summary>
    ''' 添加工具箱栏目. 
    ''' </summary>
    Private Sub AddToolboxTab()
        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("Entering AddToolboxTab for: {0}", toolboxTabString))

        ErrorHandler.ThrowOnFailure(vsToolbox2.AddTab(toolboxTabString))
    End Sub

    ''' <summary>
    ''' 检查工具箱栏目是否存在.
    ''' </summary>
    Private Function VerifyToolboxItemExist() As Boolean
        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("Entering VerifyToolboxItemExist for: {0}", Me.ToString()))
        Dim exist As Boolean = False
        Dim items As IEnumToolboxItems = Nothing
        Dim num As UInteger
        ErrorHandler.ThrowOnFailure(vsToolbox2.EnumItems(toolboxTabString, items))
        Dim rgelt = New Microsoft.VisualStudio.OLE.Interop.IDataObject(0) {}
        Dim i As Integer = items.Next(1, rgelt, num)
        Do While (ErrorHandler.Succeeded(i) AndAlso (num > 0)) AndAlso (rgelt(0) IsNot Nothing)
            Dim displayName As String = Nothing
            Dim hr = (TryCast(vsToolbox2, IVsToolbox3)).GetItemDisplayName(rgelt(0), displayName)
            ErrorHandler.ThrowOnFailure(hr)
            If displayName.Equals(toolboxItemString, StringComparison.OrdinalIgnoreCase) Then
                exist = True
                Exit Do
            End If
            i = items.Next(1, rgelt, num)
        Loop

        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("VerifyToolboxItemExist {0}: {1}", toolboxItemString, exist))

        Return exist
    End Function


    ''' <summary>
    ''' 添加工具箱栏目. 
    ''' </summary>
    Private Sub AddToolboxItem()
        LogEntry(__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, String.Format("Entering AddToolboxItem for: {0}", toolboxItemString))

        tooltipStream = SaveStringToStreamRaw(FormatTooltipData(toolboxTooltipString, toolboxDescriptionString))
        Dim toolboxData = New Microsoft.VisualStudio.Shell.OleDataObject()
        ' 设置提示信息.
        toolboxData.SetData("VSToolboxTipInfo", tooltipStream)
        ' 设置拖放文本.
        toolboxData.SetData(DataFormats.Text, toolboxItemTextString)
        Dim itemInfo(0) As TBXITEMINFO
        itemInfo(0).bstrText = toolboxItemString
        itemInfo(0).hBmp = IntPtr.Zero
        itemInfo(0).dwFlags = CUInt(__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST)
        ErrorHandler.ThrowOnFailure(vsToolbox2.AddItem(toolboxData, itemInfo, toolboxTabString))
    End Sub

    ''' <summary>
    ''' 排版工具提示.
    ''' </summary>
    Private Function FormatTooltipData(ByVal toolName As String, ByVal description As String) As String
        Const NameHeader As String = "Name:"
        Const DescriptionHeader As String = "Description:"
        Dim ch As Char = CChar(ChrW((1 + NameHeader.Length) + toolName.Length))
        Dim str As String = ch.ToString() & NameHeader & toolName
        If description IsNot Nothing Then
            ch = CChar(ChrW((1 + DescriptionHeader.Length) + description.Length))
            str = str & ch.ToString() & DescriptionHeader & description
        End If
        str &= vbNullChar
        Return str
    End Function

    Private Function SaveStringToStreamRaw(ByVal value As String) As Stream
        Dim bytes() As Byte = New UnicodeEncoding().GetBytes(value)
        Dim stream As MemoryStream = Nothing
        If (bytes IsNot Nothing) AndAlso (bytes.Length > 0) Then
            stream = New MemoryStream(bytes.Length)
            stream.Write(bytes, 0, bytes.Length)
            stream.Flush()
        Else
            stream = New MemoryStream()
        End If
        stream.WriteByte(0)
        stream.WriteByte(0)
        stream.Flush()
        stream.Position = 0L
        Return stream
    End Function

#End Region

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)

        If disposing And Me.tooltipStream IsNot Nothing Then
            Me.tooltipStream.Dispose()
        End If
    End Sub

End Class
