using System;
using System.Windows;
using ProductManagementSystem.WpfApp.ViewModels;
using ProductManagementSystem.WpfApp.Views;

namespace ProductManagementSystem.WpfApp.Services
{
    /// <summary>
    /// MVVM Pattern - ViewModelFirst подход.
    /// 
    /// SOLID - S: Класс отвечает только за создание и отображение окон на основе ViewModel.
    /// SOLID - D: Зависит от абстракций ViewModelBase, а не от конкретных реализаций.
    /// 
    /// ViewManager управляет жизненным циклом окон приложения.
    /// Реализует паттерн ViewModelFirst: сначала создается ViewModel, затем View привязывается к нему.
    /// 
    /// Преимущества ViewModelFirst:
    /// - ViewModel не зависит от View
    /// - Упрощается тестирование
    /// - Централизованное управление окнами
    /// </summary>
    public class ViewManager
    {
        /// <summary>
        /// Отображает главное окно приложения с указанной ViewModel.
        /// </summary>
        /// <param name="viewModel">ViewModel для главного окна</param>
        public void ShowMainWindow(MainViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var mainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            mainWindow.Show();
        }

        /// <summary>
        /// Отображает диалоговое окно с указанной ViewModel.
        /// </summary>
        /// <param name="viewModel">ViewModel для диалогового окна</param>
        /// <returns>Результат диалогового окна (true/false/null)</returns>
        public bool? ShowDialog(ViewModelBase viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // В будущем можно добавить сопоставление типов ViewModel к типам View
            // Пока возвращаем null для демонстрации паттерна
            return null;
        }

        /// <summary>
        /// Закрывает все окна приложения.
        /// </summary>
        public void CloseAllWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Отображает модальное диалоговое окно сообщения.
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="title">Заголовок окна</param>
        /// <param name="button">Кнопки для отображения</param>
        /// <param name="icon">Иконка сообщения</param>
        /// <returns>Результат нажатия кнопки</returns>
        public MessageBoxResult ShowMessage(
            string message,
            string title,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.Information)
        {
            return MessageBox.Show(message, title, button, icon);
        }
    }
}
