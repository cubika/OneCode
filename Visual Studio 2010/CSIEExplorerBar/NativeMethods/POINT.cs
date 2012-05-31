/****************************** 模块头 ******************************\
* 模块名称:  POINT.cs
* 项目:	    CSIEExplorerBar
* 版权： (c) Microsoft Corporation.
* 
* POINT类为菜单指定屏幕坐标。
* 
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

namespace CSIEExplorerBar.NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int X {get;set;}
        public int Y{get;set;}

        public POINT()
        {
        }

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
