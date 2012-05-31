/********************************* 模块头 ***************************************\ 
* 模块名:   EnumDescriptionConverter.cs 
* 项目名:   CSEnumStringConverter
* 版权 (c) Microsoft Corporation. 
*
* 该文件提供了一个转换器：EnumDescriptionConverter，用于枚举对象和由
* DescriptionAttribute 指定的描述字符串之间的转换. 该类继承自内置的类
* EnumConverter. 与EnumConverter不同的是，如果有 DescriptionAttribute
* 附加在枚举对象上时，EnumConverter 不考虑 DescriptionAttribute，而
* EnumDescriptionConverter将枚举对象转换为它的描述字符串.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\*******************************************************************************/

using System;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace CSEnumStringConverter
{
    /// <summary>
    /// 提供了一个类型转换器，用于枚举对象和用DescriptionAttribute指定的描述字符串之
    /// 间的转换. 该类派生于内置的EnumConverter类. 与EnumConverter不同的是，如果有
    /// DescriptionAttribute附加在枚举对象上时，EnumConverter 不考虑
    /// DescriptionAttribute，而EnumDescriptionConverter将枚举对象转换为它的描述字符
    /// 串. 当用 EnumDescriptionConverter 类来将字符串转换为枚举对象时，该类也会尝试，
    /// 用 DescriptionAttribute 指定的描述字符串，来匹配该字符串.
    /// </summary>
    public class EnumDescriptionConverter : EnumConverter
    {
        private const char enumSeperator = ',';

        public EnumDescriptionConverter(Type type) : base(type)
        {
        }


        #region ConvertFrom (String)

        /// <summary>
        /// 将给定的值转换为该转换器的枚举类型. 如果给定值是字符串，该方法会尝试将字符
        /// 串的名字，与由每一个枚举对象的 DescriptionAttribute 指定的描述字符串，匹
        /// 配起来. 
        /// </summary>
        /// <param name="context">
        /// ITypeDescriptorContext，提供一个带格式的上下文信息.
        /// </param>
        /// <param name="culture">
        /// 可选的CultureInfo. 如果没有提供，就假定为当前的文化信息.
        /// </param>
        /// <param name="value">要转换的枚举对象. </param>
        /// <returns>
        /// 用于表示转换值的枚举对象.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, 
            CultureInfo culture, object value)
        {
            if (value is string)
            {
                string strValue = (string)value;
                if (strValue.IndexOf(enumSeperator) != -1)
                {
                    ulong convertedValue = 0;
                    foreach (string v in strValue.Split(enumSeperator))
                    {
                        convertedValue |= Convert.ToUInt64(Parse(this.EnumType, v), culture);
                    }
                    return Enum.ToObject(this.EnumType, convertedValue);
                }
                else
                {
                    return Parse(this.EnumType, strValue);
                }
            }
            
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 将名称的字符串表示形式，或者由 DescriptionAttribute 指定的描述字符串，或
        /// 者一个或多个枚举常数的数值，转换为一个等效的枚举对象.
        /// </summary>
        /// <param name="enumType">一个枚举类型. </param>
        /// <param name="value">
        /// 一个字符串,包含待转换的名字，或描述，或数值. 
        /// </param>
        /// <returns>
        /// 一个enumType类型的对象，其值用值来表示.
        /// </returns>
        /// <remarks>
        /// 不同于枚举. 忽视 DescriptionAttribute, Parse 方法也能将描述字符串转换为等
        /// 效的枚举对象.
        /// 
        /// 字符串的比较非大小写敏感.
        /// </remarks>
        private static object Parse(Type enumType, string value)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            if (!enumType.IsEnum)
                throw new ArgumentException("提供的类型必须为枚举类型.\r\n参数名字: enumType");

            if (value == null)
                throw new ArgumentNullException("value");

            value = value.Trim();
            if (value.Length == 0)
                throw new ArgumentException("必须指定有效信息以用于解析字符串.");

            if (Char.IsDigit(value[0]) || value[0] == '-' || value[0] == '+')
            {
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                object temp;
                try
                {
                    temp = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                    return Enum.ToObject(enumType, temp);
                }
                catch (FormatException)
                {
                    // 我们需要分析此字符串. 
                    // 某些情况下，用类型库导入程序（Tlbimp)导入枚举，可以有"3D"格式.
                    
                }
            }

            ulong result = 0;
            Dictionary<string, ulong> dict = GetStringToEnumDictionary(enumType);

            foreach (string v in value.Split(enumSeperator))
            {
                string nameOrDesc = v.Trim();
                if (dict.ContainsKey(nameOrDesc))
                {
                    result |= dict[nameOrDesc];
                }
                else
                {
                    throw new ArgumentException(
                        String.Format("找不到请求的值 '{0}'.", value));
                }
            }

            return Enum.ToObject(enumType, result);
        }

        private static Hashtable fieldInfoHash = Hashtable.Synchronized(new Hashtable());
        private const int maxHashElements = 100; // 用于调整工作集.

        private static Dictionary<string, ulong> GetStringToEnumDictionary(Type enumType)
        {
            Debug.Assert(enumType != null);
            Debug.Assert(enumType.IsEnum);

            Dictionary<string, ulong> dict = (Dictionary<string, ulong>)fieldInfoHash[enumType];
            if (dict == null)
            {
                // 当插入元素的个数已经达到阈值，清空哈希表，
                // 以减少工作集.
                if (fieldInfoHash.Count > maxHashElements)
                {
                    fieldInfoHash.Clear();
                }

                // 用不区分大小写的字符串比较器来创建字典.
                dict = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);

                FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo info in fields)
                {
                    ulong enumValue = Convert.ToUInt64(info.GetValue(info.Name));

                    // 添加<name, enumValue>对.
                    dict.Add(info.Name, enumValue);

                    // 如果可以，添加 <description, enumValue>对.
                    DescriptionAttribute[] attrs = (DescriptionAttribute[])
                        info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attrs.Length > 0)
                    {
                        dict.Add(attrs[0].Description, enumValue);
                    }
                }

                fieldInfoHash.Add(enumType, dict);
            }
            return dict;
        }

        #endregion


        #region ConvertTo (String)

        /// <summary>
        /// 将给定的枚举值对象转换为指定的目的类型. 如果目的类型是字符串，当
        /// DescriptionAttribute 附加于枚举值时，该方法会把枚举值转换为它的
        /// 描述字符串.
        /// </summary>
        /// <param name="context">
        /// ITypeDescriptorContext，提供一个带格式的上下文信息.
        /// </param>
        /// <param name="culture">
        /// 可选的CultureInfo. 如果没有提供，就假定为当前的文化信息.
        /// </param>
        /// <param name="value"> 待转换的枚举对象. </param>
        /// <param name="destinationType">
        /// 该值要转换的目的类型.
        /// </param>
        /// <returns>
        /// 用于表示转换值的对象.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, 
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(string) && value != null)
            {
                // 如果没有定义value，或者该枚举不是标记类型，将引发一个参数异常.  
                //
                Type underlyingType = Enum.GetUnderlyingType(this.EnumType);
                if (value is IConvertible && value.GetType() != underlyingType)
                {
                    value = ((IConvertible)value).ToType(underlyingType, culture);
                }
                if (!this.EnumType.IsDefined(typeof(FlagsAttribute), false) && 
                    !Enum.IsDefined(this.EnumType, value))
                {
                    throw new ArgumentException(
                        String.Format(" 对于枚举 '{1}' 而言，值 '{0}' 不是有效值.", 
                        value.ToString(), this.EnumType.Name));
                }

                //  如果枚举值被Description attribute修饰, 返回Description value; 
                //  否则返回名字.
                string enumName = Enum.Format(this.EnumType, value, "G");
                string nameOrDesc;

                if (enumName.IndexOf(enumSeperator) != -1)
                {
                    // 这是一个标记枚举. 用“，”拆分descriptions.
                    bool firstTime = true;
                    StringBuilder retval = new StringBuilder();

                    foreach (string v in enumName.Split(enumSeperator))
                    {
                        nameOrDesc = v.Trim();

                        FieldInfo info = this.EnumType.GetField(nameOrDesc);
                        DescriptionAttribute[] attrs = (DescriptionAttribute[])
                            info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attrs.Length > 0)
                        {
                            nameOrDesc = attrs[0].Description;
                        }

                        if (firstTime)
                        {
                            retval.Append(nameOrDesc);
                            firstTime = false;
                        }
                        else
                        {
                            retval.Append(enumSeperator);
                            retval.Append(' ');
                            retval.Append(nameOrDesc);
                        }
                    }

                    return retval.ToString();
                }
                else
                {
                    FieldInfo info = this.EnumType.GetField(enumName);
                    if (info != null)
                    {
                        DescriptionAttribute[] attrs = (DescriptionAttribute[])
                            info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        nameOrDesc = (attrs.Length > 0) ? attrs[0].Description : enumName;
                    }
                    else
                    {
                        nameOrDesc = enumName;
                    }
                    return nameOrDesc;
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion

    }
}
