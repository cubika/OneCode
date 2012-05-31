
Partial Public Class MainForm
    ''' <summary>
    ''' 必须的编辑器变量.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' 清除任何正在使用的资源.
    ''' </summary>
    ''' <param name="disposing">被管理的资源需要被销毁是为真;反之,为假.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "WinFowm设计其产生的代码"

    ''' <summary>
    ''' 编辑器支持所必须的方法 - 请不要用代码编辑器修改此方法的内容
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlMenu = New System.Windows.Forms.Panel()
        Me.chkSuppressAllDialog = New System.Windows.Forms.CheckBox()
        Me.chkSuppressNavigationError = New System.Windows.Forms.CheckBox()
        Me.tbUrl = New System.Windows.Forms.TextBox()
        Me.chkSuppressJITDebugger = New System.Windows.Forms.CheckBox()
        Me.chkSuppressHtmlElementError = New System.Windows.Forms.CheckBox()
        Me.btnNavigate = New System.Windows.Forms.Button()
        Me.lbUrl = New System.Windows.Forms.Label()
        Me.pnlBrowser = New System.Windows.Forms.Panel()
        Me.wbcSample = New VBWebBrowserSuppressError.WebBrowserEx()
        Me.pnlMenu.SuspendLayout()
        Me.pnlBrowser.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlMenu
        '
        Me.pnlMenu.Controls.Add(Me.chkSuppressAllDialog)
        Me.pnlMenu.Controls.Add(Me.chkSuppressNavigationError)
        Me.pnlMenu.Controls.Add(Me.tbUrl)
        Me.pnlMenu.Controls.Add(Me.chkSuppressJITDebugger)
        Me.pnlMenu.Controls.Add(Me.chkSuppressHtmlElementError)
        Me.pnlMenu.Controls.Add(Me.btnNavigate)
        Me.pnlMenu.Controls.Add(Me.lbUrl)
        Me.pnlMenu.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlMenu.Location = New System.Drawing.Point(0, 0)
        Me.pnlMenu.Name = "pnlMenu"
        Me.pnlMenu.Size = New System.Drawing.Size(824, 73)
        Me.pnlMenu.TabIndex = 1
        '
        'chkSuppressAllDialog
        '
        Me.chkSuppressAllDialog.AutoSize = True
        Me.chkSuppressAllDialog.Location = New System.Drawing.Point(185, 44)
        Me.chkSuppressAllDialog.Name = "chkSuppressAllDialog"
        Me.chkSuppressAllDialog.Size = New System.Drawing.Size(119, 17)
        Me.chkSuppressAllDialog.TabIndex = 7
        Me.chkSuppressAllDialog.Text = "忽略所有对话框"
        Me.chkSuppressAllDialog.UseVisualStyleBackColor = True
        '
        'chkSuppressNavigationError
        '
        Me.chkSuppressNavigationError.AutoSize = True
        Me.chkSuppressNavigationError.Location = New System.Drawing.Point(314, 44)
        Me.chkSuppressNavigationError.Name = "chkSuppressNavigationError"
        Me.chkSuppressNavigationError.Size = New System.Drawing.Size(149, 17)
        Me.chkSuppressNavigationError.TabIndex = 6
        Me.chkSuppressNavigationError.Text = "忽略链接错误"
        Me.chkSuppressNavigationError.UseVisualStyleBackColor = True
        '
        'tbUrl
        '
        Me.tbUrl.Dock = System.Windows.Forms.DockStyle.Top
        Me.tbUrl.Location = New System.Drawing.Point(0, 13)
        Me.tbUrl.Name = "tbUrl"
        Me.tbUrl.Size = New System.Drawing.Size(824, 20)
        Me.tbUrl.TabIndex = 5
        '
        'chkSuppressJITDebugger
        '
        Me.chkSuppressJITDebugger.AutoSize = True
        Me.chkSuppressJITDebugger.Location = New System.Drawing.Point(469, 44)
        Me.chkSuppressJITDebugger.Name = "chkSuppressJITDebugger"
        Me.chkSuppressJITDebugger.Size = New System.Drawing.Size(138, 17)
        Me.chkSuppressJITDebugger.TabIndex = 2
        Me.chkSuppressJITDebugger.Text = "忽略JIT调试器"
        Me.chkSuppressJITDebugger.UseVisualStyleBackColor = True
        '
        'chkSuppressHtmlElementError
        '
        Me.chkSuppressHtmlElementError.AutoSize = True
        Me.chkSuppressHtmlElementError.Location = New System.Drawing.Point(12, 44)
        Me.chkSuppressHtmlElementError.Name = "chkSuppressHtmlElementError"
        Me.chkSuppressHtmlElementError.Size = New System.Drawing.Size(165, 17)
        Me.chkSuppressHtmlElementError.TabIndex = 2
        Me.chkSuppressHtmlElementError.Text = "忽略Html元素错误"
        Me.chkSuppressHtmlElementError.UseVisualStyleBackColor = True
        '
        'btnNavigate
        '
        Me.btnNavigate.Location = New System.Drawing.Point(733, 43)
        Me.btnNavigate.Name = "btnNavigate"
        Me.btnNavigate.Size = New System.Drawing.Size(79, 23)
        Me.btnNavigate.TabIndex = 1
        Me.btnNavigate.Text = "开始"
        Me.btnNavigate.UseVisualStyleBackColor = True
        '
        'lbUrl
        '
        Me.lbUrl.AutoSize = True
        Me.lbUrl.Dock = System.Windows.Forms.DockStyle.Top
        Me.lbUrl.Location = New System.Drawing.Point(0, 0)
        Me.lbUrl.Name = "lbUrl"
        Me.lbUrl.Size = New System.Drawing.Size(225, 13)
        Me.lbUrl.TabIndex = 8
        Me.lbUrl.Text = "URL (置空以便载入内部测试html页面)"
        '
        'pnlBrowser
        '
        Me.pnlBrowser.Controls.Add(Me.wbcSample)
        Me.pnlBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBrowser.Location = New System.Drawing.Point(0, 73)
        Me.pnlBrowser.Name = "pnlBrowser"
        Me.pnlBrowser.Size = New System.Drawing.Size(824, 538)
        Me.pnlBrowser.TabIndex = 2
        '
        'wbcSample
        '
        Me.wbcSample.Dock = System.Windows.Forms.DockStyle.Fill
        Me.wbcSample.HtmlElementErrorsSuppressed = False
        Me.wbcSample.Location = New System.Drawing.Point(0, 0)
        Me.wbcSample.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wbcSample.Name = "wbcSample"
        Me.wbcSample.Size = New System.Drawing.Size(824, 538)
        Me.wbcSample.TabIndex = 0
        '
        'MainForm
        '
        Me.AcceptButton = Me.btnNavigate
        Me.ClientSize = New System.Drawing.Size(824, 611)
        Me.Controls.Add(Me.pnlBrowser)
        Me.Controls.Add(Me.pnlMenu)
        Me.Name = "MainForm"
        Me.Text = "VBWebBrowserSuppressError"
        Me.pnlMenu.ResumeLayout(False)
        Me.pnlMenu.PerformLayout()
        Me.pnlBrowser.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub


#End Region


    Private pnlMenu As Panel
    Private tbUrl As TextBox
    Private WithEvents chkSuppressHtmlElementError As CheckBox
    Private WithEvents btnNavigate As Button
    Private chkSuppressJITDebugger As CheckBox
    Private pnlBrowser As Panel
    Private wbcSample As WebBrowserEx
    Private chkSuppressNavigationError As CheckBox
    Private WithEvents chkSuppressAllDialog As CheckBox
    Friend WithEvents lbUrl As System.Windows.Forms.Label
End Class


