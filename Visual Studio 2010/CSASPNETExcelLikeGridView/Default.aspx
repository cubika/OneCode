<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSExcelLikeGridView._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CSExcelLikeGridView</title>
    <script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.4.min.js" type="text/javascript">
    </script>
    <script type="text/javascript">

        //读取HidState (JSON)以保存状态颜色
        function ResetColors(color1, color2) {
            var contents = $(":hidden:last").val();

            //若非"[]", 表示有改变, 重置颜色.
            if (contents.toString() != "[]") {
                // 转换到JSON 对象.
                var objectc = eval(contents);

                // 根据是否被改变来重置颜色
                for (var i = 0; i < objectc.length; ++i) {
                    if (objectc[i].Color != '') {
                        $("td:eq(" + objectc[i].Index + ")").css("background-color", objectc[i].Color);
                    }

                    var checked = objectc[i].Deleted == "True";

                    if (parseInt(objectc[i].Index) % 2 == 0) {
                        $("td:eq(" + parseInt(objectc[i].Index - 2) + ")").find(":checkbox").attr("checked", checked);
                    }
                    else {
                        $("td:eq(" + parseInt(objectc[i].Index - 3) + ")").find(":checkbox").attr("checked", checked);
                    }

                    // 如果复选框勾选, 改变行颜色
                    if (checked) {
                        $("td:eq(" + objectc[i].Index + ")").parent().css("background-color", "red");
                    }
                }
            }

        }

        //保存验证
        function SaveValidate() {
            //首先检查是否有"Name"未填...

            if (Page_ClientValidate('Fill')) {
                return confirm('您真的要一齐保存所有更改吗?');
            }
            else {
                alert("注意! 不能插入一个空白名称!");
            }
        }

        //插入验证
        function InsertValidate() {
            //首先检查是否有"Name"未填...

            if (!Page_ClientValidate('Insert')) {
                alert("注意! 不能使名称为空!");
            }
        }

        // 添加除了底部的所有文本框的动态事件转换背景色.
        function AddEvents() {

            var rowarray = $("tr");
            for (var i = 0; i < rowarray.length - 1; ++i) {
                $(rowarray[i]).find(":text").change(function () {
                    $(this).parent().css("background", "blue");
                });
            }

        }

        $(function () {

            //保持个别行原始颜色
            var color1 = $("tr:eq(1)").css("background-color").valueOf();
            var color2 = $("tr:eq(2)").css("background-color").valueOf();
            var headercolor = $("tr:first").css("background-color").valueOf();
            var footercolor = $("tr:last").css("background-color").valueOf();

            AddEvents();

            // 头部复选框复选效果:
            $("#chkAll").click(function () {

                $(":checkbox").attr("checked", $(this).attr("checked"));

                if ($(this).attr("checked")) {
                    $(":checkbox").parent().parent().css("background-color", "red");
                    //重置头部颜色
                    $("tr:first").css("background-color", headercolor);
                }
                else {
                    $("tr:odd").css("background-color", color1);
                    $("tr:even").css("background-color", color2);

                    //重置头部和底部颜色
                    $("tr:first").css("background-color", headercolor);
                    $("tr:last").css("background-color", footercolor);
                }
            });

            //单个复选框勾选事件
            $(":checkbox").click(function () {
                if ($(this).attr("checked")) {
                    $(this).parent().parent().css("background-color", "red");
                }
                else {
                    if ($(this).parent().parent().index() % 2 == 0) {
                        $(this).parent().parent().css("background-color", color2);
                    }
                    else {
                        $(this).parent().parent().css("background-color", color1);
                    }
                }

                //重置头部颜色
                $("tr:first").css("background-color", headercolor);
            });

            ResetColors(color1, color2);
        })

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <h1>
        批操作示例</h1>
    <span style="color: Red">红色行将被删除</span>
    <br />
    <span style="color: green">绿色行将被添加</span>
    <br />
    <span style="color: blue">蓝色单元格将被修改</span>
    <br />
    <hr />
    <div>
        <asp:GridView ID="GridView1" runat="server" Width="70%" Height="50%" AutoGenerateColumns="False"
            CellPadding="4" ForeColor="#333333" GridLines="None" ShowFooter="True">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:TemplateField HeaderText="删除状态">
                    <HeaderTemplate>
                        <input id="chkAll" type="checkbox" />
                        全部删除
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkDelete" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="人员编号">
                    <ItemTemplate>
                        <%#Eval("Id") %>
                    </ItemTemplate>
                    <FooterTemplate>
                        姓名:<asp:TextBox ID="tbNewName" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbNewName"
                            ErrorMessage="不能插入一个空白名称!" ForeColor="#FFFF66" ValidationGroup="Insert"></asp:RequiredFieldValidator>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="名称">
                    <ItemTemplate>
                        <asp:TextBox ID="tbName" runat="server" Text='<%#Eval("PersonName") %>'>
                        </asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbName"
                            ErrorMessage="不能使名称为空!" ValidationGroup="Fill"></asp:RequiredFieldValidator>
                    </ItemTemplate>
                    <FooterTemplate>
                        地址:<asp:TextBox ID="tbNewAddress" runat="server"></asp:TextBox>
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="地址">
                    <ItemTemplate>
                        <asp:TextBox ID="tbAddress" runat="server" Text='<%#Eval("PersonAddress") %>'>
                        </asp:TextBox>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Button ID="btnAdd" runat="server" Text="添加新行" OnClick="btnAdd_Click"
                            ValidationGroup="Insert" OnClientClick="InsertValidate()" />
                    </FooterTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                VerticalAlign="Middle" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                VerticalAlign="Middle" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" HorizontalAlign="Center" VerticalAlign="Middle" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        </asp:GridView>
    </div>
    <asp:Button ID="btnSaveAll" runat="server" Height="30px" Text="保存所有更改"
        Width="149px" OnClick="btnSaveAll_Click" OnClientClick="SaveValidate()" ValidationGroup="Fill" />
    <asp:HiddenField ID="HidState" runat="server" Value="[]" />
    </form>
</body>
</html>
