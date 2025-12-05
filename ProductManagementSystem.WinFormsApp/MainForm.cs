using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// MVP Pattern - Main View Implementation.
    /// Implements IProductView interface for the MVP pattern.
    /// SOLID - S: Class is responsible only for UI of the main window.
    /// SOLID - D: Depends on IProductView abstraction, communicates with Presenter through events.
    /// View не имеет зависимости от Model - работает только с ProductDto.
    /// </summary>
    public partial class MainForm : Form, IProductView
    {
        #region Fields

        // Main UI elements
        private DataGridView dataGridView = null!;
        private Panel buttonPanel = null!;
        private Button btnRefresh = null!;
        private Button btnAdd = null!;
        private Button btnDelete = null!;
        private Button btnDeleteByQuantity = null!;

        #endregion

        #region IProductView Events

        /// <inheritdoc/>
        public event EventHandler? RefreshRequested;

        /// <inheritdoc/>
        public event EventHandler? AddProductRequested;

        /// <inheritdoc/>
        public event EventHandler<int>? DeleteProductRequested;

        /// <inheritdoc/>
        public event EventHandler<(int ProductId, int Quantity)>? DeleteProductByQuantityRequested;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for MainForm.
        /// Initializes UI components. Presenter will be set later via SetPresenter method.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #endregion

        #region IProductView Implementation

        /// <inheritdoc/>
        public void ShowProducts(IEnumerable<ProductDto> products)
        {
            try
            {
                dataGridView.DataSource = null;
                dataGridView.DataSource = products.ToList();

                // Configure column headers
                if (dataGridView.Columns.Count > 0)
                {
                    if (dataGridView.Columns.Contains("Id"))
                        dataGridView.Columns["Id"].HeaderText = "ID";
                    
                    if (dataGridView.Columns.Contains("Name"))
                        dataGridView.Columns["Name"].HeaderText = "Название";
                    
                    if (dataGridView.Columns.Contains("Description"))
                        dataGridView.Columns["Description"].HeaderText = "Описание";
                    
                    if (dataGridView.Columns.Contains("Price"))
                    {
                        dataGridView.Columns["Price"].HeaderText = "Цена";
                        dataGridView.Columns["Price"].DefaultCellStyle.Format = "N2";
                    }
                    
                    if (dataGridView.Columns.Contains("Category"))
                        dataGridView.Columns["Category"].HeaderText = "Категория";
                    
                    if (dataGridView.Columns.Contains("StockQuantity"))
                        dataGridView.Columns["StockQuantity"].HeaderText = "Количество";
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка", $"Ошибка при обновлении таблицы: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <inheritdoc/>
        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <inheritdoc/>
        public bool ShowConfirmation(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <inheritdoc/>
        public int? GetSelectedProductId()
        {
            if (dataGridView.CurrentRow == null)
                return null;

            if (!dataGridView.Columns.Contains("Id") || dataGridView.CurrentRow.Cells["Id"].Value == null)
                return null;

            return (int)dataGridView.CurrentRow.Cells["Id"].Value;
        }

        /// <inheritdoc/>
        public (int ProductId, int Quantity)? ShowDeleteByQuantityDialog(IEnumerable<(int Index, ProductDto Product)> products)
        {
            var productList = products.ToList();
            
            if (productList.Count == 0)
            {
                ShowMessage("Информация", "Товары не найдены.");
                return null;
            }

            using (var selectForm = new Form
            {
                Text = "Выберите товар для удаления",
                Size = new Size(500, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                // Product list
                var listBox = new ListBox
                {
                    Dock = DockStyle.Top,
                    Height = 200
                };

                foreach (var (index, product) in productList)
                {
                    listBox.Items.Add($"{index}. {product.Name}, {product.StockQuantity} шт");
                }

                // Quantity input field
                var lblQuantity = new Label
                {
                    Text = "Количество для удаления (0 - всё):",
                    Location = new Point(10, 220),
                    Size = new Size(250, 20)
                };

                var numQuantity = new NumericUpDown
                {
                    Location = new Point(10, 245),
                    Size = new Size(120, 25),
                    Minimum = 0,
                    Maximum = 999999
                };

                var btnOk = new Button
                {
                    Text = "ОК",
                    Location = new Point(10, 280),
                    Size = new Size(80, 30)
                };

                var btnCancel = new Button
                {
                    Text = "Отмена",
                    Location = new Point(100, 280),
                    Size = new Size(80, 30)
                };

                (int ProductId, int Quantity)? result = null;

                // Product selection handler
                listBox.SelectedIndexChanged += (s, args) =>
                {
                    if (listBox.SelectedIndex >= 0)
                    {
                        var selectedProduct = productList[listBox.SelectedIndex].Product;
                        numQuantity.Maximum = selectedProduct.StockQuantity;
                        numQuantity.Value = 0;
                    }
                };

                // OK button handler
                btnOk.Click += (s, args) =>
                {
                    if (listBox.SelectedIndex < 0)
                    {
                        ShowError("Предупреждение", "Выберите товар для удаления.");
                        return;
                    }

                    var selectedProduct = productList[listBox.SelectedIndex].Product;
                    result = (selectedProduct.Id, (int)numQuantity.Value);
                    selectForm.DialogResult = DialogResult.OK;
                };

                // Cancel button handler
                btnCancel.Click += (s, args) =>
                {
                    selectForm.DialogResult = DialogResult.Cancel;
                };

                selectForm.Controls.Add(listBox);
                selectForm.Controls.Add(lblQuantity);
                selectForm.Controls.Add(numQuantity);
                selectForm.Controls.Add(btnOk);
                selectForm.Controls.Add(btnCancel);

                if (selectForm.ShowDialog(this) == DialogResult.OK)
                {
                    return result;
                }

                return null;
            }
        }

        #endregion

        #region Initialize Components

        /// <summary>
        /// Initializes all form components.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "Продакт манагемент систем";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(800, 600);
            this.MaximumSize = new Size(800, 600);

            // Button panel
            buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(5)
            };

            // Refresh button
            btnRefresh = new Button
            {
                Text = "Обновить",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnRefresh.Click += BtnRefresh_Click;

            // Add button
            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(140, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnAdd.Click += BtnAdd_Click;

            // Delete button
            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(270, 10),
                Size = new Size(120, 30),
                UseVisualStyleBackColor = true
            };
            btnDelete.Click += BtnDelete_Click;

            // Delete by quantity button
            btnDeleteByQuantity = new Button
            {
                Text = "Удалить количество",
                Location = new Point(400, 10),
                Size = new Size(160, 30),
                UseVisualStyleBackColor = true
            };
            btnDeleteByQuantity.Click += BtnDeleteByQuantity_Click;

            // Add buttons to panel
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(btnAdd);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnDeleteByQuantity);

            // Create DataGridView
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = SystemColors.Window,
                BorderStyle = BorderStyle.Fixed3D
            };

            // Add elements to form
            this.Controls.Add(dataGridView);
            this.Controls.Add(buttonPanel);

            this.ResumeLayout(false);
        }

        #endregion

        #region Button Event Handlers - Fire View Events

        /// <summary>
        /// Handles Refresh button click - fires RefreshRequested event.
        /// </summary>
        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles Add button click - fires AddProductRequested event.
        /// MVP: Presenter handles showing the dialog.
        /// </summary>
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            AddProductRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles Delete button click - fires DeleteProductRequested event.
        /// </summary>
        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var productId = GetSelectedProductId();
            if (productId == null)
            {
                ShowError("Предупреждение", "Пожалуйста, выберите строку для удаления.");
                return;
            }

            DeleteProductRequested?.Invoke(this, productId.Value);
        }

        /// <summary>
        /// Handles Delete By Quantity button click - fires DeleteProductByQuantityRequested event.
        /// MVP: View просит Presenter предоставить данные для диалога.
        /// </summary>
        private void BtnDeleteByQuantity_Click(object? sender, EventArgs e)
        {
            // This event will be handled by presenter which will call ShowDeleteByQuantityDialog
            // For now, we fire an event with (-1, -1) to signal that we need to show dialog first
            DeleteProductByQuantityRequested?.Invoke(this, (-1, -1));
        }

        #endregion
    }
}
