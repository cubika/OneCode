Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.ServiceRuntime

' 备注：使IIS6或IIS7经典模式运行的指令信息,访问 http://go.microsoft.com/?LinkId=9394801

Public Class MvcApplication
    Inherits System.Web.HttpApplication

    Shared Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        ' MapRoute需要下列参数,依次是:
        ' (1) 路线名称
        ' (2) 带有参数的URL
        ' (3) 参数的默认值
        routes.MapRoute( _
            "Default", _
            "{controller}/{action}/{id}", _
            New With {.controller = "Home", .action = "Index", .id = UrlParameter.Optional} _
        )

    End Sub

    Sub Application_Start()
        CloudStorageAccount.SetConfigurationSettingPublisher(Function(configName, configSetter)
                                                                 configSetter(RoleEnvironment.GetConfigurationSettingValue(configName))
                                                                 Return Nothing
                                                             End Function)

        AreaRegistration.RegisterAllAreas()

        RegisterRoutes(RouteTable.Routes)
    End Sub
End Class
