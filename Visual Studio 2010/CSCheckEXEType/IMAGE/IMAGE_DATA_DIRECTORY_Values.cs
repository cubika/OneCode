/****************************** 模块头 ******************************\
* 模块名:  IMAGE_DATA_DIRECTORY_Values.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 数据目录是大小为16的结构体数组. 每一个数组元素都
* 预先定义了它的含义. 常量 IMAGE_DIRECTORY_ENTRY_ xxx 
* 表示数组目录的索引(从 0 到 15).
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


namespace CSCheckEXEType.IMAGE
{
    public class IMAGE_DATA_DIRECTORY_Values
    {
        // 导出目录
        public const int IMAGE_DIRECTORY_ENTRY_EXPORT = 0;

        // 导入目录
        public const int IMAGE_DIRECTORY_ENTRY_IMPORT = 1;

        // 资源目录
        public const int IMAGE_DIRECTORY_ENTRY_RESOURCE = 2;

        // 异常目录
        public const int IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3;

        // 安全目录
        public const int IMAGE_DIRECTORY_ENTRY_SECURITY = 4;

        // 基础重定向表
        public const int IMAGE_DIRECTORY_ENTRY_BASERELOC = 5;

        // 调试目录
        public const int IMAGE_DIRECTORY_ENTRY_DEBUG = 6;

        // 体系的具体数据
        public const int IMAGE_DIRECTORY_ENTRY_ARCHITECTURE = 7;

        // 全局指针的相对虚地址
        public const int IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8;

        // TLS目录
        public const int IMAGE_DIRECTORY_ENTRY_TLS = 9;

        // 加载配置目录
        public const int IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10;

        // 头部的绑定导入目录
        public const int IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11;

        // 导入地址表
        public const int IMAGE_DIRECTORY_ENTRY_IAT = 12;

        // 延迟加载导入描述符
        public const int IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT = 13;

        // COM 运行时描述符 
        public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;

        public IMAGE_DATA_DIRECTORY[] Values { get; set; }
 
        public IMAGE_DATA_DIRECTORY_Values()
        {
            Values = new IMAGE_DATA_DIRECTORY[16];
        }
    }

}
