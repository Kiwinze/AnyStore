using AnyStore.BLL;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AnyStore.UI
{
    public partial class frmProducts : Form
    {
        private readonly ProductBLL _productBll = new ProductBLL();
        private readonly CategoryBLL _categoryBll = new CategoryBLL();
        private readonly int _currentUserId;

        public frmProducts(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
        }

        private void frmProducts_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadData();
        }

        private void LoadCategories()
        {
            cmbCategory.DataSource = _categoryBll.GetAll();
            cmbCategory.DisplayMember = "Title";
            cmbCategory.ValueMember = "Id";
            cmbCategory.SelectedIndex = -1;
        }

        private void LoadData()
        {
            dgvProducts.DataSource = _productBll.GetAll();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal rate;
            int qty;
            if (!decimal.TryParse(txtRate.Text, out rate) || !int.TryParse(txtQty.Text, out qty))
            {
                MessageBox.Show("Некорректная цена или количество.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result = _productBll.Add(txtName.Text, (int)cmbCategory.SelectedValue,
                txtDescription.Text, rate, qty, _currentUserId);
            ShowResult(result);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) return;

            decimal rate;
            int qty;
            if (!decimal.TryParse(txtRate.Text, out rate) || !int.TryParse(txtQty.Text, out qty))
                return;

            string result = _productBll.Update(int.Parse(txtId.Text), txtName.Text,
                (int)cmbCategory.SelectedValue, txtDescription.Text, rate, qty);
            ShowResult(result);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) return;
            if (MessageBox.Show("Удалить товар?", "Подтверждение",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            string result = _productBll.Delete(int.Parse(txtId.Text));
            ShowResult(result);
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearFields();

        private void ShowResult(string result)
        {
            if (result == "OK")
            {
                MessageBox.Show("Операция выполнена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            else
            {
                MessageBox.Show(result, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtName.Clear();
            txtDescription.Clear();
            txtRate.Clear();
            txtQty.Clear();
            cmbCategory.SelectedIndex = -1;
            txtSearch.Clear();
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvProducts.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value?.ToString();
            txtName.Text = row.Cells["Name"].Value?.ToString();
            txtDescription.Text = row.Cells["Description"].Value?.ToString();
            txtRate.Text = row.Cells["Rate"].Value?.ToString();
            txtQty.Text = row.Cells["Qty"].Value?.ToString();
            cmbCategory.Text = row.Cells["Category"].Value?.ToString();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvProducts.DataSource = _productBll.Search(txtSearch.Text);
        }
    }
}