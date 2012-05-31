/****************************** Module Header ******************************\
* 模块:      Program.cs
* 项目:      CSXmlSerialization
* 版权 (c) Microsoft Corporation.
* 
* 此示例演示了如何将内存中的对象序列化到本地的XML文件，以及如何通过C#将XML文件
* 反序列化到内存中的对象。
* 设计的MySerializableType包括整型，字符串，泛型，以及作为自定义的类型的字段和属性。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
#endregion


namespace CSXmlSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            /////////////////////////////////////////////////////////////////
            // 将对象序列化到XML文件
            // 

            // 创建并初始化一个MySerializableType实例
            MySerializableType instance = new MySerializableType();
            instance.BoolValue = true;
            instance.IntValue = 1;
            instance.StringValue = "测试 String";
            instance.ListValue.Add("List 项 1");
            instance.ListValue.Add("List 项 2");
            instance.ListValue.Add("List 项 3");
            instance.AnotherTypeValue = new AnotherType();
            instance.AnotherTypeValue.IntValue = 2;
            instance.AnotherTypeValue.StringValue = "测试内部元素 String";

            // 创建序列化
            XmlSerializer serializer = new XmlSerializer(typeof(MySerializableType));

            // 将对象序列化到XML文件
            using (StreamWriter streamWriter = File.CreateText(
                "CSXmlSerialization.xml"))
            {
                serializer.Serialize(streamWriter, instance);
            }


            /////////////////////////////////////////////////////////////////
            // 将XML文件反序列化到一个对象的实例
            // 

            // 反序列化对象
            MySerializableType deserializedInstance;
            using (StreamReader streamReader = File.OpenText(
                "CSXmlSerialization.xml"))
            {
                deserializedInstance = serializer.Deserialize(streamReader)
                    as MySerializableType;
            }

            // 对象结果输出
            Console.WriteLine("Bool值: {0}", deserializedInstance.BoolValue);
            Console.WriteLine("Int值: {0}", deserializedInstance.IntValue);
            Console.WriteLine("String值: {0}", deserializedInstance.StringValue);
            Console.WriteLine("自定义类型元素的Int值.IntValue: {0}",
                deserializedInstance.AnotherTypeValue.IntValue);
            Console.WriteLine("自定义类型元素的String值: {0}",
                deserializedInstance.AnotherTypeValue.StringValue);
            Console.WriteLine("List值: ");
            foreach (object obj in deserializedInstance.ListValue)
            {
                Console.WriteLine(obj.ToString());
            }
        }
    }


    /// <summary>
    /// 序列化的类型声明
    /// </summary>
    [Serializable()]
    public class MySerializableType
    {
        // String类型字段和属性
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }

        // Bool类型字段和属性
        private bool boolValue;
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        // Int类型字段和属性
        private int intValue;
        public int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }

        // Another类型字段和属性
        private AnotherType anotherTypeValue;
        public AnotherType AnotherTypeValue
        {
            get { return anotherTypeValue; }
            set { anotherTypeValue = value; }
        }

        // 泛型类型字段和属性
        private List<string> listValue = new List<string>();
        public List<string> ListValue
        {
            get { return listValue; }
            set { listValue = value; }
        }

        // 使用NonSerialized属性忽视的一个字段
        [NonSerialized()]
        private int ignoredField = 1;
    }

    /// <summary>
    /// Another类型声明
    /// </summary>
    [Serializable()]
    public class AnotherType
    {
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }

        private int intValue;
        public int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }
    }
}
