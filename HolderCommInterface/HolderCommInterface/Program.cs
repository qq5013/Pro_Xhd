using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using System.Threading;

namespace HolderCommInterface
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            BindExceptionHandler();//绑定程序中的异常处理
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool initiallyOwned = true;
            bool isCreated;
            Mutex m = new Mutex(initiallyOwned, "HolderCommInterface", out isCreated);
            if (!(initiallyOwned && isCreated))
            {
                MessageBox.Show("Sorry，Only Can Open One  App！", "Message");
                Application.Exit();
            }
            else
            {
                Application.Run(new frmMain());
            }  
           
        }

        /// <summary>
        /// 绑定程序中的异常处理
        /// </summary>
        private static void BindExceptionHandler()
        {
            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理未捕获的异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        /// <summary>
        /// 处理UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogHelper.ErrorLog(null, e.Exception as Exception);
        }
        /// <summary>
        /// 处理未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.ErrorLog(null, e.ExceptionObject as Exception);
        }
    }
}
