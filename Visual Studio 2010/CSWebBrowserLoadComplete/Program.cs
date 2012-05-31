using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CSWebBrowserLoadComplete
{
    static class Program
    {
        /// <summary>
        /// 该应用程序的主要入口点.
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
