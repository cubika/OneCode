========================================================================================
                      项目名称: VBVSXSaveProject
========================================================================================

////////////////////////////////////////////////////////////////////////////////////////
摘要

此示例演示：通过集成开发环境的菜单功能，把现有的项目保存到不同的位置。它提供以下功能：
1、整个项目复制到一个新的位置。
2、提供让用户选择文件的服务。
3、在当前集成开发环境打开新项目。

注意：此范例只支持对该项目文件夹下的文件进行复制。
  
////////////////////////////////////////////////////////////////////////////////////////
要求

Visual Studio 2010 和 Visual Studio 2010 SDK.

你可以从以下链接下载Visual Studio 2010 SDK
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=47305CF4-2BEA-43C0-91CD-1B853602DCC5


////////////////////////////////////////////////////////////////////////////////////////
演示:

第一步、 在VS2010中打开该项目。

第二步、 将VBVSXSaveProject设置为启动项目，并打开其属性页。

         1. 选择“调试”选项卡。设置启动选项来启动外部程序和浏览devenv.exe
	       （默认位置是C：\ PROGRAM FILES \微软Visual Studio10.0\ Common7\ IDE\ devenv.exe），
	        并添加“/rootsuffix Exp”（没有引号）命令行参数。
       
         2. 选择VSIX选项卡，确保“在生成时创建VSIX容器”和“部署VSIX中的内容到实验实例以便调试”
	        选项被选中。

第三步、 生成解决方案。

第四步、 按F5，一个VS2010的实验实例将会启动。
       
         单击“工具”=>“插件管理器”，你会发现被加载的VBVSXSaveProject。

第五步、 在VS实验实例中打开一个现有的项目。
       
第六步、 在解决方案资源管理器中右键单击项目节点，然后单击菜单中的“VBVSXSaveProject”。
         “保存项目”对话框将显示。
       
第七步、 在“保存项目”对话框中选择要复制的文件，选中“打开新的项目”
         然后单击“另存为”按钮，选择一个目录。

         你会发现当前VS打开了这个新项目。

////////////////////////////////////////////////////////////////////////////////////////
代码逻辑：

一、创建一个名为VBVSXSaveProjectVS包。

    详细介绍：  http://msdn.microsoft.com/en-us/library/cc138589.aspx


二、将命令添加到文件菜单栏和解决方案资源管理器的上下文菜单栏。
        
		
   1. 往一个文件菜单栏中添加一个菜单项
         <Group guid="guidVBVSXSaveProjectCmdSet" id="VBVSXSaveProjectGroup" priority="0x0600">
              <Parent guid="guidSHLMainMenu" id="IDM_VS_MENU_FILE"/>
            </Group>
          
         <Button guid="guidVBVSXSaveProjectCmdSet" id="cmdidVBVSXSaveProjectCommandID"
              priority="0x0100" type="Button">
             <Parent guid="guidVBVSXSaveProjectCmdSet" id="VBVSXSaveProjectGroup" />
             <Icon guid="guidImages" id="bmpPic1" />
             <Strings>
               <CommandName>cmdidVBVSXSaveProjectCommandID</CommandName>
               <ButtonText>VBVSXSaveProject</ButtonText>
             </Strings>
         </Button>
        
         <!--Dynamic visibility-->
         <VisibilityConstraints>
            <VisibilityItem guid="guidVBVSXSaveProjectCmdSet" 
         	id="cmdidVBVSXSaveProjectCommandID" context="UICONTEXT_SolutionExists"/>
         </VisibilityConstraints>

   2. 将命令添加到“解决方案资源管理器上下文菜单”。
      
        <Group guid="guidVBVSXSaveProjectContextCmdSet" id="menuidVBVSXSaveProjectContextGroup" priority="0x01">
          <Parent guid="guidSolutionExplorerMenu" id="menuidSolutionExplorerMenu"/>
        </Group>

		<Button guid="guidVBVSXSaveProjectCmdSet" id="cmdidVBVSXSaveProjectCommandID"
		    priority="0x0100" type="Button">
			<Parent guid="guidVBVSXSaveProjectCmdSet" id="VBVSXSaveProjectGroup" />
			<Icon guid="guidImages" id="bmpPic1" />
        
			<!--Add the dynamic visibility about the command menu.-->
			<CommandFlag>DynamicVisibility</CommandFlag>
        
			<Strings>
			  <CommandName>cmdidVBVSXSaveProjectCommandID</CommandName>
			  <ButtonText>VBVSXSaveProject</ButtonText>
			</Strings>
		 </Button>	

三、从项目文件夹或项目中获取包含的文件。
   
   1. 获取项目文件夹中的文件。
	      Public Shared Function GetFilesInProjectFolder(
		          ByVal projectFilePath As String) As List(Of ProjectFileItem)
              ' 获取包括项目文件的文件夹
            Dim projFile As New FileInfo(projectFilePath)
            Dim projFolder As DirectoryInfo = projFile.Directory

            If projFolder.Exists Then
                ' 获取项目文件夹中的所有文件的信息
                Dim files = projFolder.GetFiles("*", SearchOption.AllDirectories)

                Return files.Select(Function(f) New ProjectFileItem With _
                                                {.Fileinfo = f,
                                                 .IsUnderProjectFolder = True}).ToList()
            Else
                ' 该项目文件夹不存在
                Return Nothing
            End If
        End Function
    
   2. 获取项目中的文件。

          Public Function GetIncludedFiles() As List(Of ProjectFileItem)
              Dim files = New List(Of ProjectFileItem)()

            ' 向文件列表中增加项目文件(*.csproj 或者 *.vbproj...)
            files.Add(New ProjectFileItem With _
                      {.Fileinfo = New FileInfo(Project.FullName),
                       .NeedCopy = True,
                       .IsUnderProjectFolder = True})

            '增加项目中包含的文件
            For Each item As ProjectItem In Project.ProjectItems
                GetProjectItem(item, files)
            Next item

            Return files
        End Function

        Private Sub GetProjectItem(ByVal item As ProjectItem, ByVal files As List(Of ProjectFileItem))
            ' 获取与一个项目项相关的文件。
            ' 大部分的工程项目只包括一个文件，但有些的可能不止一个，
            ' 在Visual Basic中有两种文件形式保存.FRM（文本)和.frx（二进制）文件。
            ' 详情查看 http://msdn.microsoft.com/en-us/library/envdte.projectitem.filecount.aspx
            For i As Short = 0 To item.FileCount - 1
                If File.Exists(item.FileNames(i)) Then
                    Dim fileItem As New ProjectFileItem()

                    fileItem.Fileinfo = New FileInfo(item.FileNames(i))

                    If fileItem.FullName.StartsWith(Me.ProjectFolder.FullName,
                                                    StringComparison.OrdinalIgnoreCase) Then
                        fileItem.IsUnderProjectFolder = True
                        fileItem.NeedCopy = True
                    End If

                    files.Add(fileItem)
                End If
            Next i

            ' 获取此节点下的子节点的文件。
            For Each subItem As ProjectItem In item.ProjectItems
                GetProjectItem(subItem, files)
            Next subItem
        End Sub


四、设计“保存项目对话框”的“用户界面”
   
    这个对话框是用来显示项目中的文件。用户可以选择需要复制的文件。

五、在当前集成开发环境打开新的项目
    
   
    string cmd = string.Format("File.OpenProject \"{0}\"", newProjectPath);
    this.DTEObject.ExecuteCommand(cmd);     
/////////////////////////////////////////////////////////////////////////////////////////
参考文献：
为自定义Visual Studio中通过使用VS包进行演练
http://msdn.microsoft.com/en-us/library/cc138565.aspx

创建你的第一个VS包
http://blogs.msdn.com/b/vsxue/archive/2007/11/15/tutorial-2-creating-your-first-vspackage.aspx

如何动态添加菜单项
ms-help://MS.VSCC.v90/MS.VSIPCC.v90/ms.vssdk.v90/dv_vsintegration/html/d281e9c9-b289-4d64-8d0a-094bac6c333c.htm

在Visual Studio软件包中的动态菜单命令
http://blogs.rev-net.com/ddewinter/2008/03/14/dynamic-menu-commands-in-visual-studio-packages-part-1/

管理解决方案、项目和文件
http://msdn.microsoft.com/en-us/library/wbzbtw81.aspx

解决方案成员
http://msdn.microsoft.com/en-us/library/envdte._solution_members(v=VS.90).aspx

/////////////////////////////////////////////////////////////////////////////////////////