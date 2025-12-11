using Ninject.Modules;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Logic.Services;
using ProductManagementSystem.Logic.Validators;
using ProductManagementSystem.Logic.Mappers;
using ProductManagementSystem.Model;
using ProductManagementSystem.DataAccessLayer.EF;
using ProductManagementSystem.DataAccessLayer.Dapper;

namespace ProductManagementSystem.Presenter
{
    /// <summary>
    /// MVP Pattern - Composition Root (Точка сборки).
    /// Модуль конфигурации Ninject для настройки Dependency Injection.
    /// 
    /// ВАЖНО: По требованиям MVP точка сборки находится в Presenter.
    /// Presenter связывает пассивную View и активную Model, обеспечивая 100% выполнение паттерна MVP.
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
            // ================ Дата аксес лейер 
            
            // CRUD операции - Repository
            // ПЕРЕКЛЮЧАЛКА!! Раскомментируйте нужный репозиторий
            Bind<IRepository<Product>>().To<EntityRepository<Product>>().InSingletonScope();
            // Bind<IRepository<Product>>().To<DapperRepository<Product>>().InSingletonScope();

            // ========================= Слой бизнес логики
            // Бизнес-функции (фильтрация, группировка, расчёты)
            Bind<IBusinessFunctions>().To<BusinessFunctions>().InSingletonScope();

            // ProductLogic (объединяет CRUD и бизнес-функции)
            Bind<ILogic>().To<ProductLogic>().InSingletonScope();

            // ==================== MVP паттерн - Model

            // ProductModelMvp обёртывает ProductLogic и добавляет события для MVP
            Bind<IProductModel>().To<ProductModelMvp>().InSingletonScope();
            
            // Сервисы для UI
            Bind<ProductMapper>().ToSelf().InSingletonScope();
            Bind<ProductValidator>().ToSelf().InSingletonScope();
            Bind<IProductService>().To<ProductService>().InSingletonScope();
        }
    }
}
