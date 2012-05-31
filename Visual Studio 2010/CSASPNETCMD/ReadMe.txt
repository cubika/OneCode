========================================================================
             Web 应用程序 : CSASPNETCMD 项目概述
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

步骤 1. 创建一个新的名为CSASPNETCMD的C# ASP.NET空白Web应用程序.

步骤 2. 增加App_Code文件夹, 添加一个名为BatchRunner的类, 添加一些字段和属性,
        implement ExecuteBatch method.
            try
            {
                System.Diagnostics.ProcessStartInfo psi = GenerateProcessInfo(fileName);

                System.Diagnostics.Process processBatch = System.Diagnostics.Process.Start(psi);
                System.IO.StreamReader myOutput = processBatch.StandardOutput;

                processBatch.WaitForExit();
                if (processBatch.HasExited)
                {
                    retOutPut = myOutput.ReadToEnd();
                }
            }

步骤 3. 增加一个名为RunCmd.aspx的aspx页面, 向其添加些控件, 并向这些控件添加验证器.

步骤 4. 在RunCmd.aspx.cs中, 编写处理按钮事件的代码.

        protected void btnRunBatch_Click(object sender, EventArgs e)
        {

            // Upload file
            string fileName = HttpContext.Current.Server.MapPath(@"Batchs\" + System.Guid.NewGuid().ToString() + "_" + fileUpload.FileName);
            fileUpload.SaveAs(fileName);

            // Run this batch file.
            string output = RunBatch(fileName);
            // Set result
            tbResult.Text = output;

            // Delete temp file.
            System.IO.File.Delete(fileName);
        }

        protected void btnRunCmd_Click(object sender, EventArgs e)
        {

            // Create a batch for these command.
            string commandLine = this.tbCommand.Text;
            string fileName = HttpContext.Current.Server.MapPath(@"Batchs\"+System.Guid.NewGuid().ToString() + ".bat");
            using (StreamWriter sw = System.IO.File.CreateText(fileName))
            {
                sw.Write(commandLine);
                sw.Flush();
            }

            // Run this batch file.
            string output = RunBatch(fileName);

            // Set result
            tbResult.Text = output;

            // Delete temp file.
            System.IO.File.Delete(fileName);
        }



/////////////////////////////////////////////////////////////////////////////
参考资料:

Processstartinfo 类 
http://msdn.microsoft.com/zh-cn/library/system.diagnostics.processstartinfo(VS.81).aspx

/////////////////////////////////////////////////////////////////////////////