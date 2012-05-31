using System;
using System.Windows.Forms;

namespace CSTabbedWebBrowser
{
    static class Program
    {
        /// <summary>
        /// 应用程序的入口点.
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
