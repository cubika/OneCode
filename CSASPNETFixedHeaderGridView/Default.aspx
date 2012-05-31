<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETFixedHeaderGridView.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="JScript/ScrollableGridPlugin.js" type="text/javascript"></script>
    <script type = "text/javascript">
        $(document).ready(function () {
            // 调用 Scrollable 函数.
            $('#<%=gvwList.ClientID %>').Scrollable({
                ScrollHeight: 600,        
            });
        });

</script>
</head>
<body >
    <form id="form1" runat="server">
    <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto"  >
        <asp:GridView ID="gvwList" runat="server" CellSpacing="2" 
    AutoGenerateColumns="False" >
            <Columns>
                <asp:BoundField DataField="a" FooterText="标题 a" 
            HeaderText="标题 a" />
                <asp:BoundField DataField="b" FooterText="标题 b" 
            HeaderText="标题 b" />
                <asp:BoundField DataField="c" FooterText="标题 c" 
            HeaderText="标题 c" />
                <asp:BoundField DataField="d" FooterText="标题 d" 
            HeaderText="标题 d" />
                <asp:BoundField DataField="e" FooterText="标题 e" 
            HeaderText="标题 e" />
                <asp:BoundField DataField="f" FooterText="标题 f" 
            HeaderText="标题 f" />
                <asp:BoundField DataField="g" FooterText="标题 g" 
            HeaderText="标题 g" />
                <asp:BoundField DataField="h" FooterText="标题 h" 
            HeaderText="标题 h" />
                <asp:BoundField DataField="i" FooterText="标题 i" 
            HeaderText="标题 i" />
                <asp:BoundField DataField="j" FooterText="标题 j" 
            HeaderText="标题 j" />
            </Columns>
        </asp:GridView>
    </asp:Panel>

    </form>
</body>
</html>
