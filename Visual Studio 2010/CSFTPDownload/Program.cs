using System;
using System.Windows.Forms;

namespace CSFTPDownload
{
    static class Program
    {
        /// <summary>
        /// 该应用程序的主入口点。
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
