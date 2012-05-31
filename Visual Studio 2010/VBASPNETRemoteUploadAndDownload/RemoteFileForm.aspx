<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RemoteFileForm.aspx.vb"
    Inherits="VBRemoteUploadAndDownload.RemoteFileForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-family: Calibri">
        <table>
            <tr>
                <th colspan="2">
                    <p style="font-size: x-large">
                        远程上传:</p>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbUpUrl" runat="server" Text="Url:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbUploadUrl" runat="server" Width="261px"></asp:TextBox>
                    <asp:RadioButtonList ID="rbUploadList" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True">HTTP</asp:ListItem>
                        <asp:ListItem>FTP</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbUpFile" runat="server" Text="文件:"></asp:Label>
                </td>
                <td>
                    <asp:FileUpload ID="FileUpload" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnUpload" runat="server" Text="上传" OnClick="btnUpload_Click"
                        Height="100%" Width="25%" />
                    <br />
                       <strong>说明</strong>: 假设我们要上传一个文件到远程服务器的虚拟目录 &quot;<a href="http://www.mysite.com/mydocument/">http://www.mysite.com/myfiles/</a>&quot;,&nbsp;&nbsp; 
                        我们只需要填写Url如下: &quot;<a 
                            href="http://www.mysite.com/myfiles/">http://www.mysite.com/myfiles/</a>&quot;.&nbsp;如果服务器是ftp 
                        服务器, url如下:&quot;<a href="ftp://www.mysite.com/myfiles/">ftp://www.mysite.com/myfiles/</a>&quot;. 然后单击
                        &quot;浏览&quot; 按钮选择你想上传的本地文件. 单击&quot;上传&quot; 按钮之后,结果会被显示在页面上.</p>
                </td>
            </tr>
        </table>
        <br />
        <table>
            <tr>
                <th colspan="2">
                    <p style="font-size: x-large">
                        远程下载:</p>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbDnUrl" runat="server" Text="Url:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDownloadUrl" runat="server" Width="388px"></asp:TextBox>
                    <asp:RadioButtonList ID="rbDownloadList" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True">HTTP</asp:ListItem>
                        <asp:ListItem>FTP</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbDnFile" runat="server" Text="下载文件路径:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbDownloadPath" runat="server" Width="267px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button ID="btnDownLoad" runat="server" Text="下载" OnClick="btnDownLoad_Click"
                        Width="25%" Height="100%" />
                    <br />
                    <p style="font-size: medium">
                        <strong>说明</strong> : 如果我们要从远程服务器的虚拟目录例如远程服务器&quot;<a 
                            href="http://www.mysite.com/mydocument/">http://www.mysite.com/myfile/</a>&quot; 下载指定文件,&nbsp; 
                        你可以使用这份实例来下载它. 假设我们要从远程服务器的虚拟目录下载的文件名为&quot;myimage.gif&quot;, 在下载Url 
                        textbox中可以输入类似于: &quot;<a 
                            href="http://www.mysite.com/myfile/myimage.gif">http://www.mysite.com/myfile/myimage.gif</a>&quot;. 
                        DownLoadFilePath是我们用来保存文件到本地的物理路径 例如 &quot;<a 
                            href="file:///C:/Temp/Download/">C:\Temp\Download\</a>&quot;&nbsp;. 单机 
                        &quot;下载&quot; 按钮,结果会被显示在页面上.</p>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
