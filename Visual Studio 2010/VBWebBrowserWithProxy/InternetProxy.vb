'*************************** Module Header ******************************'
' 模块名称:  InternetProxy.vb
' 项目名称:	    VBWebBrowserWithProxy
' Copyright (c) Microsoft Corporation.
' 
' 这个类是对代理服务器和其使用的证书的描述。
' 请先在ProxyList.xml文件中设定它。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Public Class InternetProxy
    Public Property ProxyName() As String
    Public Property Address() As String
    Public Property UserName() As String
    Public Property Password() As String

    Public Shared ReadOnly NoProxy As InternetProxy =
        New InternetProxy With
        {
            .Address = String.Empty,
            .Password = String.Empty,
            .ProxyName = String.Empty,
            .UserName = String.Empty
        }

    Private Shared _proxyList As List(Of InternetProxy) = Nothing
    Public Shared ReadOnly Property ProxyList() As List(Of InternetProxy)
        Get

            ' 从ProxyList.xml中获取代理服务器。
            If _proxyList Is Nothing Then
                _proxyList = New List(Of InternetProxy)()
                Try
                    Dim listXml As XElement = XElement.Load("ProxyList.xml")
                    For Each proxy In listXml.Elements("Proxy")
                        _proxyList.Add(
                            New InternetProxy With
                            {
                                .ProxyName = proxy.Element("ProxyName").Value,
                                .Address = proxy.Element("Address").Value,
                                .UserName = proxy.Element("UserName").Value,
                                .Password = proxy.Element("Password").Value
                            })
                    Next proxy
                Catch _exception As Exception
                End Try
            End If
            Return _proxyList
        End Get
    End Property
End Class
