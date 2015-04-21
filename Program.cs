using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetworkGUI
{
    static class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}