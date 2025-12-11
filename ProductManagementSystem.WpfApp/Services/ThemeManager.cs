using System;
using System.Windows;
using System.Windows.Media;

namespace ProductManagementSystem.WpfApp.Services
{
    /// <summary>
    /// MVVM Pattern - Сервис для управления темами приложения.
    /// 
    /// SOLID - S: Класс отвечает только за управление темами.
    /// SOLID - O: Открыт для расширения (можно добавить новые темы).
    /// 
    /// Управляет переключением между светлой и темной темами приложения.
    /// </summary>
    public class ThemeManager
    {
        private static ThemeManager? _instance;
        private bool _isDarkTheme;

        /// <summary>
        /// Singleton экземпляр ThemeManager
        /// </summary>
        public static ThemeManager Instance => _instance ??= new ThemeManager();

        /// <summary>
        /// Текущая тема (true = темная, false = светлая)
        /// </summary>
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            private set
            {
                _isDarkTheme = value;
                ThemeChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Событие изменения темы
        /// </summary>
        public event EventHandler<bool>? ThemeChanged;

        private ThemeManager()
        {
            _isDarkTheme = false; // По умолчанию светлая тема
        }

        /// <summary>
        /// Переключает тему на противоположную
        /// </summary>
        public void ToggleTheme()
        {
            IsDarkTheme = !IsDarkTheme;
            ApplyTheme();
        }

        /// <summary>
        /// Устанавливает конкретную тему
        /// </summary>
        /// <param name="isDark">true для темной темы, false для светлой</param>
        public void SetTheme(bool isDark)
        {
            if (IsDarkTheme != isDark)
            {
                IsDarkTheme = isDark;
                ApplyTheme();
            }
        }

        /// <summary>
        /// Применяет текущую тему к приложению
        /// </summary>
        private void ApplyTheme()
        {
            var resources = Application.Current.Resources;

            if (IsDarkTheme)
            {
                // Темная тема
                resources["AppBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
                resources["AppForegroundBrush"] = new SolidColorBrush(Colors.White);
                resources["PanelBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                resources["AlternateRowBrush"] = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                resources["HeaderBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                resources["StatusBarBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                resources["InputBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(55, 55, 55));
                resources["InputForegroundBrush"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                // Светлая тема
                resources["AppBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                resources["AppForegroundBrush"] = new SolidColorBrush(Colors.Black);
                resources["PanelBackgroundBrush"] = new SolidColorBrush(Colors.White);
                resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(204, 204, 204));
                resources["AlternateRowBrush"] = new SolidColorBrush(Color.FromRgb(249, 249, 249));
                resources["HeaderBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                resources["StatusBarBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 122, 204));
                resources["InputBackgroundBrush"] = new SolidColorBrush(Colors.White);
                resources["InputForegroundBrush"] = new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Инициализирует темы при запуске приложения
        /// </summary>
        public void Initialize()
        {
            ApplyTheme();
        }
    }
}
