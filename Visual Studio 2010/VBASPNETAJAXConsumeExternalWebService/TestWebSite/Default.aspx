<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>VBASPNETAJAXConsumeExternalWebService</title>
	<script type="text/javascript">
	    // 这个函数通过Ajax Extension调用Web服务.
		function GetServerDateTime() {
		    $get("Result").innerHTML = "请稍等...";
			BridgeWebService.GetServerTime(onSuccess, onFailed);
		}
		// 这个函数将在从服务获得回复时执行.
		function onSuccess(result) {
		    $get("Result").innerHTML = "服务器时间日期: " + result.toLocaleString();
		}

		// 这个函数将在从服务获得异常时执行.
		function onFailed(args) {
		    alert("服务器返回错误:\n" +
			"错误消息:" + args.get_message() + "\n" +
			"状态代码:" + args.get_statusCode() + "\n" +
			"异常类型:" + args.get_exceptionType());
		}
	</script>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:ScriptManager ID="ScriptManager1" runat="server">
			<Services>
				<asp:ServiceReference Path="~/BridgeWebService.asmx" />
				<%--
				<!-- 
					 如果我们如下直接地添加外部服务引用将会报错: 
				-->
				<asp:ServiceReference 
					Path="http://localhost:44969/ExternalWebSite/ExternalWebService.asmx" />
				--%>
			</Services>
		</asp:ScriptManager>

		<div id="Result" style="width: 100%; height: 100px; background-color: Black; color: White">
		</div>
		<input type="button" value="通过外部Web服务获得服务器时间" onclick="GetServerDateTime()" />
	</div>
	</form>
</body>
</html>
