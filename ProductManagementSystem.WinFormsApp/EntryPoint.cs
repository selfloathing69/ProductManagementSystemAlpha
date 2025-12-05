using System;
using System.Windows.Forms;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Entry point for standalone WinForms application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Note: This is only used when running WinFormsApp standalone.
        /// When launched from ConsoleApp, WinFormsRunner.Run() is used instead.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            MessageBox.Show(
                "Это приложение предназначено для запуска через ConsoleApp (Presenter).\n\n" +
                "Пожалуйста, запустите ProductManagementSystem.ConsoleApp.",
                "Информация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
