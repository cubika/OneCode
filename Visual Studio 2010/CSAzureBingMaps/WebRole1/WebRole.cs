using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureBingMaps.WebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            DiagnosticMonitor.Start("DiagnosticsConnectionString");

            // 有关配置更改的信息处理
            // 参见MSDN相关话题 http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // 如果配置设置改变
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // 设置e.Cancel为真重启角色实例
                e.Cancel = true;
            }
        }
    }
}
