'**************************************** 模块头 **************************************\
' 模块名:  EnumDescriptionConverter.vb 
' 项目名:  VBEnumStringConverter
' 版权 (c) Microsoft Corporation. 
'
' 该文件提供了一个转换器：EnumDescriptionConverter, 用于枚举对象和由 DescriptionAttribute
' 指定的描述字符串之间的转换.该类继承自内置的类 EnumConverter. 与EnumConverter不同的是,
' 如果有 DescriptionAttribute 附加在枚举对象上时,EnumConverter不考虑DescriptionAttribute,
' 而 EnumDescriptionConverter 将枚举对象转换为它的描述字符串. 当用该类来将字符串转换为一个
' 枚举对象时,该类也尝试将该字符串与 DescriptionAttribute 中指定的描述字符串匹配起来.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'**************************************************************************************/


Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports System.Text


''' <summary>
''' 提供了一个类型转换器，用于枚举对象和用 DescriptionAttribute 指定的描述字符串之
''' 间的转换. 该类派生于内置的 EnumConverter 类. 与 EnumConverter 不同的是，如果有 
''' DescriptionAttribute 附加在枚举对象上时, EnumConverter 不考虑
''' DescriptionAttribute, 而 EnumDescriptionConverter 将枚举对象转换为它的描述字符 
''' 串. 当用 EnumDescriptionConverter 类来将字符串转换为枚举对象时，该类也会尝试,
''' 用 DescriptionAttribute 指定的描述字符串,来匹配该字符串.
''' </summary>
Public Class EnumDescriptionConverter
    Inherits EnumConverter
    Private Const enumSeperator As Char = ","c

    Public Sub New(type As Type)
        MyBase.New(type)
    End Sub


#Region " ConvertFrom (String)"

    ''' <summary>
    ''' 将给定的值转换为该转换器的枚举类型. 如果给定值是字符串，该方法会尝试将字符
    ''' 串的名字，与由每一个枚举对象的 DescriptionAttribute 指定的描述字符串，匹
    ''' 配起来. 
    ''' </summary>
    ''' <param name="context">
    ''' ITypeDescriptorContext 提供一个带格式的上下文信息.
    ''' </param>
    ''' <param name="culture">
    ''' 可选的CultureInfo. 如果没有提供，就假定为当前的文化信息.
    ''' </param>
    ''' <param name="value">要转换的枚举对象. </param>
    ''' <returns>
    ''' 用于表示转换值的枚举对象.
    ''' </returns>
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

    ''' <summary>
    ''' 将名称的字符串表示形式,或者由 DescriptionAttribute 指定的描述字符串,或
    ''' 者一个或多个枚举常数的数值,转换为一个等效的枚举对象.
    ''' </summary>
    ''' <param name="enumType">一个枚举类型.</param>
    ''' <param name="value">
    ''' 一个字符串,包含待转换的名字，或描述，或数值. 
    ''' </param>
    ''' <returns>
    ''' 一个 enumType 类型的对象，其值用值来表示.
    ''' </returns>
    ''' <remarks>
    ''' 不同于枚举. 忽视 DescriptionAttribute, Parse 方法也能将描述字符串转换为等
    ''' 效的枚举对象.
    ''' 
    ''' 字符串的比较非大小写敏感.
    ''' </remarks>
    Private Shared Function Parse(enumType As Type, value As String) As Object
        If enumType = Nothing Then
            Throw New ArgumentNullException("enumType")
        End If

        If Not enumType.IsEnum Then
            Throw New ArgumentException("提供的类型必须为枚举类型." & vbCr & vbLf &
                                        "参数名字: enumType")
        End If

        If value = Nothing Then
            Throw New ArgumentNullException("value")
        End If

        value = value.Trim()
        If value.Length = 0 Then
            Throw New ArgumentException(
                "必须指定有效信息以用于解析字符串.")
        End If

        If [Char].IsDigit(value(0)) OrElse value(0) = "-"c OrElse value(0) = "+"c Then
            Dim underlyingType As Type = [Enum].GetUnderlyingType(enumType)
            Dim temp As Object
            Try
                temp = Convert.ChangeType(value, underlyingType,
                                          CultureInfo.InvariantCulture)
                Return [Enum].ToObject(enumType, temp)
                ' 我们需要分析此字符串.
                ' 某些情况下，用类型库导入程序（Tlbimp)导入枚举，可以有"3D"格式.
            Catch generatedExceptionName As FormatException
            End Try
        End If

        Dim result As ULong = 0
        Dim dict As Dictionary(Of String, ULong) = GetStringToEnumDictionary(enumType)

        For Each v As String In value.Split(enumSeperator)
            Dim nameOrDesc As String = v.Trim()
            If dict.ContainsKey(nameOrDesc) Then
                result = result Or dict(nameOrDesc)
            Else
                Throw New ArgumentException([String].Format(
                                          "找不到请求的值 '{0}' .", value))
            End If
        Next

        Return [Enum].ToObject(enumType, result)
    End Function

    Private Shared fieldInfoHash As Hashtable = Hashtable.Synchronized(New Hashtable())
    Private Const maxHashElements As Integer = 100
    ' 用于调整工作集.
    Private Shared Function GetStringToEnumDictionary(enumType _
                                         As Type) As Dictionary(Of String, ULong)
        Debug.Assert(enumType <> Nothing)
        Debug.Assert(enumType.IsEnum)

        Dim dict As Dictionary(Of String, ULong) = DirectCast(fieldInfoHash(enumType), 
            Dictionary(Of String, ULong))
        If dict Is Nothing Then
            ' 当插入元素的个数已经达到阈值，清空哈希表，
            ' 以减少工作集.
            If fieldInfoHash.Count > maxHashElements Then
                fieldInfoHash.Clear()
            End If

            ' 用不区分大小写的字符串比较器来创建字典.
            dict = New Dictionary(Of String, ULong)(StringComparer.OrdinalIgnoreCase)

            Dim fields As FieldInfo() = enumType.GetFields(BindingFlags.[Static] Or
                                                           BindingFlags.[Public])
            For Each info As FieldInfo In fields
                Dim enumValue As ULong = Convert.ToUInt64(info.GetValue(info.Name))

                ' 添加 <name, enumValue> 对.
                dict.Add(info.Name, enumValue)

                ' 如果可以，添加 <description, enumValue> 对.
                Dim attrs As DescriptionAttribute() =
                    DirectCast(info.GetCustomAttributes(GetType(DescriptionAttribute),
                                                        False), DescriptionAttribute())
                If attrs.Length > 0 Then
                    dict.Add(attrs(0).Description, enumValue)
                End If
            Next

            fieldInfoHash.Add(enumType, dict)
        End If
        Return dict
    End Function


#End Region


#Region " CovertFrom (Enum Value)"

    ''' <summary>
    ''' 将给定的枚举值对象转换为指定的目的类型. 如果目的类型是字符串，当
    ''' DescriptionAttribute 附加于枚举值时，该方法会把枚举值转换为它的
    ''' 描述字符串.
    ''' </summary>
    ''' <param name="context">
    ''' ITypeDescriptorContext, 提供一个带格式的上下文信息.
    ''' </param>
    ''' <param name="culture">
    ''' 可选的CultureInfo. 如果没有提供它，就假定为当前的文化信息.
    ''' </param>
    ''' <param name="value"> 待转换的枚举对象. </param>
    ''' <param name="destinationType">
    ''' 该值要转换的目的类型.
    ''' </param>
    ''' <returns>
    ''' 用于表示转换值的对象.
    ''' </returns>
    Public Overrides Function ConvertTo(context As ITypeDescriptorContext,
                                        culture As CultureInfo,
                                        value As Object,
                                        destinationType As Type) As Object
        If destinationType = Nothing Then
            Throw New ArgumentNullException("destinationType")
        End If

        If destinationType = GetType(String) AndAlso value <> Nothing Then
            ' 如果没有定义value，或者该枚举不是标记类型，将引发一个参数异常. 
            '
            Dim underlyingType As Type = [Enum].GetUnderlyingType(Me.EnumType)
            If TypeOf value Is IConvertible AndAlso
                value.[GetType]() <> underlyingType Then
                value = (DirectCast(value, IConvertible)).ToType(underlyingType, culture)
            End If
            If Not Me.EnumType.IsDefined(GetType(FlagsAttribute), False) AndAlso
                Not [Enum].IsDefined(Me.EnumType, value) Then
                Throw New ArgumentException([String].Format(
                            "The value '{0}' is not a valid value for the enum '{1}'.",
                            value.ToString(), Me.EnumType.Name))
            End If

            ' 如果枚举值被Description attribute修饰, 返回Description value; 
            ' 否则返回名字.
            Dim enumName As String = [Enum].Format(Me.EnumType, value, "G")
            Dim nameOrDesc As String

            If enumName.IndexOf(enumSeperator) <> -1 Then
                ' 这是一个标记枚举. 用“，”拆分 descriptions.
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
#End Region


End Class
