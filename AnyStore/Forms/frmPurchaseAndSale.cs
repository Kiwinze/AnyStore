using System;
using System.Windows.Forms;
using AnyStore.BLL;

namespace AnyStore.UI
{
    public partial class frmPurchaseAndSale : Form
    {
        private readonly TransactionBLL _bll = new TransactionBLL();
        private readonly int _currentUserId;
        private readonly string _currentUserName;
        private decimal _currentRate = 0;
        private int _availableQty = 0;

        public frmPurchaseAndSale(int userId, string userName)
        {
            InitializeComponent();
            _currentUserId = userId;
            _currentUserName = userName;
        }

        private void frmPurchaseAndSale_Load(object sender, EventArgs e)
        {
            cmbType.Items.AddRange(new object[] { "PURCHASE", "SALE" });
            cmbType.SelectedIndex = -1;

            cmbProduct.DataSource = _bll.GetProductsForCombo();
            cmbProduct.DisplayMember = "Name";
            cmbProduct.ValueMember = "Id";
            cmbProduct.SelectedIndex = -1;

            dgvTransactions.DataSource = _bll.GetAll();
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem is System.Data.DataRowView row)
            {
                _currentRate = Convert.ToDecimal(row["Rate"]);
                _availableQty = Convert.ToInt32(row["Qty"]);
                txtRate.Text = _currentRate.ToString("F2");
                lblStock.Text = $"На складе: {_availableQty}";
                CalculateTotal();
            }
        }

        private void txtQty_TextChanged(object sender, EventArgs e) => CalculateTotal();

        private void txtRate_TextChanged(object sender, EventArgs e) => CalculateTotal();

        private void CalculateTotal()
        {
            decimal rate;
            int qty;
            if (decimal.TryParse(txtRate.Text, out rate) && int.TryParse(txtQty.Text, out qty))
                txtTotal.Text = (rate * qty).ToString("F2");
            else
                txtTotal.Text = "0.00";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbType.SelectedIndex < 0 || cmbProduct.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите тип операции и товар.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal rate, total;
            int qty;
            if (!decimal.TryParse(txtRate.Text, out rate) ||
                !int.TryParse(txtQty.Text, out qty) ||
                !decimal.TryParse(txtTotal.Text, out total))
            {
                MessageBox.Show("Некорректные данные.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result = _bll.Add(
                cmbType.SelectedItem.ToString(),
                txtDealerCustomer.Text,
                (int)cmbProduct.SelectedValue,
                rate, qty, total,
                _currentUserId, _currentUserName);

            if (result == "OK")
            {
                MessageBox.Show("Транзакция сохранена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                frmPurchaseAndSale_Load(sender, e);
            }
            else
            {
                MessageBox.Show(result, "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearFields();

        private void ClearFields()
        {
            cmbType.SelectedIndex = -1;
            cmbProduct.SelectedIndex = -1;
            txtDealerCustomer.Clear();
            txtRate.Clear();
            txtQty.Clear();
            txtTotal.Clear();
            lblStock.Text = "На складе: -";
        }
    }
}