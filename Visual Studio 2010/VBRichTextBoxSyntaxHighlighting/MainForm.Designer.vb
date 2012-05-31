
Partial Public Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Dim XmlViewerSettings1 As VBRichTextBoxSyntaxHighlighting.XMLViewerSettings = New VBRichTextBoxSyntaxHighlighting.XMLViewerSettings()
        Me.pnlMenu = New System.Windows.Forms.Panel()
        Me.lbNote = New System.Windows.Forms.Label()
        Me.btnProcess = New System.Windows.Forms.Button()
        Me.viewer = New VBRichTextBoxSyntaxHighlighting.XMLViewer()
        Me.pnlMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlMenu
        '
        Me.pnlMenu.Controls.Add(Me.lbNote)
        Me.pnlMenu.Controls.Add(Me.btnProcess)
        Me.pnlMenu.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlMenu.Location = New System.Drawing.Point(0, 0)
        Me.pnlMenu.Name = "pnlMenu"
        Me.pnlMenu.Size = New System.Drawing.Size(775, 79)
        Me.pnlMenu.TabIndex = 1
        '
        'lbNote
        '
        Me.lbNote.AutoSize = True
        Me.lbNote.Location = New System.Drawing.Point(12, 9)
        Me.lbNote.Name = "lbNote"
        Me.lbNote.Size = New System.Drawing.Size(353, 60)
        Me.lbNote.TabIndex = 2
        Me.lbNote.Text = "将 xml 脚本复制、 粘贴到下面的 RichTextBox中，然后按""处理""" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "注：" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "1.此查看器并不支持有 Namespace 的 Xml 文件" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "2.此" & _
            "查看器将忽略 XML 中的注释。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "3.某些字符应进行编码，像 &&-&"
        '
        'btnProcess
        '
        Me.btnProcess.Location = New System.Drawing.Point(420, 38)
        Me.btnProcess.Name = "btnProcess"
        Me.btnProcess.Size = New System.Drawing.Size(75, 23)
        Me.btnProcess.TabIndex = 1
        Me.btnProcess.Text = "处理"
        Me.btnProcess.UseVisualStyleBackColor = True
        '
        'viewer
        '
        Me.viewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.viewer.Location = New System.Drawing.Point(0, 79)
        Me.viewer.Name = "viewer"
        XmlViewerSettings1.AttributeKey = System.Drawing.Color.Red
        XmlViewerSettings1.AttributeValue = System.Drawing.Color.Blue
        XmlViewerSettings1.Element = System.Drawing.Color.DarkRed
        XmlViewerSettings1.Tag = System.Drawing.Color.Blue
        XmlViewerSettings1.Value = System.Drawing.Color.Black
        Me.viewer.Settings = XmlViewerSettings1
        Me.viewer.Size = New System.Drawing.Size(775, 381)
        Me.viewer.TabIndex = 0
        Me.viewer.Text = "<?xml version=""1.0"" encoding=""utf-8"" ?><html><head><title>My home page</title></h" & _
            "ead><body bgcolor=""000000"" text=""ff0000"">Hello World!</body></html>" & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(775, 460)
        Me.Controls.Add(Me.viewer)
        Me.Controls.Add(Me.pnlMenu)
        Me.Name = "MainForm"
        Me.Text = "SimpleXMLViewer"
        Me.pnlMenu.ResumeLayout(False)
        Me.pnlMenu.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private viewer As XMLViewer
    Private pnlMenu As Panel
    Private WithEvents btnProcess As Button
    Private lbNote As Label
End Class

