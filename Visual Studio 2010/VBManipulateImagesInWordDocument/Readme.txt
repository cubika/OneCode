================================================================================
				Windows应用程序：VBManipulateImagesInWordDocument                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////

摘要：
这个实例演示了怎么样用Open XML SDK 在Word文档中导出、删除、替换图片。

/////////////////////////////////////////////////////////////////////////////
准备条件

Open XML SDK 2.0

你可以在下面的连接中下载：
http://www.microsoft.com/downloads/en/details.aspx?FamilyId=C6E744E5-36E9-45F5-8D8C-331DF206E0D0&displaylang=en


////////////////////////////////////////////////////////////////////////////////
演示:

步骤1：在Visuo Studio 2010打开这个项目
        
步骤2：建立解决方案 

步骤3：运行 VBManipulateImagesInWordDocument.exe.

步骤4：点击 "打开Word文档" 按钮 然后在对话框中选中一个Word 07/10 文档
       (*.docx file)。下面的listbox 会显示所有文档中的图片的引用ID。

步骤5. 在listbox中点击一项, 你会发现图片会显示在右边的面板中。

步骤6. 点击 "导出" 按钮，程序会显示一个SaveFileDialog，然后你就可以可这个图片保存在本地

步骤7. 点击"删除"按钮，程序会跳出一个提醒，如果你确定删除，程序会删除该图片。 
       关闭这个程序，然后在Word中打开这个文档，你会发现这个图片已经被删除了

步骤8. 再次运行步骤3，步骤4，步骤5。点击"替换"按钮，程序会显示一个 OpenFileDialog. 从本地选择
       一个图片并且确定替换。 程序会替换这个图片。关闭这个程序，在Word中打开这个文档，你会发现
       这个图片已经被替换了。

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

在文档中的图片数据储存为一个 ImagePart, 并且在 Drawing 元素中的 Blip 元素
会引用到这个 ImagePart，就像下面的结构：

<w:drawing>
  <wp:inline>  
    <a:graphic>
      <a:graphicData>
        <pic:pic>
          <pic:blipFill>
            <a:blip r:embed="rId7">
              <a:extLst>
                <a:ext uri="{28A0092B-C50C-407E-A947-70E740481C1C}">
                  <a14:useLocalDpi val="0" />
                </a:ext>
              </a:extLst>
            </a:blip>
          </pic:blipFill>
        </pic:pic>
      </a:graphicData>
    </a:graphic>
  </wp:inline>
</w:drawing>

A. 要想罗列出文档中的所有图片，我们需要先获取Drawing元素，然后在Drawing元素中得到Blip元素

    Public Function GetAllImages() As IEnumerable(Of Blip)

        ' 获取文档中的 Drawing 元素
        Dim drawingElements = From run In Document.MainDocumentPart.Document.
                                  Descendants(Of DocumentFormat.OpenXml.Wordprocessing.Run)()
                              Where run.Descendants(Of Drawing)().Count() <> 0
                              Select run.Descendants(Of Drawing)().First()

        ' 获取Drawing 元素中的 Blip 元素
        Dim blipElements = From drawing In drawingElements
                           Where drawing.Descendants(Of Blip)().Count() > 0
                           Select drawing.Descendants(Of Blip)().First()

        Return blipElements
    End Function

B. 要想删除图片，我们可以通过把含有 Blip 元素的 Drawing 元素删除
    Public Sub DeleteImage(ByVal blipElement As Blip)
        Dim parent As OpenXmlElement = blipElement.Parent
        Do While parent IsNot Nothing _
            AndAlso Not (TypeOf parent Is DocumentFormat.OpenXml.Wordprocessing.Drawing)
            parent = parent.Parent
        Loop

        If parent IsNot Nothing Then
            Dim drawing_Renamed As Drawing = TryCast(parent, Drawing)
            drawing_Renamed.Parent.RemoveChild(Of Drawing)(drawing_Renamed)

            ' 触发  ImagesChanged 事件.
            Me.OnImagesChanged()

        End If
    End Sub


C. 想要文档中的图片，我们需要先添加一个 ImagePart 到文档中，
   然后编辑 Blip 元素使它引用新的 ImagePart.

    Public Sub ReplaceImage(ByVal blipElement As Blip, ByVal newImg As FileInfo)
        Dim rid As String = AddImagePart(newImg)
        blipElement.Embed.Value = rid
        Me.OnImagesChanged()
    End Sub

     Private Function AddImagePart(ByVal newImg As FileInfo) As String
        Dim type As ImagePartType = ImagePartType.Bmp
        Select Case newImg.Extension.ToLower()
            Case ".jpeg", ".jpg"
                type = ImagePartType.Jpeg
            Case ".png"
                type = ImagePartType.Png
            Case Else
                type = ImagePartType.Bmp
        End Select

        Dim newImgPart As ImagePart = Document.MainDocumentPart.AddImagePart(type)
        Using stream As FileStream = newImg.OpenRead()
            newImgPart.FeedData(stream)
        End Using

        Dim rId As String = Document.MainDocumentPart.GetIdOfPart(newImgPart)
        Return rId
    End Function

D. 因为我们已经把图片设置成为 PictureBox的 Image 属性, 我们只要使用
   Image.Save 方法就可以把图片导出到本地。

    picView.Image.Save(dialog.FileName, ImageFormat.Jpeg);

/////////////////////////////////////////////////////////////////////////////
参考：

Welcome to the Open XML SDK 2.0 for Microsoft Office
http://msdn.microsoft.com/en-us/library/bb448854.aspx

WordprocessingDocument Class
http://msdn.microsoft.com/en-us/library/documentformat.openxml.packaging.wordprocessingdocument.aspx

ImagePart Class
http://msdn.microsoft.com/en-us/library/documentformat.openxml.packaging.imagepart.aspx
/////////////////////////////////////////////////////////////////////////////