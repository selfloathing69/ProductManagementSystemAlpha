using System;
using System.Windows.Forms;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// Главная форма Windows Forms приложения для управления товарами.
    /// Предоставляет графический интерфейс для выполнения операций CRUD над товарами.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Экземпляр класса с бизнес-логикой для работы с товарами.
        /// </summary>
        private ProductLogic _logic = new ProductLogic();
        
        /// <summary>
        /// Инициализирует новый экземпляр главной формы.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            RefreshGrid();
        }

        /// <summary>
        /// Таблица для отображения списка товаров.
        /// </summary>
        private DataGridView dataGridView;
        
        /// <summary>
        /// Кнопки управления: обновить, добавить, удалить.
        /// </summary>
        private Button btnRefresh, btnAdd, btnDelete;

        /// <summary>
        /// Инициализирует компоненты формы и настраивает их расположение.
        /// Исправлена ошибка в строке 28 - добавлена проверка существования столбца.
        /// </summary>
        private void InitializeComponent()
        {
            // Настройка формы - исправлено: увеличен размер до 1280x720
            this.Text = "Система управления товарами - Windows Forms";
            this.Width = 1280; 
            this.Height = 720;
            this.MinimumSize = new System.Drawing.Size(1280, 720);  // Минимальный размер
            this.WindowState = FormWindowState.Normal;  // Позволяет масштабирование
            
            // Настройка таблицы данных
            dataGridView = new DataGridView 
            { 
                Dock = DockStyle.Top, 
                Height = 400,  // Увеличена высота таблицы
                ReadOnly = true, 
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            
            // Настройка кнопок с улучшенным размещением
            btnRefresh = new Button 
            { 
                Text = "Обновить список", 
                Left = 20, 
                Top = 420, 
                Width = 150, 
                Height = 35 
            };
            
            btnAdd = new Button 
            { 
                Text = "Добавить товар", 
                Left = 180, 
                Top = 420, 
                Width = 150, 
                Height = 35 
            };
            
            btnDelete = new Button 
            { 
                Text = "Удалить выбранный", 
                Left = 340, 
                Top = 420, 
                Width = 150, 
                Height = 35 
            };
            
            // Привязка обработчиков событий
            btnRefresh.Click += (s, e) => RefreshGrid();
            
            btnAdd.Click += (s, e) => 
            { 
                // Добавляем пример товара с правильным регистром
                _logic.AddProduct(new Product 
                { 
                    Name = "Новый товар", 
                    Description = "Описание нового товара",
                    Price = 100, 
                    Category = "Разное", 
                    StockQuantity = 1 
                }); 
                RefreshGrid(); 
            };
            
            // Исправлено: добавлена проверка существования столбца "Id"
            btnDelete.Click += (s, e) => 
            {
                if (dataGridView.CurrentRow != null && 
                    dataGridView.Columns.Contains("Id") &&
                    dataGridView.CurrentRow.Cells["Id"].Value != null)
                {
                    var id = (int)dataGridView.CurrentRow.Cells["Id"].Value;
                    
                    // Запрашиваем подтверждение удаления
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить товар с ID {id}?",
                        "Подтверждение удаления",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        
                    if (result == DialogResult.Yes)
                    {
                        _logic.DeleteProduct(id);
                        RefreshGrid();
                        MessageBox.Show("Товар успешно удалён.", "Успех", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите строку для удаления.", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            
            // Добавление контролов на форму
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);
        }

        /// <summary>
        /// Обновляет данные в таблице, загружая актуальный список товаров.
        /// </summary>
        private void RefreshGrid()
        {
            dataGridView.DataSource = null;
            dataGridView.DataSource = _logic.GetAllProducts();
            
            // Настройка отображения столбцов
            if (dataGridView.Columns.Count > 0)
            {
                dataGridView.Columns["Id"].HeaderText = "ID";
                dataGridView.Columns["Name"].HeaderText = "Название";
                dataGridView.Columns["Description"].HeaderText = "Описание";
                dataGridView.Columns["Price"].HeaderText = "Цена";
                dataGridView.Columns["Category"].HeaderText = "Категория";
                dataGridView.Columns["StockQuantity"].HeaderText = "Количество";
                
                // Автоматическое изменение размера столбцов
                dataGridView.AutoResizeColumns();
            }
        }
    }
}
