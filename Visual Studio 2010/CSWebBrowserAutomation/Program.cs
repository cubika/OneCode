using System;
using System.Windows.Forms;

namespace CSWebBrowserAutomation
{
    static class Program
    {
        /// <summary>
        /// 应用程序主函数入口
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
