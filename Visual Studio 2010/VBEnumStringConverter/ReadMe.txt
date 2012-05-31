=========================================================================================
       CONSOLE 应用程序: VBEnumStringConverter 项目概述
=========================================================================================

/////////////////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示了如何将枚举转换为以逗号分隔的字符串，以及如何将以逗号分隔的字符串转换为枚举.
它还包括了转换时，枚举的描述属性.


/////////////////////////////////////////////////////////////////////////////////////////
演示:

下列步骤演示了枚举-字符串转换的实例.

步骤 1: 在 Visual Studio 2010中，生成并运行该实例解决方案.

步骤 2: 第一次转换过程中，它将使用.NET框架中，内置的 EnumConverter 类将字符串转换
       为枚举，以及将枚举转换为字符串.

步骤 3: 第二次转换过程中, 它将使用 EnumDescriptionConverter 类将描述字符串转换为
       枚举，以及将枚举转换为描述回字符串.

/////////////////////////////////////////////////////////////////////////////////////////
实现:

在本实例中，应用了下述的ProgrammingLanguage枚举.
		<Flags()> _
		Enum ProgrammingLanguage
			<Description("Visual Basic")> _
			VB = &H1
			<Description("Visual C#")> _
			CS = &H2
			<Description("Visual C++")> _
			Cpp = &H4
			<Description("Javascript")> _
			JS = &H8
			' XAML
			XAML = &H10
		End Enum

System.ComponentModel.EnumConverter 支持从一种类型到另一种类型的转换. 在第一种转换过
程中，将用到它.

		Dim converter As New EnumConverter(GetType(ProgrammingLanguage))

        ' 将字符串转换为枚举.
        Dim langStr As String = "VB, CS, Cpp, XAML"
        Dim lang As ProgrammingLanguage =
            DirectCast(converter.ConvertFromString(langStr), ProgrammingLanguage)

		' 将枚举转换为字符串.
		langStr = converter.ConvertToString(lang)
    
 EnumDescriptionConverter 类继承了 EnumConverter，以支持第二种转换过程中的Description
属性.

1. 它将用逗号分隔的字符串拆分为字符串数组（string[]), 然后转换为长整型，并进行位"或",来
   得到表示枚举值的长整型值.
    
        ' 将字符串转换为枚举.
		Public Overrides Function ConvertFrom(context As ITypeDescriptorContext,
											  culture As CultureInfo,
											  value As Object) As Object
			If TypeOf value Is String Then
				Dim strValue As String = DirectCast(value, String)
				If strValue.IndexOf(enumSeperator) <> -1 Then
					Dim convertedValue As ULong = 0
					For Each v As String In strValue.Split(enumSeperator)
						convertedValue = convertedValue Or
							Convert.ToUInt64(Parse(Me.EnumType, v), culture)
					Next
					Return [Enum].ToObject(Me.EnumType, convertedValue)
				Else
					Return Parse(Me.EnumType, strValue)
				End If
			End If

			Return MyBase.ConvertFrom(context, culture, value)
		End Function


2. 它会读取给定枚举的自定义属性（在本例中为Description属性），并准备对应的字符串值.在标
   志枚举的情况下,它会捕捉Description attribute[]，并准备作为输出的以逗号分隔的字符串.

    ' 转换为字符串.
    Public Overrides Function ConvertTo(context As ITypeDescriptorContext,
                                        culture As CultureInfo,
                                        value As Object,
                                        destinationType As Type) As Object
        If destinationType = Nothing Then
            Throw New ArgumentNullException("destinationType")
        End If

        If destinationType = GetType(String) AndAlso value <> Nothing Then

            ' 如果没有定义value，或者该枚举不是标记类型，将引发一个参数异常.            '
            Dim underlyingType As Type = [Enum].GetUnderlyingType(Me.EnumType)
            If TypeOf value Is IConvertible AndAlso
                value.[GetType]() <> underlyingType Then
                value = (DirectCast(value, IConvertible)).ToType(underlyingType, culture)
            End If
            If Not Me.EnumType.IsDefined(GetType(FlagsAttribute), False) AndAlso
                Not [Enum].IsDefined(Me.EnumType, value) Then
                Throw New ArgumentException([String].Format(
                            "值 '{0}' 对枚举 '{1}' 而言，不是有效值.",
                            value.ToString(), Me.EnumType.Name))
            End If

            ' 如果枚举值被Description attribute修饰, 返回Description value; 
			' 否则返回名字.
            Dim enumName As String = [Enum].Format(Me.EnumType, value, "G")
            Dim nameOrDesc As String

            If enumName.IndexOf(enumSeperator) <> -1 Then
                ' 这是标记枚举. 用“，”拆分descriptions.
                Dim firstTime As Boolean = True
                Dim retval As New StringBuilder()

                For Each v As String In enumName.Split(enumSeperator)
                    nameOrDesc = v.Trim()

                    Dim info As FieldInfo = Me.EnumType.GetField(nameOrDesc)
                    Dim attrs As DescriptionAttribute() =
                        DirectCast(info.GetCustomAttributes(GetType(DescriptionAttribute),
                                                            False), 
                                                        DescriptionAttribute())
                    If attrs.Length > 0 Then
                        nameOrDesc = attrs(0).Description
                    End If

                    If firstTime Then
                        retval.Append(nameOrDesc)
                        firstTime = False
                    Else
                        retval.Append(enumSeperator)
                        retval.Append(" "c)
                        retval.Append(nameOrDesc)
                    End If
                Next

                Return retval.ToString()
            Else
                Dim info As FieldInfo = Me.EnumType.GetField(enumName)
                If info <> Nothing Then
                    Dim attrs As DescriptionAttribute() =
                       DirectCast(info.GetCustomAttributes(GetType(DescriptionAttribute),
                                                          False), DescriptionAttribute())
                    nameOrDesc = If((attrs.Length > 0), attrs(0).Description, enumName)
                Else
                    nameOrDesc = enumName
                End If
                Return nameOrDesc
            End If
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function


/////////////////////////////////////////////////////////////////////////////////////////
参考:

EnumConverter Class
http://msdn.microsoft.com/en-us/library/system.componentmodel.enumconverter.aspx

DescriptionAttribute Members
http://msdn.microsoft.com/en-us/library/system.componentmodel.descriptionattribute_members(v=VS.85).aspx

/////////////////////////////////////////////////////////////////////////////////////////