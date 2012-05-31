'********************************* 模块头 *********************************'
' 模块名：                Module1.vb
' 项目名:                 NetTcpWCFService
' 版权 (c) Microsoft Corporation.
' 
' NetTcpWCF服务应用接入点，创建一个ServiceHost来承载WCF服务
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.ServiceModel

Module Module1

    Sub Main()
        Using host = New ServiceHost(GetType(WeatherService))
            host.Open()
            Console.WriteLine("Service is running...")
            Console.WriteLine("Service address: " + host.BaseAddresses(0).AbsoluteUri)
            Console.Read()
        End Using
    End Sub

End Module
