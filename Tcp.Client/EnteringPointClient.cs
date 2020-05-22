using System;
using System.Windows.Forms;

namespace SomeProject.TcpClient
{
    static class EnteringPointClient
    {
        /// <summary>
        /// Entry point to client
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientMainWindow());
        }
    }
}
