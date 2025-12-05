using System;
using System.Windows.Forms;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Точка входа для WinForms.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Основная точка входа для приложения.
        /// Примечание: используется только при запуске WinFormsAppы
        /// При запуске из ConsoleApp вместо этого используется WinFormsRunner.Run().
        /// </summary>
        [STAThread]
        static void Main()
        {
         // Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            MessageBox.Show(
                "Куда жмём? Открываем через ConsoleApp (Presenter.).\n\n" +
                "Запуск WinFormsApp заблокирован.",
                "Информация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
