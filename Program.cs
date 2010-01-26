using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TubeGuardian
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static frmSplash splash;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (splash == null)
                splash = new frmSplash();
            splash.Show();
            Application.Run();
        }
    }
}
