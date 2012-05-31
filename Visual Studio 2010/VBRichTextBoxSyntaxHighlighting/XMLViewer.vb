'*************************** 模块头 ******************************'
' 模块名:  XMLViewer.vb
' 项目名:	    VBRichTextBoxSyntaxHighlighting
' 版权(c) Microsoft Corporation.
' 
' XMLViewer类继承自System.Windows.Forms.RichTextBox，这个类是以规范的格式来显示xml文件 
' 
' RichTextBox使用RTF格式来显示测试。XMLViewer类通过使用在XMLViewSettings中规定的一些
' 格式来实现xml到RTF的转换，然后设置RTF属性为这个值。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Text

Public Class XMLViewer
    Inherits RichTextBox
   
    Private _settings As XMLViewerSettings
    ''' <summary>
    ''' 格式设置
    ''' </summary>
    Public Property Settings() As XMLViewerSettings
        Get
            If _settings Is Nothing Then
                _settings = New XMLViewerSettings With {
                    .AttributeKey = Color.Red,
                    .AttributeValue = Color.Blue,
                    .Tag = Color.Blue,
                    .Element = Color.DarkRed,
                    .Value = Color.Black}
            End If
            Return _settings
        End Get
        Set(ByVal value As XMLViewerSettings)
            _settings = value
        End Set
    End Property

    ''' <summary>
    ''' 通过XMLViewerSettings中的一个规范格式，xml文件转换到RTF
    ''' 然后设置RTF属性的值
    ''' </summary>
    ''' <param name="includeDeclaration">
    ''' 指定是否包括声明
    ''' </param>
    Public Sub Process(ByVal includeDeclaration As Boolean)
        Try
            'RTF包含头和内容两个部分，colortbl作为头的一部分，{1}将要用被内容替换
            Dim rtfFormat As String = "{{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe2052" _
                                      & "{{\fonttbl{{\f0\fnil Courier New;}}}}" _
                                      & "{{\colortbl ;{0}}}" _
                                      & "\viewkind4\uc1\pard\lang1033\f0 {1}}}"

            '  从Text属性中获取XDocument
            Dim xmlDoc = XDocument.Parse(Me.Text, LoadOptions.None)

            Dim xmlRtfContent As New StringBuilder()

            ' 如果includeDeclaration是真，并且Document包含声明，那么将这个声明添加
            ' 到RTF的内容中。
            If includeDeclaration AndAlso xmlDoc.Declaration IsNot Nothing Then

                ' 在XMLViewerSettings中的常量，用来指定这个在RTF中colortbl的顺序
                xmlRtfContent.AppendFormat(
                    "\cf{0} <?\cf{1} xml \cf{2} version\cf{0} =\cf0 ""\cf{3} {4}\cf0 "" " _
                    & "\cf{2} encoding\cf{0} =\cf0 ""\cf{3} {5}\cf0 ""\cf{0} ?>\par",
                    XMLViewerSettings.TagID,
                    XMLViewerSettings.ElementID,
                    XMLViewerSettings.AttributeKeyID,
                    XMLViewerSettings.AttributeValueID,
                    xmlDoc.Declaration.Version,
                    xmlDoc.Declaration.Encoding)
            End If

            ' 得到根元素的RTF
            Dim rootRtfContent As String = ProcessElement(xmlDoc.Root, 0)

            xmlRtfContent.Append(rootRtfContent)

            '构造完整的RTF，设置RTF的属性给这个值。
            Me.Rtf = String.Format(rtfFormat, Settings.ToRtfFormatString(),
                                   xmlRtfContent.ToString())


        Catch xmlException As System.Xml.XmlException
            Throw New ApplicationException("Please check the input Xml. Error:" _
                                           & xmlException.Message, xmlException)
        Catch
            Throw
        End Try
    End Sub

    ' 获取xml元素中的RTF
    Private Function ProcessElement(ByVal element As XElement,
                                    ByVal level As Integer) As String

        ' viewer不支持有命名空间的XML文件
        If Not String.IsNullOrEmpty(element.Name.Namespace.NamespaceName) Then
            Throw New ApplicationException(
                "This viewer does not support the Xml file that has Namespace.")
        End If

        Dim elementRtfFormat As String = String.Empty
        Dim childElementsRtfContent As New StringBuilder()
        Dim attributesRtfContent As New StringBuilder()

        ' 构造indent字符串
        Dim indent As New String(" "c, 4 * level)

        ' 如果元素有子元素和值, 添加这个元素到RTF中
        '  {{0}} 将要被替换为这些属性， {{1}} 将要被替换为子元素或值
        If element.HasElements OrElse (Not String.IsNullOrWhiteSpace(element.Value)) Then
            elementRtfFormat =
                String.Format("{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} >\par" _
                              & "{{1}}" _
                              & "{0}\cf{1} </\cf{2} {3}\cf{1} >\par",
                              indent,
                              XMLViewerSettings.TagID,
                              XMLViewerSettings.ElementID,
                              element.Name)

            ' 构造RTF子元素
            If element.HasElements Then
                For Each childElement In element.Elements()
                    Dim childElementRtfContent As String =
                        ProcessElement(childElement, level + 1)
                    childElementsRtfContent.Append(childElementRtfContent)
                Next childElement

                ' 如果 !string.IsNullOrWhiteSpace(element.Value), 然后构造RTF的值
            Else
                childElementsRtfContent.AppendFormat(
                    "{0}\cf{1} {2}\par",
                    New String(" "c, 4 * (level + 1)),
                    XMLViewerSettings.ValueID,
                    CharacterEncoder.Encode(element.Value.Trim()))
            End If

            ' 元素只有这些属性. {{0}} 将要被这些属性替换.
        Else
            elementRtfFormat = String.Format("{0}\cf{1} <\cf{2} {3}{{0}}\cf{1} />\par",
                                             indent,
                                             XMLViewerSettings.TagID,
                                             XMLViewerSettings.ElementID,
                                             element.Name)
        End If

        ' 构造RTF的属性
        If element.HasAttributes Then
            For Each attribute As XAttribute In element.Attributes()
                Dim attributeRtfContent As String =
                    String.Format(" \cf{0} {3}\cf{1} =\cf0 ""\cf{2} {4}\cf0 """,
                                  XMLViewerSettings.AttributeKeyID,
                                  XMLViewerSettings.TagID,
                                  XMLViewerSettings.AttributeValueID,
                                  attribute.Name,
                                  CharacterEncoder.Encode(attribute.Value))
                attributesRtfContent.Append(attributeRtfContent)
            Next attribute
            attributesRtfContent.Append(" ")
        End If

        Return String.Format(elementRtfFormat, attributesRtfContent, childElementsRtfContent)
    End Function

End Class
