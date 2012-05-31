<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETStripHtmlCode._Default" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <a href="SourcePage.aspx">查看SourcePage.aspx</a><br />
    
        <asp:TextBox ID="tbResult" runat="server" Height="416px" Width="534px" 
            TextMode="MultiLine"></asp:TextBox>
        <br />
        <asp:Button ID="btnRetrieveAll" runat="server"  
            Text="搜索完整Html" onclick="btnRetrieveAll_Click" />
    
        <asp:Button ID="btnRetrievePureText" runat="server"  
            Text="搜索纯文本" onclick="btnRetrievePureText_Click"  />
    
        <asp:Button ID="btnRetrieveSriptCode" runat="server"  
            Text="搜索脚本代码" onclick="btnRetrieveSriptCode_Click"   />
    
        <asp:Button ID="btnRetrieveImage" runat="server"  
            Text="搜索图片" onclick="btnRetrieveImage_Click"   />
    
        <asp:Button ID="btnRetrievelink" runat="server"  
            Text="搜索链接" onclick="btnRetrievelink_Click"   />
    
    </div>
    </form>
</body>
</html>
