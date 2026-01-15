using System;
using System.Windows.Forms;
using StreamVideo_Server.Winforms;

namespace StreamVideo_Server
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}
