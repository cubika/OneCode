=============================================================================
                  CSASPNETIPtoLocation 项目概述
=============================================================================

用法:

 此项目演示了如何通过免费Webservice http://freegeoip.appspot.com/ 根据IP地址获取
 地理位置. 

/////////////////////////////////////////////////////////////////////////////
示例演示.

步骤1: 浏览实例项目的Default.aspx你会发现你的IP地址显示在页面上. 
如果你在本地查看网页, 你可能得到127.0.0.1作为同一机器的客户端和主机. 
当你部署这个示例到一个主机服务器, 你将得到你的真实IP地址.

步骤2: 在TextBox中输入一个IP地址然后单击递交按钮.

步骤3: 你将在页面回传后得到基本地理位置信息,包括国名和城市名.

/////////////////////////////////////////////////////////////////////////////
代码逻辑:

步骤1: 在Visual Studio 2010中创建一个C# ASP.NET空白Web应用程序.

步骤2: 添加一个默认ASP.NET页面到应用程序.

步骤3: 向页面添加一个Label,一个TextBox以及一个Button控件.Label用来显示客户端IP地址.
TextBox用来输入IP地址,然后用户可以单击Button根据输入获得地理位置信息.

步骤4: 编写获得客户端IP地址的代码.

    string ipAddress;
    ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    if (string.IsNullOrEmpty(ipAddress))
    {
        ipAddress = Request.ServerVariables["REMOTE_ADDR"];
    }

步骤5: 编写通过免费webservice获得地理位置信息的代码 
http://freegeoip.appspot.com/. 

    using (WebClient wc = new WebClient())
    {
        string url = "http://freegeoip.appspot.com/json/" + ipAddress;
        locationJson = wc.DownloadString(url);
    }

    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
    using (MemoryStream jsonStream = new MemoryStream(encoding.GetBytes(locationJson)))
    {
        jsonStream.Position = 0;
        try
        {
            DataContractJsonSerializer ser;
            ser = new DataContractJsonSerializer(typeof(LocationInfo));
            locationInfo = (LocationInfo)ser.ReadObject(jsonStream);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

备注: 返回的Json数据类似下列格式:

    {
        "status":true,
         "ip":"***.***.***.***",
         "countrycode":"US",
         "countryname":
         "United States",
         "regioncode":"",
         "regionname":"",
         "city":"",
         "zipcode":"",
         "latitude":38.0,
         "longitude":-97.0
     }

同时我们也可以获得XML格式的地理位置信息. 请参考这个链接
http://freegeoip.appspot.com/xml/ipaddress 查看XML格式数据.

步骤6: 编写在页面显示信息的代码.

/////////////////////////////////////////////////////////////////////////////
参考资料:

# free IP geolocation webservice
http://freegeoip.appspot.com/

# MSDN: 如何：对 JSON 数据进行序列化和反序列化
http://msdn.microsoft.com/zh-cn/library/bb412179.aspx

/////////////////////////////////////////////////////////////////////////////