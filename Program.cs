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
          
            //!!!
            //OrmModelProject proj = ProgramModel.CreateProject(@"D:\123.ormproj");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(args));
        }
    }
}
