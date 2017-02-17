using System;
using System.Windows.Forms;

namespace VechimeSoftware
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {

            bool startUp = (args.Length > 0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm(startUp));
        }
    }
}
