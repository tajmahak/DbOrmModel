using System;
using System.Windows.Forms;

namespace DbOrmModel
{
    internal static class Program
    {
        public static ProgramModel ProgramModel;

        [STAThread]
        private static void Main(string[] args)
        {
            ProgramModel = new ProgramModel();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(args));
        }
    }
}
