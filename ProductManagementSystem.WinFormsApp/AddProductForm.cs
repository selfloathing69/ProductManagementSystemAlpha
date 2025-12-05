using System;
using System.Drawing;
using System.Windows.Forms;
using ProductManagementSystem.Shared;

namespace ProductManagementSystem.WinFormsApp
{
    /// <summary>
    /// MVP Pattern - Add Product View Implementation.
    /// Implements IAddProductView interface for the MVP pattern.
    /// SOLID - S: Class is responsible only for UI of adding a new product.
    /// SOLID - D: Depends on IAddProductView abstraction, communicates with Presenter through events.
    /// View не имеет зависимости от Model - работает только с примитивными типами.
    /// </summary>
    public partial class AddProductForm : Form, IAddProductView
    {
        #region Fields
        
        // UI elements
        private NumericUpDown numId = null!;
        private TextBox txtName = null!;
        private TextBox txtDescription = null!;
        private ComboBox cmbCategory = null!;
        private NumericUpDown numPrice = null!;
        private NumericUpDown numStock = null!;
        private Button btnOk = null!;
        private Button btnCancel = null!;

        #endregion

        #region IAddProductView Events

        /// <inheritdoc/>
        public event EventHandler? SaveRequested;

        /// <inheritdoc/>
        public event EventHandler? CancelRequested;

        #endregion

        #region IAddProductView Properties

        /// <inheritdoc/>
        public int ProductId => (int)numId.Value;

        /// <inheritdoc/>
        public new string ProductName => txtName.Text;

        /// <inheritdoc/>
        public string ProductDescription => txtDescription.Text;

        /// <inheritdoc/>
        public decimal ProductPrice => numPrice.Value;

        /// <inheritdoc/>
        public string ProductCategory => cmbCategory.SelectedItem?.ToString() ?? string.Empty;

        /// <inheritdoc/>
        public int StockQuantity => (int)numStock.Value;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for AddProductForm.
        /// Initializes UI components. Presenter will be attached externally.
        /// </summary>
        public AddProductForm()
        {
            InitializeComponent();
        }

        #endregion

        #region IAddProductView Implementation

        /// <inheritdoc/>
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        public void CloseWithSuccess()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <inheritdoc/>
        public void CloseWithCancel()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <inheritdoc/>
        public void FocusField(string fieldName)
        {
            switch (fieldName.ToLowerInvariant())
            {
                case "id":
                    numId.Focus();
                    break;
                case "name":
                    txtName.Focus();
                    break;
                case "description":
                    txtDescription.Focus();
                    break;
                case "price":
                    numPrice.Focus();
                    break;
                case "category":
                    cmbCategory.Focus();
                    break;
                case "stockquantity":
                case "quantity":
                    numStock.Focus();
                    break;
            }
        }

        #endregion

        #region Initialize Components

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Новый товар";
            this.ClientSize = new Size(460, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = SystemColors.Window;
            this.ShowInTaskbar = false;

            Label lblTitle = new Label
            {
                Text = "Новый товар",
                Location = new Point(10, 10),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            Label lblId = new Label
            {
                Text = "ID:",
                Location = new Point(10, 45),
                Size = new Size(100, 20)
            };
            numId = new NumericUpDown
            {
                Location = new Point(120, 40),
                Size = new Size(150, 25),
                Minimum = 1M,
                Maximum = 999999M,
                Value = 1M
            };

            Label lblName = new Label
            {
                Text = "Название:",
                Location = new Point(10, 80),
                Size = new Size(100, 20)
            };
            txtName = new TextBox
            {
                Location = new Point(120, 75),
                Size = new Size(300, 25)
            };

            Label lblDesc = new Label
            {
                Text = "Описание:",
                Location = new Point(10, 115),
                Size = new Size(100, 20)
            };
            txtDescription = new TextBox
            {
                Location = new Point(120, 110),
                Size = new Size(300, 25)
            };

            Label lblPrice = new Label
            {
                Text = "Цена:",
                Location = new Point(10, 150),
                Size = new Size(100, 20)
            };
            numPrice = new NumericUpDown
            {
                Location = new Point(120, 145),
                Size = new Size(150, 25),
                Maximum = 1000000M,
                DecimalPlaces = 2,
                Minimum = 0M
            };

            Label lblCategory = new Label
            {
                Text = "Категория:",
                Location = new Point(10, 185),
                Size = new Size(100, 20)
            };
            cmbCategory = new ComboBox
            {
                Location = new Point(120, 180),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new object[]
            {
                "Электроника",
                "Периферия",
                "Аудио",
                "Комплектующие",
                "Аксессуары",
                "Другое"
            });

            Label lblStock = new Label
            {
                Text = "Количество:",
                Location = new Point(10, 220),
                Size = new Size(100, 20)
            };
            numStock = new NumericUpDown
            {
                Location = new Point(120, 215),
                Size = new Size(150, 25),
                Maximum = 100000M,
                Minimum = 0M
            };

            // Buttons
            btnOk = new Button
            {
                Text = "ОК",
                Location = new Point(120, 260),
                Size = new Size(90, 30),
                UseVisualStyleBackColor = true
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(240, 260),
                Size = new Size(90, 30),
                UseVisualStyleBackColor = true
            };
            btnCancel.Click += BtnCancel_Click;

            // Add all elements to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblId);
            this.Controls.Add(numId);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblPrice);
            this.Controls.Add(numPrice);
            this.Controls.Add(lblCategory);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(lblStock);
            this.Controls.Add(numStock);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);

            // Set Accept and Cancel buttons for keyboard shortcuts
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            this.ResumeLayout(false);
        }

        #endregion

        #region Event Handlers - Fire View Events

        /// <summary>
        /// Handles OK button click - fires SaveRequested event.
        /// </summary>
        private void BtnOk_Click(object? sender, EventArgs e)
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles Cancel button click - fires CancelRequested event.
        /// </summary>
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
