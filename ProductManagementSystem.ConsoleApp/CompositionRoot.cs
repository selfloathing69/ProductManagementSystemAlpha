using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Services;
using ProductManagementSystem.Logic.Validators;
using ProductManagementSystem.Logic.Mappers;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.ConsoleApp
{
    /// <summary>
    /// Composition Root для консольного приложения.
    /// Модуль конфигурации Ninject для настройки Dependency Injection.
    /// 
    /// ВАЖНО: Это точка сборки для ConsoleApp (текстовое меню).
    /// Для MVP используется CompositionRoot в Presenter.
    /// 
    /// SOLID - D: Модуль использует DI-контейнер для управления зависимостями между компонентами.
    /// SOLID - O: Позволяет изменять конфигурацию без изменения клиентского кода.
    /// </summary>
    public class CompositionRoot : NinjectModule
    {
        /// <summary>
        /// Метод Load() вызывается Ninject при загрузке модуля.
        /// Здесь мы регистрируем привязки (bindings) между интерфейсами и их реализациями.
        /// </summary>
        public override void Load()
        {
            // ==================== Data Access Layer ====================
            
            // CRUD операции - Repository
            // ПЕРЕКЛЮЧАЛКА!! Раскомментируйте нужный репозиторий
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

            // ==================== Business Logic Layer ====================
            
            // Бизнес-функции (фильтрация, группировка, расчёты)
            Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();

            // ProductLogic (объединяет CRUD и бизнес-функции)
            Bind<ILogic>().To<ProductLogic>().InSingletonScope();

            // ==================== Services для консольного приложения ====================
            
            // Сервисы для консольного UI
            Bind<ProductMapper>().ToSelf().InSingletonScope();
            Bind<ProductValidator>().ToSelf().InSingletonScope();
            Bind<IProductService>().To<ProductService>().InSingletonScope();
            
            // MenuController для консольного UI
            Bind<MenuController>().ToSelf();
        }
    }
}
