using System.Linq;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace MvcWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            DiagnosticMonitor.Start("DiagnosticsConnectionString");

            // 关于处理配置的变化请查看MSDN http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // 如果配置设置数值变化
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // 将e.Cancel设为true重启这个角色实例
                e.Cancel = true;
            }
        }
    }
}
