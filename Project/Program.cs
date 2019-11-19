using System;
using System.Windows.Forms;

namespace Window
{
    //初始化RVO
    public static class program
    {

        //static MainWindow mainWindow = new MainWindow();
        ////[STAThread]
        //static void Main()
        //{
        //    //mainWindow.MW = mainWindow;
        //    mainWindow.Run(10);

        //}


        [STAThread]
        static void Main()
        {
            //mainWindow.MW = mainWindow;
            //mainWindow.Run(10);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainUI());
        }
    }




}
