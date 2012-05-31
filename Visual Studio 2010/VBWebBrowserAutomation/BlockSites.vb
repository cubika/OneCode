'*************************** 模块头 ******************************'
' 模块名:    BlockSites.vb
' 项目名:	    VBWebBrowserAutomation
' 版权(c) Microsoft Corporation.
' 
' 这是BlockSites类包含了不能访问站点列表。静态方法GetBlockSites将BlockList.xml转换为
' 一个BlockSites实例
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Public Class BlockSites

    Public Property Hosts() As List(Of String)

    Private Shared ReadOnly _instance As BlockSites = GetBlockSites()

    Public Shared ReadOnly Property Instance() As BlockSites
        Get
            Return _instance
        End Get
    End Property

    ''' <summary>
    '''把BlockList.xml反序列化成一个BlockSites实例。
    ''' </summary>
    Private Shared Function GetBlockSites() As BlockSites
        Dim path As String = String.Format("{0}\Resources\BlockList.xml",
                                           Environment.CurrentDirectory)
        Return XMLSerialization(Of BlockSites).DeserializeFromXMLToObject(path)
    End Function

End Class
