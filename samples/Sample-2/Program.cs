using System;
using System.Windows.Forms;

namespace Sample_2
{
    static class Program
    {
        static Program()
        {
            RhinoInside.Resolver.Initialize();
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
