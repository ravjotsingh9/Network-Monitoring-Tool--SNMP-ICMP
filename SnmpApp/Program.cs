using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace SnmpApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static Mutex _AppMutex = new Mutex(false,"MYAPP");
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (_AppMutex.WaitOne(0, false))
            {
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("Application Already Running");
            }
            Application.Exit();
        
            //Application.Run(new Form1());
        }
    }
}
