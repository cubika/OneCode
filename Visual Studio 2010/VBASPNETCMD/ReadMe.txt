========================================================================
    Web 应用程序 : VBASPNETCMD Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

本示例演示了如何绑定一个ASP.NET CMD在ASP.NET中运行批处理文件或者命令（命令行）. 
以及如何与其交互.


/////////////////////////////////////////////////////////////////////////////
演示:


步骤 1. 右击RunCmd.aspx, 选择"在浏览器中打开".

步骤 2. 首先输入你的验证信息. 包括:
        DomainName, UserName 和 Password. 
        
步骤 3. 然后,你可以通过两种不同的方式输入你的命令.

        1) 单击"浏览...", 选择本机上的一个.bat文件,然后单击"上传并运行" 按钮.

		2) 直接在TextBox中输入命令,然后单击"运行命令" 按钮.


步骤 4. 如果一切正常, 你可以在输出文本区域看到结果.


/////////////////////////////////////////////////////////////////////////////
生成:

步骤 1. 创建一个新的名为VBASPNETCMD的VB ASP.NET空白Web应用程序.

步骤 2. 增加App_Code文件夹, 添加一个名为BatchRunner的类, 添加一些字段和属性,
        implement ExecuteBatch method.
           Try
            Dim psi As System.Diagnostics.ProcessStartInfo = GenerateProcessInfo(fileName)

            Dim processBatch As System.Diagnostics.Process = System.Diagnostics.Process.Start(psi)
            Dim myOutput As System.IO.StreamReader = processBatch.StandardOutput

            processBatch.WaitForExit()
            If processBatch.HasExited Then
                retOutPut = myOutput.ReadToEnd()
            End If
        Catch winException As System.ComponentModel.Win32Exception
            If winException.Message.Contains("bad password") Then
                retOutPut = winException.Message
            Else
                retOutPut = "Win32Exception occured"
                ' Log exception information
            End If
        Catch exception As System.Exception
            ' Log exception information
            retOutPut = exception.Message
        End Try

步骤 3. 增加一个名为RunCmd.aspx的aspx页面, 向其添加些控件, 并向这些控件添加验证器.

步骤 4. 在RunCmd.aspx.vb中, 编写处理按钮事件的代码.

            Protected Sub btnRunBatch_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Upload file
        Dim fileName As String = HttpContext.Current.Server.MapPath("Batchs\" & System.Guid.NewGuid().ToString() & "_" & FileUpload.FileName)
        FileUpload.SaveAs(fileName)

        ' Run this batch file
        Dim output As String = RunBatch(fileName)
        ' Set result
        tbResult.Text = output

        ' Delete temp file
        System.IO.File.Delete(fileName)
    End Sub

    Protected Sub btnRunCmd_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Create a batch for these command
        Dim commandLine As String = Me.tbCommand.Text
        Dim fileName As String = HttpContext.Current.Server.MapPath("Batchs\" & System.Guid.NewGuid().ToString() & ".bat")
        Using sw As StreamWriter = System.IO.File.CreateText(fileName)
            sw.Write(commandLine)
            sw.Flush()
        End Using

        ' Run this batch file
        Dim output As String = RunBatch(fileName)
        ' Set result
        tbResult.Text = output

        ' Delete temp file
        System.IO.File.Delete(fileName)
    End Sub



/////////////////////////////////////////////////////////////////////////////
参考资料:

ProcessStartInfo 类
http://msdn.microsoft.com/zh-cn/library/system.diagnostics.processstartinfo(VS.81).aspx

/////////////////////////////////////////////////////////////////////////////