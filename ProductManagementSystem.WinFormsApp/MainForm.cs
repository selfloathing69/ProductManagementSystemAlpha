using System;
using System.Windows.Forms;
using ProductManagementSystem.Logic;
using ProductManagementSystem.Model;

namespace ProductManagementSystem.WinFormsApp
{
    public partial class MainForm : Form
    {
        private ProductLogic _logic = new ProductLogic();
        public MainForm()
        {
            InitializeComponent();
            RefreshGrid();
        }

        private DataGridView dataGridView;
        private Button btnRefresh, btnAdd, btnDelete;

        private void InitializeComponent()
        {
            this.Text = "Product Management - WinForms (simple)";
            this.Width = 800; this.Height = 500;
            dataGridView = new DataGridView { Dock = DockStyle.Top, Height = 300, ReadOnly = true, AutoGenerateColumns = true };
            btnRefresh = new Button { Text = "Обновить", Left = 10, Top = 320 };
            btnAdd = new Button { Text = "Добавить пример", Left = 110, Top = 320 };
            btnDelete = new Button { Text = "Удалить выбранный", Left = 230, Top = 320 };
            btnRefresh.Click += (s,e)=> RefreshGrid();
            btnAdd.Click += (s,e)=> { _logic.AddProduct(new Product { Name = "Новый товар", Price = 100, Category = "Другое", StockQuantity = 1 }); RefreshGrid(); };
            btnDelete.Click += (s,e)=> {
                if (dataGridView.CurrentRow != null)
                {
                    var id = (int)dataGridView.CurrentRow.Cells["Id"].Value;
                    _logic.DeleteProduct(id);
                    RefreshGrid();
                }
            };
            this.Controls.Add(dataGridView);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnDelete);
        }

        private void RefreshGrid()
        {
            dataGridView.DataSource = null;
            dataGridView.DataSource = _logic.GetAllProducts();
        }
    }
}
