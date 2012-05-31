/****************************** 模块头 *************************************\
* Module Name:	WorkflowServiceContract.cs
* Project:		Client
* Copyright (c) Microsoft Corporation.
* 
* 该文件用来保存Windows Azure Platform AppFabric账户信息.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Client
{
	[ServiceContract]
	public interface IProcessDataWorkflowService
	{
		[OperationContract]
		ProcessDataResponse ProcessData(ProcessDataRequest request);
	}

	public interface IProcessDataWorkflowServiceChannel :IProcessDataWorkflowService, IClientChannel
	{
	}

	// 信息合约.
	[MessageContract(IsWrapped = false)]
	public partial class ProcessDataRequest
	{
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/", Order = 0)]
		public System.Nullable<int> @int;

		public ProcessDataRequest()
		{
		}

		public ProcessDataRequest(System.Nullable<int> @int)
		{
			this.@int = @int;
		}
	}

	[MessageContract(IsWrapped = false)]
	public partial class ProcessDataResponse
	{
		[MessageBodyMember(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/", Order = 0)]
		public string @string;

		public ProcessDataResponse()
		{
		}

		public ProcessDataResponse(string @string)
		{
			this.@string = @string;
		}
	}
}
