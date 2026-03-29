using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            passwordForm passwordForm = new passwordForm();
            if (passwordForm.ShowDialog() == DialogResult.OK)
            {
                Form1 form1 = new Form1();
                Application.Run(form1);
            }
        }
    }
}
