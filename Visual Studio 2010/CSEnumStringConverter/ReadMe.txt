=============================================================================
       CONSOLE 应用程序 : CSEnumStringConverter Project 概述
=============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要: 

这个实例演示了如何将枚举转换为以逗号分隔的字符串，以及如何将以逗号分隔的字符串
转换为枚举.它还包括了转换时，枚举的描述属性.


/////////////////////////////////////////////////////////////////////////////
演示:

下列步骤演示了枚举-字符串转换的实例.

步骤 1: 在 Visual Studio 2010中，生成并运行该实例解决方案.

步骤 2: 第一次转换过程中，它将使用.NET框架中，内置的 EnumConverter 类将字符串转换
       为枚举，以及将枚举转换为字符串.

步骤 3: 第二次转换过程中, 它将使用 EnumDescriptionConverter 类将描述字符串转换为
       枚举，以及将枚举转换为描述回字符串.
/////////////////////////////////////////////////////////////////////////////
实现:

在本实例中，应用了下述的ProgrammingLanguage枚举.

 [Flags]
    enum ProgrammingLanguage
    {
        [Description("Visual C#")]
        CS = 0x1,
        [Description("Visual Basic")]
        VB = 0x2, 
        [Description("Visual C++")]
        Cpp = 0x4,
        [Description("Javascript")]
        JS = 0x8,
        // XAML
        XAML = 0x10
    }

System.ComponentModel.EnumConverter 支持从一种类型到另一种类型的转换. 在第一种转换过
程中，将用到它.

    EnumConverter converter = new EnumConverter(typeof(ProgrammingLanguage));
    // 将字符串转换为枚举.
    string langStr = "CS, Cpp, XAML";    
    ProgrammingLanguage lang 
        = (ProgrammingLanguage)converter.ConvertFromString(langStr);
    // 将枚举转换为字符串.
    langStr = converter.ConvertToString(lang);
    
EnumDescriptionConverter 类继承了 EnumConverter，以支持第二种转换过程中的Description
属性.

1. 它将用逗号分隔的字符串拆分为字符串数组（string[]), 然后转换为长整型，并进行位"或",来
   得到表示枚举值的长整型值.

        // 将字符串转换为枚举.
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

2. 它会读取给定枚举的自定义属性（在本例中为Description属性），并准对对应的字符串值. 在标
   志枚举的情况下,它会捕捉Description attribute[]，并准备作为输出的以逗号分隔的字符串.

         // 转换为字符串.
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
                        String.Format("值 '{0}' 对枚举 '{1}' 而言，不是有效值.", 
                        value.ToString(), this.EnumType.Name));
                }

                // 如果枚举值被Description attribute修饰, 返回Description value; 
				// 否则返回名字.
                string enumName = Enum.Format(this.EnumType, value, "G");
                string nameOrDesc;

                if (enumName.IndexOf(enumSeperator) != -1)
                {
                    // 这是标记枚举. 用“，”拆分descriptions.
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

/////////////////////////////////////////////////////////////////////////////
参考:

EnumConverter Class
http://msdn.microsoft.com/en-us/library/system.componentmodel.enumconverter.aspx

DescriptionAttribute Members
http://msdn.microsoft.com/en-us/library/system.componentmodel.descriptionattribute_members(v=VS.85).aspx

/////////////////////////////////////////////////////////////////////////////