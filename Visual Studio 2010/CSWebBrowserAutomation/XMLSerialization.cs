/****************************** 模块头 ******************************\
* 模块名:    XMLSerialization.cs
* 项目名:	    CSWebBrowserAutomation
* 并且     (c) Microsoft Corporation.
* 
*  这是一个能将一个对象序列化为XML文件或XML文件序列化为一个对象的类。
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
using System.IO;
using System.Xml.Serialization;

namespace CSWebBrowserAutomation
{
    public class XMLSerialization<T>
    {

        /// <summary>
        /// 把一个对象序列化成一个xml文件。
        /// </summary>
        public static bool SerializeFromObjectToXML(T obj, string filepath)
        {
            if (obj == null)
            {
                throw new ArgumentException("The object to serialize could not be null!");
            }

            bool successed = false;
            Type objType = obj.GetType();
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(objType);
                xs.Serialize(fs, obj);
                successed = true;
            }

            return successed;
        }

        /// <summary>
        /// 把一个xml文件序列化成一个对象。
        /// </summary>
        public static T DeserializeFromXMLToObject(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new ArgumentException("The file does not exist!");
            }

            T obj;
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                obj = (T)xs.Deserialize(fs);
            }
            return obj;
        }

    }
}
