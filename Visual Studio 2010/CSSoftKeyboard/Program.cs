using System;
using System.Windows.Forms;

namespace CSSoftKeyboard
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主程序入口。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new KeyBoardForm());
        }
    }
}
