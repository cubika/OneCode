'******************************** 模块头 *********************************\
'* 模块名:   Program.vb
'* 项目名:   Client
'* 版权 (c) Microsoft Corporation.
'* 
'* 这个类展示了怎么享用托管在Worker Role里的WCF服务.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\**************************************************************************


Module Module1

    Sub Main()
        Dim proxy As ServiceReference1.MyServiceClient = New ServiceReference1.MyServiceClient()
        Dim result = proxy.DoWork()
        Console.WriteLine(String.Format("Server Returned: {0}", result))
        Console.ReadLine()
    End Sub

End Module
