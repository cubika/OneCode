Imports System.IO
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime

Public Class AzureLocalStorageTraceListener
    Inherits XmlWriterTraceListener

    Public Sub New()
        MyBase.New(Path.Combine(AzureLocalStorageTraceListener.GetLogDirectory().Path, "StoryCreatorWebRole.svclog"))
    End Sub

    ' Suppress CA2122 自动生成代码
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")> _
    Public Shared Function GetLogDirectory() As DirectoryConfiguration
        Dim directory As New DirectoryConfiguration()
        directory.Container = "wad-tracefiles"
        directory.DirectoryQuotaInMB = 10
        directory.Path = RoleEnvironment.GetLocalResource("StoryCreatorWebRole.svclog").RootPath
        Return directory
    End Function
End Class
