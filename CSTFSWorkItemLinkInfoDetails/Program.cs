/********************************** 模块头 ********************************\
* 模块名：Program.cs
* 项目名：CSTFSWorkItemLinkInfoDetails
* 版权 (c) Microsoft Corporation.
*  
*  该应用程序的主入口。运行这个程序，使用以下命令参数：
* 
*  CSTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>                   
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CSTFSWorkItemLinkInfoDetails
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
               // 这里有两个参数。
                if (args.Length == 2)
                {

                    // 从参数中获取CollectionUrl和WorkItemID。
                    Uri collectionUri = new Uri(args[0]);
                    int workitemID = int.Parse(args[1]);

                    using (WorkItemLinkQuery query = new WorkItemLinkQuery(collectionUri))
                    {

                        // 从一个work item中获取WorkItemLinkInfoDetails列表。
                        var detailsList = query.GetWorkItemLinkInfos(workitemID);

                        foreach (WorkItemLinkInfoDetails details in detailsList)
                        {
                            Console.WriteLine(details.ToString());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("使用下面的命令参数使用本程序:");
                    Console.WriteLine("CSTFSWorkItemLinkInfoDetails.exe <CollectionUrl> <WorkItemID>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
