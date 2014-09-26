using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace USSD
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());

            }
            catch (Exception t)
            {

                MessageBox.Show(t.ToString());
            }
            
        }
    }
}
