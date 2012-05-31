using System;
using System.Windows.Forms;

namespace CSFTPUpload
{
    static class Program
    {
        /// <summary>
        /// 程序主入口函数
        /// 
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
