<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="LoginPage.aspx.vb" Inherits="AzureBingMaps.WebRole.LoginPage" %>
<%@ Import Namespace="System.Web.Configuration" %>
<%--<%@ Import Namespace="Microsoft.Live" %>
--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:wl="http://apis.live.net/js/2010">
<head id="Head1" runat="server">
    <title>登录</title>

    <script type="text/javascript" src="http://js.live.net/4.1/loader.js"></script>
    <script type="text/javascript">
        function liveIDSignInComplete(signInCompletedEventArgs) {
            if (signInCompletedEventArgs.get_resultCode() != Microsoft.Live.AsyncResultCode.success) {
                alert("登录失败: " + signInCompletedEventArgs.get_resultCode());
                return;
            }
            if (signInCompletedEventArgs.get_resultCode() == Microsoft.Live.AsyncResultCode.success) {
                document.location = 'FederationCallbackHandler.aspx';
            }
        }

        function liveIDSignIn() {
            Microsoft.Live.App.signIn(liveIDSignInComplete);
        }
    </script>
    <link href="Styles/Signin.css" rel="stylesheet" type="text/css" />
</head>

<body>
    <wl:app channel-url="<%=WebConfigurationManager.AppSettings["wl_wrap_channel_url"]%>"
        callback-url="<%=WebConfigurationManager.AppSettings["wl_wrap_client_callback"]%>?wl_session_id=<%=Session.SessionID %>"
        client-id="<%=WebConfigurationManager.AppSettings["wl_wrap_client_id"]%>" scope="WL_Profiles.View,Messenger.SignIn">
    </wl:app>

    <form id="form1" runat="server">
    <div id="Main" style="display: none">
        <div id="Banner" class="Banner">
            <b>登录AzureBingMaps</b></div>
        <div id="SignInContent" class="SignInContent">
            <div id="LeftArea" class="LeftArea" style="display: none;">
                <div class="Header">
                    使用您的账户登录:</div>
                <div id="IdentityProvidersList">
                </div>
                <br />
                <div id="MoreOptions" style="display: none;">
                    <a href="" onclick="ShowDefaultSigninPage(); return false;">显示更多选项</a></div>
            </div>
            <div id="Divider" class="Divider" style="display: none;">
                <div class="DividerLine">
                </div>
                <div>
                    Or</div>
                <div class="DividerLine">
                </div>
            </div>
            <div id="RightArea" class="RightArea" style="display: none;">
                <div class="Header">
                    使用您的e-mail地址登录:</div>
                <form>
                <input type="text" id="EmailAddressTextBox" />
                <input type="submit" id="EmailAddressSubmitButton" onclick="EmailAddressEntered(); return false;"
                    value="递交" /><br />
                <br />
                <label id="EmailAddressError">
                </label>
                </form>
            </div>
        </div>
    </div>
    <script language="javascript" type="text/javascript">
        var identityProviders = [];
        var cookieName = "ACSChosenIdentityProvider-10001624";
        var cookieExpiration = 30; // 天
        var maxImageWidth = 240;
        var maxImageHeight = 40;

        // 此函数将被HRD元数据回调, 用来反应登入页面显示.
        function ShowSigninPage(json) {
            var cookieName = GetHRDCookieName();
            var numIdentityProviderButtons = 0;
            var showEmailEntry = false;
            var showMoreOptionsLink = false;
            identityProviders = json;

            if (identityProviders.length == 0) {
                var mainDiv = document.getElementById("SignInContent");
                mainDiv.appendChild(document.createElement("br"));
                mainDiv.appendChild(document.createTextNode("错误：没有关联到此应用程序的身份提供器."));
            }

            // 创建单独的Live ID按钮.
            CreateLiveIDButton();
            // 循环遍历身份提供器
            for (var i = 0; i < identityProviders.length; i++) {
                // 如果没有已设定的cookie显示所有的登入选项
                if (cookieName === null) {
                    if (identityProviders[i].EmailAddressSuffixes.length > 0) {
                        showEmailEntry = true;
                    }
                    else {
                        // 如果身份提供器设定没有email地址时只显示一个按钮.
                        CreateIdentityProviderButton(identityProviders[i]);
                        numIdentityProviderButtons++;
                    }
                }
                // 如果有已设定的cookie只显示最后所选的登入选项
                else {
                    if (cookieName == identityProviders[i].Name) {
                        CreateIdentityProviderButton(identityProviders[i]);
                        numIdentityProviderButtons++;
                    }
                    else {
                        showMoreOptionsLink = true;
                    }
                }
            }
            // 如果用户已有cookie但不符合任何现有的身份提供器,显示所有的登入选项
            if (cookieName !== null && numIdentityProviderButtons === 0 && !showEmailEntry) {
                ShowDefaultSigninPage();
            }
            // 否则,正常渲染登入页面
            else {
                ShowSigninControls(numIdentityProviderButtons, showEmailEntry, showMoreOptionsLink);
            }

            document.getElementById("Main").style.display = "";
        }

        // 在用户登入且收到cookie之前重置登入页面为初始状态.
        function ShowDefaultSigninPage() {
            var numIdentityProviderButtons = 0;
            var showEmailEntry = false;
            document.getElementById("IdentityProvidersList").innerHTML = "";
            for (var i = 0; i < identityProviders.length; i++) {
                if (identityProviders[i].EmailAddressSuffixes.length > 0) {
                    showEmailEntry = true;
                }
                else {
                    CreateIdentityProviderButton(identityProviders[i]);
                    numIdentityProviderButtons++;
                }
            }
            CreateLiveIDButton();
            ShowSigninControls(numIdentityProviderButtons, showEmailEntry, false);
        }

        //显示了登入控件上的登入页面，并确保他们大小正确
        function ShowSigninControls(numIdentityProviderButtons, showEmailEntry, showMoreOptionsLink) {

            //显示身份提供器的链接，并调整页面为相应大小
            if (numIdentityProviderButtons > 0) {
                document.getElementById("LeftArea").style.display = "";
                if (numIdentityProviderButtons > 4) {
                    var height = 325 + ((numIdentityProviderButtons - 4) * 55);
                    document.getElementById("SignInContent").style.height = height + "px";
                }
            }
            //显示email输入表单如果已经配置email映射
            if (showEmailEntry) {
                document.getElementById("RightArea").style.display = "";
                if (numIdentityProviderButtons === 0) {
                    document.getElementById("RightArea").style.left = "0px";
                }
            }
            //显示用以重新显示所有登录选项的链接
            if (showMoreOptionsLink) {
                document.getElementById("MoreOptions").style.display = "";
            }
            else {
                document.getElementById("MoreOptions").style.display = "none";
            }
            //如果当前存在多个页面登录选项时调整页面大小
            if (numIdentityProviderButtons > 0 && showEmailEntry) {
                document.getElementById("SignInContent").style.width = "720px";
                document.getElementById("Banner").style.width = "720px";
                document.getElementById("Divider").style.display = "";
            }

        }

        function CreateLiveIDButton() {
            var idpList = document.getElementById("IdentityProvidersList");
            var button = document.createElement("input");
            button.type = "button";
            button.className = "IdentityProvider";
            button.onclick = liveIDSignIn;
            button.value = "Windows Live ID";
            idpList.appendChild(button);
        }

        //创建一个程式化的链接到身份提供器的登录页面
        function CreateIdentityProviderButton(identityProvider) {
            var idpList = document.getElementById("IdentityProvidersList");
            var button = document.createElement("button");
            button.setAttribute("name", identityProvider.Name);
            button.setAttribute("id", identityProvider.LoginUrl);
            button.className = "IdentityProvider";
            button.onclick = IdentityProviderButtonClicked;

            // 如果存在图片URL显示图片
            if (identityProvider.ImageUrl.length > 0) {

                var img = document.createElement("img");
                img.className = "IdentityProviderImage";
                img.setAttribute("src", identityProvider.ImageUrl);
                img.setAttribute("alt", identityProvider.Name);
                img.setAttribute("border", "0");
                img.onLoad = ResizeImage(img);

                button.appendChild(img);
            }
            // 否则,如果没有提供图片URL显示一个文字链接
            else if (identityProvider.ImageUrl.length === 0) {
                button.appendChild(document.createTextNode(identityProvider.Name));
            }

            idpList.appendChild(button);
        }

        // 获取的cookie记录身份提供器的名称，如果没有返回NULL.
        function GetHRDCookieName() {
            var cookie = document.cookie;
            if (cookie.length > 0) {
                var cookieStart = cookie.indexOf(cookieName + "=");
                if (cookieStart >= 0) {
                    cookieStart += cookieName.length + 1;
                    var cookieEnd = cookie.indexOf(";", cookieStart);
                    if (cookieEnd == -1) {
                        cookieEnd = cookie.length;
                    }
                    return unescape(cookie.substring(cookieStart, cookieEnd));
                }
            }
            return null;
        }

        // 以所给名称设定一个cookie
        function SetCookie(name) {
            var expiration = new Date();
            expiration.setDate(expiration.getDate() + cookieExpiration);
            document.cookie = cookieName + "=" + escape(name) + ";expires=" + expiration.toUTCString();
        }

        // 设定cookie用来记录已选择的身份提供器并导航到它.
        function IdentityProviderButtonClicked() {
            SetCookie(this.getAttribute("name"));
            window.location = this.getAttribute("id");
            return false;
        }

        function SetEmailError(string) {
            var EmailAddressError = document.getElementById("EmailAddressError");
            if (EmailAddressError.hasChildNodes()) {
                EmailAddressError.replaceChild(document.createTextNode(string), EmailAddressError.firstChild);
            }
            else {
                EmailAddressError.appendChild(document.createTextNode(string));
            }
        }

        function EmailAddressEntered() {
            var enteredEmail = document.getElementById("EmailAddressTextBox").value;
            var identityProvider = null;
            if (enteredEmail.length === 0) {
                SetEmailError("请输入e-mail地址.");
                return;
            }

            if (enteredEmail.indexOf("@") <= 0) {
                SetEmailError("请输入有效的e-mail地址.");
                return;
            }

            var enteredDomain = enteredEmail.split("@")[1].toLowerCase();
            for (var i = 0; i < identityProviders.length; i++) {
                for (var j in identityProviders[i].EmailAddressSuffixes) {
                    if (enteredDomain == identityProviders[i].EmailAddressSuffixes[j].toLowerCase()) {
                        identityProvider = identityProviders[i];
                    }
                }
            }

            if (identityProvider === null) {
                SetEmailError("'" + enteredDomain + "'不是一个可识别的e-mail域.");
                return;
            }

            // 如果我们至此识别了e-mail地址后缀.编写Cookie并重定向到登录URL.
            SetCookie(identityProvider.Name);
            window.location = identityProvider.LoginUrl;
        }

        // 如果图像比按钮大,缩放保持长宽比.
        function ResizeImage(img) {
            if (img.height > maxImageHeight || img.width > maxImageWidth) {
                var resizeRatio = 1;
                if (img.width / img.height > maxImageWidth / maxImageHeight) {
                    // 高宽比宽于按钮
                    resizeRatio = maxImageWidth / img.width;
                }
                else {
                    // 高宽比高于或等于按钮
                    resizeRatio = maxImageHeight / img.height;
                }

                img.setAttribute("height", img.height * resizeRatio);
                img.setAttribute("width", img.width * resizeRatio);
            }
        }

    </script>
    <!-- 此脚本中获取JSON的HRD的元数据和调用渲染链接的回调函数 -->
    <script src="https://azurebingmaps.accesscontrol.appfabriclabs.com:443/v2/metadata/IdentityProviders.js?protocol=wsfederation&realm=http%3a%2f%2fazurebingmaps.cloudapp.net%2f&reply_to=&context=&version=1.0&callback=ShowSigninPage"
        type="text/javascript"></script>
    <br />
    <div class="footer">
        <a id="HyperLink1" href='http://go.microsoft.com/fwlink/?LinkID=131004' target="_blank">
            隐私声明</a>
    </div>
    </form>
</body>
</html>