using System;
using System.Windows.Forms;

namespace CSWebBrowserWithProxy
{
    static class Program
    {
        /// <summary>
        /// 主程序入口
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
