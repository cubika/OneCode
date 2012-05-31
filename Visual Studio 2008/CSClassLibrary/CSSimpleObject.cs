/******************************** 模块头 ***********************************\
模块名:  CSSimpleObject.cs
项目:    CSClassLibrary
版权     (c) Microsoft Corporation.

这个实例演示一个我们在其他应用程序需所使用的C#类库. 这个类库阐述了一个被命名
为CSSimpleObject的简单的类. 


This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Diagnostics;
#endregion


namespace CSClassLibrary
{
    public class CSSimpleObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CSSimpleObject()
        {
        }

        private float fField = 0F;

        /// <summary>
        /// 这是一个公共属性。它允许你设置及获得这个字段的值 
        /// </summary>
        public float FloatProperty
        {
            get { return fField; }
            set
            {
                // 触发事件FloatPropertyChanging
                bool cancel = false;
                if (FloatPropertyChanging != null)
                {
                    FloatPropertyChanging(value, out cancel);
                }

                // 如果没有被取消，则改变值.
                if (!cancel)
                {
                    fField = value;
                }
            }
        }

        /// <summary>
        /// 返回一个String，表示当前对象
        /// 在此，我们返回fField的字符串形式
        /// </summary>
        /// <returns>返回fField的字符串形式.</returns>
        public override string ToString()
        {
            return this.fField.ToString("F2");
        }

        /// <summary>
        /// 这是一个公共静态方法. 它返回字符串中的字符数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回字符串中的字符数</returns>
        public static int GetStringLength(string str)
        {
            return str.Length;
        }

        /// <summary>
        /// 这是一个事件. 当浮点属性被设置时触发事件
        /// </summary>
        public event PropertyChangingEventHandler FloatPropertyChanging; 
    }


    /// <summary>
    /// 属性值改变事件处理
    /// </summary>
    /// <param name="NewValue">新的属性值</param>
    /// <param name="Cancel">
    /// 输出是否变更被取消
    /// </param>
    public delegate void PropertyChangingEventHandler(object NewValue, out bool Cancel);
}
