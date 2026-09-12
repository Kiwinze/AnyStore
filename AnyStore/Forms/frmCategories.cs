using System;
using System.Windows.Forms;
using AnyStore.BLL;

namespace AnyStore.UI
{
    public partial class frmCategories : Form
    {
        private readonly CategoryBLL _bll = new CategoryBLL();
        private readonly int _currentUserId;

        public frmCategories(int currentUserId)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
        }

        private void frmCategories_Load(object sender, EventArgs e) => LoadData();

        private void LoadData()
        {
            dgvCategories.DataSource = _bll.GetAll();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string result = _bll.Add(txtTitle.Text, txtDescription.Text, _currentUserId);
            ShowResult(result);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) return;
            string result = _bll.Update(int.Parse(txtId.Text), txtTitle.Text, txtDescription.Text);
            ShowResult(result);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtId.Text)) return;
            if (MessageBox.Show("Удалить категорию?", "Подтверждение",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            string result = _bll.Delete(int.Parse(txtId.Text));
            ShowResult(result);
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearFields();

        private void ShowResult(string result)
        {
            if (result == "OK")
            {
                MessageBox.Show("Операция выполнена успешно!", "Успех",
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
            txtTitle.Clear();
            txtDescription.Clear();
            txtSearch.Clear();
        }

        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvCategories.Rows[e.RowIndex];
            txtId.Text = row.Cells["Id"].Value?.ToString();
            txtTitle.Text = row.Cells["Title"].Value?.ToString();
            txtDescription.Text = row.Cells["Description"].Value?.ToString();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvCategories.DataSource = _bll.Search(txtSearch.Text);
        }
    }
}