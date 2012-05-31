/*********************************** 模块头 ***********************************\
* 模块名:  NativeMethods.cs
* 项目名:  VideoEncodingWorkerRole
* 版权 (c) Microsoft Corporation.
* 
* 定义所有非托管代码的静态类.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Runtime.InteropServices;

namespace VideoEncodingWorkerRole
{
    internal static class NativeMethods
    {
        [DllImport("NativeVideoEncoder.dll", CharSet = CharSet.Unicode)]
        internal static extern int EncoderVideo(string inputFile, string outputFile, string logFile);
    }
}
