using System;
using System.Windows;
using ProductManagementSystem.WpfApp.ViewModels;
using ProductManagementSystem.WpfApp.Views;

namespace ProductManagementSystem.WpfApp.Services
{
    /// <summary>
    /// MVVM Pattern - ViewModelFirst подход.
    /// 
    /// SOLID - S: Класс отвечает только за создание и управление окнами на основе ViewModel.
    /// SOLID - D: Зависит от абстракции ViewModelBase, а не от конкретных реализаций.
    /// 
    /// ViewManager реализует паттерн создания окон приложения.
    /// Реализует подход ViewModelFirst: сначала создается ViewModel, затем View привязывается к ней.
    /// 
    /// Преимущества ViewModelFirst:
    /// - ViewModel не зависит от View (можно тестировать отдельно)
    /// - Упрощенное тестирование бизнес-логики
    /// - Централизованное управление окнами
    /// - Возможность смены View без изменения ViewModel
    /// 
    /// Алгоритм работы ViewModelFirst:
    /// 1. Создается ViewModel (через DI контейнер)
    /// 2. ViewManager создает соответствующее окно View
    /// 3. View.DataContext = ViewModel (связывание)
    /// 4. Окно отображается пользователю
    /// </summary>
    public class ViewManager
    {
        /// <summary>
        /// Отображает главное окно приложения с указанной ViewModel.
        /// ViewModelFirst: ViewModel создается ДО View.
        /// </summary>
        /// <param name="viewModel">ViewModel для главного окна</param>
        public void ShowMainWindow(MainViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // ViewModelFirst Pattern:
            // 1. ViewModel уже создана (передана в параметре)
            // 2. Создаем View и передаем ViewModel в конструктор
            // 3. View автоматически устанавливает DataContext
            var mainWindow = new MainWindow(viewModel);

            // 4. Показываем окно пользователю
            mainWindow.Show();
        }

        /// <summary>
        /// Отображает модальное окно с указанной ViewModel.
        /// </summary>
        /// <param name="viewModel">ViewModel для модального окна</param>
        /// <returns>Результат закрытия окна (true/false/null)</returns>
        public bool? ShowDialog(ViewModelBase viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // В будущем здесь реализуется сопоставление между ViewModel и нужной View
            // Пока возвращаем null для совместимости интерфейса
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
        /// Отображает стандартное модальное окно сообщения.
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
